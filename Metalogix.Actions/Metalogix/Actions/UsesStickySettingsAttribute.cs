using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UsesStickySettingsAttribute : Attribute
    {
        private bool m_bUsesStickySettings;

        public bool UsesStickySettings
        {
            get { return this.m_bUsesStickySettings; }
        }

        public UsesStickySettingsAttribute(bool bUsesStickySettings)
        {
            this.m_bUsesStickySettings = bUsesStickySettings;
        }

        public override string ToString()
        {
            if (!this.UsesStickySettings)
            {
                return "Do Not Use StickySettings";
            }

            return "Use StickySettings";
        }
    }
}