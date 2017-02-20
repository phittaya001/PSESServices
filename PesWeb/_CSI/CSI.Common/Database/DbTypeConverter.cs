using System;
using System.Collections.Generic;
using System.Data;

namespace CSI.Common.Database
{
    public sealed class DbTypeConvertor
    {
        private struct DbTypeMapEntry
        {
            public Type Type;
            public DbType DbType;
            public SqlDbType SqlDbType;
            public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
            {
                this.Type = type;
                this.DbType = dbType;
                this.SqlDbType = sqlDbType;
            }
        };
        private static List<DbTypeMapEntry> _DbTypeList;
        #region Constructors
        static DbTypeConvertor()
        {
            _DbTypeList = new List<DbTypeMapEntry>();

            DbTypeMapEntry dbTypeMapEntry = new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit);
            _DbTypeList.Add(dbTypeMapEntry);
            dbTypeMapEntry = new DbTypeMapEntry(typeof(byte), DbType.Double, SqlDbType.TinyInt);
            _DbTypeList.Add(dbTypeMapEntry);
            dbTypeMapEntry = new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Image);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(Decimal), DbType.Decimal, SqlDbType.Decimal);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(Int16), DbType.Int16, SqlDbType.SmallInt);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(Int32), DbType.Int32, SqlDbType.Int);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(Int64), DbType.Int64, SqlDbType.BigInt);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant);
            _DbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry = new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar);
            _DbTypeList.Add(dbTypeMapEntry);
        }

        private DbTypeConvertor()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert db type to .Net data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static Type ToNetType(DbType dbType)
        {
            DbTypeMapEntry entry = Find(dbType);
            return entry.Type;
        }
        /// <summary>
        /// Convert TSQL type to .Net data type
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static Type ToNetType(SqlDbType sqlDbType)
        {
            DbTypeMapEntry entry = Find(sqlDbType);
            return entry.Type;
        }

        /// <summary>
        /// Convert .Net type to Db type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(Type type)
        {
            DbTypeMapEntry entry = Find(type);
            return entry.DbType;
        }

        /// <summary>
        /// Convert TSQL data type to DbType
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static DbType ToDbType(SqlDbType sqlDbType)
        {
            DbTypeMapEntry entry = Find(sqlDbType);
            return entry.DbType;
        }

        /// <summary>
        /// Convert .Net type to TSQL data type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(Type type)
        {
            DbTypeMapEntry entry = Find(type);
            return entry.SqlDbType;
        }

        /// <summary>
        /// Convert DbType type to TSQL data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(DbType dbType)
        {
            DbTypeMapEntry entry = Find(dbType);
            return entry.SqlDbType;
        }

        private static DbTypeMapEntry Find(Type type)
        {
            object retObj = null;
            foreach (DbTypeMapEntry entry in _DbTypeList)
            {
                if (entry.Type == type)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
                throw new ApplicationException("Referenced an unsupported Type");

            return (DbTypeMapEntry)retObj;
        }

        private static DbTypeMapEntry Find(DbType dbType)
        {
            object retObj = null;
            foreach (DbTypeMapEntry entry in _DbTypeList)
            {
                if (entry.DbType == dbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
                throw new ApplicationException("Referenced an unsupported DbType");

            return (DbTypeMapEntry)retObj;
        }

        private static DbTypeMapEntry Find(SqlDbType sqlDbType)
        {
            object retObj = null;
            foreach (DbTypeMapEntry entry in _DbTypeList)
            {
                if (entry.SqlDbType == sqlDbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
                throw new ApplicationException("Referenced an unsupported SqlDbType");

            return (DbTypeMapEntry)retObj;
        }
        #endregion

        public static DbType TypeToDbType(Type t)
        {
            DbType dbt;
            try
            {
                dbt = (DbType)Enum.Parse(typeof(DbType), t.Name);
            }
            catch
            {
                dbt = DbType.Object;
            }

            return dbt;
        }
    }

    public static class ParseExtension
    {
        public static int Parse(this int self, object s, int defaultValue)
        {
            if (null == s)
                return (self = defaultValue);

            int x = 0;
            if (false == int.TryParse(s.ToString(), out x))
                return (self = defaultValue);
            return (self = x);
        }

        public static double Parse(this double self, object s, double defaultValue)
        {
            if (null == s)
                return (self = defaultValue);

            double x = 0;
            if (false == double.TryParse(s.ToString(), out x))
                return (self = defaultValue);
            return (self = x);
        }
    }
}
