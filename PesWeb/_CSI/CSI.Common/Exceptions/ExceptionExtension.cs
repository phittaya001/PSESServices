using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Common.Exceptions
{
    public static class ExceptionExtension
    {
        public static Exception ExtractDataException(this Exception ex)
        {
            var exType = ex.GetType();
            if (typeof(EntityException).IsAssignableFrom(exType) ||
                typeof(DataException).IsAssignableFrom(exType) ||
                typeof(SqlException).IsAssignableFrom(exType))
            {
                if (ex.InnerException != null)
                    return ExtractDataException(ex.InnerException);
                else
                    return ex;
            }

            return ex;
        }
    }
}
