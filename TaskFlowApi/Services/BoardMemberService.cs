using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services;

public interface IBoardMemberService
{
    Task<BoardMember?> AddBoardMemberAsync(int boardId, AddBoardMemberDto request);
    Task<bool> RemoveBoardMemberAsync(int boardId, Guid userId);
    Task<List<BoardMemberDto>?> GetBoardMembersAsync(int boardId);
}

public class BoardMemberService(TaskFlowDbContext dbContext) : IBoardMemberService
{
    public async Task<BoardMember?> AddBoardMemberAsync(int boardId, AddBoardMemberDto request)
    {
        var board = await dbContext.Boards.FindAsync(boardId);
        if (board is null)
        {
            return null;
        }

        var user = await dbContext.Users.FindAsync(request.UserId);
        if (user is null)
        {
            return null;
        }

        var existingMember = await dbContext.BoardMembers.FirstOrDefaultAsync(bm =>
            bm.BoardId == boardId && bm.UserId == request.UserId
        );
        if (existingMember != null)
        {
            throw new InvalidOperationException("User is already a member of the board.");
        }

        var boardMember = new BoardMember
        {
            BoardId = boardId,
            UserId = request.UserId,
            Role = request.Role,
        };

        dbContext.BoardMembers.Add(boardMember);
        await dbContext.SaveChangesAsync();

        return boardMember;
    }

    public async Task<bool> RemoveBoardMemberAsync(int boardId, Guid userId)
    {
        var boardMember = await dbContext.BoardMembers.FirstOrDefaultAsync(bm =>
            bm.BoardId == boardId && bm.UserId == userId
        );

        if (boardMember is null)
        {
            return false;
        }

        dbContext.BoardMembers.Remove(boardMember);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<BoardMemberDto>?> GetBoardMembersAsync(int boardId)
    {
        var board = await dbContext.Boards.FindAsync(boardId);
        if (board is null)
        {
            return null;
        }

        return await dbContext
            .BoardMembers.Where(bm => bm.BoardId == boardId)
            .Include(bm => bm.User)
            .Select(bm => new BoardMemberDto
            {
                UserId = bm.UserId,
                DisplayName = bm.User.DisplayName,
                Email = bm.User.Email,
                Role = bm.Role,
            })
            .ToListAsync();
    }
}
