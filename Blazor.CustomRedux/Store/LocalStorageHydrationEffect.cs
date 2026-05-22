using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Blazor.CustomRedux.Store;

public sealed class LocalStorageHydrationEffect<TState>(
    IReduxStateStorage<TState> stateStorage,
    ReduxStoreOptions<TState> options) : IReduxStoreEffect<TState>
{
    private readonly CompositeDisposable _subscriptions = [];

    public void Connect(IObservable<IReduxAction> actions, IObservable<TState> states, Action<IReduxAction> dispatch)
    {
        var hydrationSubscription = actions
            .OfType<InitializeStoreAction>()
            .Take(1)
            .SelectMany(_ => Observable.FromAsync(async cancellationToken =>
            {
                var persistedState = options.EnableLocalStoragePersistence
                    ? await stateStorage.LoadAsync(cancellationToken)
                    : default;

                var state = options.HydratedStateTransform(persistedState ?? options.InitialStateFactory());

                return (IReduxAction)new HydrateStateAction<TState>(state);
            }))
            .Subscribe(dispatch);

        _subscriptions.Add(hydrationSubscription);
    }

    public void Dispose() => _subscriptions.Dispose();
}
