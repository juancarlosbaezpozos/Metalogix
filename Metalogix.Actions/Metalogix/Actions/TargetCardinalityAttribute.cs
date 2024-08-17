using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TargetCardinalityAttribute : Attribute
    {
        private Metalogix.Actions.Cardinality m_value = Metalogix.Actions.Cardinality.ZeroOrMore;

        public Metalogix.Actions.Cardinality Cardinality
        {
            get { return this.m_value; }
        }

        public TargetCardinalityAttribute(Metalogix.Actions.Cardinality cardinality_0)
        {
            this.m_value = cardinality_0;
        }

        public override string ToString()
        {
            return this.Cardinality.ToString();
        }
    }
}