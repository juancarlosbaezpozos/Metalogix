using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : Attribute
    {
        private string m_value;

        public string Name
        {
            get { return this.m_value; }
        }

        public NameAttribute(string string_0)
        {
            this.m_value = string_0;
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }
    }
}