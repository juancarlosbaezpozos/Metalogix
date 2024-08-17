using System;
using System.Reflection;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StatusLabelProviderAttribute : Attribute
    {
        private Type _type;

        public Type ProviderType
        {
            get { return this._type; }
        }

        public StatusLabelProviderAttribute(Type type)
        {
            this._type = type;
        }

        public static StatusLabelProviderAttribute GetAttributeFromType(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(StatusLabelProviderAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return null;
            }

            return (StatusLabelProviderAttribute)customAttributes[0];
        }

        public override string ToString()
        {
            return string.Format("Status Label Provider Type: {0}", this.ProviderType.FullName);
        }
    }
}