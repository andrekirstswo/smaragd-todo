using Core;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Api.Database;

public class SmaragdTodoContext : DbContext
{
    public SmaragdTodoContext(DbContextOptions<SmaragdTodoContext> options)
        : base(options)
    {
    }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Board>()
            .ToContainer(ContainerNames.Boards);

        modelBuilder
            .Entity<Board>()
            .HasPartitionKey(p => p.BoardId);

        modelBuilder
            .Entity<User>()
            .ToContainer(ContainerNames.Users);

        modelBuilder
            .Entity<User>()
            .HasPartitionKey(p => p.AuthenticationProvider);
    }
}