using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BasicModeViewAllowedAttribute : Attribute
    {
        private bool _value = true;

        public bool IsBasicModeAllowed
        {
            get { return this._value; }
        }

        public BasicModeViewAllowedAttribute(bool value)
        {
            this._value = value;
        }

        public override string ToString()
        {
            if (!this.IsBasicModeAllowed)
            {
                return "Not Allowed BasicMode";
            }

            return "Allowed BasicMode";
        }
    }
}