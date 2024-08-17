using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class UpdateDocumentOptions : UpdateItemOptions, IUpdateDocumentOptions
    {
        public UpdateDocumentOptions()
        {
        }
    }
}