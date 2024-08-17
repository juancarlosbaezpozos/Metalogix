using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class UpdateItemOptions : AdapterOptions
    {
        [DataMember]
        public bool ShallowCopyExternalizedData
        {
            get { return false; }
            set { }
        }

        public UpdateItemOptions()
        {
        }
    }
}