using Bluethread.SharePoint.StoragePoint;
using Metalogix.SharePoint.Adapters;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.StoragePoint
{
    public class StoragePointSupport : IStoragePointSupport
    {
        public StoragePointSupport()
        {
        }

        public object AddDocumentToEndpoint(string sFileUrl, byte[] contents)
        {
            return MigrationSupport.AddDocumentToEndpoint(sFileUrl, contents);
        }

        public object ConfigureFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            EndpointAPI endpointAPI;
            return MigrationSupport.ConfigureFileShareEndpointAndProfile(sNetworkPath, sSharePointPath,
                out endpointAPI);
        }

        public object FindFileSystemEndpointForPath(object oProfile, string sNetworkPath)
        {
            ProfileAPI profileAPI = oProfile as ProfileAPI;
            if (profileAPI == null)
            {
                throw new ArgumentException("Parameter must be of type Bluethread.SharePoint.StoragePoint.ProfileAPI",
                    "oProfile");
            }

            ProfileEndpointAPI profileEndpointAPI = null;
            foreach (ProfileEndpointAPI profileEndpoint in profileAPI.ProfileEndpoints)
            {
                if (!(profileEndpoint.AdapterName == "FileSystem") || !sNetworkPath.StartsWith(
                        profileEndpoint.Connection.Substring("path=".Length),
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                profileEndpointAPI = profileEndpoint;
                break;
            }

            return profileEndpointAPI;
        }

        public object GenerateBLOBReference(object oProfile, object oFileSystemEndpoint, string sNetworkPath)
        {
            ProfileAPI profileAPI = oProfile as ProfileAPI;
            if (profileAPI == null)
            {
                throw new ArgumentException("Parameter must be of type Bluethread.SharePoint.StoragePoint.ProfileAPI",
                    "oProfile");
            }

            EndpointAPI endpointAPI = oFileSystemEndpoint as EndpointAPI;
            if (endpointAPI == null)
            {
                throw new ArgumentException("Parameter must be of type Bluethread.SharePoint.StoragePoint.EndpointAPI",
                    "oFileSystemEndpoint");
            }

            return MigrationSupport.GenerateBLOBRef(profileAPI, endpointAPI, sNetworkPath);
        }

        public object GetProfileConfiguration(string sSharePointPath)
        {
            if (string.IsNullOrEmpty(sSharePointPath))
            {
                return null;
            }

            return MigrationSupport.GetProfileConfiguration(sSharePointPath);
        }

        public bool ProfileConfigured(string sSharePointPath)
        {
            return MigrationSupport.ProfileConfigured(sSharePointPath);
        }

        public string SerializeProfileConfiguration(object oProfile)
        {
            ProfileAPI profileAPI = oProfile as ProfileAPI;
            StringWriter stringWriter = new StringWriter(new StringBuilder());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            try
            {
                StoragePointSupport.WriteStoragePointAPIClassElement(ref xmlTextWriter, profileAPI);
            }
            finally
            {
                xmlTextWriter.Close();
            }

            return stringWriter.ToString();
        }

        public void SetBLOBReference(object oBlobRef, object oSPListItem)
        {
            BlobReferenceAPI blobReferenceAPI = oBlobRef as BlobReferenceAPI;
            if (blobReferenceAPI == null)
            {
                throw new ArgumentException(
                    "Parameter must be of type Bluethread.SharePoint.StoragePoint.BlobReferenceAPI", "oBlobRef");
            }

            SPListItem sPListItem = oSPListItem as SPListItem;
            if (sPListItem == null)
            {
                throw new ArgumentException("Parameter must be of type Microsoft.SharePoint.SPListItem", "oSPListItem");
            }

            MigrationSupport.SetBLOBRef(blobReferenceAPI, sPListItem);
        }

        private static void WriteStoragePointAPIClassElement(ref XmlTextWriter xmlWriter,
            ProfileEndpointAPI profileEndpoint)
        {
            if (profileEndpoint == null)
            {
                return;
            }

            xmlWriter.WriteStartElement(StoragePointClassType.ProfileEndpoint.ToString());
            StoragePointSupport.WriteStoragePointAPIClassFields(ref xmlWriter, profileEndpoint);
            xmlWriter.WriteEndElement();
        }

        private static void WriteStoragePointAPIClassElement(ref XmlTextWriter xmlWriter,
            ProfileEndpointListAPI profileEndpointList)
        {
            xmlWriter.WriteStartElement(StoragePointClassType.ProfileEndpointList.ToString());
            if (profileEndpointList != null)
            {
                StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Count,
                    profileEndpointList.Count);
                foreach (ProfileEndpointAPI profileEndpointAPI in profileEndpointList)
                {
                    StoragePointSupport.WriteStoragePointAPIClassElement(ref xmlWriter, profileEndpointAPI);
                }
            }

            xmlWriter.WriteEndElement();
        }

        private static void WriteStoragePointAPIClassElement(ref XmlTextWriter xmlWriter,
            ProfileArchiveSettingsAPI archiveSettings)
        {
            xmlWriter.WriteStartElement(StoragePointClassType.ProfileArchiveSettings.ToString());
            if (archiveSettings != null)
            {
                StoragePointSupport.WriteStoragePointAPIClassFields(ref xmlWriter, archiveSettings);
            }

            xmlWriter.WriteEndElement();
        }

        private static void WriteStoragePointAPIClassElement(ref XmlTextWriter xmlWriter, ProfileAPI profile)
        {
            xmlWriter.WriteStartElement(StoragePointClassType.Profile.ToString());
            if (profile != null)
            {
                StoragePointSupport.WriteStoragePointAPIClassFields(ref xmlWriter, profile);
                StoragePointSupport.WriteStoragePointAPIClassElement(ref xmlWriter, profile.ArchivingSettings);
                StoragePointSupport.WriteStoragePointAPIClassElement(ref xmlWriter, profile.ProfileEndpoints);
            }

            xmlWriter.WriteEndElement();
        }

        private static void WriteStoragePointAPIClassFields(ref XmlTextWriter xmlWriter, EndpointAPI endpoint)
        {
            if (endpoint == null)
            {
                return;
            }

            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.AdapterName,
                endpoint.AdapterName);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Availablespace,
                endpoint.Availablespace);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Connection,
                endpoint.Connection);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.EncryptionProviderId,
                endpoint.EncryptionProviderId);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.EndpointId,
                endpoint.EndpointId);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.EndpointState,
                endpoint.EndpointState);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.EndpointType,
                endpoint.EndpointType);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ErrorCount,
                endpoint.ErrorCount);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ErrorCountThreshold,
                endpoint.ErrorCountThreshold);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter,
                StoragePointField.ErrorCountWarningThreshold, endpoint.ErrorCountWarningThreshold);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Foldering,
                endpoint.Foldering);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.FolderingLevel,
                endpoint.FolderingLevel);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Freespace,
                endpoint.Freespace);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter,
                StoragePointField.FreespacePercentThreshold, endpoint.FreespacePercentThreshold);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter,
                StoragePointField.FreespacePercentWarningThreshold, endpoint.FreespacePercentWarningThreshold);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.FreespaceThreshold,
                endpoint.FreespaceThreshold);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter,
                StoragePointField.FreespaceWarningThreshold, endpoint.FreespaceWarningThreshold);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.HandleMissingMetadata,
                endpoint.HandleMissingMetadata);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.IsNew, endpoint.IsNew);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Name, endpoint.Name);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.OfflineForFreespace,
                endpoint.OfflineForFreespace);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.OtherNotify,
                endpoint.OtherNotify);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Status,
                endpoint.Status);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.StoreSourceMetadata,
                endpoint.StoreSourceMetadata);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.UseCompression,
                endpoint.UseCompression);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.UseDefaultContacts,
                endpoint.UseDefaultContacts);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.UseEncryption,
                endpoint.UseEncryption);
        }

        private static void WriteStoragePointAPIClassFields(ref XmlTextWriter xmlWriter,
            ProfileEndpointAPI profileEndpoint)
        {
            if (profileEndpoint == null)
            {
                return;
            }

            StoragePointSupport.WriteStoragePointAPIClassFields(ref xmlWriter, (EndpointAPI)profileEndpoint);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.AsyncPromoteExtension,
                profileEndpoint.AsyncPromoteExtension);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.AsyncPromoteFilename,
                profileEndpoint.AsyncPromoteFilename);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.AsyncPromoteFolder,
                profileEndpoint.AsyncPromoteFolder);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ProfileEndPointType,
                profileEndpoint.EndPointType);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.FileSize,
                profileEndpoint.FileSize);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.FileSizeOp,
                profileEndpoint.FileSizeOp);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.FileTypeOp,
                profileEndpoint.FileTypeOp);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.FileTypes,
                profileEndpoint.FileTypes);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.IsActive,
                profileEndpoint.IsActive);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ScopeOp,
                profileEndpoint.ScopeOp);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Scopes,
                profileEndpoint.Scopes);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.StartFolder,
                profileEndpoint.StartFolder);
        }

        private static void WriteStoragePointAPIClassFields(ref XmlTextWriter xmlWriter,
            ProfileArchiveSettingsAPI archivingSettings)
        {
            if (archivingSettings == null)
            {
                return;
            }

            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter,
                StoragePointField.AllowRuleDefinitionFromIMP, archivingSettings.AllowRuleDefinitionFromIMP);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Enabled,
                archivingSettings.Enabled);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.SelectedEndpoints,
                archivingSettings.SelectedEndpoints);
        }

        private static void WriteStoragePointAPIClassFields(ref XmlTextWriter xmlWriter, ProfileAPI profile)
        {
            if (profile == null)
            {
                return;
            }

            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.BlobRetentionDays,
                profile.BlobRetentionDays);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.CabinetLevel,
                profile.CabinetLevel);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.CabinetOn,
                profile.CabinetOn);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ProfileId,
                profile.ProfileId);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.IsActive,
                profile.IsActive);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.IsNew, profile.IsNew);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Name, profile.Name);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ScopeId,
                profile.ScopeId);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.SelectEndpointAsync,
                profile.SelectEndpointAsync);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.ProfileState,
                profile.ProfileState);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.Type, profile.Type);
            StoragePointSupport.WriteStoragePointFieldAttribute(ref xmlWriter, StoragePointField.UseRBS,
                profile.UseRBS);
        }

        private static void WriteStoragePointFieldAttribute(ref XmlTextWriter xWriter, StoragePointField field,
            object oValue)
        {
            string str = oValue as string ?? (oValue != null ? oValue.ToString() : string.Empty);
            xWriter.WriteAttributeString(field.ToString(), str);
        }
    }
}