using System;

namespace Metalogix.Data.Mapping
{
    public interface IListFilter
    {
        string Name { get; }

        bool AppliesTo(object item);

        bool Filter(object item);
    }
}