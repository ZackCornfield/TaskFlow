using System;

namespace TaskFlowApi.Models;

public class Comment
{
    public int Id { get; set; }
    public int TaskId { get; set; }

    public Guid AuthorId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
