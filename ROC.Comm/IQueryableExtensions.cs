using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ROC.Comm
{
    public static class IQueryableExtensions
    {
        #region IQueryable的扩展

        /// <summary>
        ///     根据第三方条件是否为真来决定是否执行指定条件的查询
        /// </summary>
        /// <param name="source"> 要查询的源 </param>
        /// <param name="predicate"> 查询条件 </param>
        /// <param name="condition"> 第三方条件 </param>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <returns> 查询的结果 </returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool condition)
        {
            PublicHelper.CheckArgument(predicate, "predicate");
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        ///     把IQueryable[T]集合按指定属性与排序方式进行排序
        /// </summary>
        /// <param name="source">要排序的数据集</param>
        /// <param name="propertyName">排序属性名</param>
        /// <param name="sortDirection">排序方向</param>
        /// <typeparam name="T">动态类型</typeparam>
        /// <returns>排序后的数据集</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            PublicHelper.CheckArgument(propertyName, "propertyName");
            return QueryableHelper<T>.OrderBy(source, propertyName, sortDirection);
        }

        /// <summary>
        ///     把IQueryable[T]集合按指定属性排序条件进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="sortCondition">列表属性排序条件</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, PropertySortCondition sortCondition)
        {
            PublicHelper.CheckArgument(sortCondition, "sortCondition");
            return source.OrderBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     把IOrderedQueryable[T]集合继续按指定属性排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="propertyName">排序属性名</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            PublicHelper.CheckArgument(propertyName, "propertyName");
            return QueryableHelper<T>.ThenBy(source, propertyName, sortDirection);
        }

        /// <summary>
        ///     把IOrderedQueryable[T]集合继续指定属性排序方式进行排序
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要排序的数据集</param>
        /// <param name="sortCondition">列表属性排序条件</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, PropertySortCondition sortCondition)
        {
            PublicHelper.CheckArgument(sortCondition, "sortCondition");
            return source.ThenBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
        }

        /// <summary>
        ///     从指定IQueryable[T]集合 中查询指定分页条件的子数据集
        /// </summary>
        /// <typeparam name="TEntity">动态实体类型</typeparam>
        /// <typeparam name="TKey">实体主键类型</typeparam>
        /// <param name="source">要查询的数据集</param>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">输出符合条件的总记录数</param>
        /// <param name="sortConditions">排序条件集合</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate,
            out int total,
            string orderName,
            ListSortDirection sortDirec,
            int pageIndex = 1,
            int pageSize = 10) where TEntity : class
        {
            List<PropertySortCondition> condition = new List<PropertySortCondition>();
            if (!string.IsNullOrEmpty(orderName))
            {
                condition.Add(new PropertySortCondition(orderName, sortDirec));
            }
            return Where<TEntity>(source, predicate, out total, pageIndex, pageSize, condition.ToArray());
        }
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate,
            out int total,
            int pageIndex = 1,
            int pageSize = 10,
            PropertySortCondition[] sortConditions = null) where TEntity : class
        {
            PublicHelper.CheckArgument(source, "source");
            PublicHelper.CheckArgument(predicate, "predicate");
            PublicHelper.CheckArgument(pageIndex, "pageIndex");
            PublicHelper.CheckArgument(pageSize, "pageSize");

            total = source.Count(predicate);
            if (sortConditions == null || sortConditions.Length == 0)
            {
            }
            else
            {
                int count = 0;
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (PropertySortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? source.OrderBy(sortCondition.PropertyName, sortCondition.ListSortDirection)
                        : orderSource.ThenBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }
            return source != null
                ? source.Where(predicate).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
        }

        public static IQueryable<T> SelectPage<T>(this IQueryable<T> source, int pageIndex, int pageSize, out int total) 
        {
            total = source.Count();
            return source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        #endregion

        #region 辅助操作类

        private static class QueryableHelper<T>
        {
            // ReSharper disable StaticFieldInGenericType
            private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new ConcurrentDictionary<string, LambdaExpression>();

            internal static IOrderedQueryable<T> OrderBy(IQueryable<T> source, string propertyName, ListSortDirection sortDirection)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);
                return sortDirection == ListSortDirection.Ascending
                    ? Queryable.OrderBy(source, keySelector)
                    : Queryable.OrderByDescending(source, keySelector);
            }

            internal static IOrderedQueryable<T> ThenBy(IOrderedQueryable<T> source, string propertyName, ListSortDirection sortDirection)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);
                return sortDirection == ListSortDirection.Ascending
                    ? Queryable.ThenBy(source, keySelector)
                    : Queryable.ThenByDescending(source, keySelector);
            }

            private static LambdaExpression GetLambdaExpression(string propertyName)
            {
                if (Cache.ContainsKey(propertyName))
                {
                    return Cache[propertyName];
                }
                ParameterExpression param = Expression.Parameter(typeof(T));
                MemberExpression body = Expression.Property(param, propertyName);
                LambdaExpression keySelector = Expression.Lambda(body, param);
                Cache[propertyName] = keySelector;
                return keySelector;
            }
        }

        #endregion
    }
}
