using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class LicenseFileStorageAttribute : Attribute
    {
        private LicenseStorageLocation m_value;

        public LicenseStorageLocation LicensePath
        {
            get { return this.m_value; }
        }

        public LicenseFileStorageAttribute(LicenseStorageLocation val)
        {
            this.m_value = val;
        }
    }
}