using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class ProviderTypeAttribute : Attribute
    {
        private LicenseProviderType m_value = LicenseProviderType.Common;

        public LicenseProviderType Type
        {
            get { return this.m_value; }
        }

        public ProviderTypeAttribute(LicenseProviderType val)
        {
            this.m_value = val;
        }
    }
}