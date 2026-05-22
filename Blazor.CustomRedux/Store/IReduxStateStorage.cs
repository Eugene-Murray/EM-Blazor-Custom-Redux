namespace Blazor.CustomRedux.Store;

public interface IReduxStateStorage<TState>
{
    Task<TState?> LoadAsync(CancellationToken cancellationToken = default);

    ValueTask SaveAsync(TState state, CancellationToken cancellationToken = default);
}
