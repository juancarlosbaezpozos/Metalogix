using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Metalogix.Actions.Remoting.MappingFiles
{
    public class Mapping
    {
        private List<string> _files = new List<string>();

        [XmlElement("File")]
        public List<string> Files
        {
            get { return this._files; }
            set { this._files = value; }
        }

        [XmlAttribute] public string Name { get; set; }

        public Mapping()
        {
        }
    }
}