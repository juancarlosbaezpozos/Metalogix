using Metalogix;
using Metalogix.Core;
using Metalogix.Data;
using Metalogix.Explorer;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Actions
{
    public class XMLAbleList : IXMLAbleList, IXmlable, IEnumerable
    {
        private ArrayList m_data = new ArrayList();

        public Type CollectionType
        {
            get
            {
                if (this.Count == 0 || this[0] == null)
                {
                    return null;
                }

                return this[0].GetType();
            }
        }

        public int Count
        {
            get { return this.m_data.Count; }
        }

        public object this[int index]
        {
            get { return this.m_data[index]; }
        }

        public XMLAbleList()
        {
        }

        public static IXMLAbleList CreateIXMLAbleList(string sXMLableXML)
        {
            IXMLAbleList xMLAbleLists;
            IXMLAbleList xMLAbleLists1;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXMLableXML);
            XmlNode firstChild = xmlDocument.FirstChild;
            if (firstChild.Attributes["IXMLAbleType"] == null)
            {
                throw new Exception("The 'IXMLAbleType' attribute cannot be null");
            }

            Type type = Type.GetType(TypeUtils.UpdateType(firstChild.Attributes["IXMLAbleType"].Value), false, false);
            if (type == null)
            {
                throw new Exception(string.Concat("The IXMLAbleList type '",
                    firstChild.Attributes["IXMLAbleType"].Value, "' could not be found."));
            }

            try
            {
                if (type.GetConstructor(new Type[] { typeof(XmlNode) }) == null)
                {
                    xMLAbleLists = (IXMLAbleList)Activator.CreateInstance(type);
                    xMLAbleLists.FromXML(firstChild.FirstChild);
                }
                else
                {
                    object[] objArray = new object[] { firstChild.FirstChild };
                    xMLAbleLists = (IXMLAbleList)Activator.CreateInstance(type, objArray);
                }

                xMLAbleLists1 = xMLAbleLists;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Logging.LogExceptionToTextFileWithEventLogBackup(exception,
                    "Framework, Metalogix.Actions.XMLAbleList.CreateIXMLAbleList", true);
                if (exception.InnerException != null)
                {
                    throw exception.GetBaseException();
                }

                throw;
            }

            return xMLAbleLists1;
        }

        public void FromXML(XmlNode xmlNode)
        {
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_data.GetEnumerator();
        }

        public static string SerializeXMLAbleList(IXMLAbleList iXMLAbleList)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("IXMLAbleList");
            xmlTextWriter.WriteAttributeString("IXMLAbleType", iXMLAbleList.GetType().AssemblyQualifiedName);
            iXMLAbleList.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public override string ToString()
        {
            if (this.Count == 0)
            {
                return "";
            }

            if (this.Count != 1)
            {
                return "<Multiple>";
            }

            if (this[0] is Node)
            {
                return ((Node)this[0]).Location.ToString();
            }

            if (this[0] == null)
            {
                return "";
            }

            return this[0].ToString();
        }

        public string ToXML()
        {
            return "<XmlAbleList/>";
        }

        public void ToXML(XmlWriter xmlWriter)
        {
        }
    }
}