namespace VoteUp.Portal.Util;

public record FilterRequest
(
    string? Query,
    string? SortBy,
    string? SortOrder,
    DateTime? StartDate,
    DateTime? EndDate,
    bool? ShowDeleted
);