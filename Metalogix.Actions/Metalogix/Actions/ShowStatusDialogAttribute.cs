using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ShowStatusDialogAttribute : Attribute
    {
        private bool m_value;

        public bool ShowStatusDialog
        {
            get { return this.m_value; }
        }

        public ShowStatusDialogAttribute(bool bool_0)
        {
            this.m_value = bool_0;
        }

        public override string ToString()
        {
            if (!this.ShowStatusDialog)
            {
                return "Do Not ShowStatusDialog";
            }

            return "ShowStatusDialog";
        }
    }
}