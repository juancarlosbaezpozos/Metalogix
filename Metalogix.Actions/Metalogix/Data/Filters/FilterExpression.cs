using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.Data.Filters
{
    public class FilterExpression : IEquatable<IFilterExpression>, IXmlable, IFilterExpression
    {
        protected string m_sProperty;

        protected string m_sPattern;

        protected FilterOperand m_ifilterOperand = FilterOperand.Contains;

        protected bool m_bIsCaseSensitive = true;

        protected System.Globalization.CultureInfo m_cultureInfo;

        protected bool m_bIsBaseFilter;

        protected List<string> m_AppliesToTypes;

        public List<string> AppliesToTypes
        {
            get { return this.m_AppliesToTypes; }
        }

        public System.Globalization.CultureInfo CultureInfo
        {
            get { return this.m_cultureInfo; }
        }

        public bool IsBaseFilter
        {
            get { return this.m_bIsBaseFilter; }
        }

        public bool IsCaseSensitive
        {
            get { return this.m_bIsCaseSensitive; }
        }

        public FilterOperand Operand
        {
            get { return this.m_ifilterOperand; }
        }

        public string Pattern
        {
            get { return this.m_sPattern; }
        }

        public string Property
        {
            get { return this.m_sProperty; }
        }

        public FilterExpression(XmlNode xmlNode)
        {
            if (xmlNode != null)
            {
                this.FromXML(xmlNode);
            }
        }

        public FilterExpression(FilterOperand operand, string sAppliesToTypeName, string sPropertyName, string sPattern,
            bool bIsCaseSensitive, bool bIsBaseFilter)
        {
            List<string> strs;
            this.m_ifilterOperand = operand;
            if (sAppliesToTypeName != null)
            {
                strs = new List<string>(new string[] { sAppliesToTypeName });
            }
            else
            {
                strs = null;
            }

            this.m_AppliesToTypes = strs;
            this.m_sProperty = sPropertyName;
            this.m_sPattern = sPattern;
            this.m_bIsCaseSensitive = bIsCaseSensitive;
            this.m_bIsBaseFilter = bIsBaseFilter;
        }

        public FilterExpression(FilterOperand operand, string sAppliesToTypeName, string sPropertyName, string sPattern,
            bool bIsCaseSensitive, bool bIsBaseFilter, System.Globalization.CultureInfo cultureInfo)
        {
            List<string> strs;
            this.m_ifilterOperand = operand;
            if (sAppliesToTypeName != null)
            {
                strs = new List<string>(new string[] { sAppliesToTypeName });
            }
            else
            {
                strs = null;
            }

            this.m_AppliesToTypes = strs;
            this.m_sProperty = sPropertyName;
            this.m_sPattern = sPattern;
            this.m_bIsCaseSensitive = bIsCaseSensitive;
            this.m_bIsBaseFilter = bIsBaseFilter;
            this.m_cultureInfo = cultureInfo;
        }

        public FilterExpression(FilterOperand operand, Type appliesToType, string sPropertyName, string sPattern,
            bool bIsCaseSensitive, bool bIsBaseFilter)
        {
            this.m_ifilterOperand = operand;
            string[] fullName = new string[] { appliesToType.FullName };
            this.m_AppliesToTypes = new List<string>(fullName);
            this.m_sProperty = sPropertyName;
            this.m_sPattern = sPattern;
            this.m_bIsCaseSensitive = bIsCaseSensitive;
            this.m_bIsBaseFilter = bIsBaseFilter;
        }

        public FilterExpression(FilterOperand operand, Type appliesToType, string sPropertyName, string sPattern,
            bool bIsCaseSensitive, bool bIsBaseFilter, System.Globalization.CultureInfo cultureInfo)
        {
            this.m_ifilterOperand = operand;
            string[] fullName = new string[] { appliesToType.FullName };
            this.m_AppliesToTypes = new List<string>(fullName);
            this.m_sProperty = sPropertyName;
            this.m_sPattern = sPattern;
            this.m_bIsCaseSensitive = bIsCaseSensitive;
            this.m_bIsBaseFilter = bIsBaseFilter;
            this.m_cultureInfo = cultureInfo;
        }

        public FilterExpression(FilterOperand operand, List<string> appliesToTypes, string sPropertyName,
            string sPattern, bool bIsCaseSensitive, bool bIsBaseFilter, System.Globalization.CultureInfo cultureInfo)
        {
            this.m_ifilterOperand = operand;
            this.m_AppliesToTypes = new List<string>();
            foreach (string appliesToType in appliesToTypes)
            {
                this.m_AppliesToTypes.Add(appliesToType);
            }

            this.m_sProperty = sPropertyName;
            this.m_sPattern = sPattern;
            this.m_bIsCaseSensitive = bIsCaseSensitive;
            this.m_bIsBaseFilter = bIsBaseFilter;
            this.m_cultureInfo = cultureInfo;
        }

        public FilterExpression(FilterOperand operand, List<Type> appliesToTypes, string sPropertyName, string sPattern,
            bool bIsCaseSensitive, bool bIsBaseFilter, System.Globalization.CultureInfo cultureInfo)
        {
            this.m_ifilterOperand = operand;
            this.m_AppliesToTypes = new List<string>();
            foreach (Type appliesToType in appliesToTypes)
            {
                this.m_AppliesToTypes.Add(appliesToType.FullName);
            }

            this.m_sProperty = sPropertyName;
            this.m_sPattern = sPattern;
            this.m_bIsCaseSensitive = bIsCaseSensitive;
            this.m_bIsBaseFilter = bIsBaseFilter;
            this.m_cultureInfo = cultureInfo;
        }

        public IFilterExpression Clone()
        {
            FilterExpression filterExpression = new FilterExpression(this.Operand, this.AppliesToTypes, this.Property,
                this.Pattern, this.IsCaseSensitive, this.IsBaseFilter, this.CultureInfo);
            return filterExpression;
        }

        public bool Equals(IFilterExpression iFilterExpression)
        {
            if (iFilterExpression == null)
            {
                return false;
            }

            if (this.GetType() != iFilterExpression.GetType())
            {
                return false;
            }

            bool flag = false;
            FilterExpression filterExpression = (FilterExpression)iFilterExpression;
            if (filterExpression.AppliesToTypes != null && this.AppliesToTypes != null &&
                filterExpression.AppliesToTypes.Count == this.AppliesToTypes.Count)
            {
                string[] strArrays = new string[this.AppliesToTypes.Count];
                this.AppliesToTypes.CopyTo(strArrays);
                List<string> strs = new List<string>(strArrays);
                foreach (string appliesToType in filterExpression.AppliesToTypes)
                {
                    List<string>.Enumerator enumerator = strs.GetEnumerator();
                    try
                    {
                        while (true)
                        {
                            if (enumerator.MoveNext())
                            {
                                string current = enumerator.Current;
                                if (current.Equals(appliesToType))
                                {
                                    strs.Remove(current);
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                }

                if (strs.Count == 0)
                {
                    flag = true;
                }
            }

            if (flag && this.CultureInfo == filterExpression.CultureInfo &&
                this.IsBaseFilter == filterExpression.IsBaseFilter &&
                this.IsCaseSensitive == filterExpression.IsCaseSensitive && this.Operand == filterExpression.Operand &&
                this.Pattern.Equals(filterExpression.Pattern) && this.Property.Equals(filterExpression.Property))
            {
                return true;
            }

            return false;
        }

        public static bool Evaluate(object target, string sExpression)
        {
            if (sExpression == null)
            {
                return true;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sExpression);
            return FilterExpression.ParseExpression(xmlDocument.FirstChild).Evaluate(target);
        }

        public virtual bool Evaluate(object component)
        {
            return this.Evaluate(component, (Comparison<object>)null);
        }

        public virtual bool Evaluate(object component, IComparer comparer)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.Evaluate(component, comparison);
        }

        public virtual bool Evaluate(object component, Comparison<object> comparer)
        {
            int num;
            if (component == null)
            {
                return false;
            }

            List<string> strs = new List<string>();
            Type type = component.GetType();
            do
            {
                strs.Add(type.FullName);
                type = type.BaseType;
            } while (type != typeof(object));

            bool flag = false;
            if (this.AppliesToTypes != null && this.AppliesToTypes.Count > 0)
            {
                List<string>.Enumerator enumerator = this.AppliesToTypes.GetEnumerator();
                try
                {
                    do
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }

                        string current = enumerator.Current;
                        List<string>.Enumerator enumerator1 = strs.GetEnumerator();
                        try
                        {
                            while (true)
                            {
                                if (!enumerator1.MoveNext())
                                {
                                    break;
                                }
                                else if (enumerator1.Current.Equals(current))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            ((IDisposable)enumerator1).Dispose();
                        }
                    } while (!flag);
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                if (!flag)
                {
                    return false;
                }
            }

            PropertyDescriptor propertyDescriptor = null;
            object value = this.GetValue(component, out propertyDescriptor);
            switch (this.m_ifilterOperand)
            {
                case FilterOperand.IsNull:
                {
                    return value == null;
                }
                case FilterOperand.NotNull:
                {
                    return value != null;
                }
                case FilterOperand.IsNullOrBlank:
                {
                    if (value == null)
                    {
                        return true;
                    }

                    return string.IsNullOrEmpty((value as string).Trim());
                }
                case FilterOperand.NotNullAndNotBlank:
                {
                    if (value == null)
                    {
                        return false;
                    }

                    return !string.IsNullOrEmpty((value as string).Trim());
                }
                default:
                {
                    if (value != null)
                    {
                        break;
                    }
                    else
                    {
                        return FilterExpression.IsNegation(this.Operand);
                    }
                }
            }

            TypeConverter converter = propertyDescriptor.Converter;
            string str = converter.ConvertToString(null, new System.Globalization.CultureInfo("en-us", false), value);
            bool flag1 = false;
            if (this.EvaluateAgainstString(str, propertyDescriptor, out flag1))
            {
                return flag1;
            }

            object obj = converter.ConvertFromString(null, this.m_cultureInfo, this.Pattern);
            num = (comparer != null ? comparer(value, obj) : ((IComparable)value).CompareTo(obj));
            FilterOperand mIfilterOperand = this.m_ifilterOperand;
            switch (mIfilterOperand)
            {
                case FilterOperand.Equals:
                {
                    return num == 0;
                }
                case FilterOperand.NotEquals:
                {
                    return num != 0;
                }
                default:
                {
                    switch (mIfilterOperand)
                    {
                        case FilterOperand.GreaterThan:
                        {
                            return num > 0;
                        }
                        case FilterOperand.GreaterThanOrEqualTo:
                        {
                            return num >= 0;
                        }
                        case FilterOperand.LessThan:
                        {
                            return num < 0;
                        }
                        case FilterOperand.LessThanOrEqualTo:
                        {
                            return num <= 0;
                        }
                        default:
                        {
                            return false;
                        }
                    }

                    break;
                }
            }
        }

        protected bool EvaluateAgainstString(string sCompareValue, PropertyDescriptor property,
            out bool bStringEvalResult)
        {
            string pattern = this.Pattern;
            if (!this.IsCaseSensitive)
            {
                sCompareValue = (sCompareValue != null ? sCompareValue.ToLower() : sCompareValue);
                pattern = (pattern != null ? pattern.ToLower() : pattern);
            }

            switch (this.m_ifilterOperand)
            {
                case FilterOperand.Equals:
                {
                    if (property == null || property.PropertyType == typeof(string) && sCompareValue != null)
                    {
                        bStringEvalResult = sCompareValue.Equals(pattern);
                        return true;
                    }

                    if (property.PropertyType != typeof(bool) || sCompareValue == null)
                    {
                        bStringEvalResult = false;
                        return false;
                    }

                    bStringEvalResult = string.Equals(pattern, sCompareValue,
                        (this.IsCaseSensitive
                            ? StringComparison.CurrentCultureIgnoreCase
                            : StringComparison.CurrentCulture));
                    return true;
                }
                case FilterOperand.NotEquals:
                {
                    if (property == null || property.PropertyType == typeof(string) && sCompareValue != null)
                    {
                        bStringEvalResult = !sCompareValue.Equals(pattern);
                        return true;
                    }

                    if (property.PropertyType != typeof(bool))
                    {
                        bStringEvalResult = false;
                        return false;
                    }

                    bStringEvalResult = !string.Equals(pattern, sCompareValue,
                        (this.IsCaseSensitive
                            ? StringComparison.CurrentCultureIgnoreCase
                            : StringComparison.CurrentCulture));
                    return true;
                }
                case FilterOperand.StartsWith:
                {
                    bStringEvalResult = sCompareValue.StartsWith(pattern);
                    return true;
                }
                case FilterOperand.NotStartsWith:
                {
                    bStringEvalResult = !sCompareValue.StartsWith(pattern);
                    return true;
                }
                case FilterOperand.EndsWith:
                {
                    bStringEvalResult = sCompareValue.EndsWith(pattern);
                    return true;
                }
                case FilterOperand.NotEndsWith:
                {
                    bStringEvalResult = !sCompareValue.EndsWith(pattern);
                    return true;
                }
                case FilterOperand.Contains:
                {
                    bStringEvalResult = sCompareValue.IndexOf(pattern) >= 0;
                    return true;
                }
                case FilterOperand.NotContains:
                {
                    bStringEvalResult = sCompareValue.IndexOf(pattern) < 0;
                    return true;
                }
                case FilterOperand.IsNull:
                case FilterOperand.NotNull:
                case FilterOperand.IsNullOrBlank:
                case FilterOperand.NotNullAndNotBlank:
                case FilterOperand.GreaterThan:
                case FilterOperand.GreaterThanOrEqualTo:
                case FilterOperand.LessThan:
                case FilterOperand.LessThanOrEqualTo:
                {
                    bStringEvalResult = false;
                    return false;
                }
                case FilterOperand.Regex:
                {
                    bStringEvalResult = (new Regex(pattern)).IsMatch(sCompareValue);
                    return true;
                }
                case FilterOperand.NotRegex:
                {
                    bStringEvalResult = !(new Regex(pattern)).IsMatch(sCompareValue);
                    return true;
                }
                default:
                {
                    bStringEvalResult = false;
                    return false;
                }
            }
        }

        public virtual void FromXML(XmlNode node)
        {
            string value;
            System.Globalization.CultureInfo cultureInfo;
            XmlNode xmlNodes = (node.Name == "FilterExpression" ? node : node.SelectSingleNode(".//FilterExpression"));
            if (xmlNodes != null)
            {
                string name = xmlNodes.FirstChild.Name;
                this.m_ifilterOperand = (FilterOperand)Enum.Parse(typeof(FilterOperand), name);
                this.m_sProperty = xmlNodes.FirstChild.Attributes["Property"].Value;
                if (xmlNodes.FirstChild.Attributes["Pattern"] != null)
                {
                    value = xmlNodes.FirstChild.Attributes["Pattern"].Value;
                }
                else
                {
                    value = null;
                }

                this.m_sPattern = value;
                bool flag = false;
                bool itemOf = xmlNodes.FirstChild.Attributes["CaseSensitive"] != null;
                bool flag1 = itemOf;
                this.m_bIsCaseSensitive = itemOf;
                if (flag1)
                {
                    bool.TryParse(xmlNodes.FirstChild.Attributes["CaseSensitive"].Value.ToString(), out flag);
                }

                this.m_bIsCaseSensitive = flag;
                flag = false;
                bool itemOf1 = xmlNodes.FirstChild.Attributes["BaseFilter"] != null;
                bool flag2 = itemOf1;
                this.m_bIsBaseFilter = itemOf1;
                if (flag2)
                {
                    bool.TryParse(xmlNodes.FirstChild.Attributes["BaseFilter"].Value.ToString(), out flag);
                }

                this.m_bIsBaseFilter = flag;
                this.m_AppliesToTypes = new List<string>();
                if (xmlNodes.FirstChild.SelectSingleNode(".//AppliesToTypes") != null)
                {
                    foreach (XmlNode childNode in xmlNodes.FirstChild.SelectSingleNode(".//AppliesToTypes").ChildNodes)
                    {
                        if (string.IsNullOrEmpty(childNode.InnerText))
                        {
                            continue;
                        }

                        this.m_AppliesToTypes.Add(childNode.InnerText);
                    }
                }
                else if (xmlNodes.FirstChild.Attributes["AppliesTo"] != null)
                {
                    this.m_AppliesToTypes.Add(xmlNodes.FirstChild.Attributes["AppliesTo"].Value);
                }

                if (xmlNodes.FirstChild.Attributes["CultureInfoName"] != null)
                {
                    cultureInfo =
                        new System.Globalization.CultureInfo(xmlNodes.FirstChild.Attributes["CultureInfoName"].Value);
                }
                else
                {
                    cultureInfo = null;
                }

                this.m_cultureInfo = cultureInfo;
            }
        }

        private static string GetClassName(object component)
        {
            string className = TypeDescriptor.GetClassName(component);
            return className.Substring(className.IndexOf("+") + 1);
        }

        public string GetExpressionString()
        {
            return this.ToString();
        }

        public virtual string GetLogicString()
        {
            string[] property = new string[] { this.Property, " ", this.Operand.ToString(), " ", this.Pattern };
            return string.Concat(property);
        }

        private PropertyDescriptor GetPropertyDescriptor(object component)
        {
            string lower = this.m_sProperty.ToLower();
            PropertyDescriptor propertyDescriptor = null;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component.GetType());
            if (this.MatchPropertyDescriptor(properties,
                    (PropertyDescriptor descriptor) => descriptor.Name.ToLower() == lower, out propertyDescriptor))
            {
                return propertyDescriptor;
            }

            PropertyDescriptorCollection propertyDescriptorCollections = TypeDescriptor.GetProperties(component);
            if (this.MatchPropertyDescriptor(propertyDescriptorCollections,
                    (PropertyDescriptor descriptor) => descriptor.Name.ToLower() == lower, out propertyDescriptor))
            {
                return propertyDescriptor;
            }

            if (this.MatchPropertyDescriptor(properties, (PropertyDescriptor descriptor) =>
                {
                    if (string.IsNullOrEmpty(descriptor.DisplayName))
                    {
                        return false;
                    }

                    return descriptor.DisplayName.ToLower() == lower;
                }, out propertyDescriptor))
            {
                return propertyDescriptor;
            }

            if (this.MatchPropertyDescriptor(propertyDescriptorCollections, (PropertyDescriptor descriptor) =>
                {
                    if (string.IsNullOrEmpty(descriptor.DisplayName))
                    {
                        return false;
                    }

                    return descriptor.DisplayName.ToLower() == lower;
                }, out propertyDescriptor))
            {
                return propertyDescriptor;
            }

            return null;
        }

        protected object GetValue(object component, out PropertyDescriptor property)
        {
            property = null;
            if (component == null)
            {
                return null;
            }

            property = this.GetPropertyDescriptor(component);
            if (property == null)
            {
                return null;
            }

            return property.GetValue(component);
        }

        public static bool IsNegation(FilterOperand filterOperand)
        {
            FieldInfo[] fields = filterOperand.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
            int num = 0;
            while (true)
            {
                if (num < (int)fields.Length)
                {
                    FieldInfo fieldInfo = fields[num];
                    string str = fieldInfo.ToString();
                    char[] chrArray = new char[] { ' ' };
                    if (str.Split(chrArray)[1] == filterOperand.ToString())
                    {
                        object[] customAttributes = fieldInfo.GetCustomAttributes(true);
                        if ((int)customAttributes.Length != 1)
                        {
                            break;
                        }

                        return ((IsNegationAttribute)customAttributes[0]).IsNegation;
                    }
                    else
                    {
                        num++;
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        private bool MatchPropertyDescriptor(PropertyDescriptorCollection collection,
            Predicate<PropertyDescriptor> matchCondition, out PropertyDescriptor property)
        {
            bool flag;
            IEnumerator enumerator = collection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PropertyDescriptor current = (PropertyDescriptor)enumerator.Current;
                    if (!matchCondition(current))
                    {
                        continue;
                    }

                    property = current;
                    flag = true;
                    return flag;
                }

                property = null;
                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        public static FilterExpressionList operator &(FilterExpression filter1, IFilterExpression filter2)
        {
            FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.And);
            if (filter1 != null)
            {
                filterExpressionList.Add(filter1);
            }

            if (filter2 != null)
            {
                filterExpressionList.Add(filter2);
            }

            return filterExpressionList;
        }

        public static FilterExpressionList operator |(FilterExpression filter1, IFilterExpression filter2)
        {
            FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.Or);
            if (filter1 != null)
            {
                filterExpressionList.Add(filter1);
            }

            if (filter2 != null)
            {
                filterExpressionList.Add(filter2);
            }

            return filterExpressionList;
        }

        public static IFilterExpression ParseExpression(string sExpression)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sExpression);
            return FilterExpression.ParseExpression(xmlDocument.FirstChild);
        }

        public static IFilterExpression ParseExpression(XmlNode filterXML)
        {
            IFilterExpression filterExpression = null;
            if (filterXML != null)
            {
                if (filterXML.Name == "And" || filterXML.Name == "Or")
                {
                    FilterExpressionList filterExpressionList =
                        new FilterExpressionList((filterXML.Name == "Or" ? ExpressionLogic.Or : ExpressionLogic.And));
                    foreach (XmlNode childNode in filterXML.ChildNodes)
                    {
                        filterExpressionList.Add(FilterExpression.ParseExpression(childNode));
                    }

                    if (filterXML.Attributes["IsImplicitGroup"] != null)
                    {
                        bool flag = true;
                        if (bool.TryParse(filterXML.Attributes["IsImplicitGroup"].Value, out flag))
                        {
                            filterExpressionList.IsImplicitGroup = flag;
                        }
                    }

                    return filterExpressionList;
                }

                if (!filterXML.Name.Equals("StringFilterExpression", StringComparison.OrdinalIgnoreCase))
                {
                    filterExpression = new FilterExpression(filterXML);
                }
                else
                {
                    try
                    {
                        filterExpression = new StringFilterExpression(filterXML);
                    }
                    catch
                    {
                        filterExpression = new FilterExpression(filterXML);
                    }
                }
            }

            return filterExpression;
        }

        public override string ToString()
        {
            string str = "";
            if (this.AppliesToTypes != null)
            {
                int num = 0;
                foreach (string appliesToType in this.AppliesToTypes)
                {
                    num++;
                    str = string.Concat(str,
                        (appliesToType.LastIndexOf(".") >= 0
                            ? appliesToType.Substring(appliesToType.LastIndexOf(".") + 1)
                            : appliesToType));
                    if (num >= this.AppliesToTypes.Count)
                    {
                        continue;
                    }

                    str = string.Concat(str, " And ");
                }
            }
            else
            {
                str = "Any";
            }

            object[] property = new object[] { str, this.Property, this.Operand.ToString(), this.Pattern };
            return string.Format("Filter {0} where {1} {2} {3}", property);
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXML(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public virtual void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("FilterExpression");
            xmlWriter.WriteStartElement(this.Operand.ToString());
            xmlWriter.WriteAttributeString("Property", this.Property);
            xmlWriter.WriteAttributeString("CaseSensitive", this.IsCaseSensitive.ToString());
            xmlWriter.WriteAttributeString("BaseFilter", this.IsBaseFilter.ToString());
            if (this.Pattern != null)
            {
                xmlWriter.WriteAttributeString("Pattern", this.Pattern);
            }

            if (this.m_cultureInfo != null)
            {
                xmlWriter.WriteAttributeString("CultureInfoName", this.m_cultureInfo.Name);
            }

            if (this.AppliesToTypes != null && this.AppliesToTypes.Count > 0)
            {
                xmlWriter.WriteStartElement("AppliesToTypes");
                foreach (string appliesToType in this.AppliesToTypes)
                {
                    xmlWriter.WriteElementString("Type", appliesToType);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
}