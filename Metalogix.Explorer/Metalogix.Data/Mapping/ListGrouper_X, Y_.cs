using System;

namespace Metalogix.Data.Mapping
{
    public abstract class ListGrouper<X, Y> : IListGrouper
    {
        protected ListGrouper()
        {
        }

        public virtual bool AppliesTo(object source, object target)
        {
            if (source == null || !(source is X) || target == null)
            {
                return false;
            }

            return target is Y;
        }

        public bool AppliesTo(object item)
        {
            bool flag;
            try
            {
                flag = (item == null || !(item is ListSummaryItem) ? false : this.AppliesTo((ListSummaryItem)item));
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        protected virtual bool AppliesTo(ListSummaryItem item)
        {
            if (item == null || item.Source == null || item.Source.Tag == null || !(item.Source.Tag is X) ||
                item.Target == null || item.Target.Tag == null)
            {
                return false;
            }

            return item.Target.Tag is Y;
        }

        public abstract string Group(X source, Y target);

        public string Group(object source, object target)
        {
            string message;
            try
            {
                message = this.Group((X)source, (Y)target);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public string Group(object item)
        {
            string message;
            try
            {
                ListSummaryItem listSummaryItem = item as ListSummaryItem;
                message = this.Group(listSummaryItem.Source.Tag, listSummaryItem.Target.Tag);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }
    }
}