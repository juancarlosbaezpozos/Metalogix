using System;

namespace Metalogix.Metabase.Data
{
    public class TypeFilterExpression : FilterExpression
    {
        private TypeFilterOperand m_operand;

        private Type m_targetType;

        public TypeFilterOperand Operand
        {
            get { return this.m_operand; }
            set { this.m_operand = value; }
        }

        public Type TargetType
        {
            get { return this.m_targetType; }
            set { this.m_targetType = value; }
        }

        public TypeFilterExpression(TypeFilterOperand operand, Type targetType)
        {
            this.m_operand = operand;
            this.m_targetType = targetType;
        }

        public override bool Evaluate(object objValue)
        {
            if (objValue == null)
            {
                return false;
            }

            if (this.Operand == TypeFilterOperand.Is)
            {
                return this.TargetType.IsAssignableFrom(objValue.GetType());
            }

            return !this.TargetType.IsAssignableFrom(objValue.GetType());
        }

        public override string ToString()
        {
            return string.Format("Node Type {0} '{1}'", (this.Operand == TypeFilterOperand.Is ? "is" : "is not"),
                this.TargetType);
        }
    }
}