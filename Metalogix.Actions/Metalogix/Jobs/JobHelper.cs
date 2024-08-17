using Metalogix.Actions;
using System;
using System.Collections;

namespace Metalogix.Jobs
{
    public static class JobHelper
    {
        public static bool ContainsPausedActions(IEnumerable jobs)
        {
            bool flag;
            IEnumerator enumerator = jobs.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Job current = (Job)enumerator.Current;
                    if (current.Action == null || current.Action.Status != ActionStatus.Paused)
                    {
                        continue;
                    }

                    flag = true;
                    return flag;
                }

                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        public static bool ContainsRunningActions(IEnumerable jobs)
        {
            bool flag;
            IEnumerator enumerator = jobs.GetEnumerator();
            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        Job current = (Job)enumerator.Current;
                        if (current.Action != null)
                        {
                            if (current.Action.Status == ActionStatus.Running)
                            {
                                break;
                            }

                            if (current.Action.Status == ActionStatus.Aborting)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                flag = true;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        public static bool ContainsUnassociatedJobs(IEnumerable jobs)
        {
            bool flag;
            IEnumerator enumerator = jobs.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (((Job)enumerator.Current).Action != null)
                    {
                        continue;
                    }

                    flag = true;
                    return flag;
                }

                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }
    }
}