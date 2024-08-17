using System;
using System.Reflection;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SourceTypeAttribute : Attribute
    {
        private System.Type m_value;

        private bool m_bApplyToSubTypes = true;

        public bool ApplyToSubTypes
        {
            get { return this.m_bApplyToSubTypes; }
        }

        public System.Type Type
        {
            get { return this.m_value; }
        }

        public SourceTypeAttribute(System.Type type_0, bool applyToSubTypes)
        {
            this.m_value = type_0;
            this.m_bApplyToSubTypes = applyToSubTypes;
        }

        public SourceTypeAttribute(System.Type type_0)
        {
            this.m_value = type_0;
        }

        public override string ToString()
        {
            return this.Type.Name;
        }
    }
}