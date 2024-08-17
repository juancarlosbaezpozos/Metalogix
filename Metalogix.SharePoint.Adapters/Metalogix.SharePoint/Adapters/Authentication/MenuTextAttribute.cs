using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MenuTextAttribute : Attribute
    {
        private string m_sValue;

        public string Text
        {
            get { return this.m_sValue; }
        }

        public MenuTextAttribute(string value)
        {
            this.m_sValue = value;
        }
    }
}