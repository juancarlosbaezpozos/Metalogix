using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class OldStyleLicenseNameAttribute : Attribute
    {
        private string m_value;

        public string OldStyleLicenseName
        {
            get { return this.m_value; }
        }

        public OldStyleLicenseNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}