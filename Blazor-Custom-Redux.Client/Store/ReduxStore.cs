using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;

namespace Blazor_Custom_Redux.Client.Store;

public sealed class ReduxStore : IDisposable
{
    private readonly BehaviorSubject<AppState> _stateSubject = new(AppState.CreateDefault());
    private readonly Subject<IStoreAction> _actionSubject = new();
    private readonly IReadOnlyList<IStoreEffect> _effects;
    private ImmutableArray<StoreHistoryEntry> _history = ImmutableArray<StoreHistoryEntry>.Empty;
    private int _sequence;
    private bool _initialized;

    public ReduxStore(IEnumerable<IStoreEffect> effects)
    {
        _effects = effects.ToArray();

        foreach (var effect in _effects)
        {
            effect.Connect(_actionSubject.AsObservable(), _stateSubject.AsObservable(), Dispatch);
        }

        CaptureHistory("@@INIT", _stateSubject.Value);
    }

    public AppState CurrentState => _stateSubject.Value;

    public IReadOnlyList<StoreHistoryEntry> History => _history;

    public string CurrentStateJson => JsonSerializer.Serialize(CurrentState, StoreJson.Debug);

    public IObservable<TSlice> Select<TSlice>(Func<AppState, TSlice> selector) =>
        _stateSubject.Select(selector).DistinctUntilChanged();

    public IObservable<AppState> States => _stateSubject.AsObservable();

    public Task InitializeAsync()
    {
        if (_initialized)
        {
            return Task.CompletedTask;
        }

        _initialized = true;
        Dispatch(new InitializeStoreAction());

        return Task.CompletedTask;
    }

    public void Dispatch(IStoreAction action)
    {
        var nextState = AppReducer.Reduce(_stateSubject.Value, action);

        if (!EqualityComparer<AppState>.Default.Equals(_stateSubject.Value, nextState))
        {
            _stateSubject.OnNext(nextState);
        }

        CaptureHistory(action.GetType().Name, nextState);
        _actionSubject.OnNext(action);
    }

    public void Dispose()
    {
        foreach (var effect in _effects)
        {
            effect.Dispose();
        }

        _actionSubject.Dispose();
        _stateSubject.Dispose();
    }

    private void CaptureHistory(string actionType, AppState state)
    {
        var nextEntry = new StoreHistoryEntry(
            ++_sequence,
            DateTimeOffset.UtcNow,
            actionType,
            state.Todos.Length,
            JsonSerializer.Serialize(state, StoreJson.Debug));

        _history = _history.Insert(0, nextEntry);

        if (_history.Length > 30)
        {
            _history = _history.RemoveRange(30, _history.Length - 30);
        }
    }
}
