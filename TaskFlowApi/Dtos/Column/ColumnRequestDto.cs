using System;

namespace TaskFlowApi.Dtos.Column;

public record class ColumnRequestDto
{
    public int BoardId { get; set; }
    public string Title { get; set; } = null!;
    public double SortOrder { get; set; }
}
