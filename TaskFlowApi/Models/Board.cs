using System;
using System.Text.Json.Serialization;

namespace TaskFlowApi.Models;

public class Board
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Column> Columns { get; set; } = new List<Column>();

    [JsonIgnore]
    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
}
