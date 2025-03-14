using Core.Database.Models;
using Microsoft.Azure.CosmosRepository;

namespace Api.Database;

public class TaskRepository
{
    private readonly IRepository<TaskItem> _repository;

    public TaskRepository(IRepository<TaskItem> repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(CancellationToken cancellationToken = default)
    {
    }
}