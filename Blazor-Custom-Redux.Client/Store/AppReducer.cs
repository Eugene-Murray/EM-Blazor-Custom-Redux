using System.Collections.Immutable;
using Blazor.CustomRedux.Store;

namespace Blazor_Custom_Redux.Client.Store;

public sealed class AppReducer : IReduxReducer<AppState>
{
    public AppState Reduce(AppState state, IReduxAction action) =>
        action switch
        {
            AddTodoAction addTodoAction => ReduceAddTodo(state, addTodoAction),
            ToggleTodoAction toggleTodoAction => state with
            {
                Todos = state.Todos
                    .Select(todo => todo.Id == toggleTodoAction.TodoId ? todo with { IsCompleted = !todo.IsCompleted } : todo)
                    .ToImmutableArray(),
                LastUpdatedAt = DateTimeOffset.UtcNow
            },
            RemoveTodoAction removeTodoAction => state with
            {
                Todos = state.Todos
                    .Where(todo => todo.Id != removeTodoAction.TodoId)
                    .ToImmutableArray(),
                LastUpdatedAt = DateTimeOffset.UtcNow
            },
            ClearCompletedTodosAction => state with
            {
                Todos = state.Todos
                    .Where(todo => !todo.IsCompleted)
                    .ToImmutableArray(),
                LastUpdatedAt = DateTimeOffset.UtcNow
            },
            _ => state
        };

    private static AppState ReduceAddTodo(AppState state, AddTodoAction addTodoAction)
    {
        var title = addTodoAction.Title.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            return state;
        }

        var todo = new TodoItem(Guid.NewGuid(), title, false, DateTimeOffset.UtcNow);

        return state with
        {
            Todos = state.Todos.Insert(0, todo),
            LastUpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
