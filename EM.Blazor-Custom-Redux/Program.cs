using EM.Blazor.CustomRedux.Store;
using EM.Blazor_Custom_Redux.Client.Pages;
using EM.Blazor_Custom_Redux.Client.Store;
using EM.Blazor_Custom_Redux.Components;
using EM.Blazor_Custom_Redux.Store;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Blazor Server counter store — registered as Scoped so each SignalR circuit (user session)
// gets its own isolated store instance. No localStorage: WithLocalStoragePersistence is WASM-only.
builder.Services.AddCustomReduxStore<ServerCounterState, ServerCounterReducer>(options =>
{
    options.InitialStateFactory = ServerCounterState.CreateDefault;
}, ServiceLifetime.Scoped);

// Register the client AppState store on the server so SSR prerendering of WASM components
// can resolve ReduxStore<AppState> from the server DI container.
// WithLocalStoragePersistence is WASM-only and intentionally omitted here.
builder.Services.AddCustomReduxStore<AppState, AppReducer>(options =>
{
    options.InitialStateFactory = AppState.CreateDefault;
    options.HydratedStateTransform = state => state with { IsHydrated = true };
}, ServiceLifetime.Scoped);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(EM.Blazor_Custom_Redux.Client._Imports).Assembly);

app.Run();
