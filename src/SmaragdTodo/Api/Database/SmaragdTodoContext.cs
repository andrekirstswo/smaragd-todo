using Core;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using User = Core.Database.Models.User;

namespace Api.Database;

public class SmaragdTodoContext : DbContext
{
    public SmaragdTodoContext(DbContextOptions<SmaragdTodoContext> options)
        : base(options)
    {
    }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Credential> Credentials => Set<Credential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Board>()
            .ToContainer(ContainerNames.Boards);

        modelBuilder
            .Entity<Board>()
            .HasPartitionKey(p => p.Id);

        modelBuilder
            .Entity<Board>()
            .Property(p => p.Id)
            .ToJsonProperty("id");

        modelBuilder
            .Entity<Board>()
            .OwnsMany(p => p.Accesses, builder =>
            {
                builder.ToJsonProperty("accesses");
                
                builder
                    .Property(p => p.UserId)
                    .ToJsonProperty("userId");

                builder
                    .Property(p => p.Role)
                    .ToJsonProperty("role");
            });

        modelBuilder
            .Entity<Board>()
            .Property(p => p.Name)
            .ToJsonProperty("name");

        modelBuilder
            .Entity<Board>()
            .Property(p => p.Owner)
            .ToJsonProperty("owner");

        modelBuilder
            .Entity<User>()
            .ToContainer(ContainerNames.Users);

        modelBuilder
            .Entity<User>()
            .HasPartitionKey(p => p.AuthenticationProvider);

        modelBuilder
            .Entity<User>()
            .Property(p => p.Email)
            .ToJsonProperty("email");

        modelBuilder
            .Entity<User>()
            .Property(p => p.AuthenticationProvider)
            .ToJsonProperty("authenticationProvider");

        modelBuilder
            .Entity<User>()
            .Property(p => p.Picture)
            .ToJsonProperty("picture");

        modelBuilder
            .Entity<User>()
            .OwnsOne(p => p.Name, builder =>
            {
                builder
                    .ToJsonProperty("name");

                builder
                    .Property(p => p.LastName)
                    .ToJsonProperty("lastName");

                builder
                    .Property(p => p.FirstName)
                    .ToJsonProperty("firstName");
            });

        modelBuilder
            .Entity<Credential>()
            .ToContainer(ContainerNames.Credentials);

        modelBuilder
            .Entity<Credential>()
            .HasPartitionKey(p => p.UserId);

        modelBuilder
            .Entity<Credential>()
            .Property(p => p.UserId)
            .ToJsonProperty("userId");

        modelBuilder
            .Entity<Credential>()
            .Property(p => p.AccessToken)
            .ToJsonProperty("accessToken");

        modelBuilder
            .Entity<Credential>()
            .Property(p => p.ExpiresInSeconds)
            .ToJsonProperty("expiresInSeconds");

        modelBuilder
            .Entity<Credential>()
            .Property(p => p.IdToken)
            .ToJsonProperty("idToken");

        modelBuilder
            .Entity<Credential>()
            .Property(p => p.IssuedUtc)
            .ToJsonProperty("issuedUtc");

        modelBuilder
            .Entity<Credential>()
            .Property(p => p.RefreshToken)
            .ToJsonProperty("refreshToken");
    }
}