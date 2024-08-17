using Metalogix.Actions;
using Metalogix.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.ObjectResolution.Interfaces
{
    public interface IXMLableLinkList : IXMLAbleList, IXmlable, IEnumerable
    {
        List<IObjectLink> GetObjectLinks();

        void LinksFromXML(XmlNode node);
    }
}