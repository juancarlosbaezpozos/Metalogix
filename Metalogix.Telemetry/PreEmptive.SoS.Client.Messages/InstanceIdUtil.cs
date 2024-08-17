using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace PreEmptive.SoS.Client.Messages
{
    public class InstanceIdUtil
    {
        private const string instanceIdName = "PreEmptive.RuntimeIntelligence.InstanceId";

        private static string instanceId;

        public InstanceIdUtil()
        {
        }

        public static void ClearInstanceId()
        {
            InstanceIdUtil.instanceId = null;
            InstanceIdUtil.DeleteInstanceId();
        }

        private static void DeleteInstanceId()
        {
            try
            {
                IsolatedStorageFile store =
                    IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                if ((int)store.GetFileNames("PreEmptive.RuntimeIntelligence.InstanceId").Length > 0)
                {
                    store.DeleteFile("PreEmptive.RuntimeIntelligence.InstanceId");
                }
            }
            catch (Exception exception)
            {
            }
        }

        public static string GetInstanceId()
        {
            if (InstanceIdUtil.instanceId == null)
            {
                InstanceIdUtil.instanceId = InstanceIdUtil.GetOrCreateInstanceId();
            }

            return InstanceIdUtil.instanceId;
        }

        private static string GetOrCreateInstanceId()
        {
            string str;
            try
            {
                IsolatedStorageFile store =
                    IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                if ((int)store.GetFileNames("PreEmptive.RuntimeIntelligence.InstanceId").Length <= 0)
                {
                    str = Guid.NewGuid().ToString();
                    InstanceIdUtil.WriteInstanceId(store, str);
                }
                else
                {
                    str = InstanceIdUtil.ReadInstanceId(store);
                }
            }
            catch (Exception exception)
            {
                str = null;
            }

            return str;
        }

        public static bool IsInstanceIdInIsolatedStorage()
        {
            bool flag = false;
            try
            {
                if ((int)IsolatedStorageFile
                        .GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null)
                        .GetFileNames("PreEmptive.RuntimeIntelligence.InstanceId").Length > 0)
                {
                    flag = true;
                }
            }
            catch (Exception exception)
            {
            }

            return flag;
        }

        private static string ReadInstanceId(IsolatedStorageFile isoStore)
        {
            string str = null;
            IsolatedStorageFileStream isolatedStorageFileStream = null;
            StreamReader streamReader = null;
            try
            {
                try
                {
                    isolatedStorageFileStream =
                        new IsolatedStorageFileStream("PreEmptive.RuntimeIntelligence.InstanceId", FileMode.Open,
                            isoStore);
                    streamReader = new StreamReader(isolatedStorageFileStream);
                    str = streamReader.ReadLine();
                }
                catch (Exception exception)
                {
                }
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }

                if (isolatedStorageFileStream != null)
                {
                    isolatedStorageFileStream.Close();
                }
            }

            return str;
        }

        private static void WriteInstanceId(IsolatedStorageFile isoStore, string string_0)
        {
            IsolatedStorageFileStream isolatedStorageFileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                try
                {
                    isolatedStorageFileStream =
                        new IsolatedStorageFileStream("PreEmptive.RuntimeIntelligence.InstanceId", FileMode.Create,
                            isoStore);
                    streamWriter = new StreamWriter(isolatedStorageFileStream);
                    streamWriter.WriteLine(string_0);
                }
                catch (Exception exception)
                {
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }

                if (isolatedStorageFileStream != null)
                {
                    isolatedStorageFileStream.Close();
                }
            }
        }
    }
}