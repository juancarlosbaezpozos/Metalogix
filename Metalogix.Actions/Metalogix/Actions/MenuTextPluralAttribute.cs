using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MenuTextPluralAttribute : Attribute
    {
        private string m_menuText;

        private PluralCondition m_condition;

        public PluralCondition Condition
        {
            get { return this.m_condition; }
        }

        public string MenuText
        {
            get { return this.m_menuText; }
        }

        public MenuTextPluralAttribute(string menuText, PluralCondition condition)
        {
            this.m_menuText = menuText;
            this.m_condition = condition;
        }

        public override string ToString()
        {
            return string.Concat(this.Condition.ToString(), ":", this.MenuText.ToString());
        }
    }
}