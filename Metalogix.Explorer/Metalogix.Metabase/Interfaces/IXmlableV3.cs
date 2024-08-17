using System;
using System.Xml;

namespace Metalogix.Metabase.Interfaces
{
    public interface IXmlableV3
    {
        void FromXml(string sXml);

        void FromXml(XmlNode node);

        bool IsEqual(IXmlableV3 xmlable);

        string ToXml();

        void ToXml(XmlWriter writer);
    }
}