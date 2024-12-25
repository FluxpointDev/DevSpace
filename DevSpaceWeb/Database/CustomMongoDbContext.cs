using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Utils;
using MongoDbGenericRepository;
using System.Reflection;

namespace AspNetCore.Identity.MongoDbCore;

public class CustomMongoDbContext : IMongoDbContext
{
    //
    // Summary:
    //     The IMongoClient from the official MongoDB driver
    public IMongoClient Client { get; }

    //
    // Summary:
    //     The IMongoDatabase from the official MongoDB driver
    public IMongoDatabase Database { get; }

    //
    // Summary:
    //     The constructor of the MongoDbContext, it needs an object implementing MongoDB.Driver.IMongoDatabase.
    //
    //
    // Parameters:
    //   mongoDatabase:
    //     An object implementing IMongoDatabase
    public CustomMongoDbContext(IMongoDatabase mongoDatabase)
    {
        InitializeGuidRepresentation();
        Database = mongoDatabase;
        Client = Database.Client;
    }

    //
    // Summary:
    //     The constructor of the MongoDbContext, it needs a connection string and a database
    //     name.
    //
    // Parameters:
    //   connectionString:
    //     The connections string.
    //
    //   databaseName:
    //     The name of your database.
    public CustomMongoDbContext(string connectionString, string databaseName)
    {
        InitializeGuidRepresentation();
        Client = new MongoClient(connectionString);
        Database = Client.GetDatabase(databaseName);
    }

    //
    // Summary:
    //     Initialise an instance of a MongoDbGenericRepository.IMongoDbContext using a
    //     connection string
    //
    // Parameters:
    //   connectionString:
    public CustomMongoDbContext(string connectionString)
        : this(connectionString, new MongoUrl(connectionString).DatabaseName)
    {
    }

    //
    // Summary:
    //     The constructor of the MongoDbContext, it needs a connection string and a database
    //     name.
    //
    // Parameters:
    //   client:
    //     The MongoClient.
    //
    //   databaseName:
    //     The name of your database.
    public CustomMongoDbContext(MongoClient client, string databaseName)
    {
        InitializeGuidRepresentation();
        Client = client;
        Database = (client).GetDatabase(databaseName, null);
    }

    //
    // Summary:
    //     Returns a collection for a document type. Also handles document types with a
    //     partition key.
    //
    // Parameters:
    //   partitionKey:
    //     The optional value of the partition key.
    //
    // Type parameters:
    //   TDocument:
    //     The type representing a Document.
    public virtual IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null)
    {
        return Database.GetCollection<TDocument>(GetCollectionName<TDocument>(partitionKey));
    }

    //
    // Summary:
    //     Drops a collection, use very carefully.
    //
    // Type parameters:
    //   TDocument:
    //     The type representing a Document.
    public virtual void DropCollection<TDocument>(string partitionKey = null)
    {
        Database.DropCollection(GetCollectionName<TDocument>(partitionKey));
    }

    //
    // Summary:
    //     Sets the Guid representation of the MongoDB Driver.
    //
    // Parameters:
    //   guidRepresentation:
    //     The new value of the GuidRepresentation
    public virtual void SetGuidRepresentation(GuidRepresentation guidRepresentation)
    {
        
    }

    //
    // Summary:
    //     Extracts the CollectionName attribute from the entity type, if any.
    //
    // Type parameters:
    //   TDocument:
    //     The type representing a Document.
    //
    // Returns:
    //     The name of the collection in which the TDocument is stored.
    protected virtual string GetAttributeCollectionName<TDocument>()
    {
        return (typeof(TDocument).GetTypeInfo().GetCustomAttributes(typeof(CollectionNameAttribute)).FirstOrDefault() as CollectionNameAttribute)?.Name;
    }

    //
    // Summary:
    //     Initialize the Guid representation of the MongoDB Driver. Override this method
    //     to change the default GuidRepresentation.
    protected virtual void InitializeGuidRepresentation()
    {
        SetGuidRepresentation(GuidRepresentation.Standard);
    }

    //
    // Summary:
    //     Given the document type and the partition key, returns the name of the collection
    //     it belongs to.
    //
    // Parameters:
    //   partitionKey:
    //     The value of the partition key.
    //
    // Type parameters:
    //   TDocument:
    //     The type representing a Document.
    //
    // Returns:
    //     The name of the collection.
    protected virtual string GetCollectionName<TDocument>(string partitionKey)
    {
        string text = GetAttributeCollectionName<TDocument>() ?? Pluralize<TDocument>();
        if (string.IsNullOrEmpty(partitionKey))
        {
            return text;
        }

        return partitionKey + "-" + text;
    }

    //
    // Summary:
    //     Very naively pluralizes a TDocument type name.
    //
    // Type parameters:
    //   TDocument:
    //     The type representing a Document.
    //
    // Returns:
    //     The pluralized document name.
    protected virtual string Pluralize<TDocument>()
    {
        return typeof(TDocument).Name.Pluralize().Camelize();
    }
}
