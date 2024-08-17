using Metalogix.Actions;
using System;

namespace Metalogix.Transformers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TransformerCardinalityAttribute : Attribute
    {
        private Metalogix.Actions.Cardinality m_value = Metalogix.Actions.Cardinality.One;

        public Metalogix.Actions.Cardinality Cardinality
        {
            get { return this.m_value; }
        }

        public TransformerCardinalityAttribute(Metalogix.Actions.Cardinality cardinality_0)
        {
            this.m_value = cardinality_0;
        }
    }
}