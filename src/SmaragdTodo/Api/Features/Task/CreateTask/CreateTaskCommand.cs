using Api.Infrastructure;
using Core.Models;

namespace Api.Features.Task.CreateTask;

public class CreateTaskCommand : Command<CreateTaskResponseDto>
{
    public CreateTaskCommand(string boardId, CreateTaskDto model)
    {
        BoardId = boardId;
        Model = model;
    }

    public string BoardId { get; set; }
    public CreateTaskDto Model { get; set; }
}