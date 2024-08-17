using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class ProductNameAttribute : Attribute
    {
        private string m_value;

        public string ProductName
        {
            get { return this.m_value; }
        }

        public ProductNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}