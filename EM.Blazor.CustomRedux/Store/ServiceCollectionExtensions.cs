using Microsoft.Extensions.DependencyInjection;

namespace EM.Blazor.CustomRedux.Store;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core Redux store. Safe to use on both Blazor Server and Blazor WebAssembly.
    /// <para>
    /// Use <paramref name="lifetime"/> to control the service lifetime:
    /// <list type="bullet">
    ///   <item><see cref="ServiceLifetime.Singleton"/> (default) — correct for Blazor WebAssembly, where there is one user per process.</item>
    ///   <item><see cref="ServiceLifetime.Scoped"/> — required for Blazor Server, where each SignalR circuit represents one user session.</item>
    /// </list>
    /// </para>
    /// Call <see cref="WithLocalStoragePersistence{TState}"/> afterward to opt into browser localStorage
    /// hydration and persistence (Blazor WebAssembly only).
    /// </summary>
    public static IServiceCollection AddCustomReduxStore<TState, TReducer>(
        this IServiceCollection services,
        Action<ReduxStoreOptions<TState>> configure,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TReducer : class, IReduxReducer<TState>
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new ReduxStoreOptions<TState>();
        configure(options);

        // Options are immutable configuration — always singleton regardless of store lifetime.
        services.AddSingleton(options);
        services.Add(ServiceDescriptor.Describe(typeof(TReducer), typeof(TReducer), lifetime));
        services.Add(new ServiceDescriptor(typeof(IReduxReducer<TState>), sp => sp.GetRequiredService<TReducer>(), lifetime));
        services.Add(ServiceDescriptor.Describe(typeof(ReduxStore<TState>), typeof(ReduxStore<TState>), lifetime));

        return services;
    }

    /// <summary>
    /// Adds browser localStorage hydration and persistence effects to the Redux store.
    /// Call this only from a Blazor WebAssembly project — it depends on <see cref="Microsoft.JSInterop.IJSRuntime"/>
    /// and uses <c>localStorage</c>, which are not meaningful on the server.
    /// </summary>
    public static IServiceCollection WithLocalStoragePersistence<TState>(
        this IServiceCollection services)
    {
        services.AddSingleton<IReduxStateStorage<TState>, LocalStorageReduxStateStorage<TState>>();
        services.AddSingleton<LocalStorageHydrationEffect<TState>>();
        services.AddSingleton<LocalStoragePersistenceEffect<TState>>();
        services.AddSingleton<IReduxStoreEffect<TState>>(sp => sp.GetRequiredService<LocalStorageHydrationEffect<TState>>());
        services.AddSingleton<IReduxStoreEffect<TState>>(sp => sp.GetRequiredService<LocalStoragePersistenceEffect<TState>>());

        return services;
    }
}
