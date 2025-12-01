using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services;

public interface ICommentService
{
    public Task<CommentDto> CreateCommentAsync(int TaskId, CommentRequestDto request);

    public Task<CommentDto> UpdateCommentAsync(int commentId, CommentRequestDto request);

    public Task DeleteCommentAsync(int commentId);

    public Task<List<CommentDto>> GetCommentsForTaskAsync(int taskId);
}

public class CommentService(TaskFlowDbContext dbContext) : ICommentService
{
    public async Task<CommentDto> CreateCommentAsync(int TaskId, CommentRequestDto request)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == TaskId);
        if (task is null)
        {
            throw new KeyNotFoundException($"Task with ID {TaskId} not found.");
        }

        var Comment = new Comment
        {
            TaskId = TaskId,
            AuthorId = request.AuthorId,
            Content = request.Content,
            CreatedAt = request.CreatedAt,
        };

        dbContext.Comments.Add(Comment);
        await dbContext.SaveChangesAsync();

        return new CommentDto
        {
            Id = Comment.Id,
            TaskId = Comment.TaskId,
            AuthorId = Comment.AuthorId,
            Content = Comment.Content,
            CreatedAt = Comment.CreatedAt,
        };
    }

    public async Task<CommentDto> UpdateCommentAsync(int commentId, CommentRequestDto request)
    {
        var comment = await dbContext.Comments.FindAsync(commentId);
        if (comment is null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
        }

        var updatedComment = new Comment
        {
            Id = commentId,
            TaskId = comment.TaskId,
            AuthorId = request.AuthorId,
            Content = request.Content,
            CreatedAt = request.CreatedAt,
        };

        dbContext.Entry(comment).CurrentValues.SetValues(updatedComment);
        await dbContext.SaveChangesAsync();

        return new CommentDto
        {
            Id = updatedComment.Id,
            TaskId = updatedComment.TaskId,
            AuthorId = updatedComment.AuthorId,
            Content = updatedComment.Content,
            CreatedAt = updatedComment.CreatedAt,
        };
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await dbContext.Comments.FindAsync(commentId);
        if (comment is null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
        }

        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync();
    }

    public Task<List<CommentDto>> GetCommentsForTaskAsync(int taskId)
    {
        return dbContext
            .Tasks.Where(t => t.Id == taskId)
            .Include(t => t.Comments)
            .SelectMany(t => t.Comments)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                TaskId = c.TaskId,
                AuthorId = c.AuthorId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync();
    }
}
