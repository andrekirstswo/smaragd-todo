namespace Core.Models;

public class CreateTaskDto
{
    public string Title { get; set; } = default!;
    public List<string> AssignedTo { get; set; } = new List<string>();
}

public class CreateTaskResponseDto : IHasResponseWithStatusUrl
{
    public string StatusUrl { get; } = default!;
}