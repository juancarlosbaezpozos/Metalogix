using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface ISharePointAdapterServiceChannel :
        Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService.ISharePointAdapterService, IClientChannel,
        IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>, IDisposable
    {
    }
}