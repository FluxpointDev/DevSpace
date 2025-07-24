using Radzen;

namespace DevSpaceWeb;

public static class GridOperators
{
    public static FilterOperator[] Text = new FilterOperator[] { FilterOperator.Contains, FilterOperator.Equals, FilterOperator.StartsWith, FilterOperator.EndsWith };
    public static FilterOperator[] Number = new FilterOperator[] { FilterOperator.Equals, FilterOperator.GreaterThan, FilterOperator.GreaterThanOrEquals, FilterOperator.LessThan, FilterOperator.LessThanOrEquals };
    public static FilterOperator[] Date = new FilterOperator[] { FilterOperator.GreaterThan, FilterOperator.GreaterThanOrEquals, FilterOperator.LessThan, FilterOperator.LessThanOrEquals };
}
