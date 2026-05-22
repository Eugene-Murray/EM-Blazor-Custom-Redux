using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Blazor.CustomRedux.Store;

public sealed class LocalStoragePersistenceEffect<TState>(
    IReduxStateStorage<TState> stateStorage,
    ReduxStoreOptions<TState> options) : IReduxStoreEffect<TState>
{
    private readonly CompositeDisposable _subscriptions = [];

    public void Connect(IObservable<IReduxAction> actions, IObservable<TState> states, Action<IReduxAction> dispatch)
    {
        if (!options.EnableLocalStoragePersistence)
        {
            return;
        }

        var persistenceSubscription = states
            .Skip(1)
            .Throttle(options.PersistenceThrottle)
            .Select(state => Observable.FromAsync(cancellationToken => stateStorage.SaveAsync(state, cancellationToken).AsTask()))
            .Switch()
            .Subscribe();

        _subscriptions.Add(persistenceSubscription);
    }

    public void Dispose() => _subscriptions.Dispose();
}
