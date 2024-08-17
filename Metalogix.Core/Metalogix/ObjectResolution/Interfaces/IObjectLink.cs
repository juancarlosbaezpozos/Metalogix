using Metalogix.Data;
using System;

namespace Metalogix.ObjectResolution.Interfaces
{
    public interface IObjectLink : IXmlable
    {
        Type ObjectResultType { get; }
    }
}