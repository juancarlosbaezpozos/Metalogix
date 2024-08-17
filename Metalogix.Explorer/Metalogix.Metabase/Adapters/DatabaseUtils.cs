using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using System;
using System.Collections;
using System.Data;
using System.Text;

namespace Metalogix.Metabase.Adapters
{
    public static class DatabaseUtils
    {
        public static string BuildWhereClause(ArrayList itemArrayList)
        {
            if (itemArrayList == null || itemArrayList.Count == 0)
            {
                return "''";
            }

            StringBuilder stringBuilder = new StringBuilder(1000);
            foreach (object obj in itemArrayList)
            {
                string str = null;
                DataRow dataRow = obj as DataRow;
                if (dataRow != null)
                {
                    str = (!dataRow.HasVersion(DataRowVersion.Original)
                        ? dataRow[0].ToString()
                        : dataRow[0, DataRowVersion.Original].ToString());
                }

                if (str == null)
                {
                    continue;
                }

                stringBuilder.Append("'");
                stringBuilder.Append(str);
                stringBuilder.Append("'");
                stringBuilder.Append(",");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }

        public static string GetSQLValue(object objRawValue)
        {
            if (objRawValue == null)
            {
                return "''";
            }

            Type type = objRawValue.GetType();
            if (type.Equals(typeof(string)))
            {
                return string.Concat("'", objRawValue.ToString().Replace("'", "''"), "'");
            }

            if (type.Equals(typeof(Url)))
            {
                return string.Concat("'", ((Url)objRawValue).ToString().Replace("'", "''"), "'");
            }

            if (type.Equals(typeof(DateTime)))
            {
                DateTime dateTime = (DateTime)objRawValue;
                return string.Concat("'", dateTime.ToString("s").Replace("'", "''"), "'");
            }

            if (type.Equals(typeof(int)) || type.Equals(typeof(short)) || type.Equals(typeof(long)) ||
                type.Equals(typeof(float)) || type.Equals(typeof(double)) || type.Equals(typeof(uint)) ||
                type.Equals(typeof(ushort)) || type.Equals(typeof(decimal)) || type.Equals(typeof(ulong)))
            {
                return objRawValue.ToString();
            }

            if (!typeof(Type).IsAssignableFrom(type))
            {
                return string.Concat("'", objRawValue.ToString().Replace("'", "''"), "'");
            }

            return string.Concat("'", ((Type)objRawValue).FullName.Replace("'", "''"), "'");
        }

        public static void ValidateColumn(DataColumn column, DataColumn columnSchema)
        {
            string columnName = columnSchema.ColumnName;
            if (column == null)
            {
                throw new MetabaseException(string.Concat("Invalid Project: Missing column ", columnName));
            }

            if (column.DataType != columnSchema.DataType)
            {
                throw new MetabaseException(
                    string.Concat("Invalid Project: Column type for ", columnName, " incorrect"));
            }

            if (columnSchema.MaxLength >= 0 && column.MaxLength != columnSchema.MaxLength)
            {
                throw new MetabaseException(
                    string.Concat("Invalid Project: Column size for ", columnName, " incorrect"));
            }
        }

        public static void ValidateTableSchema(DataTable table, DataColumn[] schema)
        {
            DataColumn[] dataColumnArray = schema;
            for (int i = 0; i < (int)dataColumnArray.Length; i++)
            {
                DataColumn dataColumn = dataColumnArray[i];
                DatabaseUtils.ValidateColumn(table.Columns[dataColumn.ColumnName], dataColumn);
            }
        }
    }
}