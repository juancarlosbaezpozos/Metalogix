using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.Database
{
    public class SQLDatabase
    {
        public string Name { get; set; }

        public SQLDatabase(XmlNode xml)
        {
            if (xml == null)
            {
                return;
            }

            this.Name = xml.Attributes["Name"].Value;
        }

        public string GetDisplayName()
        {
            return this.Name;
        }
    }
}