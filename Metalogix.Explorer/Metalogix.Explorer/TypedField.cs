using System;

namespace Metalogix.Explorer
{
    public interface TypedField : Field
    {
        Type UnderlyingType { get; }
    }
}