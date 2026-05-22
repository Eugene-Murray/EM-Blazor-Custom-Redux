namespace Blazor_Custom_Redux.Client.Store;

public interface IStoreEffect : IDisposable
{
    void Connect(IObservable<IStoreAction> actions, IObservable<AppState> states, Action<IStoreAction> dispatch);
}
