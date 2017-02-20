using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace CSI.ModelHelper.Paging
{
    public static class LinqOrderbyExtension
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> qry, params SortingParam[] sorting)
        {
            if (null == sorting)
                return qry;
            if (sorting.Length == 0)
                return qry;

            var param = Expression.Parameter(typeof(T), "Obj" + qry.GetHashCode().ToString());

            string function = null;
            foreach (SortingParam s in sorting)
            {
                if (string.IsNullOrEmpty(function))
                    function = s.Order == SortingOrder.Descending ? "OrderByDescending" : "OrderBy";
                else
                    function = s.Order == SortingOrder.Descending ? "ThenByDescending" : "ThenBy";

                MemberExpression property = Expression.Property(param, s.PropertyName);
                var express = Expression.Lambda(property, param);
                var method = Expression.Call(
                    typeof(Queryable), function
                    , new Type[] { typeof(T), property.Type }
                    , qry.Expression, Expression.Quote(express));

                qry = qry.Provider.CreateQuery<T>(method);
            }

            return qry;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> qry, IEnumerable<SortingParam> sorting)
        {
            if (null == sorting)
                return qry;
            return OrderBy<T>(qry, sorting.ToArray());
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, params SortingParam[] sorting)
        {
            return OrderBy<T>(enumerable.AsQueryable(), sorting);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, IEnumerable<SortingParam> sorting)
        {
            if (null == sorting)
                return enumerable;
            return OrderBy<T>(enumerable.AsQueryable(), sorting.ToArray());
        }

        private static IQueryable<T> GetPage<T>(this IQueryable<T> qry, PagingParam paging)
        {
            if (null == paging)
                return qry;
            return qry.Skip<T>(paging.RowFrom - 1).Take(paging.RowTo - paging.RowFrom + 1);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> qry, IPagingCriteria paging)
        {
            if (paging.SortParams == null)
                return qry.OrderBy(a => 1).GetPage(paging.PageParam);

            if (paging.SortParams.Length == 0)
                return qry.OrderBy(a => 1).GetPage(paging.PageParam);

            return qry.OrderBy(paging.SortParams).GetPage(paging.PageParam);
        }
    }
}
