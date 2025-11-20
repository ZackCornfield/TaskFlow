using System;

namespace TaskFlowApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int ColumnId { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public double SortOrder { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedById { get; set; }
    public Guid? AssignedToId { get; set; }

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
