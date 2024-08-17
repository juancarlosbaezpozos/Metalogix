using Metalogix.Transformers;
using System;

namespace Metalogix.Transformers.Interfaces
{
    public interface ITransformerConfig
    {
        bool Configure(TransformerConfigContext context);
    }
}