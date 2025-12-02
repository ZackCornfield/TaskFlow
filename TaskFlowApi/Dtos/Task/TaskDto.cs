using System;
using TaskFlowApi.Dtos.Comment;
using TaskFlowApi.Dtos.Tag;

namespace TaskFlowApi.Dtos.Board;

public record class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public double SortOrder { get; set; }
    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? AssignedToId { get; set; }

    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
    public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
}
