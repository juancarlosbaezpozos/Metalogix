using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LaunchAsJobAttribute : Attribute
    {
        private bool m_value = true;

        public bool LaunchAsJob
        {
            get { return this.m_value; }
        }

        public LaunchAsJobAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.LaunchAsJob)
            {
                return "Do Not LaunchAsJob";
            }

            return "LaunchAsJob";
        }
    }
}