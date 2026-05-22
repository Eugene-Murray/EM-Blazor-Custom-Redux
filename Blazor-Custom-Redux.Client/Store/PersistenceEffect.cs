using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Blazor_Custom_Redux.Client.Store;

public sealed class PersistenceEffect(BrowserStateStorage browserStateStorage) : IStoreEffect
{
    private readonly CompositeDisposable _subscriptions = [];

    public void Connect(IObservable<IStoreAction> actions, IObservable<AppState> states, Action<IStoreAction> dispatch)
    {
        var persistenceSubscription = states
            .Skip(1)
            .Where(state => state.IsHydrated)
            .Throttle(TimeSpan.FromMilliseconds(150))
            .Select(state => Observable.FromAsync(cancellationToken => browserStateStorage.SaveAsync(state, cancellationToken).AsTask()))
            .Switch()
            .Subscribe();

        _subscriptions.Add(persistenceSubscription);
    }

    public void Dispose() => _subscriptions.Dispose();
}
