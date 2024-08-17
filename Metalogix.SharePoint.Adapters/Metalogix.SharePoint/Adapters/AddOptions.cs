using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddOptions : AdapterOptions
    {
        private bool m_bOverwrite;

        [DataMember]
        public bool Overwrite
        {
            get { return this.m_bOverwrite; }
            set { this.m_bOverwrite = value; }
        }

        public AddOptions()
        {
        }
    }
}