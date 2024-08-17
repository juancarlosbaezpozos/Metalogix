using System;
using System.Data;
using System.Data.SqlClient;

namespace Metalogix.Jobs
{
    public static class JobUtils
    {
        public static string[] DateTimeFieldsInJobHistory;

        public static string[] DateTimeFieldsInJobLogItems;

        public static string[] TimeStampFieldInJobLogItems;

        static JobUtils()
        {
            JobUtils.DateTimeFieldsInJobHistory = new string[] { "Created", "Started", "Finished" };
            JobUtils.DateTimeFieldsInJobLogItems = new string[] { "TimeStamp", "FinishedTime" };
            JobUtils.TimeStampFieldInJobLogItems = new string[] { "TimeStamp" };
        }

        public static DataRow ConvertTimeFieldsToLocalTime(DataRow dataRow_0, string[] fields)
        {
            string[] strArrays = fields;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string localTime = strArrays[i];
                object item = dataRow_0[localTime];
                if (!(item is DBNull))
                {
                    dataRow_0[localTime] = ((DateTime)item).ToLocalTime();
                }
            }

            return dataRow_0;
        }

        public static object GetStringValue(string sVal)
        {
            return JobUtils.GetStringValue(sVal, 0);
        }

        public static object GetStringValue(string sVal, int iMaxChars)
        {
            if (sVal == null)
            {
                return DBNull.Value;
            }

            if (iMaxChars == 0)
            {
                return sVal;
            }

            if (sVal.Length <= iMaxChars)
            {
                return sVal;
            }

            return sVal.Substring(0, iMaxChars);
        }

        public static string MaskAdapterContext(string adapterContext)
        {
            string str;
            try
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(adapterContext);
                str = adapterContext.Replace(sqlConnectionStringBuilder.Password, "*****");
            }
            catch
            {
                str = adapterContext;
            }

            return str;
        }
    }
}