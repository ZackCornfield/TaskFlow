namespace TaskFlowApi.Dtos.Task;

public record class MoveTaskDto
{
    public int TargetColumnId { get; set; }
    public double SortOrder { get; set; }
}
