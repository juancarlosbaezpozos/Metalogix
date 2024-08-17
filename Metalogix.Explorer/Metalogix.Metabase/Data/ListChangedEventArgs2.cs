using System;
using System.ComponentModel;

namespace Metalogix.Metabase.Data
{
    public class ListChangedEventArgs2 : ListChangedEventArgs
    {
        private readonly bool m_bSoftReset;

        private bool m_bInvoked;

        private object m_objItem;

        public bool Invoked
        {
            get { return this.m_bInvoked; }
            set { this.m_bInvoked = value; }
        }

        public object Item
        {
            get { return this.m_objItem; }
        }

        public bool SoftReset
        {
            get { return this.m_bSoftReset; }
        }

        public ListChangedEventArgs2(System.ComponentModel.ListChangedType eventType, int index, int oldIndex,
            object item, bool bSoftReset) : base(eventType, index, oldIndex)
        {
            this.m_objItem = item;
            this.m_bSoftReset = bSoftReset;
        }

        public ListChangedEventArgs2(System.ComponentModel.ListChangedType eventType, int index, int oldIndex,
            object item, bool bSoftReset, bool bInvoked) : base(eventType, index, oldIndex)
        {
            this.m_objItem = item;
            this.m_bSoftReset = bSoftReset;
            this.m_bInvoked = bInvoked;
        }
    }
}