using System.Collections.Immutable;

namespace EM.Blazor_Custom_Redux.Client.Store;

public sealed record TodoItem(Guid Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt);

public sealed record AppState(
    ImmutableArray<TodoItem> Todos,
    bool IsHydrated,
    DateTimeOffset LastUpdatedAt)
{
    public static AppState CreateDefault()
    {
        var now = DateTimeOffset.UtcNow;

        return new AppState(
            ImmutableArray.Create(
                new TodoItem(Guid.Parse("5D8F87A0-65DE-4A6B-9F67-0DFE70ED2B6F"), "Create a custom immutable store.", true, now.AddMinutes(-30)),
                new TodoItem(Guid.Parse("FD4C8362-408D-417B-99EA-4D6CC828534F"), "Hydrate and persist the store to browser localStorage.", false, now.AddMinutes(-20)),
                new TodoItem(Guid.Parse("44B71376-3E3A-4BD8-8A07-D6FD24B36A2F"), "Render the same todo state in multiple Blazor components.", false, now.AddMinutes(-10))),
            false,
            now);
    }
}
