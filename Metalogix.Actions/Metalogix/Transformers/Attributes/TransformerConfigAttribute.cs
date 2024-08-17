using System;

namespace Metalogix.Transformers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransformerConfigAttribute : Attribute
    {
        private readonly Type[] m_transformerTypes;

        public Type[] TransformerTypes
        {
            get { return this.m_transformerTypes; }
        }

        public TransformerConfigAttribute(params Type[] transformerTypes)
        {
            this.m_transformerTypes = transformerTypes;
        }
    }
}