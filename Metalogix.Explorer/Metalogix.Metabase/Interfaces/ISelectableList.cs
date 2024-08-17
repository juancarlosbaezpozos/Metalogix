using System;
using System.ComponentModel;

namespace Metalogix.Metabase.Interfaces
{
    public interface ISelectableList
    {
        int CurrentIndex { get; set; }

        object CurrentObject { get; }

        int[] SelectedIndices { get; set; }

        object[] SelectedObjects { get; }

        PropertyDescriptor SelectedProperty { get; set; }

        event EventHandler CurrentIndexChanged;

        event EventHandler SelectedIndicesChanged;
    }
}