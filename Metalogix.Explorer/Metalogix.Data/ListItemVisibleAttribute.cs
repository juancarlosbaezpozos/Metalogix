using System;

namespace Metalogix.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ListItemVisibleAttribute : Attribute
    {
        private readonly bool m_value;

        public bool IsVisible
        {
            get { return this.m_value; }
        }

        public ListItemVisibleAttribute(bool val)
        {
            this.m_value = val;
        }
    }
}