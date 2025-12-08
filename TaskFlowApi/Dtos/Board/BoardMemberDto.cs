using TaskFlowApi.Dtos.Auth;

namespace TaskFlowApi.Dtos.Board;

public record class BoardMemberDto
{
    public Guid UserId { get; set; }
    public UserDto User { get; set; } = null!;
    public int BoardId { get; set; }
    public string Role { get; set; } = null!;
}
