using Metalogix.DataResolution;
using Metalogix.ObjectResolution;
using System;

namespace Metalogix
{
    public static class DataRepository
    {
        private static Metalogix.DataResolution.DataResolver s_dataResolver;

        private static object s_dataResolverLock;

        private static Metalogix.DataResolution.DataResolver DataResolver
        {
            get
            {
                if (DataRepository.s_dataResolver == null)
                {
                    lock (DataRepository.s_dataResolverLock)
                    {
                        if (DataRepository.s_dataResolver == null)
                        {
                            DataRepository.UpdateDataResolver();
                        }
                    }
                }

                return DataRepository.s_dataResolver;
            }
        }

        static DataRepository()
        {
            DataRepository.s_dataResolverLock = new object();
            ApplicationData.MainAssemblyChanged += new EventHandler(DataRepository.On_MainAssemblyChanged);
        }

        public static void DeletData(string key)
        {
            DataRepository.DataResolver.DeleteDataAtKey(key);
        }

        private static void On_MainAssemblyChanged(object sender, EventArgs args)
        {
            DataRepository.UpdateDataResolver();
        }

        public static byte[] ReadData(string key)
        {
            return DataRepository.DataResolver.GetDataAtKey(key);
        }

        public static string ReadDataAsString(string key)
        {
            return DataRepository.DataResolver.GetStringDataAtKey(key);
        }

        public static void UpdateDataResolver()
        {
            DataRepository.s_dataResolver = (new DataRepositoryLink()).Resolve();
        }

        public static void WriteData(string key, byte[] data)
        {
            DataRepository.DataResolver.WriteDataAtKey(key, data);
        }

        public static void WriteDataAsString(string key, string data)
        {
            DataRepository.DataResolver.WriteStringDataAtKey(key, data);
        }
    }
}