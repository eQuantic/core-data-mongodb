using System;
using eQuantic.Core.Data.MongoDb.Repository.Options;
using eQuantic.Core.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace eQuantic.Core.Data.MongoDb.Repository.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueryableRepositories<TQueryableUnitOfWork>(this IServiceCollection services)
        where TQueryableUnitOfWork : class, IQueryableUnitOfWork
    {
        return AddQueryableRepositories<TQueryableUnitOfWork>(services, _ => { });
    }

    public static IServiceCollection AddQueryableRepositories<TUnitOfWorkInterface, TUnitOfWorkImpl>(
        this IServiceCollection services)
        where TUnitOfWorkInterface : IQueryableUnitOfWork
        where TUnitOfWorkImpl : class, TUnitOfWorkInterface
    {
        return AddQueryableRepositories<TUnitOfWorkInterface, TUnitOfWorkImpl>(services, _ => {});
    }
    
    public static IServiceCollection AddQueryableRepositories<TUnitOfWorkInterface, TUnitOfWorkImpl>(this IServiceCollection services,
        Action<RepositoryOptions> options)
        where TUnitOfWorkInterface : IQueryableUnitOfWork
        where TUnitOfWorkImpl : class, TUnitOfWorkInterface
    {
        var repoOptions = GetOptions(options);
        var lifetime = repoOptions.GetLifetime();
        
        AddUnitOfWork<TUnitOfWorkInterface, TUnitOfWorkImpl>(services, repoOptions);
        AddGenericRepositories(services, lifetime);

        return services;
    }

    public static IServiceCollection AddQueryableRepositories<TQueryableUnitOfWork>(this IServiceCollection services,
        Action<RepositoryOptions> options)
        where TQueryableUnitOfWork : class, IQueryableUnitOfWork
    {
        var repoOptions = GetOptions(options);
        var lifetime = repoOptions.GetLifetime();
        
        AddUnitOfWork<IQueryableUnitOfWork, TQueryableUnitOfWork>(services, repoOptions);
        AddGenericRepositories(services, lifetime);

        return services;
    }
    
    private static void AddGenericRepositories(IServiceCollection services, ServiceLifetime lifetime)
    {
        services.TryAdd(new ServiceDescriptor(typeof(IQueryableRepository<,,>), typeof(QueryableRepository<,,>), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IAsyncQueryableRepository<,,>), typeof(AsyncQueryableRepository<,,>), lifetime));
    }
    
    private static void AddUnitOfWork<TUnitOfWorkInterface, TUnitOfWorkImpl>(IServiceCollection services, RepositoryOptions options)
        where TUnitOfWorkInterface : IQueryableUnitOfWork
        where TUnitOfWorkImpl : class, IQueryableUnitOfWork
    {
        var lifetime = options.GetLifetime();
        var uowInterfaceType = typeof(TUnitOfWorkInterface);
        var qUowInterfaceType = typeof(IQueryableUnitOfWork);
        
        services.TryAddSingleton(options.GetMongoOptions());
        services.TryAdd(new ServiceDescriptor(uowInterfaceType, typeof(TUnitOfWorkImpl), lifetime));
        
        if(uowInterfaceType != qUowInterfaceType)
            services.TryAdd(new ServiceDescriptor(qUowInterfaceType, sp => sp.GetRequiredService<TUnitOfWorkInterface>(), lifetime));
    }
    
    private static RepositoryOptions GetOptions(Action<RepositoryOptions> options)
    {
        var repoOptions = new RepositoryOptions();
        options.Invoke(repoOptions);
        return repoOptions;
    }
}