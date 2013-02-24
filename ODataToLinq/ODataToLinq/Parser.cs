namespace ODataToLinq
{
    using ExpressionParsers;
    using System.Collections.Specialized;
    using System.Linq;

    public class Parser<T> where T : class
    {
        internal const string OrderByParameter = "$orderby";
        internal const string SelectParameter = "$select";
        internal const string FilterParameter = "$filter";
        internal const string SkipParameter = "$skip";
        internal const string TopParameter = "$top";

        public IQueryable<T> Parse(IQueryable<T> source, NameValueCollection queryParams)
        {
            if (queryParams.Count == 0)
            {
                return source;
            }

            source = FilterExpressionParser.Parse(source, queryParams[FilterParameter]);
            source = SortExpressionParser.Parse(source, queryParams[OrderByParameter]);
            source = SelectExpressionParser.Parse(source, queryParams[SelectParameter]);

            return source;
        }
    }
}