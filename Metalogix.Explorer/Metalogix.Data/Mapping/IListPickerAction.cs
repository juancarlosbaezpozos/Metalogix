using System;

namespace Metalogix.Data.Mapping
{
    public interface IListPickerAction
    {
        string Name { get; }

        bool AppliesTo(ListPickerItem item);

        void RunAction(ListPickerItem item);
    }
}