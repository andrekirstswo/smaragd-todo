using Core;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //{
        //    foreach (var property in entityType.GetProperties())
        //    {
        //        //var jsonPropertyNameAttribute = property.PropertyInfo?.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false).FirstOrDefault() as JsonPropertyNameAttribute;
        //        var jsonPropertyNameValue = property.Get<JsonPropertyNameAttribute, string>(attribute => attribute.Name);
        //        var columnName = jsonPropertyNameValue ?? ToCamelCase(property.Name);
        //        property.SetJsonPropertyName(columnName);
        //    }
        //}

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
            });

        modelBuilder
            .Entity<Board>()
            .Property(p => p.Name)
            .ToJsonProperty("name");

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
    }

    // TODO auslagern
    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
        {
            return name;
        }

        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}

public static class AttrExt
{
    public static TResult? Get<TAttribute, TResult>(this IMutableProperty value, Func<TAttribute, TResult> func)
        where TAttribute : Attribute
    {
        return value.PropertyInfo?
            .GetCustomAttributes(typeof(TAttribute), false)
            .FirstOrDefault() is not TAttribute attribute
            ? default
            : func(attribute);
    }
}