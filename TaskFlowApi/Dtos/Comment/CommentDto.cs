using System;

namespace TaskFlowApi.Dtos.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
