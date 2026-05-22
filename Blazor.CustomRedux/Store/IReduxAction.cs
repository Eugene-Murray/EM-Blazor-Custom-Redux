namespace Blazor.CustomRedux.Store;

public interface IReduxAction;

public sealed record InitializeStoreAction : IReduxAction;

public sealed record HydrateStateAction<TState>(TState State) : IReduxAction;
