using System;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.Aspects
{
    public class NotCSOMClientAdapterException : NotSupportedException
    {
        public NotCSOMClientAdapterException() : base("Aspect can only be used for type 'CSOMClientAdapter'.")
        {
        }
    }
}