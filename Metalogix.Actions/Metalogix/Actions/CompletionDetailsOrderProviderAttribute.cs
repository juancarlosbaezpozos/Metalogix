using System;
using System.Reflection;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CompletionDetailsOrderProviderAttribute : Attribute
    {
        private Type _type;

        public Type ProviderType
        {
            get { return this._type; }
        }

        public CompletionDetailsOrderProviderAttribute(Type type)
        {
            this._type = type;
        }

        public static CompletionDetailsOrderProviderAttribute GetAttributeFromType(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(CompletionDetailsOrderProviderAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return null;
            }

            return (CompletionDetailsOrderProviderAttribute)customAttributes[0];
        }

        public override string ToString()
        {
            return string.Format("Completion Details Order Provider Type: {0}", this.ProviderType.FullName);
        }
    }
}