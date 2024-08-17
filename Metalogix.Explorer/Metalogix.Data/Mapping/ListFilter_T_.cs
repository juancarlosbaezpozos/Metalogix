using System;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public abstract class ListFilter<T> : IListFilter
    {
        public virtual string Name
        {
            get { return string.Concat(this.GetType().Name, " Filter"); }
        }

        protected ListFilter()
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

        public abstract bool Filter(T item);

        public bool Filter(object item)
        {
            bool flag;
            try
            {
                flag = this.Filter((T)item);
            }
            catch
            {
                flag = false;
            }

            return flag;
        }
    }
}