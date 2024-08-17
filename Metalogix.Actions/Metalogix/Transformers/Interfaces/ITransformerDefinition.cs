using Metalogix.Transformers;
using System;

namespace Metalogix.Transformers.Interfaces
{
    public interface ITransformerDefinition
    {
        Type ActionType { get; }

        Type DataObjectType { get; }

        bool Hidden { get; }

        string Name { get; }

        Type SourceCollectionType { get; }

        Type TargetCollectionType { get; }

        TransformerCollection GetMatchingAvailableTransformers();

        TransformerCollection GetMatchingTransformers(TransformerCollection availableTransformers);
    }
}