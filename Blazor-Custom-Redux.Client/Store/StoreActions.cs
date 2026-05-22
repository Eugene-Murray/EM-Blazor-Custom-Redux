namespace Blazor_Custom_Redux.Client.Store;

public interface IStoreAction;

public sealed record InitializeStoreAction : IStoreAction;

public sealed record HydrateStateAction(AppState State) : IStoreAction;

public sealed record AddTodoAction(string Title) : IStoreAction;

public sealed record ToggleTodoAction(Guid TodoId) : IStoreAction;

public sealed record RemoveTodoAction(Guid TodoId) : IStoreAction;

public sealed record ClearCompletedTodosAction : IStoreAction;
