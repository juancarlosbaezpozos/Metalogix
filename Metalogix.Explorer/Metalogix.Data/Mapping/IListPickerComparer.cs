using System;
using System.Collections.Generic;

namespace Metalogix.Data.Mapping
{
    public interface IListPickerComparer : IComparer<ListPickerItem>
    {
        bool AppliesTo(ListPickerItem source, ListPickerItem target);
    }
}