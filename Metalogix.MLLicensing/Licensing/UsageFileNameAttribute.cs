using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class UsageFileNameAttribute : Attribute
    {
        private string m_value;

        public string UsageFileName
        {
            get { return this.m_value; }
        }

        public UsageFileNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}