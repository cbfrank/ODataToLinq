using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataToLinq.ExpressionParsers
{
    using System.Linq;

    public static class SortExpressionParser
    {
        private static readonly MethodInfo OrderByMethodInfo = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);
        private static readonly MethodInfo OrderByDescMethodInfo = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);
        public static IQueryable<T> Parse<T>(IQueryable<T> source, string sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return source;
            }

            
            var orders = sort.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var order in orders)
            {
                var orderParts = order.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var desc = orderParts.Length > 1 && orderParts.Last().Equals("desc", StringComparison.InvariantCultureIgnoreCase);
                var expParam = Expression.Parameter(typeof(T), "entity");
                var expProp = Expression.Property(expParam, typeof(T), orderParts[0]);
                var exp = Expression.Lambda<Func<T,string>>(expProp, expParam);
                if (desc)
                {
                    source = OrderByDescMethodInfo.MakeGenericMethod(typeof(T), expProp.Type).Invoke(null, new object[] {source, exp}) as IQueryable<T>;
                }
                else
                {
                    source = OrderByMethodInfo.MakeGenericMethod(typeof(T), expProp.Type).Invoke(null, new object[] { source, exp }) as IQueryable<T>;
                }
            }
            return source;
        }
    }
}