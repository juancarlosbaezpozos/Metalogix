using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequiresWriteAccessAttribute : Attribute
    {
        private bool m_value = true;

        public bool RequiresWriteAccess
        {
            get { return this.m_value; }
        }

        public RequiresWriteAccessAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.RequiresWriteAccess)
            {
                return "Does Not RequiresWriteAccess";
            }

            return "RequiresWriteAccess";
        }
    }
}