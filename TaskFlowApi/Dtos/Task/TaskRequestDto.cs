using System;

namespace TaskFlowApi.Dtos.Task;

public class TaskRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public double SortOrder { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? AssignedToId { get; set; }
    public bool IsCompleted { get; set; } = false;
}
