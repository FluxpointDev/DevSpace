using MongoDB.Driver;

namespace DevSpaceWeb.Database;

public static class DBPaginationFunction
{
    public static async Task<(int totalPages, IReadOnlyList<TDocument> data)> AggregateByPage<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filterDefinition,
        SortDefinition<TDocument> sortDefinition,
        int page,
        int pageSize)
    {
        AggregateFacet<TDocument, AggregateCountResult> countFacet = AggregateFacet.Create("count",
            PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TDocument>()
            }));

        AggregateFacet<TDocument, TDocument> dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<TDocument, TDocument>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Sort(sortDefinition),
                PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
            }));


        List<AggregateFacetResults> aggregation = await collection.Aggregate()
            .Match(filterDefinition)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        long? count = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count;

        int totalPages = (int)Math.Ceiling((double)count / pageSize);

        IReadOnlyList<TDocument> data = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<TDocument>();

        return (totalPages, data);
    }
}
