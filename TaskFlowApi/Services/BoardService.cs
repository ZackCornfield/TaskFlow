using System;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Dtos.Tag;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services;

public interface IBoardService
{
    Task<List<BoardDto>> GetBoardsAsync();
    Task<BoardDto> CreateBoardAsync(BoardRequestDto request);
    Task<BoardDto?> GetBoardByIdAsync(int boardId, Guid userId); // Allow nullable return
    Task<BoardDto?> UpdateBoardAsync(int boardId, BoardRequestDto request); // Allow nullable return
    Task<bool> DeleteBoardAsync(int boardId);
    Task<List<BoardDto>> GetUserBoardsAsync(Guid userId);
}

public class BoardService(TaskFlowDbContext dbContext, IBoardMemberService boardMemberService)
    : IBoardService
{
    public async Task<List<BoardDto>> GetBoardsAsync()
    {
        return await dbContext
            .Boards.Select(b => new BoardDto
            {
                Id = b.Id,
                Title = b.Title,
                OwnerId = b.OwnerId,
                CreatedAt = b.CreatedAt,
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<BoardDto> CreateBoardAsync(BoardRequestDto request)
    {
        var board = new Board
        {
            Title = request.Title,
            OwnerId = request.OwnerId,
            CreatedAt = request.CreatedAt,
        };

        dbContext.Boards.Add(board);
        await dbContext.SaveChangesAsync();

        await boardMemberService.AddBoardMemberAsync(
            new AddBoardMemberDto
            {
                UserId = board.OwnerId,
                BoardId = board.Id,
                Role = "Owner",
            }
        );

        return new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            OwnerId = board.OwnerId,
            CreatedAt = board.CreatedAt,
        };
    }

    public async Task<BoardDto?> GetBoardByIdAsync(int boardId, Guid userId)
    {
        try
        {
            var hasAccess = await dbContext.BoardMembers.AnyAsync(bm =>
                bm.UserId == userId && bm.BoardId == boardId
            );

            if (!hasAccess)
            {
                Console.WriteLine($"Access denied for user {userId} to board {boardId}");
                return null;
            }

            var board = await dbContext
                .Boards.AsNoTracking()
                .Where(b => b.Id == boardId)
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    OwnerId = b.OwnerId,
                    CreatedAt = b.CreatedAt,
                    Columns = b
                        .Columns.OrderBy(c => c.SortOrder)
                        .Select(c => new ColumnDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            SortOrder = c.SortOrder,
                            Tasks = c
                                .Tasks.OrderBy(t => t.SortOrder)
                                .Select(t => new TaskDto
                                {
                                    Id = t.Id,
                                    ColumnId = t.ColumnId,
                                    Title = t.Title,
                                    Description = t.Description,
                                    SortOrder = t.SortOrder,
                                    DueDate = t.DueDate,
                                    CreatedAt = t.CreatedAt,
                                    CreatedById = t.CreatedById,
                                    AssignedToId = t.AssignedToId,
                                    Tags = t
                                        .Tags.Select(tt => new TagDto
                                        {
                                            Id = tt.Id,
                                            Name = tt.Name,
                                            Color = tt.Color,
                                        })
                                        .ToList(),
                                    Comments = t
                                        .Comments.Select(c => new CommentDto
                                        {
                                            Id = c.Id,
                                            TaskId = c.TaskId,
                                            AuthorId = c.AuthorId,
                                            AuthorName = c.AuthorName,
                                            Content = c.Content,
                                            CreatedAt = c.CreatedAt,
                                        })
                                        .ToList(),
                                })
                                .ToList(),
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();

            if (board == null)
            {
                Console.WriteLine($"Board with ID {boardId} not found.");
            }

            return board;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetBoardByIdAsync: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    public async Task<BoardDto?> UpdateBoardAsync(int boardId, BoardRequestDto request)
    {
        var board = await dbContext.Boards.FindAsync(boardId);
        if (board == null)
        {
            return null;
        }

        board.Title = request.Title;
        board.OwnerId = request.OwnerId;
        board.CreatedAt = request.CreatedAt;

        await dbContext.SaveChangesAsync();

        return new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            OwnerId = board.OwnerId,
            CreatedAt = board.CreatedAt,
        };
    }

    public async Task<bool> DeleteBoardAsync(int boardId)
    {
        var board = await dbContext.Boards.FindAsync(boardId);
        if (board == null)
        {
            return false;
        }

        dbContext.Boards.Remove(board);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<BoardDto>> GetUserBoardsAsync(Guid userId)
    {
        return await dbContext
            .BoardMembers.Where(bm => bm.UserId == userId)
            .Include(bm => bm.Board)
            .Select(bm => new BoardDto
            {
                Id = bm.Board.Id,
                Title = bm.Board.Title,
                OwnerId = bm.Board.OwnerId,
                CreatedAt = bm.Board.CreatedAt,
            })
            .ToListAsync();
    }
}
