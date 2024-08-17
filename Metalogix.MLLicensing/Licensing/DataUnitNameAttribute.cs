using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class DataUnitNameAttribute : Attribute
    {
        private string m_value;

        public string DataUnitName
        {
            get { return this.m_value; }
        }

        public DataUnitNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}