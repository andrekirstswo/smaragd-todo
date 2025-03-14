﻿using Core;

namespace Api;

public static class KnownErrors
{
    public static class Authentication
    {
        public static readonly Error EmailNotVerified = new Error("EMAIL_NOT_VERIFIED", "Email not verified");
    }

    public static class Board
    {
        public static Error NotFoundById(string boardId) => new Error(ErrorCodes.Board.NotFoundById, $"Board with id \"{boardId}\" not found");
        public static Error HasNoName(string boardId) => new Error(ErrorCodes.Board.HasNoName, $"Board with id {boardId} has no name");
        public static Error AccessDenied() => new Error(ErrorCodes.Board.AccessDenied, "Access denied");
    }

    public static class Task
    {
        public static Error HasNoTitle(string boardId) => new Error(ErrorCodes.Task.HasNoTitle, $"Task for board with id {boardId} has no title");
        public static Error HasNoAssignee(string boardId) => new Error(ErrorCodes.Task.HasNoAssignee, $"Task for board with id {boardId} is not assigned to a user");
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

    public static class Task
    {
        public const string HasNoTitle = "TASK_HAS_NO_TITLE";
        public const string HasNoAssignee = "TASK_HAS_NO_ASSIGNEE";
    }
}