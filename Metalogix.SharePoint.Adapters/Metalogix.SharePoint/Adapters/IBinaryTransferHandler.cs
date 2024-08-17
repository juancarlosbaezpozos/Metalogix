using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface IBinaryTransferHandler
    {
        [OperationContract]
        string CloseFileCopySession(Guid sessionId);

        [OperationContract]
        Guid OpenFileCopySession(StreamType streamType, int retentionTime);

        [OperationContract]
        byte[] ReadChunk(Guid sessionId, long bytesToRead);

        [OperationContract]
        void WriteChunk(Guid sessionId, byte[] data);
    }
}