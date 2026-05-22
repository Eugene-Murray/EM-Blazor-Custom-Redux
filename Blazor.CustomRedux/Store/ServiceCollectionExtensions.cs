using Microsoft.Extensions.DependencyInjection;

namespace Blazor.CustomRedux.Store;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomReduxStore<TState, TReducer>(
        this IServiceCollection services,
        Action<ReduxStoreOptions<TState>> configure)
        where TReducer : class, IReduxReducer<TState>
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new ReduxStoreOptions<TState>();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton<TReducer>();
        services.AddSingleton<IReduxReducer<TState>>(serviceProvider => serviceProvider.GetRequiredService<TReducer>());
        services.AddSingleton<IReduxStateStorage<TState>, LocalStorageReduxStateStorage<TState>>();
        services.AddSingleton<LocalStorageHydrationEffect<TState>>();
        services.AddSingleton<LocalStoragePersistenceEffect<TState>>();
        services.AddSingleton<IReduxStoreEffect<TState>>(serviceProvider => serviceProvider.GetRequiredService<LocalStorageHydrationEffect<TState>>());
        services.AddSingleton<IReduxStoreEffect<TState>>(serviceProvider => serviceProvider.GetRequiredService<LocalStoragePersistenceEffect<TState>>());
        services.AddSingleton<ReduxStore<TState>>();

        return services;
    }
}
