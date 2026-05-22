using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Blazor_Custom_Redux.Client.Store;

public sealed class HydrationEffect(BrowserStateStorage browserStateStorage) : IStoreEffect
{
    private readonly CompositeDisposable _subscriptions = [];

    public void Connect(IObservable<IStoreAction> actions, IObservable<AppState> states, Action<IStoreAction> dispatch)
    {
        var hydrationSubscription = actions
            .OfType<InitializeStoreAction>()
            .Take(1)
            .SelectMany(_ => Observable.FromAsync(async cancellationToken =>
            {
                var persistedState = await browserStateStorage.LoadAsync(cancellationToken);
                var hydratedState = (persistedState ?? AppState.CreateDefault()) with
                {
                    IsHydrated = true
                };

                return (IStoreAction)new HydrateStateAction(hydratedState);
            }))
            .Subscribe(dispatch);

        _subscriptions.Add(hydrationSubscription);
    }

    public void Dispose() => _subscriptions.Dispose();
}
