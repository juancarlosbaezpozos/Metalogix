using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class SupportedAdaptersAttribute : Attribute
    {
        private Type _value;

        public Type Value
        {
            get { return this._value; }
        }

        public SupportedAdaptersAttribute(Type value)
        {
            this._value = value;
        }

        public static Type GetSupportedAdapters(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(SupportedAdaptersAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return typeof(SharePointAdapter);
            }

            return ((SupportedAdaptersAttribute)customAttributes[0]).Value;
        }
    }
}