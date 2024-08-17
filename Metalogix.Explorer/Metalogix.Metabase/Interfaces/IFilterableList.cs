using Metalogix.Metabase.Data;
using System;

namespace Metalogix.Metabase.Interfaces
{
    public interface IFilterableList
    {
        bool IsFiltered { get; }

        void AddFilter(PropertyFilterExpression filter);

        void ClearFilters(bool removeBaseFilters);

        void ClearFilterSort();

        int Find(PropertyFilterExpression filter, int startRow, bool searchAllProperties);

        void UpdateFilters(PropertyFilterExpression filter);
    }
}