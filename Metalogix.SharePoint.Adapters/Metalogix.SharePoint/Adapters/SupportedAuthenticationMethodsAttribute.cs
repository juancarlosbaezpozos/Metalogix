using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class SupportedAuthenticationMethodsAttribute : Attribute
    {
        private Type _value;

        public Type Value
        {
            get { return this._value; }
        }

        public SupportedAuthenticationMethodsAttribute(Type value)
        {
            this._value = value;
        }

        public static Type GetSupportedAuthenticationMethods(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(SupportedAuthenticationMethodsAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return typeof(AuthenticationInitializer);
            }

            return ((SupportedAuthenticationMethodsAttribute)customAttributes[0]).Value;
        }
    }
}