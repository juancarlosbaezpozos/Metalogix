using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Metalogix.Metabase.Data
{
    public class OrFilterExpression : PropertyFilterExpression
    {
        private List<string> m_listFilterValues = new List<string>();

        public List<string> FilterValues
        {
            get { return this.m_listFilterValues; }
        }

        public OrFilterExpression(PropertyDescriptor filterProperty, List<string> listFilterValues,
            PropertyFilterOperand filterOperand, string strNullText, bool bIsBaseFilter) : base(filterProperty, "",
            filterOperand, strNullText, bIsBaseFilter)
        {
            this.m_filterProperty = filterProperty;
            this.m_listFilterValues = listFilterValues;
            this.m_ifilterOperand = filterOperand;
            this.m_strNullText = strNullText;
            this.m_bIsBaseFilter = bIsBaseFilter;
        }

        public override bool Evaluate(object objValue)
        {
            bool flag;
            if (this.m_listFilterValues == null || this.m_listFilterValues.Count == 0)
            {
                return false;
            }

            if (objValue == null)
            {
                return false;
            }

            string currentValue = this.GetCurrentValue(objValue);
            if (currentValue == null)
            {
                return false;
            }

            List<string>.Enumerator enumerator = this.m_listFilterValues.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (!base.EvaluateSingleString(enumerator.Current, currentValue))
                    {
                        continue;
                    }

                    flag = true;
                    return flag;
                }

                return false;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return flag;
        }

        public override string ToString()
        {
            string[] name;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("");
            if (this.m_listFilterValues != null && this.m_listFilterValues.Count > 0)
            {
                int num = 0;
                foreach (string mListFilterValue in this.m_listFilterValues)
                {
                    num++;
                    stringBuilder.Append(string.Concat("'", mListFilterValue, "'"));
                    if (num >= this.m_listFilterValues.Count)
                    {
                        continue;
                    }

                    stringBuilder.Append(" or ");
                }
            }

            string str = "";
            switch (base.Operand)
            {
                case PropertyFilterOperand.Equals:
                {
                    str = "equals";
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.StartsWith:
                {
                    str = "starts with";
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.NotContains:
                {
                    str = "doesn't contain";
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.Contains:
                {
                    str = "contains";
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.ContainedBy:
                {
                    str = "contained by";
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.RegularExpression:
                {
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.EndsWith:
                {
                    str = "ends with";
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
                default:
                {
                    name = new string[] { base.Property.Name, " ", str, ": ", stringBuilder.ToString() };
                    return string.Concat(name);
                }
            }
        }
    }
}