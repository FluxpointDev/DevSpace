using DevSpaceWeb.Components.Layout;
using MongoDB.Driver;
using Radzen;

namespace DevSpaceWeb;

public static class GridFilters
{
    public static void Parse<T>(SessionProvider session, ref List<FilterDefinition<T>> filterList, LoadDataArgs args)
    {
        foreach (FilterDescriptor? i in args.Filters)
        {
            FilterDefinition<T>? firstFilter = null;
            FilterDefinition<T>? secondFilter = null;

            string Prop = i.FilterProperty ?? i.Property;
            if (string.IsNullOrEmpty(Prop))
                continue;

            if (i.FilterValue != null)
            {
                try
                {
                    if ((i.FilterValue as dynamic).Count == 0)
                        continue;
                }
                catch { }

                firstFilter = ParseFilter<T>(session, i, Prop, i.FilterValue);
            }

            if (i.SecondFilterValue != null)
                secondFilter = ParseFilter<T>(session, i, Prop, i.SecondFilterValue);

            if (firstFilter != null && secondFilter != null)
            {
                if (i.LogicalFilterOperator == LogicalFilterOperator.And)
                    filterList.Add(new FilterDefinitionBuilder<T>().And(firstFilter, secondFilter));
                else
                    filterList.Add(new FilterDefinitionBuilder<T>().Or(firstFilter, secondFilter));
            }
            else
            {
                if (firstFilter != null)
                    filterList.Add(firstFilter);

                if (secondFilter != null)
                    filterList.Add(secondFilter);
            }
        }
    }

    private static FilterDefinition<T>? ParseFilter<T>(SessionProvider session, FilterDescriptor filter, string prop, object value)
    {
        if (value is DateTime date)
            value = date.AddMinutes(session.UserDateOffset);

        switch (filter.FilterOperator)
        {
            case FilterOperator.Contains:
                return new FilterDefinitionBuilder<T>().Regex(prop, "(?i)" + System.Text.RegularExpressions.Regex.Escape(value.ToString()));
            case FilterOperator.StartsWith:
                return new FilterDefinitionBuilder<T>().Regex(prop, "(?i)^" + System.Text.RegularExpressions.Regex.Escape(value.ToString()));
            case FilterOperator.EndsWith:
                return new FilterDefinitionBuilder<T>().Regex(prop, "(?i)" + System.Text.RegularExpressions.Regex.Escape(value.ToString()) + "$");
            case FilterOperator.Equals:
                return new FilterDefinitionBuilder<T>().Eq(prop, value.ToString());
            case FilterOperator.GreaterThan:
                return new FilterDefinitionBuilder<T>().Gt(prop, value.ToString());
            case FilterOperator.GreaterThanOrEquals:
                return new FilterDefinitionBuilder<T>().Gte(prop, value.ToString());
            case FilterOperator.LessThan:
                return new FilterDefinitionBuilder<T>().Lt(prop, value.ToString());
            case FilterOperator.LessThanOrEquals:
                return new FilterDefinitionBuilder<T>().Lte(prop, value.ToString());
            case FilterOperator.In:
                {
                    HashSet<FilterDefinition<T>> List = new HashSet<FilterDefinition<T>>();
                    foreach (object? i in (dynamic)value)
                    {
                        List.Add(new FilterDefinitionBuilder<T>().Eq(prop, (int)i));
                    }
                    return new FilterDefinitionBuilder<T>().Or(List);
                }
        }

        return null;
    }

}
