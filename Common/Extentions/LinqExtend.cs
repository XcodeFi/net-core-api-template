using Common.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Extentions
{
    public static class LinqExtend
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property, bool isDesc) where T : IRowIndex
        {
            IOrderedQueryable<T> rs;

            if (string.IsNullOrEmpty(property))
            {
                rs = (IOrderedQueryable<T>)source;

                goto GOTO;
            }

            if (isDesc)
            {
                rs = ApplyOrder<T>(source, property, "OrderByDescending");
            }
            else
            {
                rs = ApplyOrder<T>(source, property, "OrderBy");
            }

        GOTO:

            int startRow = 1;
            var tempList = rs.ToList();
            tempList.ForEach(t => t.RowIndex = startRow++);

            return (IOrderedQueryable<T>)(tempList.AsQueryable());
        }

        public static IOrderedQueryable<T> ThenBy<T>(
            this IQueryable<T> source,
            string property,
            bool isDesc)
        {
            if (isDesc)
            {
                return ApplyOrder<T>(source, property, "ThenByDescending");
            }
            else
            {
                return ApplyOrder<T>(source, property, "ThenBy");

            }
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(
            this IQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }


        public static IOrderedQueryable<T> ThenBy<T>(
            this IOrderedQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(
            this IOrderedQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source,
            string property,
            string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(char.ToUpper(prop[0]) + prop.Substring(1));
                if (pi == null) continue;

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}
