using System;
using System.Collections.Generic;

namespace TaskFlowApi.Dtos.Board;

public record class BoardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ColumnDto> Columns { get; set; } = new List<ColumnDto>();
    public ICollection<BoardMemberDto> Members { get; set; } = new List<BoardMemberDto>();
}
