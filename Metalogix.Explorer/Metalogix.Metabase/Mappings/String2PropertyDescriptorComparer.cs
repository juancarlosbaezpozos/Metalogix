using Metalogix.Data.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Metalogix.Metabase.Mappings
{
    public class String2PropertyDescriptorComparer : IListPickerComparer, IComparer<ListPickerItem>
    {
        public String2PropertyDescriptorComparer()
        {
        }

        public bool AppliesTo(ListPickerItem source, ListPickerItem target)
        {
            if (source == null || string.IsNullOrEmpty(source.Target) || target == null)
            {
                return false;
            }

            return target.Tag is PropertyDescriptor;
        }

        public int Compare(ListPickerItem x, ListPickerItem y)
        {
            PropertyDescriptor tag = y.Tag as PropertyDescriptor;
            return string.Compare(x.Target, tag.Name, true);
        }
    }
}