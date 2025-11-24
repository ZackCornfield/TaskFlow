using System;

namespace TaskFlowApi.Dtos.Board;

public class BoardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}
