namespace Blazor_Custom_Redux.Client.Store;

public sealed record StoreHistoryEntry(
    int Sequence,
    DateTimeOffset OccurredAt,
    string ActionType,
    int TodoCount,
    string StateJson);
