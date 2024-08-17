using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ShowInMenusAttribute : Attribute
    {
        private bool m_value = true;

        public bool ShowInMenus
        {
            get { return this.m_value; }
        }

        public ShowInMenusAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.ShowInMenus)
            {
                return "Do Not ShowInMenus";
            }

            return "ShowInMenus";
        }
    }
}