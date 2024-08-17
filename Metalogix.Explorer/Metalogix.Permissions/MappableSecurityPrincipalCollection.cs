using System;
using System.Xml;

namespace Metalogix.Permissions
{
    public class MappableSecurityPrincipalCollection : SecurityPrincipalCollection
    {
        public MappableSecurityPrincipalCollection(XmlNode xml) : base(xml)
        {
        }
    }
}