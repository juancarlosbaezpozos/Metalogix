using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Metalogix.Core
{
    public static class ExecuteWithRetry
    {
        public const int DefaultThreadSleepDelay = 1000;

        public const int NumberOfRetries = 20;

        public readonly static List<int> ValidRetryCodes;

        public readonly static List<string> ValidRetryWords;

        static ExecuteWithRetry()
        {
            List<int> nums = new List<int>()
            {
                -2147217873,
                -2147467259,
                -2130246376,
                -2130246262,
                -2130242547,
                -2147022884,
                -2146232060
            };
            ExecuteWithRetry.ValidRetryCodes = nums;
            List<string> strs = new List<string>()
            {
                "timeout",
                "try again",
                "cannot complete",
                "deadlock",
                "Rerun",
                "unexpected error",
                "manipulating",
                "navigational structure",
                "HandleComException",
                "UpdateMembers"
            };
            ExecuteWithRetry.ValidRetryWords = strs;
        }

        public static T AttemptToExecuteWithRetry<T>(Func<T> codeBlockToRetry, StringBuilder logTrace = null)
        {
            bool flag;
            T t = default(T);
            try
            {
                if (logTrace != null)
                {
                    logTrace.AppendLine("----------------- AttemptToExecuteWithRetry<T> -----------------");
                }

                int num = 1000;
                int num1 = 1;
                while (num1 <= 20)
                {
                    try
                    {
                        t = codeBlockToRetry();
                        flag = true;
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        flag = false;
                        IList<int> exceptionErrorCodes = ExceptionUtils.GetExceptionErrorCodes(exception);
                        if (num1 != 20)
                        {
                            IList<int> nums = exceptionErrorCodes;
                            if (ExecuteWithRetry.ValidRetryCodes.Any<int>(new Func<int, bool>(nums.Contains)) ||
                                ExecuteWithRetry.ValidRetryWords.Any<string>(
                                    new Func<string, bool>(exception.Message.Contains)) ||
                                ExecuteWithRetry.ValidRetryWords.Any<string>(
                                    new Func<string, bool>(exception.StackTrace.Contains)))
                            {
                                ExecuteWithRetry.LogRetry(logTrace, num1, num, exception);
                                Thread.Sleep(num);
                                num = 1000 * (num1 + 1);
                                goto Label0;
                            }
                        }

                        ExecuteWithRetry.LogThrow(logTrace);
                        throw;
                    }

                    Label0:
                    if (!flag)
                    {
                        num1++;
                    }
                    else
                    {
                        ExecuteWithRetry.LogSuccess(logTrace);
                        break;
                    }
                }
            }
            finally
            {
                ExecuteWithRetry.LogEnd(logTrace);
            }

            return t;
        }

        public static void AttemptToExecuteWithRetry(Action codeBlockToRetry, StringBuilder logTrace = null)
        {
            bool flag;
            try
            {
                if (logTrace != null)
                {
                    logTrace.AppendLine("------------------ AttemptToExecuteWithRetry -------------------");
                }

                int num = 1000;
                int num1 = 1;
                while (num1 <= 20)
                {
                    try
                    {
                        codeBlockToRetry();
                        flag = true;
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        flag = false;
                        IList<int> exceptionErrorCodes = ExceptionUtils.GetExceptionErrorCodes(exception);
                        if (num1 != 20)
                        {
                            IList<int> nums = exceptionErrorCodes;
                            if (ExecuteWithRetry.ValidRetryCodes.Any<int>(new Func<int, bool>(nums.Contains)) ||
                                ExecuteWithRetry.ValidRetryWords.Any<string>(
                                    new Func<string, bool>(exception.Message.Contains)) ||
                                ExecuteWithRetry.ValidRetryWords.Any<string>(
                                    new Func<string, bool>(exception.StackTrace.Contains)))
                            {
                                ExecuteWithRetry.LogRetry(logTrace, num1, num, exception);
                                Thread.Sleep(num);
                                num = 1000 * (num1 + 1);
                                goto Label0;
                            }
                        }

                        ExecuteWithRetry.LogThrow(logTrace);
                        throw;
                    }

                    Label0:
                    if (!flag)
                    {
                        num1++;
                    }
                    else
                    {
                        ExecuteWithRetry.LogSuccess(logTrace);
                        break;
                    }
                }
            }
            finally
            {
                ExecuteWithRetry.LogEnd(logTrace);
            }
        }

        private static void LogEnd(StringBuilder logTrace)
        {
            if (logTrace != null)
            {
                logTrace.AppendLine("----------------------------------------------------------------");
            }
        }

        private static void LogRetry(StringBuilder logTrace, int retryCount, int retryDelay, Exception ex)
        {
            if (logTrace == null)
            {
                return;
            }

            logTrace.AppendLine();
            logTrace.AppendLine(string.Format("RETRY Attempt:{0} Thread.ManagedThreadId:{1} sleeping:{2} ms",
                retryCount, Thread.CurrentThread.ManagedThreadId, retryDelay));
            logTrace.AppendLine(string.Format("Exception: {0}", ex.Message));
            logTrace.AppendLine(ex.StackTrace);
        }

        private static void LogSuccess(StringBuilder logTrace)
        {
            if (logTrace != null)
            {
                logTrace.AppendLine();
                logTrace.AppendLine("SUCCESS");
            }
        }

        private static void LogThrow(StringBuilder logTrace)
        {
            if (logTrace != null)
            {
                logTrace.AppendLine();
                logTrace.AppendLine("throw; [exception]");
            }
        }
    }
}