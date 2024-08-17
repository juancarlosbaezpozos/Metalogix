using Metalogix.Transformers.Interfaces;
using System;
using System.Collections.Generic;

namespace Metalogix.Transformers
{
    public class TrasformerDefinitionSorter : IComparer<ITransformerDefinition>
    {
        public TrasformerDefinitionSorter()
        {
        }

        public int Compare(ITransformerDefinition itransformerDefinition_0,
            ITransformerDefinition itransformerDefinition_1)
        {
            return itransformerDefinition_0.Name.CompareTo(itransformerDefinition_1.Name);
        }
    }
}