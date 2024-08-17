using System;

namespace Metalogix.Data.Mapping
{
    public abstract class ListGrouper<T> : IListGrouper
    {
        protected ListGrouper()
        {
        }

        public virtual bool AppliesTo(object item)
        {
            if (item == null)
            {
                return false;
            }

            return item is T;
        }

        public abstract string Group(T item);

        public string Group(object item)
        {
            string message;
            try
            {
                message = this.Group((T)item);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }
    }
}