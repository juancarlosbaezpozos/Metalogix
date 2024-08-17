using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SupportsThreeStateConfigurationAttribute : Attribute
    {
        private bool m_value = true;

        public bool SupportsThreeStateConfiguration
        {
            get { return this.m_value; }
        }

        public SupportsThreeStateConfigurationAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.SupportsThreeStateConfiguration)
            {
                return "Does Not Support ThreeStateConfiguration";
            }

            return "Supports ThreeStateConfiguration";
        }
    }
}