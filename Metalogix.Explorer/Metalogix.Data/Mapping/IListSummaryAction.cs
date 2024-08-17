using System;
using System.Drawing;

namespace Metalogix.Data.Mapping
{
    public interface IListSummaryAction
    {
        System.Drawing.Image Image { get; }

        string Name { get; }

        bool AppliesTo(ListSummaryItem item);

        void RunAction(ListSummaryItem item);
    }
}