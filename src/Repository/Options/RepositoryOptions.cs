using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace eQuantic.Core.Data.MongoDb.Repository.Options;

public class RepositoryOptions
{
    private readonly List<Assembly> _assemblies = new();
    private readonly MongoOptions _mongoOptions = new();
    private ServiceLifetime _lifetime = ServiceLifetime.Transient;

    private static IEnumerable<Assembly> AllAssemblies => AppDomain.CurrentDomain.GetAssemblies();

    public RepositoryOptions FromAllAssemblies()
    {
        _assemblies.AddRange(AllAssemblies);
        return this;
    }

    public RepositoryOptions FromAssemblies(IEnumerable<Assembly> assemblies)
    {
        _assemblies.AddRange(assemblies);
        return this;
    }

    public RepositoryOptions FromAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
        return this;
    }
    
    public RepositoryOptions AddConnectionString(string connectionString)
    {
        _mongoOptions.ConnectionString = connectionString;
        return this;
    }
    
    public RepositoryOptions AddDatabase(string database)
    {
        _mongoOptions.Database = database;
        return this;
    }

    public RepositoryOptions AddLifetime(ServiceLifetime lifetime)
    {
        _lifetime = lifetime;
        return this;
    }

    internal MongoOptions GetMongoOptions() => _mongoOptions;
    internal List<Assembly> GetAssemblies() => _assemblies.Any() ? _assemblies : AllAssemblies.ToList();
    internal ServiceLifetime GetLifetime() => _lifetime;
}