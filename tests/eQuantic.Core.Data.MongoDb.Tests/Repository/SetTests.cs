using System;
using eQuantic.Core.Data.MongoDb.Repository;
using eQuantic.Core.Data.Repository;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace eQuantic.Core.Data.MongoDb.Tests.Repository
{
    public class SetTests
    {
        [Fact]
        public void Set_Update_Success()
        {
            var collectionMock = new Mock<IMongoCollection<EntityTest>>();

            var databaseMock = new Mock<IMongoDatabase>();
            databaseMock
                .Setup(d => d.GetCollection<EntityTest>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(() => collectionMock.Object);

            var set = new Set<EntityTest>(databaseMock.Object);

            set.Update(o => o.Prop1 == 1, o => new EntityTest
            {
                Prop1 = 1,
                Prop2 = "test",
                Prop6 = new SubEntityTest
                {
                    SubProp1 = 2,
                    SubProp2 = "teste2"
                }
            });

            var entityTest = new EntityTest
            {
                Prop1 = 1,
                Prop2 = "test",
                Prop6 = new SubEntityTest
                {
                    SubProp1 = 2,
                    SubProp2 = "teste2"
                }
            };

            set.Update(o => o.Prop1 == 1, o => entityTest);
        }

        public class EntityTest : IEntity
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public DateTime Prop3 { get; set; }
            public decimal Prop4 { get; set; }
            public int Prop5 { get; set; }
            public SubEntityTest Prop6 { get; set; }
        }

        public class SubEntityTest
        {
            public int SubProp1 { get; set; }
            public string SubProp2 { get; set; }
        }
    }
}