using System;

namespace PreEmptive.SoS.Client.Actions
{
    public class OfflineCachingActions
    {
        public OfflineCachingActions()
        {
        }

        public static void OfflineStorageResultSink(bool success)
        {
            if (!success)
            {
                Environment.Exit(0);
            }
        }
    }
}