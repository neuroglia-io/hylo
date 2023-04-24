using MongoDB.Bson;
using MongoDB.Driver;

namespace Hylo.Providers;

/// <summary>
/// Defines extensions for <see cref="LabelSelector"/>s
/// </summary>
public static class LabelSelectorExtensions
{

    /// <summary>
    /// Converts the <see cref="LabelSelector"/>s to a new <see cref="FilterDefinition{TDocument}"/>
    /// </summary>
    /// <param name="selectors">An <see cref="IEnumerable{T}"/> containing the <see cref="LabelSelector"/>s to convert</param>
    /// <returns>A new <see cref="FilterDefinition{TDocument}"/></returns>
    public static FilterDefinition<BsonDocument> ToMongoQuery(this IEnumerable<LabelSelector> selectors)
    {
        FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
        FilterDefinition<BsonDocument> filter = null!;
        foreach (var selector in selectors)
        {
            var selectorFilter = selector.Operator switch
            {
                LabelSelectionOperator.Contains => filterBuilder.StringIn($"metadata.labels.{selector.Key}", selector.Values!.Select(v => new StringOrRegularExpression(v))),
                LabelSelectionOperator.Equals => filterBuilder.Eq($"metadata.labels.{selector.Key}", selector.Value),
                LabelSelectionOperator.NotContains => filterBuilder.Not(filterBuilder.StringIn($"metadata.labels.{selector.Key}", selector.Values!.Select(v => new StringOrRegularExpression(v)))),
                LabelSelectionOperator.NotEquals => filterBuilder.Not(filterBuilder.Eq($"metadata.labels.{selector.Key}", selector.Value)),
                _ => throw new NotSupportedException($"The specified {nameof(LabelSelectionOperator)} '{selector.Operator}' is not supported")
            };
            filter = filter == null ? selectorFilter : Builders<BsonDocument>.Filter.And(selectorFilter);
        }
        return filter;
    }

}