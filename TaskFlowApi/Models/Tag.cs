using System;

namespace TaskFlowApi.Models;

public class Tag
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "#2196f3";
}
