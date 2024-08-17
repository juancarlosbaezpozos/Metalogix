using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IServiceManagerChannel :
        Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager.IServiceManager, IClientChannel,
        IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>, IDisposable
    {
    }
}