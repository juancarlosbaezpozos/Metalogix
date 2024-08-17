using System;

namespace Metalogix.Data.Mapping
{
    public abstract class ListPickerAction : IListPickerAction
    {
        public abstract string Name { get; }

        protected ListPickerAction()
        {
        }

        public virtual bool AppliesTo(ListPickerItem item)
        {
            return item != null;
        }

        public abstract void RunAction(ListPickerItem item);
    }
}