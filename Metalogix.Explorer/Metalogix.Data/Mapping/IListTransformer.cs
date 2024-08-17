using System;

namespace Metalogix.Data.Mapping
{
    public interface IListTransformer
    {
        string Name { get; }

        bool AppliesTo(object item);

        string Transform(object item);

        string TransformColumn(object item, string propertyName);

        string TransformType(object item);
    }
}