using System;

namespace Metalogix.Actions
{
    public interface IXMLAbleListConverter
    {
        T ConvertTo<T>(IXMLAbleList list)
            where T : class;

        object ConvertTo(IXMLAbleList list, Type targetType);

        bool IsSupported(Type targetType);
    }
}