using System;

namespace Metalogix.Transformers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TransformerVisibleAttribute : Attribute
    {
        private readonly bool m_isVisible;

        public bool IsVisible
        {
            get { return this.m_isVisible; }
        }

        public TransformerVisibleAttribute() : this(true)
        {
        }

        public TransformerVisibleAttribute(bool isVisible)
        {
            this.m_isVisible = isVisible;
        }
    }
}