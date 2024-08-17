using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Data
{
    public class ExpressionList : IXmlableV3, IEnumerable<Expression>, IEnumerable
    {
        private List<Expression> m_data = new List<Expression>();

        public int Count
        {
            get { return this.m_data.Count; }
        }

        public Expression this[int iIndex]
        {
            get { return this.m_data[iIndex]; }
        }

        public ExpressionList()
        {
        }

        public void Append(Expression expr)
        {
            this.m_data.Add(expr);
        }

        public bool Contains(Expression expr)
        {
            return this.m_data.Contains(expr);
        }

        public bool EvaluateAll(string sValue)
        {
            bool flag;
            List<Expression>.Enumerator enumerator = this.m_data.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Evaluate(sValue))
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }

                return true;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return flag;
        }

        public void FromXml(string sXml)
        {
            if (sXml == null)
            {
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXml(xmlDocument.DocumentElement);
        }

        public void FromXml(XmlNode node)
        {
            if (node == null)
            {
                return;
            }

            if (node.Name != "ExpressionList")
            {
                node = node.SelectSingleNode("./ExpressionList");
                if (node == null)
                {
                    return;
                }
            }

            this.m_data.Clear();
            foreach (XmlNode xmlNodes in node.SelectNodes("./Expression"))
            {
                Expression expression = new Expression(xmlNodes);
                this.m_data.Add(expression);
            }
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return this.m_data.GetEnumerator();
        }

        public int IndexOf(Expression expr)
        {
            return this.m_data.IndexOf(expr);
        }

        public bool IsEqual(IXmlableV3 xmlable)
        {
            if (xmlable == null)
            {
                return false;
            }

            return this.ToXml() == xmlable.ToXml();
        }

        public void RemoveAt(int iIndex)
        {
            this.m_data.RemoveAt(iIndex);
        }

        public void Sync(bool bEnsureExists, Expression expr)
        {
            int num = this.IndexOf(expr);
            if (bEnsureExists && num < 0)
            {
                this.Append(expr);
                return;
            }

            if (!bEnsureExists && num >= 0)
            {
                this.RemoveAt(num);
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXml(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("ExpressionList");
            foreach (Expression mDatum in this.m_data)
            {
                if (mDatum == null)
                {
                    continue;
                }

                mDatum.ToXml(writer);
            }

            writer.WriteEndElement();
        }

        private struct XmlNames
        {
            public const string ExpressionList = "ExpressionList";
        }
    }
}