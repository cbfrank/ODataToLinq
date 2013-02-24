namespace ODataToLinq
{
    using System.Collections.Specialized;
    using System.Linq;

    public static class QueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, NameValueCollection queryParams) where T : class
        {
            var parser = new Parser<T>();

            return parser.Parse(source, queryParams);
        }
    }
}