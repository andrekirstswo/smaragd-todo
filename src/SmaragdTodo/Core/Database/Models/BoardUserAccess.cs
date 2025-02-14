namespace Core.Database.Models;

public class BoardUserAccess : BaseEntity
{
    public string UserId { get; set; } = default!;
    public string Role { get; set; } = BoardUserAccessRoles.Reader;
}

public static class BoardUserAccessRoles
{
    public const string Reader = "Reader";
    public const string Writer = "Writer";
    public const string Admin = "Admin";
}