using System.Text.Json;
using Microsoft.JSInterop;

namespace Blazor_Custom_Redux.Client.Store;

public sealed class BrowserStateStorage(IJSRuntime jsRuntime)
{
    private const string StorageKey = "blazor-custom-redux.store";

    public async Task<AppState?> LoadAsync(CancellationToken cancellationToken = default)
    {
        var json = await jsRuntime.InvokeAsync<string?>("blazorCustomReduxStorage.get", cancellationToken, StorageKey);

        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<AppState>(json, StoreJson.Persistence);
    }

    public ValueTask SaveAsync(AppState state, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(state, StoreJson.Persistence);

        return jsRuntime.InvokeVoidAsync("blazorCustomReduxStorage.set", cancellationToken, StorageKey, json);
    }
}
