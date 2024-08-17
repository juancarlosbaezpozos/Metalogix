using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ISharePointAdapterService : ISharePointReader, ISharePointWriter, IBinaryTransferHandler,
        IServerHealthMonitor, IMySitesConnector, IMigrationPipeline, ISP2013WorkflowAdapter, ISharePointAdapterCommand
    {
        [OperationContract]
        void CheckConnection();

        [OperationContract]
        void EndAdapterService();

        [OperationContract]
        string GetAdapterConfiguration();

        [OperationContract]
        IList<Cookie> GetCookieManagerCookies();

        [OperationContract]
        bool GetCookieManagerIsActive();

        [OperationContract]
        bool GetCookieManagerLocksCookies();

        [OperationContract(IsInitiating = true)]
        void InitializeAdapterConfiguration(string sXml);

        [OperationContract]
        void SetCookieManagerCookies(IList<Cookie> cookies);

        [OperationContract]
        void UpdateCookieManagerCookies();
    }
}