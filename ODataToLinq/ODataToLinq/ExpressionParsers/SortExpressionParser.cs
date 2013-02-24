namespace ODataToLinq.ExpressionParsers
{
    using System.Linq;
    using System.Linq.Dynamic;

    public static class SortExpressionParser
    {
        public static IQueryable<T> Parse<T>(IQueryable<T> source, string sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return source;
            }

            // Todo : Check what we can do with Descending

            return source.OrderBy(sort);
        }
    }
}