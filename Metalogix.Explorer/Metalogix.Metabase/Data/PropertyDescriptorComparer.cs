using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Metalogix.Metabase.Data
{
    public class PropertyDescriptorComparer : IComparer<PropertyDescriptor>, IComparer
    {
        private PropertyDescriptorCompareMethod m_compareMethod = PropertyDescriptorCompareMethod.DisplayName;

        public PropertyDescriptorComparer() : this(PropertyDescriptorCompareMethod.DisplayName)
        {
        }

        public PropertyDescriptorComparer(PropertyDescriptorCompareMethod compareMethod)
        {
            this.m_compareMethod = compareMethod;
        }

        public int Compare(PropertyDescriptor x, PropertyDescriptor y)
        {
            string name;
            string displayName;
            if (this.m_compareMethod != PropertyDescriptorCompareMethod.DisplayName)
            {
                name = x.Name;
                displayName = y.Name;
            }
            else
            {
                name = x.DisplayName;
                displayName = y.DisplayName;
            }

            return string.Compare(name, displayName);
        }

        public int Compare(object x, object y)
        {
            PropertyDescriptor propertyDescriptor = x as PropertyDescriptor;
            PropertyDescriptor propertyDescriptor1 = y as PropertyDescriptor;
            if (propertyDescriptor != null && propertyDescriptor1 != null)
            {
                return this.Compare(propertyDescriptor, propertyDescriptor1);
            }

            if (propertyDescriptor == null && propertyDescriptor1 == null)
            {
                return 0;
            }

            if (propertyDescriptor == null)
            {
                return -1;
            }

            return 1;
        }
    }
}