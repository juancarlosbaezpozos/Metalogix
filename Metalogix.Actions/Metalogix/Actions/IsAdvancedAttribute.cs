using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IsAdvancedAttribute : Attribute
    {
        private bool m_value;

        public bool IsAdvanced
        {
            get { return this.m_value; }
        }

        public IsAdvancedAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.IsAdvanced)
            {
                return "Is Not Advanced";
            }

            return "Is Advanced";
        }
    }
}