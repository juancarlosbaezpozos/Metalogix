using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AutoDetectPriorityAttribute : Attribute
    {
        private int _value;

        public int Value
        {
            get { return this._value; }
        }

        public AutoDetectPriorityAttribute(int value)
        {
            this._value = value;
        }

        public static int GetAutoDetectPriority(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AutoDetectPriorityAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return 2147483647;
            }

            return ((AutoDetectPriorityAttribute)customAttributes[0]).Value;
        }
    }
}