using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RunSubActionAsyncAttribute : Attribute
    {
        private bool m_value = true;

        public bool RunSubActionAsync
        {
            get { return this.m_value; }
        }

        public RunSubActionAsyncAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.RunSubActionAsync)
            {
                return "Do Not RunSubActionAsync";
            }

            return "RunSubActionAsync";
        }
    }
}