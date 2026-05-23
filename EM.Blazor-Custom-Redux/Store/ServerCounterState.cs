namespace EM.Blazor_Custom_Redux.Store;

public sealed record ServerCounterState(int Count, DateTimeOffset LastUpdatedAt)
{
    public static ServerCounterState CreateDefault() =>
        new(0, DateTimeOffset.UtcNow);
}
