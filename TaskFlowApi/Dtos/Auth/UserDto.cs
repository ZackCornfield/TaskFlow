namespace TaskFlowApi.Dtos.Auth;

public record class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Token { get; set; } = null!;
}
