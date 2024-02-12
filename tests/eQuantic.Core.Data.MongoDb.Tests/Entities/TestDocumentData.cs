using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eQuantic.Core.Data.Repository;

namespace eQuantic.Core.Data.MongoDb.Tests.Entities;

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