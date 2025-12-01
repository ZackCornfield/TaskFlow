using System;
using System.Text.Json.Serialization;

namespace TaskFlowApi.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<BoardMember> Boards { get; set; } = new List<BoardMember>();
}
