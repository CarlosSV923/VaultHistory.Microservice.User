namespace VaultHistory.User.Api.Utils;

public sealed record ApiErrorResponse(
    string Code,
    string Message,
    IEnumerable<object>? Errors = null);
