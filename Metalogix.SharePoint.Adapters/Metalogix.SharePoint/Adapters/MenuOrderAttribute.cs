using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MenuOrderAttribute : Attribute
    {
        private int _value;

        public int Value
        {
            get { return this._value; }
        }

        public MenuOrderAttribute(int value)
        {
            this._value = value;
        }

        public static int GetMenuOrder(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(MenuOrderAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return 2147483647;
            }

            return ((MenuOrderAttribute)customAttributes[0]).Value;
        }
    }
}