using System;

namespace TaskFlowApi.Dtos.Board;

public class BoardRequestDto
{
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}
