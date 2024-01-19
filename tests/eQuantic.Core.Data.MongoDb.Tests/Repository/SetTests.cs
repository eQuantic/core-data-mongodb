using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.MongoDb.Repository;
using eQuantic.Core.Data.Repository;
using FluentAssertions;
using Mongo2Go;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Moq;
using Xunit;

namespace eQuantic.Core.Data.MongoDb.Tests.Repository;

public class SetTests
{
    private const string Database = "IntegrationTest";
    
    [Fact]
    public async Task Set_UpdateAsync_Success()
    {
        using var runner = MongoDbRunner.Start();

        var client = new MongoClient(runner.ConnectionString);
        var database = client.GetDatabase(Database);
        var set = new Set<EntityTest>(database);
        var insertedEntity = new EntityTest
        {
            Prop1 = 1
        };
        await set.InsertAsync(insertedEntity);
        
        await set.UpdateAsync(o => o.Id == insertedEntity.Id, () => new EntityTest
        {
            Prop1 = 1,
            Prop2 = "test",
            Prop6 = new SubEntityTest
            {
                SubProp1 = 2,
                SubProp2 = "teste2"
            }
        });
        
        var entity = await set.Where(o => o.Id == insertedEntity.Id).FirstAsync();

        entity.Should().NotBeNull();
        entity.Prop2.Should().Be("test");
    }

    public class EntityTest : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public DateTime Prop3 { get; set; }
        public decimal Prop4 { get; set; }
        public int Prop5 { get; set; }
        public SubEntityTest Prop6 { get; set; } = new();
    }

    public class SubEntityTest
    {
        public int SubProp1 { get; set; }
        public string SubProp2 { get; set; }
    }
}