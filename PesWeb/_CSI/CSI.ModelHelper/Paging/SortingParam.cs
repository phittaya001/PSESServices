using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSI.ModelHelper.Paging
{
    public class SortingParam
    {
        public string PropertyName { get; set; }

        public SortingOrder Order { get; set; }

        public static string ToOrderStringSQL(params SortingParam[] sorting)
        {
            string sort = "";
            int lastIdx = sorting.Length - 1;
            for (int i = 0; i < sorting.Length; i++)
            {
                SortingParam s = sorting[i];
                sort += s.PropertyName + (s.Order == SortingOrder.Descending ? " desc" : "");
                if (i < lastIdx)
                    sort += ", ";
            }

            return sort;
        }
    }

    public enum SortingOrder
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    };

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SortedByAttribute : Attribute
    {
        public string[] PropertyNames { get; private set; }

        public SortedByAttribute(params string[] properyName)
        {
            PropertyNames = properyName;
        }
    }
}