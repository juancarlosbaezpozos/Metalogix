using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IsConnectivityActionAttribute : Attribute
    {
        private bool m_value = true;

        public bool IsConnectivityAction
        {
            get { return this.m_value; }
        }

        public IsConnectivityActionAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.IsConnectivityAction)
            {
                return "Not ConnectivityAction";
            }

            return "IsConnectivityAction";
        }
    }
}