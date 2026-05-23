namespace EM.Blazor.CustomRedux.Store;

public interface IReduxStoreEffect<TState> : IDisposable
{
    void Connect(IObservable<IReduxAction> actions, IObservable<TState> states, Action<IReduxAction> dispatch);
}
