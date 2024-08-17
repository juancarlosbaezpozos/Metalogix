using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class ProxyFilePathAttribute : Attribute
    {
        public LicenseStorageLocation ProxyFileLocation { get; private set; }

        public string ProxyFileName { get; private set; }

        public LicenseStorageType ProxyStorageType { get; private set; }

        public ProxyFilePathAttribute(string name, LicenseStorageLocation storageLocation,
            LicenseStorageType storageType)
        {
            this.ProxyFileName = name;
            this.ProxyFileLocation = storageLocation;
            this.ProxyStorageType = storageType;
        }
    }
}