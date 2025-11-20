using System;

namespace TaskFlowApi.Models;

public class BoardMember
{
    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Role { get; set; } = "Member"; // Member, Admin
}
