using Metalogix.DataResolution;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix
{
    public class ResourceTable
    {
        private readonly DataResolver _innerResolver;

        public static ThreadSafeDictionary<ResourceTableLink, ResourceTable> CachedResourceTables { get; private set; }

        static ResourceTable()
        {
            ResourceTable.CachedResourceTables = new ThreadSafeDictionary<ResourceTableLink, ResourceTable>();
            ApplicationData.MainAssemblyChanged += new EventHandler(ResourceTable.ApplicationData_MainAssemblyChanged);
        }

        public ResourceTable(DataResolver resourceResolver)
        {
            this._innerResolver = resourceResolver;
        }

        private static void ApplicationData_MainAssemblyChanged(object sender, EventArgs e)
        {
            ResourceTable.CachedResourceTables.Clear();
        }

        public void DeleteResource(string key)
        {
            this._innerResolver.DeleteDataAtKey(key);
        }

        public IEnumerable<string> GetAvailableResources()
        {
            return this._innerResolver.GetAvailableDataKeys();
        }

        public string GetResource(string key)
        {
            return this._innerResolver.GetStringDataAtKey(key);
        }

        public void WriteResource(string key, string data)
        {
            this._innerResolver.WriteStringDataAtKey(key, data);
        }
    }
}