﻿namespace ErrorHandling;

public sealed record Error(string Code, string? Message = null);