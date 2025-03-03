namespace Core.Models;

public class CreateBoardDto
{
    public string Name { get; set; } = null!;
}

public class CreateBoardResponseDto
{
    public string StatusUrl { get; set; } = default!;
}