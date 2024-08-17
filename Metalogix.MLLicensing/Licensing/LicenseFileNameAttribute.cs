using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class LicenseFileNameAttribute : Attribute
    {
        private string m_value;

        public string LicenseFileName
        {
            get { return this.m_value; }
        }

        public LicenseFileNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}