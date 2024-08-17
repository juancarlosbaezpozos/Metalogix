using System;
using System.Reflection;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StatusSummaryProviderAttribute : Attribute
    {
        private Type _type;

        public Type ProviderType
        {
            get { return this._type; }
        }

        public StatusSummaryProviderAttribute(Type type)
        {
            this._type = type;
        }

        public static StatusSummaryProviderAttribute GetAttributeFromType(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(StatusSummaryProviderAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return null;
            }

            return (StatusSummaryProviderAttribute)customAttributes[0];
        }

        public override string ToString()
        {
            return string.Format("Status Summary Provider Type: {0}", this.ProviderType.FullName);
        }
    }
}