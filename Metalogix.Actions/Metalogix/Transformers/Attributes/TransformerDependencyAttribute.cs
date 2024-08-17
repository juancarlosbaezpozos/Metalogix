using System;

namespace Metalogix.Transformers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransformerDependencyAttribute : Attribute
    {
        private readonly Type[] m_transformerTypes;

        public Type[] TransformerTypes
        {
            get { return this.m_transformerTypes; }
        }

        public TransformerDependencyAttribute(params Type[] transformerTypes)
        {
            this.m_transformerTypes = transformerTypes;
        }
    }
}