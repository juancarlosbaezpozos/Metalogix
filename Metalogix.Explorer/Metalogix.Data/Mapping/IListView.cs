using System;

namespace Metalogix.Data.Mapping
{
    public interface IListView
    {
        string Name { get; }

        bool AppliesTo(object item);

        string Render(object item);

        string RenderColumn(object item, string propertyName);

        string RenderGroup(object item);

        string RenderType(object item);
    }
}