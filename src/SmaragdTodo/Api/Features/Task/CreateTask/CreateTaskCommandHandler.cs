using Api.Infrastructure;
using Core;
using Core.Models;

namespace Api.Features.Task.CreateTask;

public class CreateTaskCommandHandler : CommandHandler<CreateTaskCommand, CreateTaskResponseDto>
{
    public CreateTaskCommandHandler()
    {
    }

    public override Task<Result<CreateTaskResponseDto, Error>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}