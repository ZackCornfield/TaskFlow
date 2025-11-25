namespace TaskFlowApi.Dtos.Tag;

public record class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "#2196f3";
}
