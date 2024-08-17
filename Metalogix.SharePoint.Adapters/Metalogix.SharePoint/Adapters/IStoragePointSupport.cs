using System;

namespace Metalogix.SharePoint.Adapters
{
    public interface IStoragePointSupport
    {
        object AddDocumentToEndpoint(string sFileUrl, byte[] contents);

        object ConfigureFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath);

        object FindFileSystemEndpointForPath(object oProfile, string sNetworkPath);

        object GenerateBLOBReference(object oProfile, object oFileSystemEndpoint, string sNetworkPath);

        object GetProfileConfiguration(string sSharePointPath);

        bool ProfileConfigured(string sSharePointPath);

        string SerializeProfileConfiguration(object oProfile);

        void SetBLOBReference(object oBlobRef, object oSPListItem);
    }
}