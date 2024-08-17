using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RunAsyncAttribute : Attribute
    {
        private bool m_value = true;

        public bool RunAsync
        {
            get { return this.m_value; }
        }

        public RunAsyncAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.RunAsync)
            {
                return "Do Not RunAsync";
            }

            return "RunAsync";
        }
    }
}