namespace TaskFlowApi.Dtos.Board;

public record class AddBoardMemberDto
{
    public Guid UserId { get; set; }
    public int BoardId { get; set; }
    public string Role { get; set; } = null!;
}
