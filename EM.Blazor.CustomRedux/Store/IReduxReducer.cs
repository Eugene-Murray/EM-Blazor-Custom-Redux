namespace EM.Blazor.CustomRedux.Store;

public interface IReduxReducer<TState>
{
    TState Reduce(TState state, IReduxAction action);
}
