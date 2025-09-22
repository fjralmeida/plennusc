using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Mappers.MappersGestao
{
    public static class ReaderExtensions
    {
        public static bool HasColumn(this IDataRecord reader, string columnName)
        {
            if (reader == null || string.IsNullOrEmpty(columnName)) return false;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string GetStringOrNull(this IDataRecord reader, string columnName)
        {
            if (!reader.HasColumn(columnName)) return null;
            int idx = reader.GetOrdinal(columnName);
            return reader.IsDBNull(idx) ? null : reader.GetString(idx);
        }

        public static int GetInt(this IDataRecord reader, string columnName, int defaultValue = 0)
        {
            if (!reader.HasColumn(columnName)) return defaultValue;
            int idx = reader.GetOrdinal(columnName);
            return reader.IsDBNull(idx) ? defaultValue : Convert.ToInt32(reader.GetValue(idx));
        }

        public static int? GetNullableInt(this IDataRecord reader, string columnName)
        {
            if (!reader.HasColumn(columnName)) return null;
            int idx = reader.GetOrdinal(columnName);
            return reader.IsDBNull(idx) ? (int?)null : Convert.ToInt32(reader.GetValue(idx));
        }

        public static DateTime? GetNullableDateTime(this IDataRecord reader, string columnName)
        {
            if (!reader.HasColumn(columnName)) return null;
            int idx = reader.GetOrdinal(columnName);
            return reader.IsDBNull(idx) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(idx));
        }

        public static T GetValueOrDefault<T>(this IDataRecord reader, string columnName, T defaultValue = default)
        {
            if (!reader.HasColumn(columnName)) return defaultValue;
            int idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return defaultValue;
            object val = reader.GetValue(idx);
            return (T)Convert.ChangeType(val, typeof(T));
        }
    }
}