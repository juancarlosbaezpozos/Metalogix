using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AdapterSupportsAttribute : Attribute
    {
        private AdapterSupportedFlags _value;

        public AdapterSupportedFlags Value
        {
            get { return this._value; }
        }

        public AdapterSupportsAttribute(AdapterSupportedFlags flags)
        {
            this._value = flags;
        }

        public static AdapterSupportedFlags GetAdapterSupportedFlags(Type type)
        {
            AdapterSupportedFlags value = (AdapterSupportedFlags)0;
            object[] customAttributes = type.GetCustomAttributes(typeof(AdapterSupportsAttribute), true);
            for (int i = 0; i < (int)customAttributes.Length; i++)
            {
                value |= ((AdapterSupportsAttribute)customAttributes[i]).Value;
            }

            return value;
        }
    }
}