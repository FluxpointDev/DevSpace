using MongoDB.Driver;
using System.Collections.Concurrent;

namespace DevSpaceWeb.Database;

public class ICollection<T>
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



    public IFindFluent<T, T> Find(FilterDefinition<T> filter, FindOptions? options = null) => Collection.Find<T>(filter, options);

}

public class ICacheCollection<T> : ICollection<T>
{
    public ICacheCollection(string dbname) : base(dbname)
    {
    }

    public ConcurrentDictionary<string, T> Cache = new ConcurrentDictionary<string, T>();
}
