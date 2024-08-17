using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DisallowSharePointAdapter : Attribute
    {
        private string m_sValue;

        public string Adapter
        {
            get { return this.m_sValue; }
        }

        public DisallowSharePointAdapter(string value)
        {
            this.m_sValue = value;
        }
    }
}