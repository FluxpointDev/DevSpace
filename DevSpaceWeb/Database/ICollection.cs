using DevSpaceWeb.Data.Teams;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace DevSpaceWeb.Database;

public class ICollection<T> where T : IBaseObject
{
    public ICollection(string dbname)
    {
        Collection = _DB.Run.GetCollection<T>(dbname);
    }

    public IMongoCollection<T> Collection;

    public async Task CreateAsync(T value)
    {
        await Collection.InsertOneAsync(value);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        await Collection.DeleteOneAsync(new FilterDefinitionBuilder<T>().Eq(x => (x as IObject).Id, id));
    }

    public async Task DeleteAsync(T value)
    {
        await Collection.DeleteOneAsync(new FilterDefinitionBuilder<T>().Eq(x => (x as IObject).Id, (value as IObject).Id));
    }

    public IFindFluent<T, T> Find(FilterDefinition<T> filter, FindOptions? options = null) => Collection.Find(filter, options);

}

public class ICacheCollection<T> : ICollection<T> where T : IObject
{
    public ICacheCollection(string dbname) : base(dbname)
    {
    }

    public ConcurrentDictionary<ObjectId, T> Cache = new ConcurrentDictionary<ObjectId, T>();
}