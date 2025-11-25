using System;
using System.Collections.Generic;

namespace TaskFlowApi.Dtos.Board;

public record class ColumnDto
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public string Title { get; set; } = null!;
    public double SortOrder { get; set; }
    public ICollection<TaskDto> Tasks { get; set; } = new List<TaskDto>();
}
