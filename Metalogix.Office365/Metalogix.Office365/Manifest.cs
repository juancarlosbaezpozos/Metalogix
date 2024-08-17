using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Office365
{
    public class Manifest : IManifestBuilder
    {
        private DirectoryInfo _targetDirectory;

        private PackagingStatus _packagingStatus;

        private Guid _siteId;

        private Guid _webId;

        private Guid _listId;

        private Guid _rootWebFolderId;

        private Guid _listFolderId;

        private Guid _attachmentsFolderId;

        private string _baseDirectory = "/";

        private string _parentWebAbsoluteUrl;

        private string _parentWebServerRelativeUrl;

        private string _targetBasePath;

        private string _targetRootRelativeUrl;

        private string _targetWebRelativeUrl;

        private int _fileNumber;

        private XmlElement _rootManifestXMLElement;

        private XmlElement _rootRoleAssignmentXMLElement;

        private XmlElement _deploymentRoleAssignmentElement;

        private string _listName;

        private string _listTitle;

        private string _tempStorageDirectory = string.Empty;

        private string _commonDocumentBinaryDirectory = string.Empty;

        private ICommonGlobalManifestOperations _commonGlobalManifestOperations;

        private DateTime _targetFolderCreated;

        private DateTime _targetFolderLastModified;

        private string _listBaseTemplate;

        private string _listBaseType;

        private string _webTemplate;

        private bool _isMySiteTemplate;

        private XmlNode _pointerToAddDependencyFolders;

        private List<string> _folderIdsAddedInManifest = new List<string>();

        private List<ManifestAlert> _alerts = new List<ManifestAlert>();

        public string TempStorageDirectory
        {
            get { return this._tempStorageDirectory; }
        }

        public PackagingStatus Status
        {
            get { return this._packagingStatus; }
        }

        private XmlDocument CreateSPMigrationPackageRequirements(DirectoryInfo targetDirectory,
            ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("Requirements");
            xmlElement.SetAttribute("xmlns", "urn:deployment-requirements-schema");
            xmlDocument.AppendChild(xmlElement);
            string text = targetDirectory.FullName + "\\Requirements.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("Requirements.xml", text);
            return xmlDocument;
        }

        private XmlDocument CreateSPMigrationPackageSystemData(DirectoryInfo targetDirectory, string targetParentPath,
            string parentWebURL, Guid webId, string listUrl, Guid listId, Guid webFolderId,
            ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("SystemData");
            xmlElement.SetAttribute("xmlns", "urn:deployment-systemdata-schema");
            xmlDocument.AppendChild(xmlElement);
            XmlElement xmlElement2 = xmlDocument.CreateElement("SchemaVersion");
            xmlElement2.SetAttribute("Version", "15.0.0.0");
            xmlElement2.SetAttribute("Build", "16.0.3111.1200");
            xmlElement2.SetAttribute("DatabaseVersion", "11552");
            xmlElement2.SetAttribute("SiteVersion", "15");
            xmlElement2.SetAttribute("ObjectsProcessed", this._packagingStatus.ObjectCount.ToString());
            xmlElement.AppendChild(xmlElement2);
            XmlElement xmlElement3 = xmlDocument.CreateElement("ManifestFiles");
            xmlElement.AppendChild(xmlElement3);
            XmlElement xmlElement4 = xmlDocument.CreateElement("ManifestFile");
            xmlElement4.SetAttribute("Name", "Manifest.xml");
            xmlElement3.AppendChild(xmlElement4);
            XmlElement xmlElement5 = xmlDocument.CreateElement("SystemObjects");
            xmlElement.AppendChild(xmlElement5);
            XmlElement xmlElement6 = xmlDocument.CreateElement("SystemObject");
            xmlElement6.SetAttribute("Id", webFolderId.ToString());
            xmlElement6.SetAttribute("Type", "Folder");
            xmlElement6.SetAttribute("Url", parentWebURL);
            xmlElement5.AppendChild(xmlElement6);
            XmlElement xmlElement7 = xmlDocument.CreateElement("SystemObject");
            xmlElement7.SetAttribute("Id", webId.ToString());
            xmlElement7.SetAttribute("Type", "Web");
            xmlElement7.SetAttribute("Url", parentWebURL);
            xmlElement5.AppendChild(xmlElement7);
            XmlElement xmlElement8 = xmlDocument.CreateElement("SystemObject");
            xmlElement8.SetAttribute("Id", listId.ToString());
            xmlElement8.SetAttribute("Type", "List");
            xmlElement8.SetAttribute("Url", listUrl);
            xmlElement5.AppendChild(xmlElement8);
            XmlElement xmlElement9 = xmlDocument.CreateElement("SystemObject");
            xmlElement9.SetAttribute("Id", Guid.NewGuid().ToString());
            xmlElement9.SetAttribute("Type", "List");
            xmlElement9.SetAttribute("Url", parentWebURL.TrimEnd(new char[]
            {
                '/'
            }) + "/_catalog/users");
            xmlElement5.AppendChild(xmlElement9);
            XmlElement newChild2 = xmlDocument.CreateElement("RootWebOnlyLists");
            xmlElement.AppendChild(newChild2);
            string text = targetDirectory.FullName + "\\SystemData.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("SystemData.xml", text);
            return xmlDocument;
        }

        private XmlDocument CreateSPMigrationPackageRootObjectMap(DirectoryInfo targetDirectory,
            string targetParentPath, string parentWebURL, Guid webId, Guid listId, ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("RootObjects");
            xmlElement.SetAttribute("xmlns", "urn:deployment-rootobjectmap-schema");
            xmlDocument.AppendChild(xmlElement);
            XmlElement xmlElement2 = xmlDocument.CreateElement("RootObject");
            xmlElement2.SetAttribute("Id", listId.ToString());
            xmlElement2.SetAttribute("Type", "List");
            xmlElement2.SetAttribute("ParentId", webId.ToString());
            xmlElement2.SetAttribute("WebUrl", parentWebURL);
            xmlElement2.SetAttribute("Url", parentWebURL.TrimEnd(new char[]
            {
                '/'
            }) + '/' + targetParentPath);
            xmlElement2.SetAttribute("IsDependency", "false");
            xmlElement.AppendChild(xmlElement2);
            string text = targetDirectory.FullName + "\\RootObjectMap.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("RootObjectMap.xml", text);
            return xmlDocument;
        }

        private XmlDocument CreateSPMigrationPackageLookupListMap(DirectoryInfo targetDirectory,
            ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("LookupLists");
            xmlElement.SetAttribute("xmlns", "urn:deployment-lookuplistmap-schema");
            xmlDocument.AppendChild(xmlElement);
            string text = targetDirectory.FullName + "\\LookupListMap.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("LookupListMap.xml", text);
            return xmlDocument;
        }

        private XmlDocument CreateSPMigrationPackageViewFormsList(DirectoryInfo targetDirectory,
            ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("ViewFormsList");
            xmlElement.SetAttribute("xmlns", "urn:deployment-viewformlist-schema");
            xmlDocument.AppendChild(xmlElement);
            string text = targetDirectory.FullName + "\\ViewFormsList.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("ViewFormsList.xml", text);
            return xmlDocument;
        }

        private XmlDocument CreateSPMigrationPackageExportSettings(DirectoryInfo targetDirectory, string parentWebURL,
            Guid webId, Guid listId, ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("ExportSettings");
            xmlElement.SetAttribute("xmlns", "urn:deployment-exportsettings-schema");
            xmlElement.SetAttribute("SiteUrl", parentWebURL);
            xmlElement.SetAttribute("IncludeSecurity", "All");
            xmlDocument.AppendChild(xmlElement);
            string text = targetDirectory.FullName + "\\ExportSettings.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("ExportSettings.xml", text);
            return xmlDocument;
        }

        private XmlDocument CreateSPMigrationPackageUserGroup(DirectoryInfo targetDirectory,
            ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("UserGroupMap");
            xmlElement.SetAttribute("xmlns", "urn:deployment-usergroupmap-schema");
            xmlDocument.AppendChild(xmlElement);
            string allUsersXml = this._commonGlobalManifestOperations.GetAllUsersXml();
            XmlDocumentFragment xmlDocumentFragment = xmlDocument.CreateDocumentFragment();
            xmlDocumentFragment.InnerXml = allUsersXml;
            xmlElement.AppendChild(xmlDocumentFragment);
            string allGroupsXml = this._commonGlobalManifestOperations.GetAllGroupsXml();
            XmlDocumentFragment xmlDocumentFragment2 = xmlDocument.CreateDocumentFragment();
            xmlDocumentFragment2.InnerXml = allGroupsXml;
            xmlElement.AppendChild(xmlDocumentFragment2);
            string text = targetDirectory.FullName + "\\UserGroup.xml";
            xmlDocument.Save(text);
            packagingStatus.AddManifest("UserGroup.xml", text);
            return xmlDocument;
        }

        private XmlElement CreateSPMigrationPackageManifest(Guid siteId, ref PackagingStatus packagingStatus)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("SPObjects");
            xmlElement.SetAttribute("xmlns", "urn:deployment-manifest-schema");
            xmlDocument.AppendChild(xmlElement);
            return xmlElement;
        }

        private void ValidateSPFilePathInfo(string dirName, string leafName, string serverRelativeURL, string fileType,
            PackagingStatus packagingStatus)
        {
            int num = Encoding.UTF8.GetBytes(dirName).Length;
            int num2 = Encoding.UTF8.GetBytes(leafName).Length;
            int num3 = Encoding.UTF8.GetBytes(serverRelativeURL).Length;
            List<string> list = new List<string>
            {
                ".jason",
                ".rtf"
            };
            if (num > packagingStatus.MaxDirNameLength)
            {
                packagingStatus.MaxDirNameLength = num;
            }

            if (num2 > packagingStatus.MaxFileNameLength)
            {
                packagingStatus.MaxFileNameLength = num2;
            }

            if (num3 > packagingStatus.MaxURLLength)
            {
                packagingStatus.MaxURLLength = num3;
            }

            if (num > 128)
            {
                packagingStatus.DirNameLengthWarnings++;
                packagingStatus.Warnings++;
            }

            if (num2 > 128)
            {
                packagingStatus.FileNameLengthWarnings++;
                packagingStatus.Warnings++;
            }

            if (num3 > 260)
            {
                packagingStatus.URLLengthWarnings++;
                packagingStatus.Warnings++;
            }

            if (fileType != null && list.Contains(fileType.ToLowerInvariant()))
            {
                packagingStatus.FileTypeWarnings++;
                packagingStatus.Warnings++;
            }

            if (leafName.Contains(".."))
            {
                packagingStatus.FileNameWarnings++;
                packagingStatus.Warnings++;
            }
        }

        private string XmlDateTime(DateTime dateTime)
        {
            string str = XmlConvert.ToString(dateTime, "yyyy-MM-dd");
            string str2 = XmlConvert.ToString(dateTime, "HH:mm:ss");
            return str + "T" + str2;
        }

        public void AddListItemToManifest(ManifestListItem manifestListItem)
        {
            string targetParentPath = manifestListItem.TargetParentPath;
            string text = string.Concat(new string[]
            {
                this._parentWebServerRelativeUrl.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                targetParentPath.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                manifestListItem.Filename
            });
            string value = targetParentPath.TrimEnd(new char[]
            {
                '/'
            }) + "/" + manifestListItem.Filename;
            string value2 = (this._parentWebServerRelativeUrl.TrimEnd(new char[]
            {
                '/'
            }) + "/" + targetParentPath).TrimStart(new char[]
            {
                '/'
            });
            if (manifestListItem.Attachments.Count > 0)
            {
                this.AddAttachmentsSubfolderXml(manifestListItem);
            }

            Guid guid = Guid.NewGuid();
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement.SetAttribute("Id", guid.ToString());
            xmlElement.SetAttribute("ObjectType", "SPListItem");
            xmlElement.SetAttribute("ParentId", this._listId.ToString());
            xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement.SetAttribute("Url", text);
            this._rootManifestXMLElement.AppendChild(xmlElement);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
            xmlElement2.SetAttribute("FileUrl", value);
            xmlElement2.SetAttribute("DocType", "File");
            xmlElement2.SetAttribute("ParentFolderId", manifestListItem.ParentFolderId);
            xmlElement2.SetAttribute("Order", (manifestListItem.ListItemIntegerId * 100).ToString());
            xmlElement2.SetAttribute("Id", guid.ToString());
            xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement2.SetAttribute("ParentListId", this._listId.ToString());
            xmlElement2.SetAttribute("Name", manifestListItem.Filename);
            xmlElement2.SetAttribute("DirName", value2);
            xmlElement2.SetAttribute("IntId", manifestListItem.ListItemIntegerId.ToString());
            xmlElement2.SetAttribute("DocId", manifestListItem.ItemGuid.ToString());
            xmlElement2.SetAttribute("Version", manifestListItem.Version);
            if (manifestListItem.HasVersioning)
            {
                xmlElement2.SetAttribute("CheckinComment", manifestListItem.CheckinComment);
            }

            if (!string.IsNullOrEmpty(manifestListItem.ContentTypeId))
            {
                xmlElement2.SetAttribute("ContentTypeId", manifestListItem.ContentTypeId);
            }

            xmlElement2.SetAttribute("Author", manifestListItem.Author.ToString());
            xmlElement2.SetAttribute("ModifiedBy", manifestListItem.ModifiedBy.ToString());
            xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(manifestListItem.TimeCreated));
            xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(manifestListItem.TimeLastModified));
            xmlElement2.SetAttribute("ModerationStatus", manifestListItem.ModerationStatus);
            xmlElement.AppendChild(xmlElement2);
            this._packagingStatus.ListItemCount++;
            this.AddFieldValuesXml(manifestListItem.FieldValues, xmlElement2);
            this._packagingStatus.TotalSize += (ulong)manifestListItem.FileSize;
            if (manifestListItem.HasVersioning)
            {
                XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Versions");
                foreach (ManifestListItem current in manifestListItem.Versions)
                {
                    XmlElement xmlElement4 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
                    xmlElement4.SetAttribute("Id", guid.ToString());
                    xmlElement4.SetAttribute("ParentWebId", this._webId.ToString());
                    xmlElement4.SetAttribute("ParentListId", this._listId.ToString());
                    xmlElement4.SetAttribute("Name", current.Filename);
                    xmlElement4.SetAttribute("DirName", value2);
                    xmlElement4.SetAttribute("IntId", current.ListItemIntegerId.ToString());
                    xmlElement4.SetAttribute("Version", current.Version);
                    xmlElement4.SetAttribute("CheckinComment", current.CheckinComment);
                    if (!string.IsNullOrEmpty(current.ContentTypeId))
                    {
                        xmlElement4.SetAttribute("ContentTypeId", current.ContentTypeId);
                    }

                    xmlElement4.SetAttribute("Author", current.Author.ToString());
                    xmlElement4.SetAttribute("ModifiedBy", current.ModifiedBy.ToString());
                    xmlElement4.SetAttribute("TimeCreated", this.XmlDateTime(current.TimeCreated));
                    xmlElement4.SetAttribute("TimeLastModified", this.XmlDateTime(current.TimeLastModified));
                    xmlElement4.SetAttribute("ModerationStatus", current.ModerationStatus);
                    xmlElement3.AppendChild(xmlElement4);
                    this.AddFieldValuesXml(current.FieldValues, xmlElement4);
                    this._packagingStatus.TotalSize += (ulong)current.FileSize;
                }

                xmlElement2.AppendChild(xmlElement3);
            }

            if (manifestListItem.Attachments.Count > 0)
            {
                this.AddAttachmentsXml(manifestListItem, xmlElement2);
            }

            this.PopulateRoleAssignments(manifestListItem, text);
        }

        private void AddAttachmentsXml(BaseManifestItemWithAttachments manifestItem, XmlElement listItemElement)
        {
            string str = this._targetRootRelativeUrl + "/Attachments/" + manifestItem.ListItemIntegerId;
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("Attachments");
            foreach (ManifestAttachment current in manifestItem.Attachments)
            {
                Guid guid = Guid.NewGuid();
                XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Attachment");
                xmlElement2.SetAttribute("Url", str + "/" + current.Filename);
                xmlElement2.SetAttribute("Id", guid.ToString());
                xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement2.SetAttribute("Name", current.Filename);
                xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(manifestItem.TimeCreated));
                xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(manifestItem.TimeLastModified));
                xmlElement2.SetAttribute("FileValue", current.LocalFilename);
                xmlElement2.SetAttribute("Author", manifestItem.Author.ToString());
                xmlElement2.SetAttribute("ModifiedBy", manifestItem.ModifiedBy.ToString());
                xmlElement.AppendChild(xmlElement2);
                string filePath = Path.Combine(this._commonDocumentBinaryDirectory, current.LocalFilename);
                this._packagingStatus.AddFile(current.LocalFilename, filePath);
                this._packagingStatus.TotalSize += (ulong)((long)current.FileSize);
            }

            listItemElement.AppendChild(xmlElement);
        }

        private void AddAttachmentsSubfolderXml(BaseManifestItemWithAttachments manifestItem)
        {
            string value = this._targetRootRelativeUrl + "/Attachments/" + manifestItem.ListItemIntegerId;
            string value2 = this._targetWebRelativeUrl + "/Attachments/" + manifestItem.ListItemIntegerId;
            Guid guid = Guid.NewGuid();
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement.SetAttribute("Id", guid.ToString());
            xmlElement.SetAttribute("ObjectType", "SPFolder");
            xmlElement.SetAttribute("ParentId", this._attachmentsFolderId.ToString());
            xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement.SetAttribute("Url", value);
            this._rootManifestXMLElement.AppendChild(xmlElement);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
            xmlElement2.SetAttribute("Id", guid.ToString());
            xmlElement2.SetAttribute("Url", value2);
            xmlElement2.SetAttribute("Name", manifestItem.ListItemIntegerId.ToString());
            xmlElement2.SetAttribute("ParentFolderId", this._attachmentsFolderId.ToString());
            xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement2.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement2.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
            xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
            xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
            xmlElement2.SetAttribute("SortBehavior", "1");
            xmlElement2.SetAttribute("ModifiedBy", "1073741823");
            xmlElement.AppendChild(xmlElement2);
            this._packagingStatus.FolderCount++;
        }

        public void AddDiscussionItemToManifest(ManifestDiscussionItem manifestDiscussionItem)
        {
            string targetParentPath = manifestDiscussionItem.TargetParentPath;
            string text = string.Concat(new string[]
            {
                this._parentWebServerRelativeUrl.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                targetParentPath.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                manifestDiscussionItem.Filename
            });
            string value = targetParentPath.TrimEnd(new char[]
            {
                '/'
            }) + "/" + manifestDiscussionItem.Filename;
            string value2 = (this._parentWebServerRelativeUrl.TrimEnd(new char[]
            {
                '/'
            }) + "/" + targetParentPath).TrimStart(new char[]
            {
                '/'
            });
            string text2 = this._targetRootRelativeUrl + "/Attachments/" + manifestDiscussionItem.ListItemIntegerId;
            string value3 = this._targetWebRelativeUrl + "/Attachments/" + manifestDiscussionItem.ListItemIntegerId;
            if (manifestDiscussionItem.Attachments.Count > 0)
            {
                Guid guid = Guid.NewGuid();
                XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement.SetAttribute("Id", guid.ToString());
                xmlElement.SetAttribute("ObjectType", "SPFolder");
                xmlElement.SetAttribute("ParentId", this._attachmentsFolderId.ToString());
                xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement.SetAttribute("Url", text2);
                this._rootManifestXMLElement.AppendChild(xmlElement);
                this._packagingStatus.ObjectCount++;
                XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
                xmlElement2.SetAttribute("Id", guid.ToString());
                xmlElement2.SetAttribute("Url", value3);
                xmlElement2.SetAttribute("Name", manifestDiscussionItem.ListItemIntegerId.ToString());
                xmlElement2.SetAttribute("ParentFolderId", this._attachmentsFolderId.ToString());
                xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement2.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement2.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
                xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
                xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
                xmlElement2.SetAttribute("SortBehavior", "1");
                xmlElement2.SetAttribute("ModifiedBy", "1073741823");
                xmlElement.AppendChild(xmlElement2);
                this._packagingStatus.FolderCount++;
            }

            Guid guid2 = Guid.NewGuid();
            XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement3.SetAttribute("Id", guid2.ToString());
            xmlElement3.SetAttribute("ObjectType", "SPListItem");
            xmlElement3.SetAttribute("ParentId", this._listId.ToString());
            xmlElement3.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement3.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement3.SetAttribute("Url", text);
            this._rootManifestXMLElement.AppendChild(xmlElement3);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement4 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
            xmlElement4.SetAttribute("FileUrl", value);
            xmlElement4.SetAttribute("DocType", "File");
            xmlElement4.SetAttribute("ParentFolderId", manifestDiscussionItem.ParentFolderId);
            xmlElement4.SetAttribute("Order", (manifestDiscussionItem.ListItemIntegerId * 100).ToString());
            xmlElement4.SetAttribute("ThreadIndex", manifestDiscussionItem.ThreadIndex);
            xmlElement4.SetAttribute("Id", guid2.ToString());
            xmlElement4.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement4.SetAttribute("ParentListId", this._listId.ToString());
            xmlElement4.SetAttribute("Name", manifestDiscussionItem.Filename);
            xmlElement4.SetAttribute("DirName", value2);
            xmlElement4.SetAttribute("IntId", manifestDiscussionItem.ListItemIntegerId.ToString());
            xmlElement4.SetAttribute("DocId", manifestDiscussionItem.ItemGuid.ToString());
            xmlElement4.SetAttribute("Version", manifestDiscussionItem.Version);
            if (!string.IsNullOrEmpty(manifestDiscussionItem.ContentTypeId))
            {
                xmlElement4.SetAttribute("ContentTypeId", manifestDiscussionItem.ContentTypeId);
            }

            xmlElement4.SetAttribute("Author", manifestDiscussionItem.Author.ToString());
            xmlElement4.SetAttribute("ModifiedBy", manifestDiscussionItem.ModifiedBy.ToString());
            xmlElement4.SetAttribute("TimeCreated", this.XmlDateTime(manifestDiscussionItem.TimeCreated));
            xmlElement4.SetAttribute("TimeLastModified", this.XmlDateTime(manifestDiscussionItem.TimeLastModified));
            xmlElement4.SetAttribute("ModerationStatus", manifestDiscussionItem.ModerationStatus);
            xmlElement3.AppendChild(xmlElement4);
            this._packagingStatus.ListItemCount++;
            this.AddFieldValuesXml(manifestDiscussionItem.FieldValues, xmlElement4);
            this._packagingStatus.TotalSize += (ulong)manifestDiscussionItem.FileSize;
            if (manifestDiscussionItem.HasVersioning)
            {
                XmlElement xmlElement5 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Versions");
                foreach (ManifestListItem current in manifestDiscussionItem.Versions)
                {
                    XmlElement xmlElement6 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
                    xmlElement6.SetAttribute("Id", guid2.ToString());
                    xmlElement6.SetAttribute("ParentWebId", this._webId.ToString());
                    xmlElement6.SetAttribute("ParentListId", this._listId.ToString());
                    xmlElement6.SetAttribute("Name", current.Filename);
                    xmlElement6.SetAttribute("DirName", value2);
                    xmlElement6.SetAttribute("IntId", current.ListItemIntegerId.ToString());
                    xmlElement6.SetAttribute("Version", current.Version);
                    if (!string.IsNullOrEmpty(current.ContentTypeId))
                    {
                        xmlElement6.SetAttribute("ContentTypeId", current.ContentTypeId);
                    }

                    xmlElement6.SetAttribute("Author", current.Author.ToString());
                    xmlElement6.SetAttribute("ModifiedBy", current.ModifiedBy.ToString());
                    xmlElement6.SetAttribute("TimeCreated", this.XmlDateTime(current.TimeCreated));
                    xmlElement6.SetAttribute("TimeLastModified", this.XmlDateTime(current.TimeLastModified));
                    xmlElement6.SetAttribute("ModerationStatus", current.ModerationStatus);
                    xmlElement5.AppendChild(xmlElement6);
                    this.AddFieldValuesXml(current.FieldValues, xmlElement6);
                    this._packagingStatus.TotalSize += (ulong)current.FileSize;
                }

                xmlElement4.AppendChild(xmlElement5);
            }

            if (manifestDiscussionItem.Attachments.Count > 0)
            {
                XmlElement xmlElement7 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Attachments");
                foreach (ManifestAttachment current2 in manifestDiscussionItem.Attachments)
                {
                    Guid guid3 = Guid.NewGuid();
                    XmlElement xmlElement8 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Attachment");
                    xmlElement8.SetAttribute("Url", text2 + "/" + current2.Filename);
                    xmlElement8.SetAttribute("Id", guid3.ToString());
                    xmlElement8.SetAttribute("ParentWebId", this._webId.ToString());
                    xmlElement8.SetAttribute("Name", current2.Filename);
                    xmlElement8.SetAttribute("TimeCreated", this.XmlDateTime(manifestDiscussionItem.TimeCreated));
                    xmlElement8.SetAttribute("TimeLastModified",
                        this.XmlDateTime(manifestDiscussionItem.TimeLastModified));
                    xmlElement8.SetAttribute("FileValue", current2.LocalFilename);
                    xmlElement8.SetAttribute("Author", manifestDiscussionItem.Author.ToString());
                    xmlElement8.SetAttribute("ModifiedBy", manifestDiscussionItem.ModifiedBy.ToString());
                    xmlElement7.AppendChild(xmlElement8);
                    string filePath = Path.Combine(this._commonDocumentBinaryDirectory, current2.LocalFilename);
                    this._packagingStatus.AddFile(current2.LocalFilename, filePath);
                    this._packagingStatus.TotalSize += (ulong)((long)current2.FileSize);
                }

                xmlElement4.AppendChild(xmlElement7);
            }

            this.PopulateRoleAssignments(manifestDiscussionItem, text);
        }

        public void AddAlertToManifest(ManifestAlert alert)
        {
            this._alerts.Add(alert);
        }

        private void PopulateRoleAssignments(BaseManifestItem baseManifestItem, string targetRootRelativeUrl)
        {
            if (baseManifestItem.RoleAssignments.Count > 0 || (this._isMySiteTemplate &&
                                                               (baseManifestItem.HasUniquePermissions ||
                                                                baseManifestItem is ManifestList)))
            {
                XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("RoleAssignment");
                xmlElement.SetAttribute("ScopeId", Guid.NewGuid().ToString());
                xmlElement.SetAttribute("RoleDefWebId", this._webId.ToString());
                xmlElement.SetAttribute("RoleDefWebUrl", this._parentWebServerRelativeUrl);
                xmlElement.SetAttribute("ObjectId", baseManifestItem.ItemGuid.ToString());
                string value = "2";
                if (baseManifestItem.ObjectType == ManifestObjectType.List)
                {
                    value = "1";
                }

                xmlElement.SetAttribute("ObjectType", value);
                xmlElement.SetAttribute("ObjectUrl", targetRootRelativeUrl);
                xmlElement.SetAttribute("AnonymousPermMask", "0");
                foreach (ManifestRoleAssignment current in baseManifestItem.RoleAssignments)
                {
                    XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Assignment");
                    xmlElement2.SetAttribute("RoleId", current.RoleId.ToString());
                    xmlElement2.SetAttribute("PrincipalId", current.PrincipalId.ToString());
                    xmlElement.AppendChild(xmlElement2);
                }

                this._rootRoleAssignmentXMLElement.AppendChild(xmlElement);
            }
        }

        public void AddFolderToManifest(ManifestFolderItem manifestFolderItem)
        {
            string text = string.Concat(new string[]
            {
                this._parentWebServerRelativeUrl.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                manifestFolderItem.TargetParentPath.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                manifestFolderItem.Foldername
            });
            string value = manifestFolderItem.TargetParentPath + "/" + manifestFolderItem.Foldername;
            string value2 = (this._parentWebServerRelativeUrl.TrimEnd(new char[]
            {
                '/'
            }) + "/" + manifestFolderItem.TargetParentPath).TrimStart(new char[]
            {
                '/'
            });
            if (manifestFolderItem.Attachments.Count > 0)
            {
                this.AddAttachmentsSubfolderXml(manifestFolderItem);
            }

            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement.SetAttribute("Id", manifestFolderItem.ItemGuid.ToString());
            xmlElement.SetAttribute("ObjectType", "SPFolder");
            xmlElement.SetAttribute("ParentId", manifestFolderItem.ParentFolderId);
            xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement.SetAttribute("Url", text);
            this._rootManifestXMLElement.AppendChild(xmlElement);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
            xmlElement2.SetAttribute("Url", value);
            xmlElement2.SetAttribute("Id", manifestFolderItem.ItemGuid.ToString());
            xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement2.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement2.SetAttribute("Name", manifestFolderItem.Foldername);
            xmlElement2.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
            xmlElement2.SetAttribute("ParentFolderId", manifestFolderItem.ParentFolderId);
            if (!manifestFolderItem.IsReferenceOnly)
            {
                xmlElement2.SetAttribute("ListItemIntId", manifestFolderItem.ListItemIntegerId.ToString());
                xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(manifestFolderItem.TimeCreated));
                xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(manifestFolderItem.TimeLastModified));
                xmlElement2.SetAttribute("SortBehavior", "1");
                xmlElement2.SetAttribute("Author", manifestFolderItem.Author.ToString());
                xmlElement2.SetAttribute("ModifiedBy", manifestFolderItem.ModifiedBy.ToString());
            }

            xmlElement.AppendChild(xmlElement2);
            this._packagingStatus.FolderCount++;
            string folderXml;
            if (manifestFolderItem.IsReferenceOnly)
            {
                folderXml = xmlElement.OuterXml;
            }
            else
            {
                Guid guid = Guid.NewGuid();
                XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement3.SetAttribute("Id", guid.ToString());
                xmlElement3.SetAttribute("ObjectType", "SPListItem");
                xmlElement3.SetAttribute("ParentId", this._listId.ToString());
                xmlElement3.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement3.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement3.SetAttribute("Url", text);
                this._rootManifestXMLElement.AppendChild(xmlElement3);
                this._packagingStatus.ObjectCount++;
                XmlElement xmlElement4 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
                xmlElement4.SetAttribute("FileUrl", value);
                xmlElement4.SetAttribute("DocType", "Folder");
                xmlElement4.SetAttribute("ParentFolderId", manifestFolderItem.ParentFolderId);
                xmlElement4.SetAttribute("Id", guid.ToString());
                xmlElement4.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement4.SetAttribute("ParentListId", this._listId.ToString());
                xmlElement4.SetAttribute("Name", manifestFolderItem.Foldername);
                xmlElement4.SetAttribute("DirName", value2);
                xmlElement4.SetAttribute("Order", (manifestFolderItem.ListItemIntegerId * 100).ToString());
                xmlElement4.SetAttribute("IntId", manifestFolderItem.ListItemIntegerId.ToString());
                xmlElement4.SetAttribute("DocId", manifestFolderItem.ItemGuid.ToString());
                xmlElement4.SetAttribute("Version",
                    manifestFolderItem.HasVersioning ? manifestFolderItem.Version : "1.0");
                if (manifestFolderItem.HasVersioning)
                {
                    xmlElement4.SetAttribute("CheckinComment", manifestFolderItem.CheckinComment);
                }

                if (!string.IsNullOrEmpty(manifestFolderItem.ContentTypeId))
                {
                    xmlElement4.SetAttribute("ContentTypeId", manifestFolderItem.ContentTypeId);
                }

                xmlElement4.SetAttribute("Author", manifestFolderItem.Author.ToString());
                xmlElement4.SetAttribute("ModifiedBy", manifestFolderItem.ModifiedBy.ToString());
                xmlElement4.SetAttribute("TimeLastModified", this.XmlDateTime(manifestFolderItem.TimeLastModified));
                xmlElement4.SetAttribute("TimeCreated", this.XmlDateTime(manifestFolderItem.TimeCreated));
                xmlElement4.SetAttribute("ModerationStatus",
                    string.IsNullOrEmpty(manifestFolderItem.ModerationStatus)
                        ? "Approved"
                        : manifestFolderItem.ModerationStatus);
                xmlElement3.AppendChild(xmlElement4);
                this._packagingStatus.ListItemCount++;
                this.AddFieldValuesXml(manifestFolderItem.FieldValues, xmlElement4);
                if (manifestFolderItem.HasVersioning)
                {
                    XmlElement xmlElement5 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Versions");
                    foreach (ManifestFolderItem current in manifestFolderItem.Versions)
                    {
                        XmlElement xmlElement6 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
                        xmlElement6.SetAttribute("Id", guid.ToString());
                        xmlElement6.SetAttribute("ParentWebId", this._webId.ToString());
                        xmlElement6.SetAttribute("ParentListId", this._listId.ToString());
                        xmlElement6.SetAttribute("Name", current.Foldername);
                        xmlElement6.SetAttribute("DirName", value2);
                        xmlElement6.SetAttribute("IntId", current.ListItemIntegerId.ToString());
                        xmlElement6.SetAttribute("Version", current.Version);
                        xmlElement4.SetAttribute("CheckinComment", manifestFolderItem.CheckinComment);
                        if (!string.IsNullOrEmpty(current.ContentTypeId))
                        {
                            xmlElement6.SetAttribute("ContentTypeId", current.ContentTypeId);
                        }

                        xmlElement6.SetAttribute("Author", current.Author.ToString());
                        xmlElement6.SetAttribute("ModifiedBy", current.ModifiedBy.ToString());
                        xmlElement6.SetAttribute("TimeCreated", this.XmlDateTime(current.TimeCreated));
                        xmlElement6.SetAttribute("TimeLastModified", this.XmlDateTime(current.TimeLastModified));
                        xmlElement6.SetAttribute("ModerationStatus", current.ModerationStatus);
                        xmlElement5.AppendChild(xmlElement6);
                        this.AddFieldValuesXml(current.FieldValues, xmlElement6);
                    }

                    xmlElement4.AppendChild(xmlElement5);
                }

                if (manifestFolderItem.Attachments.Count > 0)
                {
                    this.AddAttachmentsXml(manifestFolderItem, xmlElement4);
                }

                folderXml = xmlElement.OuterXml + xmlElement3.OuterXml;
            }

            this._folderIdsAddedInManifest.Add(manifestFolderItem.ItemGuid.ToString());
            this._commonGlobalManifestOperations.AddDependencyFolder(manifestFolderItem.ItemGuid.ToString(),
                manifestFolderItem.ParentFolderId, folderXml);
            this.PopulateRoleAssignments(manifestFolderItem, text);
        }

        private void AddFieldValuesXml(IEnumerable<Field> fieldValues, XmlElement listItemElement)
        {
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("Fields");
            foreach (Field current in fieldValues)
            {
                XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Field");
                xmlElement2.SetAttribute("Name", XmlConvert.DecodeName(current.Name));
                if (!string.IsNullOrEmpty(current.ListItemOverrideValue))
                {
                    xmlElement2.SetAttribute("Value", current.ListItemOverrideValue);
                }
                else if (!string.IsNullOrEmpty(current.Value))
                {
                    if (current.FieldId.Equals("3b63724f-3418-461f-868b-7706f69b029c",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.SetDocUrlFieldValues(current.Value, xmlElement2);
                    }
                    else
                    {
                        xmlElement2.SetAttribute("Value", current.Value);
                    }
                }

                xmlElement2.SetAttribute("FieldId", current.FieldId);
                xmlElement.AppendChild(xmlElement2);
            }

            listItemElement.AppendChild(xmlElement);
        }

        public void InitialiseManifest(string tempStorageDirectory, string commonDocumentBinaryDirectory, Guid webId,
            Guid siteId, Guid listId, Guid rootWebFolderId, Guid listFolderId, Guid attachmentsFolderId,
            string parentWebAbsoluteUrl, string parentWebServerRelativeUrl, string targetBasePath, string listName,
            string listTitle, ICommonGlobalManifestOperations commonGlobalManifestOperations,
            DateTime targetFolderCreated, DateTime targetFolderLastModified, string listBaseTemplate,
            string listBaseType, string webTemplate)
        {
            this._targetFolderCreated = targetFolderCreated;
            this._targetFolderLastModified = targetFolderLastModified;
            this._commonGlobalManifestOperations = commonGlobalManifestOperations;
            this._webId = webId;
            this._siteId = siteId;
            this._listId = listId;
            this._listTitle = listTitle;
            this._listName = listName;
            this._tempStorageDirectory = tempStorageDirectory;
            this._commonDocumentBinaryDirectory = commonDocumentBinaryDirectory;
            this._rootWebFolderId = rootWebFolderId;
            this._listFolderId = listFolderId;
            this._attachmentsFolderId = attachmentsFolderId;
            this._listBaseTemplate = listBaseTemplate;
            this._listBaseType = listBaseType;
            this._webTemplate = webTemplate;
            this._isMySiteTemplate = webTemplate.StartsWith("SPSPERS", StringComparison.InvariantCultureIgnoreCase);
            this._packagingStatus = new PackagingStatus();
            this._targetDirectory = new DirectoryInfo(tempStorageDirectory);
            this._parentWebAbsoluteUrl = parentWebAbsoluteUrl;
            this._parentWebServerRelativeUrl = parentWebServerRelativeUrl;
            this._targetBasePath = targetBasePath;
            if (string.IsNullOrEmpty(this._parentWebServerRelativeUrl))
            {
                this._parentWebServerRelativeUrl = "/";
            }

            if (!this._targetDirectory.Exists)
            {
                this._targetDirectory.Create();
            }

            this._targetRootRelativeUrl = this._parentWebServerRelativeUrl.TrimEnd(new char[]
            {
                '/'
            }) + "/" + targetBasePath;
            this._targetWebRelativeUrl = targetBasePath;
        }

        public void CreateBasePackageXml()
        {
            this.CreateSPMigrationPackageRequirements(this._targetDirectory, ref this._packagingStatus);
            this.CreateSPMigrationPackageRootObjectMap(this._targetDirectory, this._targetBasePath,
                this._parentWebServerRelativeUrl, this._webId, this._listId, ref this._packagingStatus);
            this.CreateSPMigrationPackageExportSettings(this._targetDirectory, this._parentWebAbsoluteUrl, this._webId,
                this._listId, ref this._packagingStatus);
            this.CreateSPMigrationPackageLookupListMap(this._targetDirectory, ref this._packagingStatus);
            this.CreateSPMigrationPackageViewFormsList(this._targetDirectory, ref this._packagingStatus);
            this._rootManifestXMLElement =
                this.CreateSPMigrationPackageManifest(this._siteId, ref this._packagingStatus);
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement.SetAttribute("Id", this._listFolderId.ToString());
            xmlElement.SetAttribute("ObjectType", "SPFolder");
            xmlElement.SetAttribute("ParentId", this._rootWebFolderId.ToString());
            xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement.SetAttribute("Url", this._targetRootRelativeUrl);
            this._rootManifestXMLElement.AppendChild(xmlElement);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
            xmlElement2.SetAttribute("Id", this._listFolderId.ToString());
            xmlElement2.SetAttribute("Url", this._targetWebRelativeUrl);
            xmlElement2.SetAttribute("Name", this._listName);
            xmlElement2.SetAttribute("ParentFolderId", this._rootWebFolderId.ToString());
            xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement2.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement2.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
            xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
            xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
            xmlElement2.SetAttribute("SortBehavior", "1");
            xmlElement2.SetAttribute("ModifiedBy", "1073741823");
            xmlElement.AppendChild(xmlElement2);
            this._packagingStatus.FolderCount++;
            if (!string.Equals(this._listBaseType, "DocumentLibrary", StringComparison.Ordinal) &&
                !string.Equals(this._listBaseTemplate, "Survey", StringComparison.Ordinal))
            {
                string value = this._targetRootRelativeUrl + "/Attachments";
                string value2 = this._targetWebRelativeUrl + "/Attachments";
                XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement3.SetAttribute("Id", this._attachmentsFolderId.ToString());
                xmlElement3.SetAttribute("ObjectType", "SPFolder");
                xmlElement3.SetAttribute("ParentId", this._listFolderId.ToString());
                xmlElement3.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement3.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement3.SetAttribute("Url", value);
                this._rootManifestXMLElement.AppendChild(xmlElement3);
                this._packagingStatus.ObjectCount++;
                XmlElement xmlElement4 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
                xmlElement4.SetAttribute("Id", this._attachmentsFolderId.ToString());
                xmlElement4.SetAttribute("Url", value2);
                xmlElement4.SetAttribute("Name", "Attachments");
                xmlElement4.SetAttribute("ParentFolderId", this._listFolderId.ToString());
                xmlElement4.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement4.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement4.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
                xmlElement4.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
                xmlElement4.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
                xmlElement4.SetAttribute("SortBehavior", "1");
                xmlElement4.SetAttribute("ModifiedBy", "1073741823");
                xmlElement3.AppendChild(xmlElement4);
                this._packagingStatus.FolderCount++;
            }

            if (string.Equals(this._listBaseTemplate, "GenericList"))
            {
                string value3 = this._targetRootRelativeUrl + "/Item";
                string value4 = this._targetWebRelativeUrl + "/Item";
                Guid guid = Guid.NewGuid();
                XmlElement xmlElement5 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement5.SetAttribute("Id", guid.ToString());
                xmlElement5.SetAttribute("ObjectType", "SPFolder");
                xmlElement5.SetAttribute("ParentId", this._listFolderId.ToString());
                xmlElement5.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement5.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement5.SetAttribute("Url", value3);
                this._rootManifestXMLElement.AppendChild(xmlElement5);
                this._packagingStatus.ObjectCount++;
                XmlElement xmlElement6 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
                xmlElement6.SetAttribute("Id", guid.ToString());
                xmlElement6.SetAttribute("Url", value4);
                xmlElement6.SetAttribute("Name", "Item");
                xmlElement6.SetAttribute("ParentFolderId", this._listFolderId.ToString());
                xmlElement6.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement6.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement6.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
                xmlElement6.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
                xmlElement6.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
                xmlElement6.SetAttribute("SortBehavior", "1");
                xmlElement6.SetAttribute("ModifiedBy", "1073741823");
                xmlElement5.AppendChild(xmlElement6);
                this._packagingStatus.FolderCount++;
            }
            else if (string.Equals(this._listBaseTemplate, "DiscussionBoard"))
            {
                string value5 = this._targetRootRelativeUrl + "/Discussion";
                string value6 = this._targetWebRelativeUrl + "/Discussion";
                Guid guid2 = Guid.NewGuid();
                XmlElement xmlElement7 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement7.SetAttribute("Id", guid2.ToString());
                xmlElement7.SetAttribute("ObjectType", "SPFolder");
                xmlElement7.SetAttribute("ParentId", this._listFolderId.ToString());
                xmlElement7.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement7.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement7.SetAttribute("Url", value5);
                this._rootManifestXMLElement.AppendChild(xmlElement7);
                this._packagingStatus.ObjectCount++;
                XmlElement xmlElement8 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
                xmlElement8.SetAttribute("Id", guid2.ToString());
                xmlElement8.SetAttribute("Url", value6);
                xmlElement8.SetAttribute("Name", "Discussion");
                xmlElement8.SetAttribute("ParentFolderId", this._listFolderId.ToString());
                xmlElement8.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement8.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement8.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
                xmlElement8.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
                xmlElement8.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
                xmlElement8.SetAttribute("SortBehavior", "1");
                xmlElement8.SetAttribute("ModifiedBy", "1073741823");
                xmlElement7.AppendChild(xmlElement8);
                this._packagingStatus.FolderCount++;
                string value7 = this._targetRootRelativeUrl + "/Message";
                string value8 = this._targetWebRelativeUrl + "/Message";
                Guid guid3 = Guid.NewGuid();
                XmlElement xmlElement9 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement9.SetAttribute("Id", guid3.ToString());
                xmlElement9.SetAttribute("ObjectType", "SPFolder");
                xmlElement9.SetAttribute("ParentId", this._listFolderId.ToString());
                xmlElement9.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement9.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement9.SetAttribute("Url", value7);
                this._rootManifestXMLElement.AppendChild(xmlElement9);
                this._packagingStatus.ObjectCount++;
                XmlElement xmlElement10 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Folder");
                xmlElement10.SetAttribute("Id", guid3.ToString());
                xmlElement10.SetAttribute("Url", value8);
                xmlElement10.SetAttribute("Name", "Message");
                xmlElement10.SetAttribute("ParentFolderId", this._listFolderId.ToString());
                xmlElement10.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement10.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement10.SetAttribute("ContainingDocumentLibrary", this._listId.ToString());
                xmlElement10.SetAttribute("TimeCreated", this.XmlDateTime(this._targetFolderCreated));
                xmlElement10.SetAttribute("TimeLastModified", this.XmlDateTime(this._targetFolderLastModified));
                xmlElement10.SetAttribute("SortBehavior", "1");
                xmlElement10.SetAttribute("ModifiedBy", "1073741823");
                xmlElement9.AppendChild(xmlElement10);
                this._packagingStatus.FolderCount++;
            }

            XmlElement xmlElement11 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement11.SetAttribute("Id", this._listId.ToString());
            string listBaseTemplate;
            string value9;
            string name;
            if ((listBaseTemplate = this._listBaseTemplate) != null && listBaseTemplate == "DocumentLibrary")
            {
                value9 = "SPDocumentLibrary";
                name = "DocumentLibrary";
            }
            else
            {
                value9 = "SPList";
                name = "List";
            }

            xmlElement11.SetAttribute("ObjectType", value9);
            xmlElement11.SetAttribute("ParentId", this._webId.ToString());
            xmlElement11.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement11.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement11.SetAttribute("Url", this._targetRootRelativeUrl);
            this._pointerToAddDependencyFolders = this._rootManifestXMLElement.AppendChild(xmlElement11);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement12 = this._rootManifestXMLElement.OwnerDocument.CreateElement(name);
            xmlElement12.SetAttribute("Id", this._listId.ToString());
            xmlElement12.SetAttribute("BaseTemplate", this._listBaseTemplate);
            xmlElement12.SetAttribute("RootFolderId", this._listFolderId.ToString());
            xmlElement12.SetAttribute("RootFolderUrl", this._targetRootRelativeUrl);
            xmlElement12.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement12.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement12.SetAttribute("Title", this._listTitle);
            xmlElement11.AppendChild(xmlElement12);
            this._packagingStatus.ListCount++;
            XmlElement newChild = this._rootManifestXMLElement.OwnerDocument.CreateElement("ContentTypes");
            xmlElement12.AppendChild(newChild);
            this._deploymentRoleAssignmentElement =
                this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            this._deploymentRoleAssignmentElement.SetAttribute("Id", Guid.NewGuid().ToString());
            this._deploymentRoleAssignmentElement.SetAttribute("ObjectType", "DeploymentRoleAssignments");
            this._deploymentRoleAssignmentElement.SetAttribute("ParentId", this._webId.ToString());
            this._deploymentRoleAssignmentElement.SetAttribute("ParentWebId", this._webId.ToString());
            this._deploymentRoleAssignmentElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            this._rootRoleAssignmentXMLElement =
                this._rootManifestXMLElement.OwnerDocument.CreateElement("RoleAssignments");
            this._deploymentRoleAssignmentElement.AppendChild(this._rootRoleAssignmentXMLElement);
        }

        public void SaveManifest()
        {
            this.AddDependentFolders();
            this.AddAlertObjectsToManifest();
            this.AddRolesToManifest();
            this.AddRolesAssignmentsToManifest();
            string text = this._targetDirectory.FullName + "\\Manifest.xml";
            this._rootManifestXMLElement.OwnerDocument.Save(text);
            this._packagingStatus.AddManifest("Manifest.xml", text);
            this.CreateSPMigrationPackageUserGroup(this._targetDirectory, ref this._packagingStatus);
            this.CreateSPMigrationPackageSystemData(this._targetDirectory, this._targetBasePath,
                this._parentWebServerRelativeUrl, this._webId, this._targetRootRelativeUrl, this._listId,
                this._rootWebFolderId, ref this._packagingStatus);
        }

        private void AddDependentFolders()
        {
            XmlNodeList xmlNodeList = this._rootManifestXMLElement.OwnerDocument.SelectNodes("//*[@ParentFolderId]");
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            if (xmlNodeList != null)
            {
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    if (xmlNode.Attributes != null && xmlNode.Attributes["ParentFolderId"] != null)
                    {
                        list2.Clear();
                        string text = xmlNode.Attributes["ParentFolderId"].Value;
                        for (DependencyFolder dependencyFolder =
                                 this._commonGlobalManifestOperations.GetDependencyFolder(text);
                             dependencyFolder != null;
                             dependencyFolder = this._commonGlobalManifestOperations.GetDependencyFolder(text))
                        {
                            if (!this._folderIdsAddedInManifest.Contains(text))
                            {
                                this._folderIdsAddedInManifest.Add(text);
                                list2.Insert(0, dependencyFolder.FolderXml);
                            }

                            text = dependencyFolder.ParentFolderId;
                        }

                        if (list2.Count > 0)
                        {
                            list.AddRange(list2);
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                list.ForEach(delegate(string folderXml) { sb.AppendLine(folderXml); });
                XmlDocumentFragment xmlDocumentFragment =
                    this._rootManifestXMLElement.OwnerDocument.CreateDocumentFragment();
                xmlDocumentFragment.InnerXml = sb.ToString();
                this._rootManifestXMLElement.InsertAfter(xmlDocumentFragment, this._pointerToAddDependencyFolders);
            }
        }

        private void AddAlertObjectsToManifest()
        {
            if (this._alerts == null || this._alerts.Count == 0)
            {
                return;
            }

            foreach (ManifestAlert current in this._alerts)
            {
                string text = this._parentWebServerRelativeUrl.TrimEnd(new char[]
                {
                    '/'
                }) + "/" + this._listName.TrimEnd(new char[]
                {
                    '/'
                });
                XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
                xmlElement.SetAttribute("Id", current.Id.ToString("D"));
                xmlElement.SetAttribute("ObjectType", "SPAlert");
                xmlElement.SetAttribute("ParentId", this._listId.ToString("D"));
                xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
                xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                xmlElement.SetAttribute("Url", text);
                this.AddAlertElementToObjectElement(xmlElement, current, text);
                this._rootManifestXMLElement.AppendChild(xmlElement);
                this._packagingStatus.ObjectCount++;
            }
        }

        private void AddAlertElementToObjectElement(XmlElement objectElement, ManifestAlert alert, string listUrl)
        {
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("Alert");
            xmlElement.SetAttribute("Id", alert.Id.ToString("D"));
            xmlElement.SetAttribute("Title", alert.Title);
            xmlElement.SetAttribute("AlertType", alert.AlertType.ToString());
            xmlElement.SetAttribute("EventType", alert.EventType.ToString());
            xmlElement.SetAttribute("UserId", alert.UserId.ToString());
            xmlElement.SetAttribute("WebId", this._webId.ToString("D"));
            xmlElement.SetAttribute("ListId", this._listId.ToString("D"));
            xmlElement.SetAttribute("DocId", alert.DocId.ToString("D"));
            xmlElement.SetAttribute("ParentId", this._listId.ToString("D"));
            xmlElement.SetAttribute("ListUrl", listUrl);
            xmlElement.SetAttribute("ObjectUrl", this._listTitle);
            xmlElement.SetAttribute("NotifyFrequency", alert.NotifyFrequency.ToString());
            xmlElement.SetAttribute("AlwaysNotify", alert.AlwaysNotify ? "true" : "false");
            xmlElement.SetAttribute("DeliveryChannel", alert.DeliveryChannel.ToString());
            xmlElement.SetAttribute("Status", alert.Status.ToString());
            if (!string.IsNullOrEmpty(alert.AlertTemplateName))
            {
                xmlElement.SetAttribute("AlertTemplateName", alert.AlertTemplateName);
            }

            if (alert.NotifyTime.HasValue)
            {
                string value = Convert.ToDateTime(alert.NotifyTime).ToUniversalTime().ToString("o");
                xmlElement.SetAttribute("NotifyTime", value);
            }

            if (alert.ListItemIntId.HasValue)
            {
                xmlElement.SetAttribute("ListItemIntId", alert.ListItemIntId.ToString());
            }

            if (!string.IsNullOrEmpty(alert.Filter))
            {
                XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Filter");
                xmlElement2.InnerXml = alert.Filter;
                xmlElement.AppendChild(xmlElement2);
            }

            XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Properties");
            if (alert.FieldValues != null)
            {
                foreach (Field current in alert.FieldValues)
                {
                    XmlElement xmlElement4 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Property");
                    xmlElement4.SetAttribute("Name", XmlConvert.DecodeName(current.Name));
                    xmlElement4.SetAttribute("Type", current.Type);
                    xmlElement4.SetAttribute("Access", current.Access);
                    xmlElement4.SetAttribute("Value", current.Value);
                    xmlElement3.AppendChild(xmlElement4);
                }
            }

            xmlElement.AppendChild(xmlElement3);
            objectElement.AppendChild(xmlElement);
            this._packagingStatus.AlertCount++;
        }

        private string MapType(string type)
        {
            string result = "String";
            switch (type)
            {
                case "Currency":
                case "Number":
                    result = "Double";
                    break;
                case "User":
                    result = "Integer";
                    break;
                case "UserMulti":
                    result = "String";
                    break;
                case "Boolean":
                    result = "Boolean";
                    break;
                case "LongText":
                case "Note":
                case "HTML":
                    result = "LongText";
                    break;
            }

            return result;
        }

        public void AddFileToManifest(ManifestFileItem manifestFileItem)
        {
            string targetParentPath = manifestFileItem.TargetParentPath;
            string text = string.Concat(new string[]
            {
                this._parentWebServerRelativeUrl.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                targetParentPath.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                manifestFileItem.Filename
            });
            string value = targetParentPath.TrimEnd(new char[]
            {
                '/'
            }) + "/" + manifestFileItem.Filename;
            string value2 = (this._parentWebServerRelativeUrl.TrimEnd(new char[]
            {
                '/'
            }) + "/" + targetParentPath).TrimStart(new char[]
            {
                '/'
            });
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement.SetAttribute("Id", manifestFileItem.ItemGuid.ToString());
            xmlElement.SetAttribute("ObjectType", "SPFile");
            xmlElement.SetAttribute("ParentId", manifestFileItem.ParentFolderId);
            xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement.SetAttribute("Url", text);
            this._rootManifestXMLElement.AppendChild(xmlElement);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("File");
            xmlElement2.SetAttribute("Url", value);
            xmlElement2.SetAttribute("Id", manifestFileItem.ItemGuid.ToString());
            xmlElement2.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement2.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement2.SetAttribute("Name", manifestFileItem.Filename);
            xmlElement2.SetAttribute("ListItemIntId", manifestFileItem.ListItemIntegerId.ToString());
            xmlElement2.SetAttribute("ListId", this._listId.ToString());
            xmlElement2.SetAttribute("ParentId", manifestFileItem.ParentFolderId);
            xmlElement2.SetAttribute("TimeCreated", this.XmlDateTime(manifestFileItem.TimeCreated));
            xmlElement2.SetAttribute("TimeLastModified", this.XmlDateTime(manifestFileItem.TimeLastModified));
            xmlElement2.SetAttribute("Version", manifestFileItem.Version);
            if (manifestFileItem.HasVersioning)
            {
                xmlElement2.SetAttribute("CheckinComment", manifestFileItem.CheckinComment);
            }
            else
            {
                xmlElement2.SetAttribute("FileValue", manifestFileItem.LocalFilename);
            }

            xmlElement2.SetAttribute("Author", manifestFileItem.Author.ToString());
            xmlElement2.SetAttribute("ModifiedBy", manifestFileItem.ModifiedBy.ToString());
            xmlElement.AppendChild(xmlElement2);
            this._packagingStatus.FileCount++;
            if (manifestFileItem.HasVersioning)
            {
                XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Versions");
                foreach (ManifestFileItem current in manifestFileItem.Versions)
                {
                    XmlElement xmlElement4 = this._rootManifestXMLElement.OwnerDocument.CreateElement("File");
                    xmlElement4.SetAttribute("Url", value);
                    xmlElement4.SetAttribute("Id", current.ItemGuid.ToString());
                    xmlElement4.SetAttribute("ParentWebId", this._webId.ToString());
                    xmlElement4.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
                    xmlElement4.SetAttribute("Name", current.Filename);
                    xmlElement4.SetAttribute("ListItemIntId", current.ListItemIntegerId.ToString());
                    xmlElement4.SetAttribute("ListId", this._listId.ToString());
                    xmlElement4.SetAttribute("ParentId", current.ParentFolderId);
                    xmlElement4.SetAttribute("TimeCreated", this.XmlDateTime(current.TimeCreated));
                    xmlElement4.SetAttribute("TimeLastModified", this.XmlDateTime(current.TimeLastModified));
                    xmlElement4.SetAttribute("Version", current.Version);
                    xmlElement4.SetAttribute("CheckinComment", current.CheckinComment);
                    xmlElement4.SetAttribute("FileValue", current.LocalFilename);
                    xmlElement4.SetAttribute("Author", current.Author.ToString());
                    xmlElement4.SetAttribute("ModifiedBy", current.ModifiedBy.ToString());
                    XmlNode filePropertiesElement =
                        this.GetFilePropertiesElement(manifestFileItem, current.FieldValues);
                    xmlElement4.AppendChild(filePropertiesElement);
                    xmlElement3.AppendChild(xmlElement4);
                }

                xmlElement2.AppendChild(xmlElement3);
            }
            else
            {
                XmlNode filePropertiesElement2 =
                    this.GetFilePropertiesElement(manifestFileItem, manifestFileItem.FieldValues);
                xmlElement2.AppendChild(filePropertiesElement2);
            }

            Guid guid = Guid.NewGuid();
            XmlElement xmlElement5 = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement5.SetAttribute("Id", guid.ToString());
            xmlElement5.SetAttribute("ObjectType", "SPListItem");
            xmlElement5.SetAttribute("ParentId", this._listId.ToString());
            xmlElement5.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement5.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            xmlElement5.SetAttribute("Url", text);
            this._rootManifestXMLElement.AppendChild(xmlElement5);
            this._packagingStatus.ObjectCount++;
            XmlElement xmlElement6 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
            xmlElement6.SetAttribute("FileUrl", value);
            xmlElement6.SetAttribute("DocType", "File");
            xmlElement6.SetAttribute("ParentFolderId", manifestFileItem.ParentFolderId);
            xmlElement6.SetAttribute("Order", (manifestFileItem.ListItemIntegerId * 100).ToString());
            xmlElement6.SetAttribute("Id", guid.ToString());
            xmlElement6.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement6.SetAttribute("ParentListId", this._listId.ToString());
            xmlElement6.SetAttribute("Name", manifestFileItem.Filename);
            xmlElement6.SetAttribute("DirName", value2);
            xmlElement6.SetAttribute("IntId", manifestFileItem.ListItemIntegerId.ToString());
            xmlElement6.SetAttribute("DocId", manifestFileItem.ItemGuid.ToString());
            xmlElement6.SetAttribute("Version", manifestFileItem.Version);
            if (!string.IsNullOrEmpty(manifestFileItem.ContentTypeId))
            {
                xmlElement6.SetAttribute("ContentTypeId", manifestFileItem.ContentTypeId);
            }

            xmlElement6.SetAttribute("Author", manifestFileItem.Author.ToString());
            xmlElement6.SetAttribute("ModifiedBy", manifestFileItem.ModifiedBy.ToString());
            xmlElement6.SetAttribute("TimeCreated", this.XmlDateTime(manifestFileItem.TimeCreated));
            xmlElement6.SetAttribute("TimeLastModified", this.XmlDateTime(manifestFileItem.TimeLastModified));
            xmlElement6.SetAttribute("ModerationStatus", manifestFileItem.ModerationStatus);
            xmlElement5.AppendChild(xmlElement6);
            this._packagingStatus.ListItemCount++;
            this.AddFieldValuesXml(manifestFileItem.FieldValues, xmlElement6);
            if (manifestFileItem.HasVersioning)
            {
                XmlElement xmlElement7 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Versions");
                xmlElement6.AppendChild(xmlElement7);
                using (List<ManifestFileItem>.Enumerator enumerator2 = manifestFileItem.Versions.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        ManifestFileItem current2 = enumerator2.Current;
                        XmlElement xmlElement8 = this._rootManifestXMLElement.OwnerDocument.CreateElement("ListItem");
                        xmlElement8.SetAttribute("FileUrl", value);
                        xmlElement8.SetAttribute("DocType", "File");
                        xmlElement8.SetAttribute("ParentFolderId", current2.ParentFolderId);
                        xmlElement8.SetAttribute("Order", (current2.ListItemIntegerId * 100).ToString());
                        xmlElement8.SetAttribute("Id", guid.ToString());
                        xmlElement8.SetAttribute("ParentWebId", this._webId.ToString());
                        xmlElement8.SetAttribute("ParentListId", this._listId.ToString());
                        xmlElement8.SetAttribute("Name", current2.Filename);
                        xmlElement8.SetAttribute("DirName", value2);
                        xmlElement8.SetAttribute("IntId", current2.ListItemIntegerId.ToString());
                        xmlElement8.SetAttribute("DocId", current2.ItemGuid.ToString());
                        xmlElement8.SetAttribute("Version", current2.Version);
                        if (!string.IsNullOrEmpty(manifestFileItem.ContentTypeId))
                        {
                            xmlElement8.SetAttribute("ContentTypeId", current2.ContentTypeId);
                        }

                        xmlElement8.SetAttribute("Author", current2.Author.ToString());
                        xmlElement8.SetAttribute("ModifiedBy", current2.ModifiedBy.ToString());
                        xmlElement8.SetAttribute("TimeLastModified", this.XmlDateTime(current2.TimeLastModified));
                        xmlElement8.SetAttribute("TimeCreated", this.XmlDateTime(current2.TimeCreated));
                        xmlElement8.SetAttribute("ModerationStatus", current2.ModerationStatus);
                        this.AddFieldValuesXml(current2.FieldValues, xmlElement8);
                        xmlElement7.AppendChild(xmlElement8);
                        this._packagingStatus.TotalSize += (ulong)((long)current2.FileSize);
                        string filePath = Path.Combine(this._commonDocumentBinaryDirectory, current2.LocalFilename);
                        this._packagingStatus.AddFile(current2.LocalFilename, filePath);
                    }

                    goto IL_AA1;
                }
            }

            this._packagingStatus.TotalSize += (ulong)((long)manifestFileItem.FileSize);
            string filePath2 = Path.Combine(this._commonDocumentBinaryDirectory, manifestFileItem.LocalFilename);
            this._packagingStatus.AddFile(manifestFileItem.LocalFilename, filePath2);
            IL_AA1:
            this.PopulateRoleAssignments(manifestFileItem, text);
        }

        private XmlNode GetFilePropertiesElement(ManifestFileItem manifestFileItem, List<Field> fieldValues)
        {
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("Properties");
            if (!string.IsNullOrEmpty(manifestFileItem.ContentTypeId))
            {
                XmlElement xmlElement2 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Property");
                xmlElement2.SetAttribute("Name", "ContentTypeId");
                xmlElement2.SetAttribute("Type", "LongText");
                xmlElement2.SetAttribute("Access", "ReadWrite");
                xmlElement2.SetAttribute("Value", manifestFileItem.ContentTypeId);
                xmlElement.AppendChild(xmlElement2);
            }

            foreach (Field current in fieldValues)
            {
                if (!string.Equals(current.Type, "User") && !string.Equals(current.Type, "UserMulti"))
                {
                    XmlElement xmlElement3 = this._rootManifestXMLElement.OwnerDocument.CreateElement("Property");
                    xmlElement3.SetAttribute("Name", XmlConvert.DecodeName(current.Name));
                    xmlElement3.SetAttribute("Type", this.MapType(current.Type));
                    xmlElement3.SetAttribute("Access", current.Access);
                    if (!string.IsNullOrEmpty(current.Value))
                    {
                        xmlElement3.SetAttribute("Value", current.Value);
                    }

                    xmlElement.AppendChild(xmlElement3);
                }
            }

            return xmlElement;
        }

        private void AddRolesToManifest()
        {
            XmlElement xmlElement = this._rootManifestXMLElement.OwnerDocument.CreateElement("SPObject");
            xmlElement.SetAttribute("Id", Guid.NewGuid().ToString());
            xmlElement.SetAttribute("ObjectType", "DeploymentRoles");
            xmlElement.SetAttribute("ParentId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebId", this._webId.ToString());
            xmlElement.SetAttribute("ParentWebUrl", this._parentWebServerRelativeUrl);
            string allRolesXml = this._commonGlobalManifestOperations.GetAllRolesXml(this._webTemplate);
            XmlDocumentFragment xmlDocumentFragment =
                this._rootManifestXMLElement.OwnerDocument.CreateDocumentFragment();
            xmlDocumentFragment.InnerXml = allRolesXml;
            xmlElement.AppendChild(xmlDocumentFragment);
            this._rootManifestXMLElement.AppendChild(xmlElement);
            this._packagingStatus.ObjectCount++;
        }

        private void AddRolesAssignmentsToManifest()
        {
            this.AddListRoleAssignments();
            this._rootManifestXMLElement.AppendChild(this._deploymentRoleAssignmentElement);
            this._packagingStatus.ObjectCount++;
        }

        private void AddListRoleAssignments()
        {
            if (this._commonGlobalManifestOperations.ListManifest != null)
            {
                this.PopulateRoleAssignments(this._commonGlobalManifestOperations.ListManifest,
                    this._commonGlobalManifestOperations.ListManifest.ServerRelativeURL);
            }
        }

        private void SetDocUrlFieldValues(string docUrl, XmlElement fieldProp)
        {
            try
            {
                string[] array = docUrl.Split(new char[]
                {
                    ','
                }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length == 2)
                {
                    string text = array[0];
                    if (text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        text.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        array[0] = new Uri(text).PathAndQuery;
                    }

                    fieldProp.SetAttribute("Value", array[0].Trim());
                    fieldProp.SetAttribute("Value2", array[1].Trim());
                }
                else
                {
                    fieldProp.SetAttribute("Value", docUrl);
                }
            }
            catch (Exception arg)
            {
                Trace.WriteLine(string.Format(
                    "Error occurred while setting doc url field value for field '_dlc_DocIdUrl': Value '{0}'. Error '{1}'",
                    docUrl, arg));
                fieldProp.SetAttribute("Value", docUrl);
            }
        }
    }
}