using System;

namespace TaskFlowApi.Models;

public class Column
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public string Title { get; set; } = null!;
    public double SortOrder { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
