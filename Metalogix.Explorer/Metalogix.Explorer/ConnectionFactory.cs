using Metalogix;
using System;
using System.Xml;

namespace Metalogix.Explorer
{
    public static class ConnectionFactory
    {
        public static Connection CreateConnection(string sConnectionStringXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sConnectionStringXML);
            return ConnectionFactory.CreateConnection(xmlDocument.FirstChild);
        }

        public static Connection CreateConnection(XmlNode connectionNode)
        {
            if (connectionNode.Attributes["NodeType"] == null)
            {
                throw new Exception("No NodeType parameter was specfified in the connection string");
            }

            string value = connectionNode.Attributes["NodeType"].Value;
            Type type = Type.GetType(TypeUtils.UpdateType(value));
            if (type == null)
            {
                throw new Exception(string.Concat("Could not load type: ", value));
            }

            object[] objArray = new object[] { connectionNode };
            object obj = Activator.CreateInstance(type, objArray);
            if (!(obj is Connection))
            {
                throw new Exception("The created type was not a Metalogix.Explorer.Connection");
            }

            return (Connection)obj;
        }
    }
}