using Blazor.CustomRedux.Store;
using Blazor_Custom_Redux.Client.Store;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCustomReduxStore<AppState, AppReducer>(options =>
{
    options.InitialStateFactory = AppState.CreateDefault;
    options.HydratedStateTransform = state => state with { IsHydrated = true };
    options.LocalStorageKey = "blazor-custom-redux.todo-store";
});

await builder.Build().RunAsync();
