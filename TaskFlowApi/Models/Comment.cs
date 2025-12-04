using System;
using System.Text.Json.Serialization;

namespace TaskFlowApi.Models;

public class Comment
{
    public int Id { get; set; }
    public int TaskId { get; set; }

    [JsonIgnore]
    public TaskItem Task { get; set; } = null!;

    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
