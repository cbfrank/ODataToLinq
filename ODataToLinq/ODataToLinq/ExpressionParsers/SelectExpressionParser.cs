namespace ODataToLinq.ExpressionParsers
{
    using System.Linq;

    public static class SelectExpressionParser
    {
        public static IQueryable<T> Parse<T>(IQueryable<T> source, string select)
        {
            if (string.IsNullOrWhiteSpace(select))
            {
                return source;
            }

            // Todo : add the actual select expression

            return source;
        }
    }
}