using Metalogix.Metabase.Data;
using System;
using System.Collections;
using System.ComponentModel;

namespace Metalogix.Metabase
{
    public class RecordPropertyDescriptorComparer : IComparer
    {
        private RecordPropertyDescriptorSortOrder[] m_sortOrder;

        public RecordPropertyDescriptorSortOrder[] SortOrder
        {
            get { return this.m_sortOrder; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.m_sortOrder = value;
            }
        }

        public RecordPropertyDescriptorComparer()
        {
            this.SortOrder = new RecordPropertyDescriptorSortOrder[1];
        }

        public RecordPropertyDescriptorComparer(RecordPropertyDescriptorSortOrder[] order)
        {
            this.SortOrder = order;
        }

        private int Compare(PropertyDescriptor x, PropertyDescriptor y, int iSortLevel)
        {
            IComparable value = RecordPropertyDescriptorComparer.GetValue(x, this.SortOrder[iSortLevel]);
            IComparable comparable = RecordPropertyDescriptorComparer.GetValue(y, this.SortOrder[iSortLevel]);
            int num = value.CompareTo(comparable);
            if (num != 0 || iSortLevel >= (int)this.SortOrder.Length - 1)
            {
                return num;
            }

            return this.Compare(x, y, iSortLevel + 1);
        }

        public int Compare(object x, object y)
        {
            PropertyDescriptor propertyDescriptor = x as PropertyDescriptor;
            PropertyDescriptor propertyDescriptor1 = y as PropertyDescriptor;
            if (propertyDescriptor == null)
            {
                return 1;
            }

            if (propertyDescriptor1 == null)
            {
                return -1;
            }

            return this.Compare(propertyDescriptor, propertyDescriptor1, 0);
        }

        private static IComparable GetValue(PropertyDescriptor propDesc, RecordPropertyDescriptorSortOrder order)
        {
            switch (order)
            {
                case RecordPropertyDescriptorSortOrder.ByOrdinal:
                {
                    ColumnOrdinalAttribute item =
                        (ColumnOrdinalAttribute)propDesc.Attributes[typeof(ColumnOrdinalAttribute)];
                    if (item == null)
                    {
                        return 2147483647;
                    }

                    return item.Ordinal;
                }
                case RecordPropertyDescriptorSortOrder.ByCategory:
                {
                    return propDesc.Category;
                }
                case RecordPropertyDescriptorSortOrder.ByFillFactorLevel:
                {
                    FillFactorAttribute fillFactorAttribute =
                        (FillFactorAttribute)propDesc.Attributes[typeof(FillFactorAttribute)];
                    if (fillFactorAttribute == null)
                    {
                        return Convert.ToInt32(FillFactorLevel.Empty);
                    }

                    return Convert.ToInt32(fillFactorAttribute.Level);
                }
                case RecordPropertyDescriptorSortOrder.ByName:
                {
                    return propDesc.Name;
                }
            }

            throw new ArgumentException("Unhandled RecordPropertyDescriptorSortOrder", "order");
        }
    }
}