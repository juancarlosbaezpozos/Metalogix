using System;

namespace Metalogix.Interfaces
{
    public interface IConfigurationVariable
    {
        IConvertible GetValue();

        TConvertible GetValue<TConvertible>()
            where TConvertible : IConvertible;

        Type GetValueType();

        void SetValue(IConvertible value);
    }
}