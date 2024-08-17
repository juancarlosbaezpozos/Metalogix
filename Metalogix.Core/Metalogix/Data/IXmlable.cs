using System;
using System.Xml;

namespace Metalogix.Data
{
    public interface IXmlable
    {
        string ToXML();

        void ToXML(XmlWriter xmlWriter);
    }
}