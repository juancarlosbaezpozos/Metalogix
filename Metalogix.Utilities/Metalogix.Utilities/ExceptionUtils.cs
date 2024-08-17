using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Metalogix.Utilities
{
    public static class ExceptionUtils
    {
        private readonly static List<string> ErrorCodeProperties;

        static ExceptionUtils()
        {
            ExceptionUtils.ErrorCodeProperties = new List<string>()
            {
                "ErrorCode",
                "HResult"
            };
        }

        public static IList<int> GetExceptionErrorCodes(Exception exception)
        {
            List<int> nums = new List<int>();
            ExceptionUtils.GetExceptionErrorCodesRecursive(exception, nums);
            return nums;
        }

        private static void GetExceptionErrorCodesRecursive(Exception exception, ICollection<int> errorCodes)
        {
            if (exception == null)
            {
                return;
            }

            foreach (string errorCodeProperty in ExceptionUtils.ErrorCodeProperties)
            {
                PropertyInfo property = exception.GetType().GetProperty(errorCodeProperty);
                object obj = (property == null || !property.CanRead ? null : property.GetValue(exception, null));
                if (obj == null)
                {
                    continue;
                }

                int num = Convert.ToInt32(obj);
                if (errorCodes.Contains(num))
                {
                    continue;
                }

                errorCodes.Add(num);
            }

            if (exception.InnerException != null)
            {
                ExceptionUtils.GetExceptionErrorCodesRecursive(exception.InnerException, errorCodes);
            }
        }

        public static void GetExceptionMessage(Exception exception, StringBuilder message)
        {
            if (exception != null)
            {
                message.AppendLine(exception.Message);
                message.AppendLine(string.Format("Type: {0}", exception.GetType().ToString()));
                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    message.AppendLine(string.Format("Stack: {0}", exception.StackTrace));
                }

                foreach (string errorCodeProperty in ExceptionUtils.ErrorCodeProperties)
                {
                    PropertyInfo property = exception.GetType().GetProperty(errorCodeProperty);
                    object obj = (property == null || !property.CanRead ? null : property.GetValue(exception, null));
                    if (obj == null)
                    {
                        continue;
                    }

                    int num = Convert.ToInt32(obj);
                    message.AppendLine(string.Format("{0}:{1}", errorCodeProperty, num));
                }

                if (exception.Data != null && exception.Data.Count > 0)
                {
                    message.AppendLine("Data Items:");
                    foreach (DictionaryEntry datum in exception.Data)
                    {
                        message.AppendLine(string.Format("{0}={1}", datum.Key, datum.Value));
                    }
                }

                if (exception.InnerException != null)
                {
                    message.AppendLine();
                    message.AppendLine("Inner Exception:");
                    ExceptionUtils.GetExceptionMessage(exception.InnerException, message);
                }
            }
        }

        public static void GetExceptionMessageAndDetail(Exception exception, out string exceptionMessage,
            out string exceptionStackDetails)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder();
            ExceptionUtils.GetExceptionMessageAndDetailRecursively(exception, stringBuilder, stringBuilder1);
            exceptionMessage = stringBuilder.ToString();
            exceptionStackDetails = stringBuilder1.ToString();
        }

        public static ExceptionDetail GetExceptionMessageAndDetail(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder();
            ExceptionUtils.GetExceptionMessageAndDetailRecursively(exception, stringBuilder, stringBuilder1);
            PropertyInfo property = exception.GetType().GetProperty("HResult");
            object obj = (property == null || !property.CanRead ? null : property.GetValue(exception, null));
            PropertyInfo propertyInfo = exception.GetType().GetProperty("ErrorCode");
            object obj1 =
                (propertyInfo == null || !propertyInfo.CanRead ? null : propertyInfo.GetValue(exception, null));
            return new ExceptionDetail(stringBuilder.ToString(), stringBuilder1.ToString(),
                (obj != null ? (int)obj : 0), (obj1 != null ? (int)obj1 : 0));
        }

        private static void GetExceptionMessageAndDetailRecursively(Exception exception, StringBuilder message,
            StringBuilder details)
        {
            if (exception != null)
            {
                message.AppendLine(string.Format("{0}{1}{2}", exception.Message, Environment.NewLine,
                    (details == null
                        ? string.Format("Type: {0}{1}", exception.GetType().ToString(), Environment.NewLine)
                        : string.Empty)));
                if (details != null)
                {
                    if (!string.IsNullOrEmpty(exception.StackTrace))
                    {
                        object[] objArray = new object[]
                            { exception.Message, Environment.NewLine, exception.StackTrace, Environment.NewLine };
                        details.AppendLine(string.Format("Error: {0}{1}Stack: {2}{3}", objArray));
                    }

                    details.AppendLine(string.Format("Type: {0}{1}", exception.GetType().ToString(),
                        Environment.NewLine));
                    bool flag = false;
                    foreach (string errorCodeProperty in ExceptionUtils.ErrorCodeProperties)
                    {
                        PropertyInfo property = exception.GetType().GetProperty(errorCodeProperty);
                        object obj = (property == null || !property.CanRead
                            ? null
                            : property.GetValue(exception, null));
                        if (obj == null)
                        {
                            continue;
                        }

                        int num = Convert.ToInt32(obj);
                        details.AppendLine(string.Format("{0}:{1}", errorCodeProperty, num));
                        flag = true;
                    }

                    if (flag)
                    {
                        details.AppendLine();
                    }

                    if (exception.Data != null && exception.Data.Count > 0)
                    {
                        foreach (DictionaryEntry datum in exception.Data)
                        {
                            object[] key = new object[]
                                { datum.Key, Environment.NewLine, datum.Value, Environment.NewLine };
                            details.AppendLine(string.Format("Data Item '{0}':{1}{2}{3}", key));
                        }
                    }
                }

                if (exception.InnerException != null)
                {
                    message.Append("Inner Exception: ");
                    if (details != null && !string.IsNullOrEmpty(exception.InnerException.StackTrace))
                    {
                        details.Append("Inner Exception ");
                    }

                    ExceptionUtils.GetExceptionMessageAndDetailRecursively(exception.InnerException, message, details);
                }
            }
        }

        public static string GetExceptionMessageOnly(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ExceptionUtils.GetExceptionMessageAndDetailRecursively(exception, stringBuilder, null);
            return stringBuilder.ToString();
        }

        public static Exception GetInnerMostException(Exception exception)
        {
            return exception.GetBaseException();
        }
    }
}