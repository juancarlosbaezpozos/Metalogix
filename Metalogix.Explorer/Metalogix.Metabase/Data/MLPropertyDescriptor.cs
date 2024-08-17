using System;
using System.ComponentModel;

namespace Metalogix.Metabase.Data
{
    public class MLPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor m_baseDescriptor;

        public override AttributeCollection Attributes
        {
            get { return this.m_baseDescriptor.Attributes; }
        }

        public override string Category
        {
            get { return this.m_baseDescriptor.Category; }
        }

        public override Type ComponentType
        {
            get { return this.m_baseDescriptor.ComponentType; }
        }

        public override string Description
        {
            get { return this.m_baseDescriptor.Description; }
        }

        public override string DisplayName
        {
            get { return this.m_baseDescriptor.DisplayName; }
        }

        public override bool IsReadOnly
        {
            get { return this.m_baseDescriptor.IsReadOnly; }
        }

        public override string Name
        {
            get { return this.m_baseDescriptor.Name; }
        }

        public override Type PropertyType
        {
            get { return this.m_baseDescriptor.PropertyType; }
        }

        public MLPropertyDescriptor(PropertyDescriptor baseDescriptor) : base(baseDescriptor.Name, null)
        {
            this.m_baseDescriptor = baseDescriptor;
        }

        public override bool CanResetValue(object component)
        {
            return this.m_baseDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return this.m_baseDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            this.m_baseDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this.m_baseDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return this.m_baseDescriptor.ShouldSerializeValue(component);
        }
    }
}