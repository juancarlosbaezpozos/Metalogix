using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class SecureStorageAttribute : Attribute
    {
        private bool m_value;

        public bool SecureStorageEnabled
        {
            get { return this.m_value; }
        }

        public SecureStorageAttribute(bool val)
        {
            this.m_value = val;
        }
    }
}