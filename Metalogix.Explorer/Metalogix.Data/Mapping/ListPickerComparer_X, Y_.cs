using System;
using System.Collections.Generic;

namespace Metalogix.Data.Mapping
{
    public abstract class ListPickerComparer<X, Y> : IListPickerComparer, IComparer<ListPickerItem>
    {
        protected ListPickerComparer()
        {
        }

        public virtual bool AppliesTo(ListPickerItem source, ListPickerItem target)
        {
            if (source == null || source.Tag == null || !(source.Tag is X) || target == null || target.Tag == null)
            {
                return false;
            }

            return target.Tag is Y;
        }

        public abstract int Compare(X source, Y target);

        public int Compare(ListPickerItem x, ListPickerItem y)
        {
            int num;
            try
            {
                if (x == null || y == null)
                {
                    num = -1;
                }
                else
                {
                    num = (x.Tag == null || y.Tag == null ? -1 : this.Compare((X)x.Tag, (Y)y.Tag));
                }
            }
            catch
            {
                num = -1;
            }

            return num;
        }
    }
}