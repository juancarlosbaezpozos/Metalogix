using Metalogix.Data;
using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace Metalogix.Actions
{
    public interface IXMLAbleList : IXmlable, IEnumerable
    {
        Type CollectionType { get; }

        int Count { get; }

        object this[int index] { get; }

        void FromXML(XmlNode xmlNode);
    }
}