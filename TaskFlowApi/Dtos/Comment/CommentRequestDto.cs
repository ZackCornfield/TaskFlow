using System;

namespace TaskFlowApi.Dtos.Comment;

public class CommentRequestDto
{
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
