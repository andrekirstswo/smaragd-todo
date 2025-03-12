using Core;
using Core.Models.ValueObjects;

namespace Api;

public static class KnownErrors
{
    public static class Authentication
    {
        public static readonly Error EmailNotVerified = new Error("EMAIL_NOT_VERIFIED", "Email not verified");
    }

    public static class Board
    {
        public static Error NotFoundById(BoardId boardId) => new Error(ErrorCodes.Board.NotFoundById, $"Board with id \"{boardId.Value}\" not found");
        public static Error HasNoName(BoardId boardId) => new Error(ErrorCodes.Board.HasNoName, "Board with id {boardId.Value} has no name");
        public static Error AccessDenied => new Error(ErrorCodes.Board.AccessDenied, "Access denied");
    }
}

public static class ErrorCodes
{
    public static class Board
    {
        public const string NotFoundById = "BOARD_NOT_FOUND_BY_ID";
        public const string HasNoName = "BOARD_HAS_NO_NAME";
        public const string AccessDenied = "BOARD_ACCESS_DENIED";
    }
}