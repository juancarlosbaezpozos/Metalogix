using Metalogix.Metabase.DataTypes;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Metalogix.Metabase.Data
{
    public class PropertyFilterExpression : FilterExpression
    {
        protected PropertyDescriptor m_filterProperty;

        protected string m_sFilterValue = string.Empty;

        protected PropertyFilterOperand m_ifilterOperand = PropertyFilterOperand.Contains;

        protected string m_strNullText = PropertyFilterExpression.DefaultNullText;

        protected bool m_bIsBaseFilter;

        protected bool m_bIsCaseSensitive = true;

        protected string m_sFormat = string.Empty;

        protected bool m_bActionCreatedFilter;

        public static string DefaultNullText
        {
            get { return "null"; }
        }

        public string Format
        {
            get { return this.m_sFormat; }
            set { this.m_sFormat = value; }
        }

        public new bool IsActionCreatedFilter
        {
            get { return this.m_bActionCreatedFilter; }
            set { this.m_bActionCreatedFilter = value; }
        }

        public new bool IsBaseFilter
        {
            get { return this.m_bIsBaseFilter; }
            set { this.m_bIsBaseFilter = value; }
        }

        public bool IsCaseSensitive
        {
            get { return this.m_bIsCaseSensitive; }
            set { this.m_bIsCaseSensitive = value; }
        }

        public string NullText
        {
            get { return this.m_strNullText; }
            set { this.m_strNullText = value; }
        }

        public PropertyFilterOperand Operand
        {
            get { return this.m_ifilterOperand; }
            set { this.m_ifilterOperand = value; }
        }

        public PropertyDescriptor Property
        {
            get { return this.m_filterProperty; }
            set { this.m_filterProperty = value; }
        }

        public string Value
        {
            get { return this.m_sFilterValue; }
            set { this.m_sFilterValue = value; }
        }

        public PropertyFilterExpression()
        {
        }

        public PropertyFilterExpression(PropertyDescriptor filterProperty, string strFilterValue,
            PropertyFilterOperand filterOperand, string strNullText, bool bIsBaseFilter)
        {
            this.m_filterProperty = filterProperty;
            this.m_sFilterValue = strFilterValue;
            this.m_ifilterOperand = filterOperand;
            this.m_strNullText = strNullText;
            this.m_bIsBaseFilter = bIsBaseFilter;
        }

        public PropertyFilterExpression(PropertyDescriptor filterProperty, string strFilterValue)
        {
            this.m_filterProperty = filterProperty;
            this.m_sFilterValue = strFilterValue;
            this.m_ifilterOperand = PropertyFilterOperand.Contains;
            this.m_strNullText = PropertyFilterExpression.DefaultNullText;
        }

        public PropertyFilterExpression(PropertyDescriptor filterProperty, string strFilterValue,
            PropertyFilterOperand filterOperand)
        {
            this.m_filterProperty = filterProperty;
            this.m_sFilterValue = strFilterValue;
            this.m_ifilterOperand = filterOperand;
            this.m_strNullText = PropertyFilterExpression.DefaultNullText;
        }

        public override bool Evaluate(object objValue)
        {
            if (objValue == null)
            {
                return false;
            }

            string currentValue = this.GetCurrentValue(objValue);
            if (currentValue == null)
            {
                return false;
            }

            return this.EvaluateSingleString(this.m_sFilterValue, currentValue);
        }

        protected bool EvaluateSingleString(string sFilterValue, string sCurVal)
        {
            if (sFilterValue == null)
            {
                return false;
            }

            if (!this.m_bIsCaseSensitive && this.Operand != PropertyFilterOperand.RegularExpression)
            {
                sCurVal = sCurVal.ToLower();
                sFilterValue = sFilterValue.ToLower();
            }

            switch (this.m_ifilterOperand)
            {
                case PropertyFilterOperand.Equals:
                {
                    if (!sCurVal.Equals(sFilterValue))
                    {
                        break;
                    }

                    return true;
                }
                case PropertyFilterOperand.StartsWith:
                {
                    if (!sCurVal.StartsWith(sFilterValue))
                    {
                        break;
                    }

                    return true;
                }
                case PropertyFilterOperand.NotContains:
                {
                    if (sCurVal.IndexOf(sFilterValue) >= 0)
                    {
                        break;
                    }

                    return true;
                }
                case PropertyFilterOperand.Contains:
                {
                    if (sCurVal.IndexOf(sFilterValue) < 0)
                    {
                        break;
                    }

                    return true;
                }
                case PropertyFilterOperand.ContainedBy:
                {
                    if (sFilterValue.IndexOf(sCurVal) < 0)
                    {
                        break;
                    }

                    return true;
                }
                case PropertyFilterOperand.RegularExpression:
                {
                    RegexOptions regexOption = RegexOptions.Multiline;
                    if (!this.m_bIsCaseSensitive)
                    {
                        regexOption |= RegexOptions.IgnoreCase;
                    }

                    if (!(new Regex(sFilterValue, regexOption)).IsMatch(sCurVal))
                    {
                        break;
                    }

                    return true;
                }
                case PropertyFilterOperand.EndsWith:
                {
                    if (!sCurVal.EndsWith(sFilterValue))
                    {
                        break;
                    }

                    return true;
                }
            }

            return false;
        }

        protected virtual string GetCurrentValue(object objValue)
        {
            if (objValue == null)
            {
                return null;
            }

            object value = this.m_filterProperty.GetValue(objValue);
            string mStrNullText = null;
            if (value == null)
            {
                mStrNullText = this.m_strNullText;
            }
            else if (string.IsNullOrEmpty(this.m_sFormat))
            {
                mStrNullText = (!typeof(ITextMoniker).IsAssignableFrom(this.m_filterProperty.PropertyType)
                    ? value.ToString()
                    : ((ITextMoniker)value).GetFullText());
            }
            else
            {
                mStrNullText = string.Format(string.Concat("{0:", this.m_sFormat, "}"), value);
            }

            if (mStrNullText == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(mStrNullText.Trim()))
            {
                mStrNullText = this.m_strNullText;
            }

            return mStrNullText;
        }

        public override string ToString()
        {
            string[] name;
            string str = "";
            switch (this.Operand)
            {
                case PropertyFilterOperand.Equals:
                {
                    str = "equals";
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.StartsWith:
                {
                    str = "starts with";
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.NotContains:
                {
                    str = "doesn't contain";
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.Contains:
                {
                    str = "contains";
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.ContainedBy:
                {
                    str = "contained by";
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.RegularExpression:
                {
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                case PropertyFilterOperand.EndsWith:
                {
                    str = "ends with";
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
                default:
                {
                    name = new string[] { this.Property.Name, " ", str, ": '", this.Value, "'" };
                    return string.Concat(name);
                }
            }
        }
    }
}