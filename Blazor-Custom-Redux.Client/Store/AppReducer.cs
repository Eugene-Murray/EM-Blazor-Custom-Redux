using System.Collections.Immutable;

namespace Blazor_Custom_Redux.Client.Store;

public static class AppReducer
{
    public static AppState Reduce(AppState state, IStoreAction action) =>
        action switch
        {
            HydrateStateAction hydrateAction => hydrateAction.State with
            {
                IsHydrated = true
            },
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
