using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsDefaultO365Authenticator : Attribute
    {
        private bool _value;

        public bool Value
        {
            get { return this._value; }
        }

        public IsDefaultO365Authenticator(bool value)
        {
            this._value = value;
        }

        public static bool GetIsDefaulO365Authenticator(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(IsDefaultO365Authenticator), true);
            if ((int)customAttributes.Length == 0)
            {
                return false;
            }

            return ((IsDefaultO365Authenticator)customAttributes[0]).Value;
        }
    }
}