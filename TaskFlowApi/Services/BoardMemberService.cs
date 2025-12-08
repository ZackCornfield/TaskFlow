using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Auth;
using TaskFlowApi.Dtos.Board;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services;

public interface IBoardMemberService
{
    Task<BoardMemberDto?> AddBoardMemberAsync(AddBoardMemberDto request);
    Task<bool> RemoveBoardMemberAsync(int boardId, Guid userId);
    Task<List<BoardMemberDto>?> GetBoardMembersAsync(int boardId);
}

public class BoardMemberService(TaskFlowDbContext dbContext) : IBoardMemberService
{
    public async Task<BoardMemberDto?> AddBoardMemberAsync(AddBoardMemberDto request)
    {
        var board = await dbContext.Boards.FindAsync(request.BoardId);
        if (board is null)
        {
            return null;
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return null;
        }

        var existingMember = await dbContext.BoardMembers.FirstOrDefaultAsync(bm =>
            bm.BoardId == request.BoardId && bm.UserId == user.Id
        );
        if (existingMember != null)
        {
            throw new InvalidOperationException("User is already a member of the board.");
        }

        var boardMember = new BoardMember
        {
            BoardId = request.BoardId,
            UserId = user.Id,
            Role = request.Role,
        };

        dbContext.BoardMembers.Add(boardMember);
        await dbContext.SaveChangesAsync();

        return new BoardMemberDto
        {
            UserId = boardMember.UserId,
            User = new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
            },
            BoardId = boardMember.BoardId,
            Role = boardMember.Role,
        };
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
                User = new UserDto
                {
                    Id = bm.User.Id,
                    DisplayName = bm.User.DisplayName,
                    Email = bm.User.Email,
                },
                BoardId = bm.BoardId,
                Role = bm.Role,
            })
            .ToListAsync();
    }
}
