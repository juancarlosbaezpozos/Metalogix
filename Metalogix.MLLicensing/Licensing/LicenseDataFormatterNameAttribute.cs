using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class LicenseDataFormatterNameAttribute : Attribute
    {
        private string m_value;

        public string LicenseDataFormatterName
        {
            get { return this.m_value; }
        }

        public LicenseDataFormatterNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}