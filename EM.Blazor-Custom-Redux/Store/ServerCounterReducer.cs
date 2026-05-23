using EM.Blazor.CustomRedux.Store;

namespace EM.Blazor_Custom_Redux.Store;

public sealed class ServerCounterReducer : IReduxReducer<ServerCounterState>
{
    public ServerCounterState Reduce(ServerCounterState state, IReduxAction action) =>
        action switch
        {
            IncrementCounterAction => state with { Count = state.Count + 1, LastUpdatedAt = DateTimeOffset.UtcNow },
            DecrementCounterAction => state with { Count = state.Count - 1, LastUpdatedAt = DateTimeOffset.UtcNow },
            ResetCounterAction     => state with { Count = 0,               LastUpdatedAt = DateTimeOffset.UtcNow },
            _                      => state
        };
}
