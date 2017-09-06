namespace ODataToLinq.ExpressionParsers
{
    using System.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Linq.Dynamic.Core;

    public static class FilterExpressionParser
    {
        private static readonly NameValueCollection Operations = new NameValueCollection
            {
                { "eq", "=" },
                { "ne", "!=" },
                { "gt", ">" },
                { "ge", ">=" },
                { "lt", "<" },
                { "le", "<=" } 
            };

        private static readonly NameValueCollection BooleanMethods = new NameValueCollection 
            { 
                {"substringof", "Contains"}, 
                {"endswith", "EndsWith"}, 
                {"startswith", "StartsWith"} 
            };

        /// <summary>
        /// Parses the given filter into a Dynamic Linq expression and adds it to the source.
        /// </summary>
        /// <param name="source">The source IQueryable.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An IQueryable with the filter applied to it.</returns>
        public static IQueryable<T> Parse<T>(IQueryable<T> source, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return source;
            }

            var values = new List<object>();
            var convertedFilter = ParseFilter(filter, values);

            return source.Where(convertedFilter, values.ToArray());
        }

        /// <summary>
        /// Parses the filter and adds each expression's value to the values-list
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="values"></param>
        /// <returns>The converted filter</returns>
        private static string ParseFilter(string filter, ICollection<object> values)
        {
            // Samples
            // (FirstName eq 'de vries')
            // (FirstName eq 'Jan's')

            // (startswith(LastName,'de vr') eq true)
            // (startswith(LastName,'Jan's') eq true)

            // (FirstName eq 'B') or (startswith(LastName,'ja') eq true)
            // ((FirstName eq 'B') and (Id eq 10)) or (startswith(LastName,'ja') eq true)


            var convertedFilter = filter;
            foreach (var expression in filter.Split(new[] { "and", "or" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tmpExpression = RemoveParenthesis(expression);

                var convertedExpression = ParseExpression(tmpExpression, values);

                // Maybe this replacing part can be improved.
                // This is now done with replacing because otherwise we lose the Ands and Ors and parenthesis
                convertedFilter = convertedFilter.Replace(tmpExpression, convertedExpression);
            }
            return convertedFilter;
        }

        /// <summary>
        /// Parses the given OData expression and returns a Dynamic Linq expression.
        /// The values list is updated with the values supplied in the OData expression.
        /// </summary>
        /// <param name="expression">The OData expression to parse.</param>
        /// <param name="values">The list of values.</param>
        /// <returns>The OData expression converted to Dynamic Linq expression.</returns>
        private static string ParseExpression(string expression, ICollection<object> values)
        {
            var splitExpression = expression.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (splitExpression.Length == 3)
            {
                var leftPart = splitExpression[0];
                var operation = Operations[splitExpression[1]];
                var rightPart = ConvertValueToObject(splitExpression[2]);

                if (IsMethod(leftPart))
                {
                    leftPart = ParseBooleanMethod(leftPart, values);
                }

                values.Add(rightPart);
                return 
                    string.Format("{0} {1} {2}",
                        leftPart,
                        operation,
                        "@" + (values.Count - 1).ToString(CultureInfo.InvariantCulture)
                    );
            }

            throw new NotSupportedException(string.Format("Expression not supported : {0}", expression));
        }

        /// <summary>
        /// Parse the leftPart method to a Dynamic Linq method
        /// </summary>
        /// <param name="leftPart"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string ParseBooleanMethod(string leftPart, ICollection<object> values)
        {
            // Samples
            // startswith(LastName,'ja')

            var parts = leftPart.Split('(');
            var methodName = BooleanMethods[parts[0]];

            parts[1] = RemoveParenthesis(parts[1]);
            parts = parts[1].Split(',');
            var propertyName = parts[0];
            
            values.Add(ConvertValueToObject(parts[1]));
            return string.Format("{0}.{1}({2})",
                        propertyName,
                        methodName,
                        "@" + (values.Count - 1).ToString(CultureInfo.InvariantCulture)
                );
        }

        /// <summary>
        /// Convert the value to an object.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>The value as the appropriate type.</returns>
        private static object ConvertValueToObject(string value)
        {
            // Boolean values
            if (value.ToLower() == "true" || value.ToLower() == "false")
            {
                return Convert.ToBoolean(value);
            }

            // Integer values
            int integerValue;
            if (int.TryParse(value, out integerValue))
            {
                return integerValue;
            }

            // Decimal values
            float decimalValue;
            if (float.TryParse(value, out decimalValue))
            {
                return decimalValue;
            }

            // String values
            return value.Replace("'", string.Empty);

        }

        /// <summary>
        /// Remove the opening and closing parenthesis from the string.
        /// </summary>
        /// <param name="value">The string to process.</param>
        /// <returns>The result.</returns>
        private static string RemoveParenthesis(string value)
        {
            var result = value.Trim();

            while (result[0] == '(')
            {
                result = result.Substring(1);
            }

            while (result[result.Length - 1] == ')')
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        /// <summary>
        /// Check if the leftPart is a method call
        /// </summary>
        /// <param name="leftPart"></param>
        /// <returns></returns>
        private static bool IsMethod(string leftPart)
        {
            return BooleanMethods.AllKeys.Any(leftPart.StartsWith);
        }
    }
}