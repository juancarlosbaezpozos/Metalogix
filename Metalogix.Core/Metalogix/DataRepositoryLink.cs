using Metalogix.DataResolution;
using Metalogix.ObjectResolution;
using System;
using System.Xml;

namespace Metalogix
{
    public class DataRepositoryLink : ObjectLink<DataResolver>
    {
        public DataRepositoryLink()
        {
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("DataRepositoryLink");
            xmlWriter.WriteEndElement();
        }
    }
}