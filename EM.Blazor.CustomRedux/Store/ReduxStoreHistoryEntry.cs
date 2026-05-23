namespace EM.Blazor.CustomRedux.Store;

public sealed record ReduxStoreHistoryEntry(
    int Sequence,
    DateTimeOffset OccurredAt,
    string ActionType,
    string StateJson);
