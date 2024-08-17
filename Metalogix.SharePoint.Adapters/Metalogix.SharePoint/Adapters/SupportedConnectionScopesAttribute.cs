using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class SupportedConnectionScopesAttribute : Attribute
    {
        private ConnectionScope _value;

        public ConnectionScope Value
        {
            get { return this._value; }
        }

        public SupportedConnectionScopesAttribute(ConnectionScope value)
        {
            this._value = value;
        }

        public static ConnectionScope GetSupportedConnectionScopes(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(SupportedConnectionScopesAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return ConnectionScope.Site;
            }

            return ((SupportedConnectionScopesAttribute)customAttributes[0]).Value;
        }
    }
}