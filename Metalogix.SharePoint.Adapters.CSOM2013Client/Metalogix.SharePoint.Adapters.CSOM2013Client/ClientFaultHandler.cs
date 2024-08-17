using Metalogix.SharePoint.Adapters;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client
{
    public class ClientFaultHandler : IClientMessageInspector
    {
        public ClientFaultHandler()
        {
        }

        [DebuggerStepThrough]
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.Headers.Action.Contains("ExceptionThrown"))
            {
                ExceptionFault body = reply.GetBody<ExceptionFault>();
                if (body != null)
                {
                    throw new ServiceError(body);
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }
    }
}