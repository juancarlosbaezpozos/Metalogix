using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ShortcutAttribute : Attribute
    {
        private ShortcutAction m_shortcut;

        private bool m_bUse;

        public ShortcutAction Shortcut
        {
            get { return this.m_shortcut; }
        }

        public bool UseShortcut
        {
            get { return this.m_bUse; }
        }

        public ShortcutAttribute(bool useShortcut, ShortcutAction shortcut)
        {
            this.m_shortcut = shortcut;
            this.m_bUse = useShortcut;
        }

        public ShortcutAttribute(ShortcutAction shortcut)
        {
            this.m_bUse = true;
            this.m_shortcut = shortcut;
        }

        public override string ToString()
        {
            return this.Shortcut.ToString();
        }
    }
}