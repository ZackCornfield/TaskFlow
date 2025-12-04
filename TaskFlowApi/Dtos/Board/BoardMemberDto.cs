namespace TaskFlowApi.Dtos.Board;

public record class BoardMemberDto
{
    public Guid UserId { get; set; }
    public int BoardId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
