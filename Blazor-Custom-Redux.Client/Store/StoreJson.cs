using System.Text.Json;

namespace Blazor_Custom_Redux.Client.Store;

public static class StoreJson
{
    public static JsonSerializerOptions Persistence { get; } = new(JsonSerializerDefaults.Web);

    public static JsonSerializerOptions Debug { get; } = new(Persistence)
    {
        WriteIndented = true
    };
}
