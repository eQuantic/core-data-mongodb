using System;
using System.ComponentModel.DataAnnotations;
using eQuantic.Core.Data.Repository;

namespace eQuantic.Core.Data.MongoDb.Tests.Entities;

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