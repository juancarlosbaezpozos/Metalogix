using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Data.Filters
{
    public class FilterExpressionList : SerializableList<IFilterExpression>, IEquatable<IFilterExpression>, IXmlable,
        IFilterExpression
    {
        private ExpressionLogic m_logic;

        private bool m_bIsImplicitGroup = true;

        public bool IsImplicitGroup
        {
            get { return this.m_bIsImplicitGroup; }
            set { this.m_bIsImplicitGroup = value; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override IFilterExpression this[IFilterExpression ifilterExpression_0]
        {
            get { throw new NotImplementedException(); }
        }

        public ExpressionLogic Logic
        {
            get { return this.m_logic; }
        }

        public FilterExpressionList(XmlNode node)
        {
            if (node != null)
            {
                this.FromXML(node);
            }
        }

        public FilterExpressionList(ExpressionLogic logic)
        {
            this.m_logic = logic;
        }

        public FilterExpressionList()
        {
        }

        public void Clear(bool removeBaseFilters)
        {
            if (removeBaseFilters)
            {
                this.ClearCollection();
                return;
            }

            for (int i = base.Count - 1; i >= 0; i--)
            {
                if (!((FilterExpression)this[i]).IsBaseFilter)
                {
                    base.RemoveIndex(i);
                }
            }
        }

        public new IFilterExpression Clone()
        {
            FilterExpressionList filterExpressionList = new FilterExpressionList(this.Logic);
            foreach (IFilterExpression filterExpression in this)
            {
                filterExpressionList.Add(filterExpression.Clone());
            }

            return filterExpressionList;
        }

        public bool Equals(IFilterExpression iFilterExpression)
        {
            if (iFilterExpression == null)
            {
                return false;
            }

            if (base.GetType() != iFilterExpression.GetType())
            {
                return false;
            }

            List<IFilterExpression> filterExpressions =
                new List<IFilterExpression>((FilterExpressionList)iFilterExpression);
            if (base.Count != filterExpressions.Count)
            {
                return false;
            }

            foreach (IFilterExpression filterExpression in this)
            {
                List<IFilterExpression>.Enumerator enumerator = filterExpressions.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            IFilterExpression current = enumerator.Current;
                            if (filterExpression.Equals(current))
                            {
                                filterExpressions.Remove(current);
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

            if (filterExpressions.Count > 0)
            {
                return false;
            }

            return true;
        }

        public virtual bool Evaluate(object objValue)
        {
            return this.Evaluate(objValue, (Comparison<object>)null);
        }

        public virtual bool Evaluate(object objValue, IComparer comparer)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.Evaluate(objValue, comparison);
        }

        public virtual bool Evaluate(object objValue, Comparison<object> comparer)
        {
            bool flag;
            if (base.Count == 0)
            {
                return true;
            }

            if (this.Logic != ExpressionLogic.And)
            {
                foreach (IFilterExpression filterExpression in this)
                {
                    if (!filterExpression.Evaluate(objValue, comparer))
                    {
                        continue;
                    }

                    flag = true;
                    return flag;
                }

                return false;
            }
            else
            {
                foreach (IFilterExpression filterExpression1 in this)
                {
                    if (filterExpression1.Evaluate(objValue, comparer))
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }

                return true;
            }

            return flag;
        }

        public override void FromXML(XmlNode xmlNode)
        {
            bool flag;
            XmlNode xmlNodes = xmlNode;
            if (xmlNodes != null)
            {
                this.m_logic = (ExpressionLogic)Enum.Parse(typeof(ExpressionLogic), xmlNodes.Name);
                if (xmlNodes.Attributes["IsImplicitGroup"] != null &&
                    bool.TryParse(xmlNodes.Attributes["IsImplicitGroup"].Value, out flag))
                {
                    this.IsImplicitGroup = flag;
                }

                foreach (XmlNode childNode in xmlNodes.ChildNodes)
                {
                    this.Add(FilterExpression.ParseExpression(childNode));
                }
            }
        }

        public string GetExpressionString()
        {
            string str = string.Concat(" ", this.Logic.ToString(), (this.Logic == ExpressionLogic.And ? "\n" : " "));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (IFilterExpression filterExpression in this)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(str);
                }

                stringBuilder.Append(filterExpression.GetExpressionString());
            }

            return stringBuilder.ToString();
        }

        public string GetLogicString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            foreach (IFilterExpression filterExpression in this)
            {
                if (stringBuilder.Length > 1)
                {
                    stringBuilder.Append(string.Concat(" ", this.Logic.ToString(), " "));
                }

                stringBuilder.Append(filterExpression.GetLogicString());
            }

            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        public static FilterExpressionList operator &(FilterExpressionList filter1, IFilterExpression filter2)
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

        public static FilterExpressionList operator |(FilterExpressionList filter1, IFilterExpression filter2)
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

        public override string ToString()
        {
            return this.Logic.ToString();
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(this.Logic.ToString());
            xmlWriter.WriteAttributeString("IsImplicitGroup", this.m_bIsImplicitGroup.ToString());
            foreach (IFilterExpression filterExpression in this)
            {
                filterExpression.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }
    }
}