using System;

namespace TaskFlowApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public double SortOrder { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; } = null;

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public bool IsCompleted { get; set; } = false;
}
