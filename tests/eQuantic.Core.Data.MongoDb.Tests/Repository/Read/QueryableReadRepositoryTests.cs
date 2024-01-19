using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using eQuantic.Core.Data.MongoDb.Repository;
using eQuantic.Core.Data.MongoDb.Repository.Options;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.MongoDb.Repository.Write;
using eQuantic.Core.Data.Repository;
using FluentAssertions;
using Mongo2Go;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Moq;
using Xunit;

namespace eQuantic.Core.Data.MongoDb.Tests.Repository.Read;

public class QueryableReadRepositoryTests
{
    private const string Database = "IntegrationTest";
    
    private readonly Mock<IServiceProvider> _serviceProvider = new Mock<IServiceProvider>();

    static QueryableReadRepositoryTests()
    {
        BsonClassMap.RegisterClassMap<TestDocumentData>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(o => o.Id).SetIdGenerator(CombGuidGenerator.Instance);
        });
    }
    
    [Fact]
    public void QueryableReadRepository_GetFiltered_Successfully()
    {
        using var runner = MongoDbRunner.Start();
        
        var unitOfWork = new UnitOfWork(new MongoOptions
        {
            ConnectionString = runner.ConnectionString,
            Database = Database
        }, _serviceProvider.Object);

        SetInitialData(unitOfWork);
        
        var repository = new QueryableReadRepository<UnitOfWork, TestDocumentData, Guid>(unitOfWork);
        
        var result = repository
            .GetFiltered(o => o.Name == "test1")
            .ToArray();
        
        result.Should().OnlyContain(o => o.Name == "test1");
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public void QueryableReadRepository_GetAll_WithProperties_Successfully()
    {
        using var runner = MongoDbRunner.Start();
        
        var unitOfWork = new UnitOfWork(new MongoOptions
        {
            ConnectionString = runner.ConnectionString,
            Database = Database
        }, _serviceProvider.Object);

        SetInitialData(unitOfWork);
        
        var repository = new QueryableReadRepository<UnitOfWork, TestDocumentData, Guid>(unitOfWork);
        
        var result = repository
            .GetAll(opt =>
            {
                opt.WithProperties(nameof(TestDocumentData.SubDocument));
            })
            .ToArray();
        
        result.Should().HaveCount(4);
    }

    private static void SetInitialData(UnitOfWork unitOfWork)
    {
        
        
        var subDocs = new List<TestSubDocumentData>
        {
            new() { Name = "sub1"},
            new() { Name = "sub2"},
            new() { Name = "sub3"},
            new() { Name = "sub4"}
        };

        InsertMany(unitOfWork, subDocs);
        
        var docs = new List<TestDocumentData>
        {
            new() { Name = "test1"},
            new() { Name = "test2"},
            new() { Name = "test3"},
            new() { Name = "test4"}
        };
        
        InsertMany(unitOfWork, docs, (item, idx) =>
        {
            item.SubDocumentId = subDocs[idx].Id;
        });
    }

    private static void InsertMany<TEntity>(UnitOfWork unitOfWork, IEnumerable<TEntity> items, Action<TEntity, int> action = null) 
        where TEntity : class, IEntity, new()
    {
        var writeRepo = new WriteRepository<UnitOfWork, TEntity, Guid>(unitOfWork);
        var i = 0;
        foreach (var item in items)
        {
            action?.Invoke(item, i);
            writeRepo.Add(item);
            i++;
        }
    }
}

[Table(TableName)]
public class TestDocumentData : IEntity<Guid>
{
    public const string TableName = "TestDocuments";
    
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    [ForeignKey(nameof(SubDocument))]
    public Guid SubDocumentId { get; set; }
    
    public TestSubDocumentData SubDocument { get; set; }
    
    public Guid GetKey()
    {
        return Id;
    }

    public void SetKey(Guid key)
    {
        Id = key;
    }
}

public class TestSubDocumentData : IEntity<Guid>
{
    public const string TableName = "TestSubDocuments";
    
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public Guid GetKey()
    {
        return Id;
    }

    public void SetKey(Guid key)
    {
        Id = key;
    }
}