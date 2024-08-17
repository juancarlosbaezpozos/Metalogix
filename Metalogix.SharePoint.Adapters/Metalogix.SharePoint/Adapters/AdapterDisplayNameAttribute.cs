using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AdapterDisplayNameAttribute : Attribute
    {
        private string _value;

        public string Value
        {
            get { return this._value; }
        }

        public AdapterDisplayNameAttribute(string val)
        {
            this._value = val;
        }

        public static string GetAdapterDisplayName(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AdapterDisplayNameAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return null;
            }

            return ((AdapterDisplayNameAttribute)customAttributes[0]).Value;
        }
    }
}