using System;
using System.Collections;
using System.ComponentModel;

namespace Metalogix.Explorer
{
    public class ListFieldCollection : FieldCollection
    {
        private ArrayList m_data;

        public int Count
        {
            get { return this.m_data.Count; }
        }

        public string XML
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ListFieldCollection(Type oTargetType, Attribute[] attrFilters) : this((object)oTargetType, attrFilters)
        {
        }

        public ListFieldCollection(object oTarget, Attribute[] attrFilters)
        {
            PropertyDescriptorCollection propertyDescriptorCollections = null;
            Type type = oTarget as Type;
            if (attrFilters == null || (int)attrFilters.Length == 0)
            {
                propertyDescriptorCollections = (type != null
                    ? TypeDescriptor.GetProperties(type)
                    : TypeDescriptor.GetProperties(oTarget));
            }
            else
            {
                propertyDescriptorCollections = (type != null
                    ? TypeDescriptor.GetProperties(type, attrFilters)
                    : TypeDescriptor.GetProperties(oTarget, attrFilters));
            }

            this.m_data = new ArrayList(propertyDescriptorCollections.Count);
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollections)
            {
                this.m_data.Add(new ListField(propertyDescriptor.Name, propertyDescriptor.DisplayName,
                    propertyDescriptor.PropertyType));
            }
        }

        public void Add(PropertyDescriptor pd)
        {
            if (pd == null)
            {
                return;
            }

            this.m_data.Add(pd);
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_data.GetEnumerator();
        }
    }
}