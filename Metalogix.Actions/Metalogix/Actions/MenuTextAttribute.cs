using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MenuTextAttribute : Attribute
    {
        private string m_value;

        public string MenuText
        {
            get { return this.m_value; }
        }

        public MenuTextAttribute(string string_0)
        {
            this.m_value = string_0;
        }

        public override string ToString()
        {
            return this.MenuText.ToString();
        }
    }
}