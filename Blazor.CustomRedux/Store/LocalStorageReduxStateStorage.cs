using System.Text.Json;
using Microsoft.JSInterop;

namespace Blazor.CustomRedux.Store;

public sealed class LocalStorageReduxStateStorage<TState>(
    IJSRuntime jsRuntime,
    ReduxStoreOptions<TState> options) : IReduxStateStorage<TState>
{
    public async Task<TState?> LoadAsync(CancellationToken cancellationToken = default)
    {
        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, options.LocalStorageKey);

        return string.IsNullOrWhiteSpace(json)
            ? default
            : JsonSerializer.Deserialize<TState>(json, options.SerializerOptions);
    }

    public ValueTask SaveAsync(TState state, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(state, options.SerializerOptions);

        return jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, options.LocalStorageKey, json);
    }
}
