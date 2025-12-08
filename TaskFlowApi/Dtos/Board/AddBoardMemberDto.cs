namespace TaskFlowApi.Dtos.Board;

public record class AddBoardMemberDto
{
    public string Email { get; set; } = null!;
    public int BoardId { get; set; }
    public string Role { get; set; } = null!;
}
