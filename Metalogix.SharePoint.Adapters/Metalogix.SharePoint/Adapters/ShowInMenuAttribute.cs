using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ShowInMenuAttribute : Attribute
    {
        private bool _value;

        public bool Value
        {
            get { return this._value; }
        }

        public ShowInMenuAttribute(bool value)
        {
            this._value = value;
        }

        public static bool GetAdapterShowInMenu(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(ShowInMenuAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return false;
            }

            return ((ShowInMenuAttribute)customAttributes[0]).Value;
        }
    }
}