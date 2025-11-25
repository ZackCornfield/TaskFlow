using System;

namespace TaskFlowApi.Dtos.Tag;

public class TagRequestDto
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "#2196f3";
}
