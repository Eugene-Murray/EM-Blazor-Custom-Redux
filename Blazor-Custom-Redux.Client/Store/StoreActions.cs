using Blazor.CustomRedux.Store;

namespace Blazor_Custom_Redux.Client.Store;

public sealed record AddTodoAction(string Title) : IReduxAction;

public sealed record ToggleTodoAction(Guid TodoId) : IReduxAction;

public sealed record RemoveTodoAction(Guid TodoId) : IReduxAction;

public sealed record ClearCompletedTodosAction : IReduxAction;
