using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;

namespace EM.Blazor.CustomRedux.Store;

public sealed class ReduxStore<TState> : IDisposable
{
    private readonly BehaviorSubject<TState> _stateSubject;
    private readonly Subject<IReduxAction> _actionSubject = new();
    private readonly IReduxReducer<TState> _reducer;
    private readonly JsonSerializerOptions _debugSerializerOptions;
    private readonly IReadOnlyList<IReduxStoreEffect<TState>> _effects;
    private ImmutableArray<ReduxStoreHistoryEntry> _history = ImmutableArray<ReduxStoreHistoryEntry>.Empty;
    private int _sequence;
    private bool _initialized;

    public ReduxStore(
        IReduxReducer<TState> reducer,
        ReduxStoreOptions<TState> options,
        IEnumerable<IReduxStoreEffect<TState>> effects)
    {
        _reducer = reducer;
        _debugSerializerOptions = options.CreateDebugSerializerOptions();
        _effects = effects.ToArray();
        _stateSubject = new BehaviorSubject<TState>(options.InitialStateFactory());

        foreach (var effect in _effects)
        {
            effect.Connect(_actionSubject.AsObservable(), _stateSubject.AsObservable(), Dispatch);
        }

        CaptureHistory("@@INIT", _stateSubject.Value);
    }

    public TState CurrentState => _stateSubject.Value;

    public IReadOnlyList<ReduxStoreHistoryEntry> History => _history;

    public string CurrentStateJson => JsonSerializer.Serialize(CurrentState, _debugSerializerOptions);

    public IObservable<TState> States => _stateSubject.AsObservable();

    public IObservable<TSlice> Select<TSlice>(Func<TState, TSlice> selector) =>
        _stateSubject.Select(selector).DistinctUntilChanged();

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

    public void Dispatch(IReduxAction action)
    {
        var currentState = _stateSubject.Value;
        var nextState = action is HydrateStateAction<TState> hydrateAction
            ? hydrateAction.State
            : _reducer.Reduce(currentState, action);

        if (!EqualityComparer<TState>.Default.Equals(currentState, nextState))
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

    private void CaptureHistory(string actionType, TState state)
    {
        var nextEntry = new ReduxStoreHistoryEntry(
            ++_sequence,
            DateTimeOffset.UtcNow,
            actionType,
            JsonSerializer.Serialize(state, _debugSerializerOptions));

        _history = _history.Insert(0, nextEntry);

        if (_history.Length > 30)
        {
            _history = _history.RemoveRange(30, _history.Length - 30);
        }
    }
}
