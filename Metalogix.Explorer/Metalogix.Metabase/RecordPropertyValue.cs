using Metalogix.Metabase.DataTypes;
using System;
using System.ComponentModel;

namespace Metalogix.Metabase
{
    public class RecordPropertyValue
    {
        private Record m_item;

        private PropertyDescriptor m_propertyDescriptor;

        public string DisplayName
        {
            get { return this.m_propertyDescriptor.DisplayName; }
        }

        public string Name
        {
            get { return this.m_propertyDescriptor.Name; }
        }

        public System.Type Type
        {
            get { return this.m_propertyDescriptor.PropertyType; }
        }

        public object Value
        {
            get { return this.m_propertyDescriptor.GetValue(this.m_item); }
            set
            {
                if (!typeof(TextMoniker).IsAssignableFrom(this.m_propertyDescriptor.PropertyType) &&
                    this.m_propertyDescriptor.IsReadOnly)
                {
                    throw new Exception(string.Concat("Property: '", this.Name, "' is read only."));
                }

                this.m_propertyDescriptor.SetValue(this.m_item, value);
            }
        }

        internal RecordPropertyValue(Record item, PropertyDescriptor propertyDescriptor)
        {
            this.m_item = item;
            this.m_propertyDescriptor = propertyDescriptor;
        }
    }
}