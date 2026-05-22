using System.Text.Json;

namespace Blazor.CustomRedux.Store;

public sealed class ReduxStoreOptions<TState>
{
    public Func<TState> InitialStateFactory { get; set; } =
        static () => throw new InvalidOperationException("InitialStateFactory must be configured for the Redux store.");

    public Func<TState, TState> HydratedStateTransform { get; set; } = static state => state;

    public string LocalStorageKey { get; set; } = $"{typeof(TState).FullName}.store";

    public bool EnableLocalStoragePersistence { get; set; } = true;

    public TimeSpan PersistenceThrottle { get; set; } = TimeSpan.FromMilliseconds(150);

    public JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web);

    internal JsonSerializerOptions CreateDebugSerializerOptions() => new(SerializerOptions)
    {
        WriteIndented = true
    };
}
