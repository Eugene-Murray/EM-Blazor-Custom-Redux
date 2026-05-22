using Blazor_Custom_Redux.Client.Store;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<BrowserStateStorage>();
builder.Services.AddSingleton<HydrationEffect>();
builder.Services.AddSingleton<PersistenceEffect>();
builder.Services.AddSingleton<IStoreEffect>(serviceProvider => serviceProvider.GetRequiredService<HydrationEffect>());
builder.Services.AddSingleton<IStoreEffect>(serviceProvider => serviceProvider.GetRequiredService<PersistenceEffect>());
builder.Services.AddSingleton<ReduxStore>();

await builder.Build().RunAsync();
