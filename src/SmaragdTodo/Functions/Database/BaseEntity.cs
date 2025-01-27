namespace Functions.Database;

public class BaseEntity
{
    // ReSharper disable once InconsistentNaming
    // You need the id to crate item in container
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string id { get; set; } = null!;
    public DateTimeOffset? CreatedAt { get; set; }
}