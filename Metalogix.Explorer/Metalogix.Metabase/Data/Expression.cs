using Metalogix.Metabase.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Data
{
    public class Expression : IXmlableV3
    {
        private char[] sepChars = new char[] { ' ', ',', ';' };

        private string m_sTokens = string.Empty;

        private ExpressionCondition m_condition;

        private bool m_bMatchCase;

        public ExpressionCondition Condition
        {
            get { return this.m_condition; }
            set { this.m_condition = value; }
        }

        public string DisplayName
        {
            get
            {
                string[] enumDescription = new string[]
                    { EnumDescriptions.GetEnumDescription(this.m_condition), ": [", this.Tokens, "]", null };
                enumDescription[4] = (this.MatchCase ? " - Match case" : "");
                return string.Concat(enumDescription);
            }
        }

        public bool MatchCase
        {
            get { return this.m_bMatchCase; }
            set { this.m_bMatchCase = value; }
        }

        public string Tokens
        {
            get { return this.m_sTokens; }
            set
            {
                if (value == null || string.IsNullOrEmpty(value.Trim()))
                {
                    this.m_sTokens = string.Empty;
                    return;
                }

                string[] strArrays = value.Split(this.sepChars);
                string empty = string.Empty;
                string[] strArrays1 = strArrays;
                for (int i = 0; i < (int)strArrays1.Length; i++)
                {
                    string str = strArrays1[i];
                    if (!string.IsNullOrEmpty(str.Trim()))
                    {
                        empty = string.Concat(empty, (!string.IsNullOrEmpty(empty) ? ", " : string.Empty), str);
                    }
                }

                this.m_sTokens = empty;
            }
        }

        public Expression()
        {
        }

        public Expression(XmlNode nodeExpression)
        {
            this.FromXml(nodeExpression);
        }

        public override bool Equals(object obj)
        {
            Expression expression = obj as Expression;
            if (expression == null)
            {
                return false;
            }

            if (this.Tokens != expression.Tokens)
            {
                return false;
            }

            return this.Condition == expression.Condition;
        }

        public bool Evaluate(string sValue)
        {
            if (string.IsNullOrEmpty(sValue))
            {
                return true;
            }

            string str = (!this.MatchCase ? sValue.ToLower() : sValue);
            string[] strArrays = ((!this.MatchCase ? this.Tokens.ToLower() : this.Tokens)).Split(this.sepChars);
            bool flag = false;
            bool flag1 = false;
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str1 = strArrays1[i];
                if (str1.Length > 0)
                {
                    if (str.IndexOf(str1) < 0)
                    {
                        flag1 = true;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }

            switch (this.Condition)
            {
                case ExpressionCondition.MustContainAll:
                {
                    return !flag1;
                }
                case ExpressionCondition.MustNotContainAny:
                {
                    return !flag;
                }
                case ExpressionCondition.MustContainAny:
                {
                    return flag;
                }
            }

            return true;
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

            if (node.Name != "Expression")
            {
                node = node.SelectSingleNode("./Expression");
                if (node == null)
                {
                    return;
                }
            }

            XmlNode xmlNodes = node.SelectSingleNode("./Tokens");
            if (xmlNodes != null && xmlNodes.HasChildNodes && xmlNodes.FirstChild is XmlCDataSection)
            {
                XmlCDataSection firstChild = xmlNodes.FirstChild as XmlCDataSection;
                if (firstChild != null)
                {
                    this.m_sTokens = firstChild.InnerText;
                }
            }

            XmlNode xmlNodes1 = node.SelectSingleNode("./Condition");
            if (xmlNodes1 != null)
            {
                string innerText = xmlNodes1.InnerText;
                string str = innerText;
                if (innerText != null)
                {
                    if (str == "MustContainAll")
                    {
                        this.m_condition = ExpressionCondition.MustContainAll;
                    }
                    else if (str == "MustContainAny")
                    {
                        this.m_condition = ExpressionCondition.MustContainAny;
                    }
                    else if (str == "MustNotContainAny")
                    {
                        this.m_condition = ExpressionCondition.MustNotContainAny;
                    }
                }
            }

            XmlNode xmlNodes2 = node.SelectSingleNode("./MatchCase");
            if (xmlNodes2 != null)
            {
                bool.TryParse(xmlNodes2.InnerText, out this.m_bMatchCase);
            }
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public bool IsEqual(IXmlableV3 xmlable)
        {
            if (xmlable == null)
            {
                return false;
            }

            return this.ToXml() == xmlable.ToXml();
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXml(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Expression");
            writer.WriteStartElement("Tokens");
            writer.WriteCData(this.m_sTokens);
            writer.WriteEndElement();
            writer.WriteStartElement("Condition");
            switch (this.m_condition)
            {
                case ExpressionCondition.MustContainAll:
                {
                    writer.WriteValue("MustContainAll");
                    break;
                }
                case ExpressionCondition.MustNotContainAny:
                {
                    writer.WriteValue("MustNotContainAny");
                    break;
                }
                case ExpressionCondition.MustContainAny:
                {
                    writer.WriteValue("MustContainAny");
                    break;
                }
            }

            writer.WriteEndElement();
            writer.WriteElementString("MatchCase", this.m_bMatchCase.ToString());
            writer.WriteEndElement();
        }

        public struct XmlNames
        {
            public const string Expression = "Expression";

            public const string Condition = "Condition";

            public const string MustContainAll = "MustContainAll";

            public const string MustNotContainAny = "MustNotContainAny";

            public const string MustContainAny = "MustContainAny";

            public const string MatchCase = "MatchCase";

            public const string Tokens = "Tokens";
        }
    }
}