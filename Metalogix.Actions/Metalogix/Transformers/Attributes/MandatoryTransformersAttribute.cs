using System;

namespace Metalogix.Transformers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MandatoryTransformersAttribute : Attribute
    {
        private readonly Type[] m_transformerTypes;

        public Type[] MandatoryTransformers
        {
            get { return this.m_transformerTypes; }
        }

        public MandatoryTransformersAttribute(params Type[] transformerTypes)
        {
            this.m_transformerTypes = transformerTypes;
        }
    }
}