using System;
using System.Reflection;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AllowsSameSourceTarget : Attribute
    {
        private bool _value;

        public bool Value
        {
            get { return this._value; }
        }

        public AllowsSameSourceTarget(bool value)
        {
            this._value = value;
        }

        public static bool? GetAllowSameSourceTarget(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(AllowsSameSourceTarget), true);
            if ((int)customAttributes.Length == 0)
            {
                return null;
            }

            return new bool?(((AllowsSameSourceTarget)customAttributes[0]).Value);
        }

        public override string ToString()
        {
            if (!this.Value)
            {
                return "Do Not Allow SameSourceTarget";
            }

            return "Allow SameSourceTarget";
        }
    }
}