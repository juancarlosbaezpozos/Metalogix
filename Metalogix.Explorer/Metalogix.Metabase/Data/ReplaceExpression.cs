using Metalogix.Metabase.DataTypes;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalogix.Metabase.Data
{
    public class ReplaceExpression : PropertyFilterExpression
    {
        private string m_strReplaceText;

        private bool m_bMultiLine;

        public bool MultiLine
        {
            get { return this.m_bMultiLine; }
            set { this.m_bMultiLine = value; }
        }

        public string ReplaceValue
        {
            get { return this.m_strReplaceText; }
            set { this.m_strReplaceText = value; }
        }

        public ReplaceExpression()
        {
        }

        public bool EvaluateReplace(object objComponent)
        {
            if (!this.Evaluate(objComponent))
            {
                return false;
            }

            object value = null;
            string empty = string.Empty;
            if (base.Property.PropertyType != typeof(TextMoniker))
            {
                value = base.Property.GetValue(objComponent);
                if (value != null)
                {
                    empty = value.ToString();
                }
            }
            else
            {
                TextMoniker textMoniker = (TextMoniker)base.Property.GetValue(objComponent);
                if (textMoniker != null)
                {
                    empty = textMoniker.GetFullText();
                    value = empty;
                }
            }

            if (value == null)
            {
                return false;
            }

            string replaceValue = null;
            switch (base.Operand)
            {
                case PropertyFilterOperand.Equals:
                {
                    replaceValue = this.ReplaceValue;
                    break;
                }
                case PropertyFilterOperand.StartsWith:
                {
                    replaceValue = string.Concat(this.ReplaceValue, empty.Substring(base.Value.Length));
                    break;
                }
                case PropertyFilterOperand.NotContains:
                case PropertyFilterOperand.ContainedBy:
                {
                    throw new Exception("Expression uses unhandled operand");
                }
                case PropertyFilterOperand.Contains:
                {
                    replaceValue = (this.m_bIsCaseSensitive
                        ? empty.Replace(base.Value, this.ReplaceValue)
                        : ReplaceExpression.ReplaceIgnoreCase(empty, base.Value, this.ReplaceValue));
                    break;
                }
                case PropertyFilterOperand.RegularExpression:
                {
                    RegexOptions regexOption = RegexOptions.None;
                    if (!this.m_bIsCaseSensitive)
                    {
                        regexOption |= RegexOptions.IgnoreCase;
                    }

                    if (this.m_bMultiLine)
                    {
                        regexOption |= RegexOptions.Multiline;
                    }

                    if (string.IsNullOrEmpty(empty))
                    {
                        break;
                    }

                    replaceValue = Regex.Replace(empty, base.Value, this.ReplaceValue, regexOption);
                    break;
                }
                case PropertyFilterOperand.EndsWith:
                {
                    replaceValue = string.Concat(empty.Substring(0, empty.Length - base.Value.Length),
                        this.ReplaceValue);
                    break;
                }
                default:
                {
                    throw new Exception("Expression uses unhandled operand");
                }
            }

            object obj = TypeDescriptor.GetConverter(value).ConvertFrom(replaceValue);
            base.Property.SetValue(objComponent, obj);
            return true;
        }

        protected override string GetCurrentValue(object objValue)
        {
            if (objValue == null)
            {
                return null;
            }

            object value = null;
            string fullText = null;
            if (this.m_filterProperty.PropertyType != typeof(TextMoniker))
            {
                value = this.m_filterProperty.GetValue(objValue);
            }
            else
            {
                fullText = ((TextMoniker)this.m_filterProperty.GetValue(objValue)).GetFullText();
                value = fullText;
            }

            if (value != null)
            {
                fullText = (string.IsNullOrEmpty(this.m_sFormat)
                    ? value.ToString()
                    : string.Format(string.Concat("{0:", this.m_sFormat, "}"), value));
            }
            else
            {
                fullText = this.m_strNullText;
            }

            if (fullText == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(fullText.Trim()))
            {
                fullText = this.m_strNullText;
            }

            string mSFilterValue = this.m_sFilterValue;
            if (!this.m_bIsCaseSensitive && base.Operand != PropertyFilterOperand.RegularExpression)
            {
                fullText = fullText.ToLower();
                mSFilterValue = mSFilterValue.ToLower();
            }

            return fullText;
        }

        private static string ReplaceIgnoreCase(string sOriginal, string sPattern, string sReplacement)
        {
            if (sOriginal == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(sPattern))
            {
                return sOriginal;
            }

            int num = 0;
            int length = sPattern.Length;
            int num1 = sOriginal.IndexOf(sPattern, StringComparison.CurrentCultureIgnoreCase);
            StringBuilder stringBuilder = new StringBuilder(Math.Min(4096, sOriginal.Length));
            while (num1 >= 0)
            {
                stringBuilder.Append(sOriginal, num, num1 - num);
                stringBuilder.Append(sReplacement);
                num = num1 + length;
                num1 = sOriginal.IndexOf(sPattern, num, StringComparison.CurrentCultureIgnoreCase);
            }

            stringBuilder.Append(sOriginal, num, sOriginal.Length - num);
            return stringBuilder.ToString();
        }
    }
}