using System;

namespace Metalogix.Data.Mapping
{
    public abstract class ListPickerAction<T> : IListPickerAction
    {
        public abstract string Name { get; }

        protected ListPickerAction()
        {
        }

        public virtual bool AppliesTo(ListPickerItem item)
        {
            if (item == null || item.Tag == null)
            {
                return false;
            }

            return item.Tag is T;
        }

        public abstract void RunAction(T item);

        public void RunAction(ListPickerItem item)
        {
            try
            {
                this.RunAction((T)item.Tag);
            }
            catch (Exception exception)
            {
            }
        }
    }
}