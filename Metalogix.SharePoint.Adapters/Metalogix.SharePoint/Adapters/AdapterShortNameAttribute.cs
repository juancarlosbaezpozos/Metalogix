using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AdapterShortNameAttribute : Attribute
    {
        private string m_value;

        public string AdapterShortName
        {
            get { return this.m_value; }
        }

        public AdapterShortNameAttribute(string val)
        {
            this.m_value = val;
        }

        public static string GetAdapterShortName(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AdapterShortNameAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return null;
            }

            return ((AdapterShortNameAttribute)customAttributes[0]).AdapterShortName;
        }
    }
}