using Metalogix.Licensing.LicenseServer;
using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class LegacyProductAttribute : Attribute
    {
        private Metalogix.Licensing.LicenseServer.Product m_value;

        public Metalogix.Licensing.LicenseServer.Product Product
        {
            get { return this.m_value; }
        }

        public LegacyProductAttribute(Metalogix.Licensing.LicenseServer.Product val)
        {
            this.m_value = val;
        }
    }
}