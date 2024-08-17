using System;

namespace Metalogix.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IsSystemAttribute : Attribute
    {
        private bool m_value;

        public bool IsSystem
        {
            get { return this.m_value; }
        }

        public IsSystemAttribute(bool val)
        {
            this.m_value = val;
        }
    }
}