using System;

namespace Metalogix.Data.Mapping
{
    public interface IListGrouper
    {
        bool AppliesTo(object item);

        string Group(object item);
    }
}