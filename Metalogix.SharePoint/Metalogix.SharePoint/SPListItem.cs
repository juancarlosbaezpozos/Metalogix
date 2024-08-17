using Metalogix;
using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
    [Name("List Item or Document")]
    [PluralName("List Items and Documents")]
    public class SPListItem : SPNode, ISecurableObject, ListItem, Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
    {
        public static CultureInfo PropertyCulture;

        private int _noOfLatestVersionsToGet = 0;

        private SPList m_parentList;

        private SPFolder m_parentFolder;

        private SPListItemCollection m_parentCollection;

        protected XmlNode m_xmlNode;

        private int m_iID;

        private Guid m_Guid;

        private Guid _uniqueId;

        protected Dictionary<string, string> m_terseData = null;

        private string m_sConstantFileLeafRef = null;

        private string m_sConstantFileDirRef = null;

        private int m_iConstantID;

        private string m_sTitle = null;

        private string m_sFileLeafRef = null;

        private string m_sFileDirRef = null;

        private string m_sEncodedAbsUrl = null;

        private DateTime? m_dateModified;

        private XmlNode m_itemXML = null;

        private byte[] m_binary = null;

        private bool? m_bHasUniquePermissions = null;

        private SPListItemVersionCollection m_versionHistory = null;

        private bool? m_bIsExternalized = null;

        private bool? m_bIsCheckedOut = null;

        private bool? m_bBinaryAvailable = null;

        private bool? _isDBGhostedCached = null;

        private SPWebPartPage m_webPartPage = null;

        private bool? m_bIsWebPartPage = null;

        private bool? bIsWelcomePage = null;

        private bool? m_bHasWorkflows = null;

        private SPListItemType? m_itemType = null;

        private SPAttachmentCollection m_attachments;

        private SPAlertCollection m_alerts;

        private object m_oLockRoleAssignments = new object();

        private SPRoleAssignmentCollection m_roleAssignments = null;

        public SPAlertCollection Alerts
        {
            get
            {
                this.m_alerts = new SPAlertCollection(this);
                this.m_alerts.FetchData();
                return this.m_alerts;
            }
        }

        public SPAttachmentCollection Attachments
        {
            get
            {
                if (this.m_attachments == null)
                {
                    this.m_attachments = new SPAttachmentCollection(this);
                    if (this.HasAttachments)
                    {
                        this.m_attachments.FetchData();
                    }
                }
                return this.m_attachments;
            }
        }

        public byte[] Binary
        {
            get
            {
                byte[] mBinary;
                byte[] binary;
                if (this.m_binary != null)
                {
                    mBinary = this.m_binary;
                }
                else
                {
                    if (this.BinaryAvailable)
                    {
                        binary = this.GetBinary();
                    }
                    else
                    {
                        binary = null;
                    }
                    mBinary = binary;
                }
                return mBinary;
            }
            set
            {
                this.m_binary = value;
            }
        }

        public bool BinaryAvailable
        {
            get
            {
                if (base.GetExternalConnectionsOfType<StoragePointExternalConnection>(true).Count > 0)
                {
                    this.m_bBinaryAvailable = new bool?(true);
                }
                if (!this.m_bBinaryAvailable.HasValue)
                {
                    if ((this.ItemXML.Attributes["BinaryAvailable"] == null ? true : bool.Parse(this.ItemXML.Attributes["BinaryAvailable"].Value)))
                    {
                        this.m_bBinaryAvailable = new bool?(true);
                    }
                    else
                    {
                        this.m_bBinaryAvailable = new bool?(false);
                    }
                }
                return this.m_bBinaryAvailable.Value;
            }
        }

        public bool CanHaveVersions
        {
            get
            {
                bool flag;
                bool flag1;
                if (this is SPListItemVersion)
                {
                    flag = false;
                }
                else if (this.ParentList.EnableVersioning)
                {
                    if (this.ItemType == SPListItemType.Item)
                    {
                        flag1 = true;
                    }
                    else
                    {
                        flag1 = (this.ItemType != SPListItemType.Folder || this.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard ? false : this.ParentList.IsUsingMigrationPipeline);
                    }
                    flag = flag1;
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
        }

        internal string ConstantFileDirRef
        {
            get
            {
                return this.m_sConstantFileDirRef;
            }
        }

        internal string ConstantFileLeafRef
        {
            get
            {
                return this.m_sConstantFileLeafRef;
            }
        }

        internal int ConstantID
        {
            get
            {
                return this.m_iConstantID;
            }
        }

        public string ConstantServerRelativeFolderLeafRef
        {
            get
            {
                return string.Concat(this.ConstantFileDirRef, "/", this.ConstantFileLeafRef);
            }
        }

        public string ConstantServerRelativeUrl
        {
            get
            {
                string str;
                if ((this.ParentList.IsDocumentLibrary ? false : this.ItemType != SPListItemType.Folder))
                {
                    string constantServerRelativeUrl = this.ParentList.ConstantServerRelativeUrl;
                    string listASPXPageString = this.GetListASPXPageString(this.ParentList.BaseTemplate);
                    int constantID = this.ConstantID;
                    str = string.Concat(constantServerRelativeUrl, listASPXPageString, constantID.ToString());
                }
                else
                {
                    str = string.Concat(this.ConstantFileDirRef, "/", this.ConstantFileLeafRef);
                }
                return str;
            }
        }

        public string ContentTypeId
        {
            get
            {
                string str;
                string value;
                if (this.ItemXML != null)
                {
                    XmlNode itemOf = this.ItemXML.Attributes["ContentTypeId"];
                    if (itemOf != null)
                    {
                        value = itemOf.Value;
                    }
                    else
                    {
                        value = null;
                    }
                    str = value;
                }
                else
                {
                    str = null;
                }
                return str;
            }
        }

        public DateTime Created
        {
            get
            {
                return Metalogix.SharePoint.Adapters.Utils.ParseDateAsUtc(this["Created"]);
            }
        }

        public string CreatedBy
        {
            get
            {
                return this["Author"];
            }
        }

        public string EncodedAbsUrl
        {
            get
            {
                string mSEncodedAbsUrl;
                if (this.m_sEncodedAbsUrl == null)
                {
                    mSEncodedAbsUrl = (this.ItemXML.Attributes["EncodedAbsUrl"] != null ? this.ItemXML.Attributes["EncodedAbsUrl"].Value : "");
                }
                else
                {
                    mSEncodedAbsUrl = this.m_sEncodedAbsUrl;
                }
                return mSEncodedAbsUrl;
            }
            set
            {
                this.m_sEncodedAbsUrl = value;
                this["EncodedAbsUrl"] = value;
            }
        }

        public string FileDirRef
        {
            get
            {
                string mSFileDirRef;
                if (this.m_sFileDirRef == null)
                {
                    mSFileDirRef = (this.ItemXML.Attributes["FileDirRef"] != null ? this.ItemXML.Attributes["FileDirRef"].Value : "");
                }
                else
                {
                    mSFileDirRef = this.m_sFileDirRef;
                }
                return mSFileDirRef;
            }
            set
            {
                this.m_sFileDirRef = value;
                this["FileDirRef"] = value;
            }
        }

        public string FileLeafRef
        {
            get
            {
                string mSFileLeafRef;
                if (this.m_sFileLeafRef == null)
                {
                    mSFileLeafRef = (this.ItemXML.Attributes["FileLeafRef"] != null ? this.ItemXML.Attributes["FileLeafRef"].Value : "");
                }
                else
                {
                    mSFileLeafRef = this.m_sFileLeafRef;
                }
                return mSFileLeafRef;
            }
            set
            {
                this.m_sFileLeafRef = value;
                this["FileLeafRef"] = value;
            }
        }

        public virtual DocumentFileLevel FileLevel
        {
            get
            {
                return this.ItemXML.GetAttributeValueAsEnumValue<DocumentFileLevel>("_Level");
            }
        }

        public string FileRef
        {
            get
            {
                return string.Concat(this.FileDirRef, (this.FileDirRef.Length > 0 ? "/" : ""), this.FileLeafRef);
            }
        }

        public long FileSize
        {
            get
            {
                long num = (long)0;
                return ((this.ItemXML.Attributes["_FileSize"] == null ? true : !long.TryParse(this.ItemXML.Attributes["_FileSize"].Value, out num)) ? (long)0 : num);
            }
        }

        public Guid GUID
        {
            get
            {
                if ((this.m_Guid != Guid.Empty ? false : this.ItemXML.Attributes["GUID"] != null))
                {
                    this.m_Guid = new Guid(this.ItemXML.Attributes["GUID"].Value);
                }
                return this.m_Guid;
            }
        }

        public bool HasAttachments
        {
            get
            {
                bool flag;
                bool flag1;
                string value;
                if (this.ItemXML.Attributes["Attachments"] != null)
                {
                    value = this.ItemXML.Attributes["Attachments"].Value;
                }
                else
                {
                    value = null;
                }
                string str = value;
                if (!string.IsNullOrEmpty(str))
                {
                    if (bool.TryParse(str, out flag))
                    {
                        flag1 = flag;
                        return flag1;
                    }
                }
                flag1 = false;
                return flag1;
            }
        }

        public bool HasBinary
        {
            get
            {
                return this.m_binary != null;
            }
        }

        public bool HasFullXML
        {
            get
            {
                return this.m_itemXML != null;
            }
        }

        public bool HasPublishingPageContent
        {
            get
            {
                return ((this.ItemXML.Attributes["PublishingPageContent"] == null ? true : string.IsNullOrEmpty(this.ItemXML.Attributes["PublishingPageContent"].Value)) ? false : true);
            }
        }

        public bool HasPublishingPageLayout
        {
            get
            {
                return ((this.ItemXML.Attributes["PublishingPageLayout"] == null ? true : string.IsNullOrEmpty(this.ItemXML.Attributes["PublishingPageLayout"].Value)) ? false : true);
            }
        }

        public bool HasUniquePermissions
        {
            get
            {
                bool value;
                if (!(this is SPListItemVersion))
                {
                    if (!this.m_bHasUniquePermissions.HasValue)
                    {
                        if (this.ItemXML.Attributes["HasUniquePermissions"] != null)
                        {
                            this.m_bHasUniquePermissions = new bool?(bool.Parse(this.ItemXML.Attributes["HasUniquePermissions"].Value));
                        }
                        else if (!base.Adapter.IsNws)
                        {
                            this.m_bHasUniquePermissions = new bool?(false);
                        }
                        else
                        {
                            bool flag = false;
                            bool.TryParse(base.Adapter.Reader.HasUniquePermissions(this.m_parentList.ID, this.ID), out flag);
                            this.m_bHasUniquePermissions = new bool?(flag);
                        }
                    }
                    value = this.m_bHasUniquePermissions.Value;
                }
                else
                {
                    value = false;
                }
                return value;
            }
        }

        public bool HasWikiField
        {
            get
            {
                bool flag;
                flag = (this.ItemXML.Attributes["WikiField"] == null ? false : true);
                return flag;
            }
        }

        public bool HasWorkflows
        {
            get
            {
                bool value;
                bool flag = false;
                if (!this.m_bHasWorkflows.HasValue)
                {
                    if (this.m_itemXML.Attributes["GUID"] == null)
                    {
                        if (this.m_itemXML.Attributes["UniqueId"] != null)
                        {
                            bool.TryParse(this.ParentList.Adapter.Reader.HasWorkflows(this.ParentList.ID, this.m_itemXML.Attributes["UniqueId"].Value.ToString()), out flag);
                            this.m_bHasWorkflows = new bool?(flag);
                            value = this.m_bHasWorkflows.Value;
                            return value;
                        }
                        value = false;
                        return value;
                    }
                    else
                    {
                        bool.TryParse(this.ParentList.Adapter.Reader.HasWorkflows(this.ParentList.ID, this.m_itemXML.Attributes["GUID"].Value.ToString()), out flag);
                        this.m_bHasWorkflows = new bool?(flag);
                    }
                }
                value = this.m_bHasWorkflows.Value;
                return value;
            }
        }

        public int ID
        {
            get
            {
                return this.m_iID;
            }
        }

        public override System.Drawing.Image Image
        {
            get
            {
                System.Drawing.Image iconByExtensionAsImage;
                if (this.ImageName != null)
                {
                    iconByExtensionAsImage = ImageCache.GetIconByExtensionAsImage(this.ImageName);
                }
                else
                {
                    iconByExtensionAsImage = null;
                }
                return iconByExtensionAsImage;
            }
        }

        public override string ImageName
        {
            get
            {
                string str;
                if (this.FileLeafRef == null)
                {
                    str = null;
                }
                else if (this.FileLeafRef.IndexOf(".") >= 0)
                {
                    str = string.Concat("file.", this.FileLeafRef.Substring(this.FileLeafRef.LastIndexOf('.') + 1));
                }
                else
                {
                    str = null;
                }
                return str;
            }
        }

        public bool IsCheckedOut
        {
            get
            {
                bool flag;
                if (!this.m_bIsCheckedOut.HasValue)
                {
                    if (this.ParentList.IsDocumentLibrary)
                    {
                        if (this.ItemXML.Attributes["CheckedOutUserId"] == null || string.IsNullOrEmpty(this.ItemXML.Attributes["CheckedOutUserId"].Value))
                        {
                            flag = (this.ItemXML.Attributes["CheckoutUser"] == null ? false : !string.IsNullOrEmpty(this.ItemXML.Attributes["CheckoutUser"].Value));
                        }
                        else
                        {
                            flag = true;
                        }
                        this.m_bIsCheckedOut = new bool?(flag);
                    }
                    else
                    {
                        this.m_bIsCheckedOut = new bool?(false);
                    }
                }
                return this.m_bIsCheckedOut.Value;
            }
        }

        public virtual bool IsCurrentVersion
        {
            get
            {
                return true;
            }
        }

        public bool IsDBGhosted
        {
            get
            {
                if (!this._isDBGhostedCached.HasValue)
                {
                    this._isDBGhostedCached = new bool?(false);
                    if ((this.ItemXML == null || this.ItemXML.Attributes == null ? false : this.ItemXML.Attributes["_DocFlags"] != null))
                    {
                        string value = this.ItemXML.Attributes["_DocFlags"].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            this._isDBGhostedCached = new bool?((((long)Convert.ToInt32(value) & (long)64) > (long)0 ? false : true));
                        }
                    }
                }
                return this._isDBGhostedCached.Value;
            }
        }

        public bool IsDocumentSet
        {
            get
            {
                bool flag;
                bool flag1;
                if (this.ItemType == SPListItemType.Folder)
                {
                    XmlNode nodeXML = this.GetNodeXML();
                    string[] strArrays = new string[] { "SharePoint.DocumentSet" };
                    if (nodeXML.IsAttributeValueInSet("ProgId", strArrays))
                    {
                        flag1 = false;
                    }
                    else
                    {
                        strArrays = new string[] { "SharePoint.DocumentSet" };
                        flag1 = !nodeXML.IsAttributeValueInSet("HTML_x0020_File_x0020_Type", strArrays);
                    }
                    if (!flag1)
                    {
                        flag = true;
                        return flag;
                    }
                }
                flag = false;
                return flag;
            }
        }

        public bool IsExternalized
        {
            get
            {
                if (!this.m_bIsExternalized.HasValue)
                {
                    if ((this.ItemXML.Attributes["IsExternalized"] == null ? true : !bool.Parse(this.ItemXML.Attributes["IsExternalized"].Value)))
                    {
                        this.m_bIsExternalized = new bool?(false);
                    }
                    else
                    {
                        this.m_bIsExternalized = new bool?(true);
                    }
                }
                return this.m_bIsExternalized.Value;
            }
        }

        public bool IsInfoPathDocument
        {
            get
            {
                bool flag;
                if (this.ItemType == SPListItemType.Item)
                {
                    XmlNode nodeXML = this.GetNodeXML();
                    if ((nodeXML == null ? false : nodeXML.GetAttributeValueAsString("ProgId").StartsWith("InfoPath.Document")))
                    {
                        flag = true;
                        return flag;
                    }
                }
                flag = false;
                return flag;
            }
        }

        public bool IsNameConverted
        {
            get
            {
                return this.Name != this.FileLeafRef;
            }
        }

        public bool IsOneNoteFolder
        {
            get
            {
                bool flag;
                bool flag1;
                if (this.ItemType == SPListItemType.Folder)
                {
                    XmlNode nodeXML = this.GetNodeXML();
                    string[] strArrays = new string[] { "OneNote.Notebook" };
                    if (nodeXML.IsAttributeValueInSet("ProgId", strArrays))
                    {
                        flag1 = false;
                    }
                    else
                    {
                        strArrays = new string[] { "OneNote.Notebook" };
                        flag1 = !nodeXML.IsAttributeValueInSet("HTML_x0020_File_x0020_Type", strArrays);
                    }
                    if (!flag1)
                    {
                        flag = true;
                        return flag;
                    }
                }
                flag = false;
                return flag;
            }
        }

        public bool IsUnghostedPublishingPage
        {
            get
            {
                bool flag;
                XmlAttribute itemOf = this.ItemXML.Attributes["PublishingPageLayout"];
                flag = ((itemOf == null ? false : !string.IsNullOrEmpty(itemOf.Value)) ? itemOf.Value.StartsWith("http://www.microsoft.com/publishing?DisconnectedPublishingPage") : false);
                return flag;
            }
        }

        public virtual bool IsWebPartPage
        {
            get
            {
                if (!this.m_bIsWebPartPage.HasValue)
                {
                    this.m_bIsWebPartPage = new bool?(SPWebPartPage.IsWebPartPage(this));
                }
                return this.m_bIsWebPartPage.Value;
            }
        }

        public bool IsWelcomePage
        {
            get
            {
                bool flag;
                if (!this.bIsWelcomePage.HasValue)
                {
                    this.bIsWelcomePage = new bool?(false);
                    if ((this.ParentList == null ? false : this.ParentList.IsDocumentLibrary))
                    {
                        string serverRelativeUrl = this.ParentList.ParentWeb.ServerRelativeUrl;
                        char[] chrArray = new char[] { '/' };
                        string str = string.Concat(serverRelativeUrl.Trim(chrArray), "/default.aspx");
                        if (this.ParentList.ParentWeb.WelcomePageUrl == null)
                        {
                            flag = string.Equals(str, this.ConstantServerRelativeUrl, StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            string welcomePageUrl = this.ParentList.ParentWeb.WelcomePageUrl;
                            chrArray = new char[] { '/' };
                            string str1 = welcomePageUrl.Trim(chrArray);
                            string constantServerRelativeUrl = this.ConstantServerRelativeUrl;
                            chrArray = new char[] { '/' };
                            flag = string.Equals(str1, constantServerRelativeUrl.Trim(chrArray), StringComparison.OrdinalIgnoreCase);
                        }
                        this.bIsWelcomePage = new bool?(flag);
                    }
                }
                return this.bIsWelcomePage.Value;
            }
        }

        // Metalogix.SharePoint.SPListItem
        public override string this[string sFieldName]
        {
            get
            {
                SPField fieldByNames = ((SPFieldCollection)this.ParentList.Fields).GetFieldByNames(null, sFieldName);
                string result;
                if (fieldByNames != null)
                {
                    string name = XmlUtility.EncodeNameStartChars(sFieldName);
                    string text;
                    if (this.m_terseData != null && this.m_terseData.ContainsKey(sFieldName))
                    {
                        text = this.m_terseData[sFieldName];
                    }
                    else
                    {
                        if (this.ItemXML.Attributes[name] == null)
                        {
                            result = null;
                            return result;
                        }
                        text = this.ItemXML.Attributes[name].Value;
                    }
                    result = text;
                }
                else
                {
                    result = base[sFieldName];
                }
                return result;
            }
            set
            {
                if (this.ItemXML.Attributes[sFieldName] != null)
                {
                    this.ItemXML.Attributes[sFieldName].Value = value;
                }
                else
                {
                    XmlAttribute xmlAttribute = this.ItemXML.OwnerDocument.CreateAttribute(sFieldName);
                    xmlAttribute.Value = value;
                    this.ItemXML.Attributes.Append(xmlAttribute);
                }
            }
        }

        public SPListItemType ItemType
        {
            get
            {
                if (!this.m_itemType.HasValue)
                {
                    if (this.ItemXML.Attributes["FSObjType"] == null)
                    {
                        this.m_itemType = new SPListItemType?(SPListItemType.Item);
                    }
                    else if (this.ItemXML.Attributes["FSObjType"].Value == "0")
                    {
                        this.m_itemType = new SPListItemType?(SPListItemType.Item);
                    }
                    else if (this.ItemXML.Attributes["FSObjType"].Value == "1")
                    {
                        this.m_itemType = new SPListItemType?(SPListItemType.Folder);
                    }
                    else if (!(this.ItemXML.Attributes["FSObjType"].Value == "2"))
                    {
                        this.m_itemType = new SPListItemType?(SPListItemType.Invalid);
                    }
                    else
                    {
                        this.m_itemType = new SPListItemType?(SPListItemType.Web);
                    }
                }
                return this.m_itemType.Value;
            }
        }

        protected XmlNode ItemXML
        {
            get
            {
                if (this.m_itemXML == null)
                {
                    GetListItemOptions getListItemOption = new GetListItemOptions()
                    {
                        IncludeExternalizationData = true,
                        IncludePermissionsInheritance = true
                    };
                    ISharePointReader reader = base.Adapter.Reader;
                    string constantID = this.m_parentList.ConstantID;
                    int num = this.ConstantID;
                    string listItems = reader.GetListItems(constantID, num.ToString(), null, null, true, ListItemQueryType.ListItem | ListItemQueryType.Folder, this.m_parentList.ListSettingsXML, getListItemOption);
                    if (listItems == null)
                    {
                        throw new Exception("Internal Error: GetListItems() returned no items");
                    }
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(listItems);
                    XmlNode xmlNodes = base.AttachVirtualData(xmlDocument.SelectSingleNode("//ListItem"), "XML");
                    this.SetFullXML(xmlNodes);
                }
                return this.m_itemXML;
            }
        }

        public override string LinkableUrl
        {
            get
            {
                string str;
                if ((base.Adapter.ServerLinkName == null ? false : !(base.Adapter.ServerLinkName.Trim() == "")))
                {
                    string serverLinkName = base.Adapter.ServerLinkName;
                    StringBuilder stringBuilder = new StringBuilder(this.ParentList.ServerRelativeUrl);
                    stringBuilder.Append((this.ParentList.IsDocumentLibrary ? "/Forms" : ""));
                    stringBuilder.Append(this.GetListASPXPageString(this.ParentList.BaseTemplate));
                    stringBuilder.Append(this.ID.ToString());
                    string str1 = stringBuilder.ToString();
                    str = (!str1.StartsWith("/") ? string.Concat(serverLinkName, "/", str1) : string.Concat(serverLinkName, str1));
                }
                else
                {
                    str = null;
                }
                return str;
            }
        }

        public DateTime Modified
        {
            get
            {
                DateTime dateTime;
                dateTime = (!this.m_dateModified.HasValue ? Metalogix.SharePoint.Adapters.Utils.ParseDateAsUtc(this["Modified"]) : this.m_dateModified.Value);
                return dateTime;
            }
        }

        public string ModifiedBy
        {
            get
            {
                return this["Editor"];
            }
        }

        public override string Name
        {
            get
            {
                string str;
                str = (!this.ParentList.IsDocumentLibrary ? string.Concat("Item ID='", this.ID, "'") : this.FileLeafRef);
                return str;
            }
        }

        public int NoOfLatestVersionsToGet
        {
            get
            {
                return this._noOfLatestVersionsToGet;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("NoOfLatestVersionsToGet", string.Format("value:{0} is less than 0", value));
                }
                this._noOfLatestVersionsToGet = value;
            }
        }

        public virtual SPListItemCollection ParentCollection
        {
            get
            {
                return this.m_parentCollection;
            }
        }

        public SPFolder ParentFolder
        {
            get
            {
                return this.m_parentFolder;
            }
        }

        public SPList ParentList
        {
            get
            {
                return this.m_parentList;
            }
        }

        public string ParentRelativePath
        {
            get
            {
                string str;
                string dirName = this.ParentFolder.DirName;
                char[] chrArray = new char[] { '/' };
                string str1 = dirName.Trim(chrArray);
                string fileDirRef = this.FileDirRef;
                chrArray = new char[] { '/' };
                string str2 = fileDirRef.Trim(chrArray);
                string lower = str1.ToLower();
                string lower1 = str2.ToLower();
                if (!(lower1 == lower))
                {
                    str = (!lower1.StartsWith(lower) ? str2 : str2.Substring(str1.Length + 1));
                }
                else
                {
                    str = "";
                }
                return str;
            }
        }

        public SecurityPrincipalCollection Principals
        {
            get
            {
                return this.ParentList.ParentWeb.Principals;
            }
        }

        public RoleAssignmentCollection RoleAssignments
        {
            get
            {
                lock (this.m_oLockRoleAssignments)
                {
                    if (this.m_roleAssignments == null)
                    {
                        string roleAssignments = null;
                        roleAssignments = base.Adapter.Reader.GetRoleAssignments(this.ParentList.ConstantID, this.ConstantID);
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(roleAssignments);
                        XmlNode xmlNodes = base.AttachVirtualData(xmlDocument.DocumentElement, "RoleAssignments");
                        this.m_roleAssignments = new SPRoleAssignmentCollection(this, xmlNodes);
                        this.m_roleAssignments.RoleAssignmentCollectionChanged += new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
                    }
                }
                return this.m_roleAssignments;
            }
        }

        public RoleCollection Roles
        {
            get
            {
                return this.ParentList.ParentWeb.Roles;
            }
        }

        public string ServerRelativeFolderLeafRef
        {
            get
            {
                return string.Concat(this.FileDirRef, "/", this.FileLeafRef);
            }
        }

        public override string ServerRelativeUrl
        {
            get
            {
                string str;
                if ((this.ParentList.IsDocumentLibrary ? false : this.ItemType != SPListItemType.Folder))
                {
                    string serverRelativeUrl = this.ParentList.ServerRelativeUrl;
                    string listASPXPageString = this.GetListASPXPageString(this.ParentList.BaseTemplate);
                    int d = this.ID;
                    str = string.Concat(serverRelativeUrl, listASPXPageString, d.ToString());
                }
                else
                {
                    str = string.Concat(this.FileDirRef, "/", this.FileLeafRef);
                }
                return str;
            }
        }

        public string Title
        {
            get
            {
                string mSTitle;
                if (this.m_sTitle == null)
                {
                    mSTitle = (this.ItemXML.Attributes["Title"] != null ? this.ItemXML.Attributes["Title"].Value : "");
                }
                else
                {
                    mSTitle = this.m_sTitle;
                }
                return mSTitle;
            }
            set
            {
                this.m_sTitle = value;
                this["Title"] = value;
            }
        }

        public Guid UniqueId
        {
            get
            {
                if (this._uniqueId == Guid.Empty)
                {
                    this._uniqueId = this.ItemXML.GetAttributeValueAsGuid("UniqueId");
                }
                return this._uniqueId;
            }
        }

        public override string Url
        {
            get
            {
                string str = string.Concat(base.Parent.Url, this.ParentRelativePath, "/", this.FileLeafRef);
                return str;
            }
        }

        public virtual string VersionComments
        {
            get
            {
                return "";
            }
        }

        public virtual ListItemVersionCollection VersionHistory
        {
            get
            {
                if (this.m_versionHistory == null)
                {
                    this.m_versionHistory = new SPListItemVersionCollection(this);
                    this.m_versionHistory.FetchData();
                }
                return this.m_versionHistory;
            }
        }

        public virtual string VersionString
        {
            get
            {
                string str = "_UIVersionString";
                return (this.ItemXML.Attributes[str] != null ? this.ItemXML.Attributes[str].Value : "");
            }
        }

        public virtual SPWebPartPage WebPartPage
        {
            get
            {
                if (this.m_webPartPage == null)
                {
                    if (this.IsWebPartPage)
                    {
                        this.m_webPartPage = new SPWebPartPage(this, null);
                    }
                }
                return this.m_webPartPage;
            }
        }

        public override string XML
        {
            get
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("ListItem");
                foreach (XmlAttribute attribute in this.ItemXML.Attributes)
                {
                    xmlTextWriter.WriteAttributeString(attribute.Name, attribute.Value);
                }
                if (this.HasAttachments)
                {
                    xmlTextWriter.WriteStartElement("Attachments");
                    foreach (SPAttachment attachment in this.Attachments)
                    {
                        xmlTextWriter.WriteRaw(attachment.XML);
                    }
                    xmlTextWriter.WriteEndElement();
                }
                xmlTextWriter.WriteEndElement();
                return stringWriter.ToString();
            }
        }

        public string XMLWithVersions
        {
            get
            {
                bool flag;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("ListItem");
                foreach (XmlAttribute attribute in this.ItemXML.Attributes)
                {
                    xmlTextWriter.WriteAttributeString(attribute.Name, attribute.Value);
                }
                if (this.HasAttachments)
                {
                    xmlTextWriter.WriteStartElement("Attachments");
                    foreach (SPAttachment attachment in this.Attachments)
                    {
                        xmlTextWriter.WriteRaw(attachment.XML);
                    }
                    xmlTextWriter.WriteEndElement();
                }
                if (!this.CanHaveVersions)
                {
                    flag = true;
                }
                else if (this.ItemXML.Attributes["_Level"] == null || !(this.ItemXML.Attributes["_Level"].Value == "255"))
                {
                    flag = false;
                }
                else if (!this.ParentList.EnableMinorVersions || this.ItemXML.Attributes["_UIVersionString"] == null || !(this.ItemXML.Attributes["_UIVersionString"].Value == "0.1"))
                {
                    flag = (this.ParentList.EnableMinorVersions || this.ItemXML.Attributes["_UIVersionString"] == null ? false : this.ItemXML.Attributes["_UIVersionString"].Value == "1.0");
                }
                else
                {
                    flag = true;
                }
                if (!flag)
                {
                    xmlTextWriter.WriteStartElement("Versions");
                    foreach (SPListItemVersion versionHistory in this.VersionHistory)
                    {
                        xmlTextWriter.WriteRaw(versionHistory.XML);
                    }
                    xmlTextWriter.WriteEndElement();
                }
                xmlTextWriter.WriteEndElement();
                return stringWriter.ToString();
            }
        }

        static SPListItem()
        {
            SPListItem.PropertyCulture = new CultureInfo(1033);
        }

        protected SPListItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, int iID, string sFileLeafRef, string sFileDirRef, DateTime? dateModified, bool? bIsExternalized, bool? bBinaryAvailable, SPListItemType itemType) : base(parentList.Adapter, parentFolder)
        {
            this.m_parentFolder = parentFolder;
            this.m_parentList = parentList;
            this.m_parentCollection = parentCollection;
            this.m_iID = iID;
            this.m_sFileLeafRef = sFileLeafRef;
            this.m_sFileDirRef = sFileDirRef;
            this.m_dateModified = dateModified;
            this.m_bIsExternalized = bIsExternalized;
            this.m_bBinaryAvailable = bBinaryAvailable;
            this.m_itemType = new SPListItemType?(itemType);
            this.m_iConstantID = iID;
            this.m_sConstantFileDirRef = sFileDirRef;
            this.m_sConstantFileLeafRef = sFileLeafRef;
            Dictionary<string, string> strs = new Dictionary<string, string>();
            this.SaveTerseProperties(strs);
            this.m_terseData = strs;
        }

        protected SPListItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, int iID, Dictionary<string, string> terseProperties) : base(parentList.Adapter, parentFolder)
        {
            this.m_parentFolder = parentFolder;
            this.m_parentList = parentList;
            this.m_parentCollection = parentCollection;
            this.m_iID = iID;
            this.m_iConstantID = iID;
            this.UpdateTersePropertiesFromDictionary(terseProperties);
            this.m_terseData = terseProperties;
        }

        protected SPListItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, XmlNode listItemXML) : base(parentList.Adapter, parentFolder)
        {
            this.m_parentFolder = parentFolder;
            this.m_parentList = parentList;
            this.m_parentCollection = parentCollection;
            this.m_iID = Convert.ToInt32(listItemXML.Attributes["ID"].Value);
            this.m_iConstantID = this.m_iID;
            this.m_itemXML = listItemXML;
            this.UpdateTersePropertiesFromXml(listItemXML);
            Dictionary<string, string> strs = new Dictionary<string, string>();
            this.SaveTerseProperties(strs);
            this.m_terseData = strs;
        }

        public override bool AnalyzeChurn(DateTime pivotDate, bool bRecursive, out long lByteschanged, out long lItemsChanged)
        {
            bool flag;
            lByteschanged = (long)0;
            lItemsChanged = (long)0;
            if (this.ParentList.ParentWeb.IsSearchable)
            {
                string str = base.Adapter.Reader.AnalyzeChurn(pivotDate, this.ParentList.ConstantID, this.ConstantID, bRecursive);
                if (!string.IsNullOrEmpty(str))
                {
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
                    string str1 = (xmlNode.Attributes["ItemsChanged"] != null ? xmlNode.Attributes["ItemsChanged"].Value : "0");
                    string str2 = (xmlNode.Attributes["BytesChanged"] != null ? xmlNode.Attributes["BytesChanged"].Value : "0");
                    bool flag1 = long.TryParse(str1, out lItemsChanged);
                    flag1 = long.TryParse(str2, out lByteschanged);
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        protected override void ClearChildNodes()
        {
        }

        protected override void ClearExcessNodeData()
        {
            base.ClearExcessNodeData();
            this.m_itemXML = null;
            this.UpdateTersePropertiesFromDictionary(this.m_terseData);
            this.ReleasePermissionsData();
            this.m_attachments = null;
            this.m_versionHistory = null;
            this.m_terseData = null;
            this.m_bHasWorkflows = null;
            this.m_alerts = null;
            this.m_binary = null;
            this.m_webPartPage = null;
            this.m_bIsWebPartPage = null;
        }

        public static SPListItem CreateListItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, XmlNode listItemXML)
        {
            SPListItem sPDiscussionItem;
            if (parentList is SPDiscussionList)
            {
                sPDiscussionItem = new SPDiscussionItem(parentList, parentFolder, parentCollection, listItemXML);
            }
            else if (!SPPageLayout.IsPageLayout(listItemXML))
            {
                sPDiscussionItem = new SPListItem(parentList, parentFolder, parentCollection, listItemXML);
            }
            else
            {
                sPDiscussionItem = new SPPageLayout(parentList, parentFolder, parentCollection, listItemXML);
            }
            return sPDiscussionItem;
        }

        public static SPListItem CreateListItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, int iID, string sFileLeafRef, string sFileDirRef, DateTime? dateModified, bool? bIsExternalized, bool? bBinaryAvailable, SPListItemType itemType)
        {
            SPListItem sPListItem;
            if (!(parentList is SPDiscussionList))
            {
                sPListItem = new SPListItem(parentList, parentFolder, parentCollection, iID, sFileLeafRef, sFileDirRef, dateModified, bIsExternalized, bBinaryAvailable, itemType);
            }
            else
            {
                sPListItem = new SPDiscussionItem(parentList, parentFolder, parentCollection, iID);
            }
            return sPListItem;
        }

        protected static SPListItem CreateListItemFromTerseData(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, int iID, Dictionary<string, string> terseProperties)
        {
            SPListItem sPListItem;
            if (!(parentList is SPDiscussionList))
            {
                sPListItem = new SPListItem(parentList, parentFolder, parentCollection, iID, terseProperties);
            }
            else
            {
                sPListItem = new SPDiscussionItem(parentList, parentFolder, parentCollection, iID, terseProperties);
            }
            return sPListItem;
        }

        public static SPListItem CreateListItemFromTerseXml(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, XmlNode terseItemXML)
        {
            int num = Convert.ToInt32(terseItemXML.Attributes["ID"].Value);
            Dictionary<string, string> strs = new Dictionary<string, string>(terseItemXML.Attributes.Count - 1);
            foreach (XmlAttribute attribute in terseItemXML.Attributes)
            {
                if (attribute.Name != "ID")
                {
                    strs.Add(attribute.Name, attribute.Value);
                }
            }
            if (terseItemXML.Attributes["FileRef"] == null)
            {
                if (!strs.ContainsKey("FileLeafRef"))
                {
                    strs.Add("FileLeafRef", "");
                }
                if (!strs.ContainsKey("FileDirRef"))
                {
                    strs.Add("FileDirRef", "");
                }
            }
            else
            {
                string value = terseItemXML.Attributes["FileRef"].Value;
                string str = value.Substring(value.LastIndexOf("/") + 1);
                string str1 = value.Substring(0, value.LastIndexOf("/"));
                if (!strs.ContainsKey("FileLeafRef"))
                {
                    strs.Add("FileLeafRef", str);
                }
                else
                {
                    strs["FileLeafRef"] = str;
                }
                if (!strs.ContainsKey("FileDirRef"))
                {
                    strs.Add("FileDirRef", str1);
                }
                else
                {
                    strs["FileDirRef"] = str1;
                }
            }
            return SPListItem.CreateListItemFromTerseData(parentList, parentFolder, parentCollection, num, strs);
        }

        public override bool Equals(object obj)
        {
            bool flag;
            flag = (base.GetType() == obj.GetType() ? this == obj as SPListItem : false);
            return flag;
        }

        protected override Node[] FetchChildNodes()
        {
            return new Node[0];
        }

        public byte[] GetBinary()
        {
            byte[] documentVersion;
            string value;
            string str;
            if ((!this.m_parentList.IsDocumentLibrary ? false : this.BinaryAvailable))
            {
                Dictionary<int, ExternalConnection> externalConnectionsOfType = base.GetExternalConnectionsOfType<StoragePointExternalConnection>(true);
                if (externalConnectionsOfType.Count <= 0)
                {
                    if (this.ItemXML.Attributes["UniqueId"] == null)
                    {
                        value = null;
                    }
                    else
                    {
                        value = this.ItemXML.Attributes["UniqueId"].Value;
                    }
                    string str1 = value;
                    if ((this.ItemXML.Attributes["_IsCurrentVersion"] != null ? !bool.Parse(this.ItemXML.Attributes["_IsCurrentVersion"].Value) : false))
                    {
                        string str2 = (base.Adapter.SharePointVersion.IsSharePoint2003 ? "_UIVersionString" : "_UIVersion");
                        if (this.ItemXML.Attributes[str2] == null)
                        {
                            str = "0";
                        }
                        else if (base.Adapter.SharePointVersion.IsSharePoint2003)
                        {
                            double num = Convert.ToDouble(this.ItemXML.Attributes[str2].Value);
                            str = num.ToString();
                        }
                        else
                        {
                            str = this.ItemXML.Attributes[str2].Value;
                        }
                        int num1 = Convert.ToInt32(str);
                        string str3 = string.Concat(this.m_sConstantFileLeafRef, (this.m_parentList.IsPublishingLibrary ? string.Concat("?VersionNo=", num1) : ""));
                        string mSConstantFileDirRef = this.m_sConstantFileDirRef;
                        char[] chrArray = new char[] { '/' };
                        string str4 = string.Concat(mSConstantFileDirRef.TrimEnd(chrArray), (!this.m_parentList.IsPublishingLibrary ? string.Concat("/_vti_history/", num1) : ""));
                        chrArray = new char[] { '/' };
                        string str5 = str4.TrimStart(chrArray);
                        documentVersion = base.Adapter.Reader.GetDocumentVersion(str1, str5, str3, num1);
                    }
                    else
                    {
                        int num2 = (this.ItemXML.Attributes["_Level"] != null ? Convert.ToInt32(this.ItemXML.Attributes["_Level"].Value) : 1);
                        documentVersion = base.Adapter.Reader.GetDocument(str1, this.m_sConstantFileDirRef, this.m_sConstantFileLeafRef, num2);
                    }
                }
                else
                {
                    KeyValuePair<int, ExternalConnection> keyValuePair = externalConnectionsOfType.First<KeyValuePair<int, ExternalConnection>>();
                    documentVersion = ((StoragePointExternalConnection)keyValuePair.Value).GetBLOB(this.GetBlobRef());
                }
            }
            else
            {
                documentVersion = null;
            }
            return documentVersion;
        }

        public byte[] GetBlobRef()
        {
            byte[] documentVersionBlobRef;
            string value;
            if (this.IsExternalized)
            {
                if (this.ItemXML.Attributes["UniqueId"] == null)
                {
                    value = null;
                }
                else
                {
                    value = this.ItemXML.Attributes["UniqueId"].Value;
                }
                string str = value;
                if ((this.ItemXML.Attributes["_IsCurrentVersion"] != null ? !bool.Parse(this.ItemXML.Attributes["_IsCurrentVersion"].Value) : false))
                {
                    int num = (this.ItemXML.Attributes["_UIVersion"] != null ? Convert.ToInt32(this.ItemXML.Attributes["_UIVersion"].Value) : 0);
                    string str1 = string.Concat(this.m_sConstantFileLeafRef, (this.m_parentList.IsPublishingLibrary ? string.Concat("?VersionNo=", num) : ""));
                    string mSConstantFileDirRef = this.m_sConstantFileDirRef;
                    char[] chrArray = new char[] { '/' };
                    string str2 = string.Concat(mSConstantFileDirRef.TrimEnd(chrArray), (!this.m_parentList.IsPublishingLibrary ? string.Concat("/_vti_history/", num) : ""));
                    chrArray = new char[] { '/' };
                    string str3 = str2.TrimStart(chrArray);
                    documentVersionBlobRef = base.Adapter.Reader.GetDocumentVersionBlobRef(str, str3, str1, num);
                }
                else
                {
                    int num1 = (this.ItemXML.Attributes["_Level"] != null ? Convert.ToInt32(this.ItemXML.Attributes["_Level"].Value) : 1);
                    documentVersionBlobRef = base.Adapter.Reader.GetDocumentBlobRef(str, this.m_sConstantFileDirRef, this.m_sConstantFileLeafRef, num1);
                }
            }
            else
            {
                documentVersionBlobRef = null;
            }
            return documentVersionBlobRef;
        }

        public SPContentType GetContentType()
        {
            SPContentType item;
            string contentTypeId = this.ContentTypeId;
            if (string.IsNullOrEmpty(contentTypeId))
            {
                item = null;
            }
            else
            {
                item = this.m_parentList.ContentTypes[contentTypeId];
            }
            return item;
        }

        public override int GetHashCode()
        {
            return this.ParentList.GetHashCode() + this.ID.GetHashCode();
        }

        private string GetListASPXPageString(ListTemplateType templateType)
        {
            string str;
            if (templateType == ListTemplateType.BlogPosts)
            {
                str = "/Post.aspx?ID=";
            }
            else if (templateType != ListTemplateType.BlogCategories)
            {
                str = (templateType != ListTemplateType.BlogComments ? "/DispForm.aspx?ID=" : "/ViewComment.aspx?ID=");
            }
            else
            {
                str = "/ViewCategory.aspx?ID=";
            }
            return str;
        }

        public override XmlNode GetNodeXML()
        {
            return this.ItemXML;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();
            foreach (SPField field in this.ParentList.Fields)
            {
                SPPropertyDescriptor sPPropertyDescriptor = new SPPropertyDescriptor(field);
                if (sPPropertyDescriptor.Attributes.Contains(attributes))
                {
                    propertyDescriptors.Add(sPPropertyDescriptor);
                }
            }
            return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            object obj;
            if (pd == null)
            {
                obj = this;
            }
            else if (this.GetProperties().Contains(pd))
            {
                obj = this;
            }
            else if (!TypeDescriptor.GetProperties(base.GetType()).Contains(pd))
            {
                obj = null;
            }
            else
            {
                obj = this;
            }
            return obj;
        }

        public string GetReferencedManagedMetadata()
        {
            string referencedTaxonomyFullXml;
            SPFieldCollection taxonomyFields = this.ParentList.FieldCollection.GetTaxonomyFields();
            if (taxonomyFields.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings);
                try
                {
                    xmlWriter.WriteStartElement("TaxonomyFields");
                    foreach (SPField taxonomyField in taxonomyFields)
                    {
                        if (this.ItemXML.Attributes[taxonomyField.Name] != null)
                        {
                            taxonomyField.SetReferencedManagedMetadata(xmlWriter, this.ItemXML.Attributes[taxonomyField.Name].Value);
                        }
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                }
                finally
                {
                    if (xmlWriter != null)
                    {
                        ((IDisposable)xmlWriter).Dispose();
                    }
                }
                referencedTaxonomyFullXml = this.ParentList.Adapter.Reader.GetReferencedTaxonomyFullXml(stringBuilder.ToString());
            }
            else
            {
                referencedTaxonomyFullXml = string.Empty;
            }
            return referencedTaxonomyFullXml;
        }

        public SecurityPrincipalCollection GetReferencedPrincipals()
        {
            string value;
            SecurityPrincipalCollection principals;
            SPFieldCollection fields = (SPFieldCollection)this.m_parentList.Fields;
            string[] strArrays = new string[] { "User", "UserMulti" };
            SPFieldCollection fieldsOfTypes = fields.GetFieldsOfTypes(strArrays);
            bool flag = false;
            if (this.m_parentList.Adapter.SharePointVersion.IsSharePoint2007OrLater)
            {
                foreach (SPField fieldsOfType in fieldsOfTypes)
                {
                    if (fieldsOfType.AllowsGroups)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            Hashtable hashtables = new Hashtable();
            foreach (SPField sPField in fieldsOfTypes)
            {
                if (this.ItemXML.Attributes[sPField.Name] != null)
                {
                    value = this.ItemXML.Attributes[sPField.Name].Value;
                }
                else
                {
                    value = null;
                }
                string str = value;
                if (str != null)
                {
                    char[] chrArray = new char[] { ',' };
                    string[] strArrays1 = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < (int)strArrays1.Length; i++)
                    {
                        string str1 = strArrays1[i];
                        if (!hashtables.ContainsKey(str1))
                        {
                            hashtables.Add(str1, str1);
                        }
                    }
                }
            }
            if (flag)
            {
                principals = this.m_parentList.ParentWeb.Principals;
            }
            else
            {
                principals = this.m_parentList.ParentWeb.SiteUsers;
            }
            return SPUtils.GetReferencedPrincipals(hashtables, principals, flag);
        }

        public override bool IsEqual(Metalogix.DataStructures.IComparable comparableItem, DifferenceLog differencesOutput, ComparisonOptions options)
        {
            bool flag;
            string[] versionString;
            bool flag1;
            bool flag2;
            bool flag3;
            if (comparableItem == null)
            {
                throw new Exception("Cannot compare an SPListItem to a null value");
            }
            if (!(comparableItem is SPListItem))
            {
                throw new Exception("SPListItem can only be compared to another SPListItem");
            }
            bool flag4 = true;
            SPListItem sPListItem = (SPListItem)comparableItem;
            XmlNode itemXML = this.ItemXML;
            XmlNode xmlNodes = sPListItem.ItemXML;
            bool flag5 = false;
            if (flag5)
            {
                if (!(new SPWebPartPage(this, null)).IsEqual(new SPWebPartPage(sPListItem, null), differencesOutput, options))
                {
                    flag4 = false;
                }
            }
            foreach (XmlAttribute attribute in itemXML.Attributes)
            {
                if (!string.IsNullOrEmpty(attribute.Value))
                {
                    if ((attribute.Name == "DiscussionTitleLookup" || attribute.Name == "DiscussionTitle" || attribute.Name == "ReplyNoGif" || attribute.Name == "ThreadingControls" || attribute.Name == "IndentLevel" || attribute.Name == "Indentation" || attribute.Name == "BodyAndMore" || attribute.Name == "MessageBody" || attribute.Name == "BodyWasExpanded" || attribute.Name == "PermMask" || attribute.Name == "QuotedTextWasExpanded" || attribute.Name == "CorrectBodyToShow" || attribute.Name == "FullBody" || attribute.Name == "LimitedBody" || attribute.Name == "MoreLink" || attribute.Name == "LessLink" || attribute.Name == "ToggleQuotedText" || attribute.Name == "Threading" || attribute.Name == "PersonImage" || attribute.Name == "PersonViewMinimal" || attribute.Name == "IsRootPost" || attribute.Name == "ItemChildCount" || attribute.Name == "LinkTitleNoMenu" || attribute.Name == "LinkTitle" || attribute.Name == "LinkDiscussionTitleNoMenu" || attribute.Name == "LinkDiscussionTitle" || attribute.Name == "StatusBar" || attribute.Name == "Completed" || attribute.Name == "DisplayResponse" || attribute.Name == "DisplayResponseNoMenu" ? false : !(attribute.Name == "PermMask")))
                    {
                        if (attribute.Name == "ParentFolderId" || attribute.Name == "FileRef" || attribute.Name == "GUID" || attribute.Name == "UniqueId" || attribute.Name == "Order" || attribute.Name == "_VersionNumber" || attribute.Name == "_VersionModified" || attribute.Name == "_DocFlags" || attribute.Name == "MetaInfo" || attribute.Name == "_VersionMetaInfo" || attribute.Name == "ContentTypeId" || attribute.Name == "ScopeUrl" || attribute.Name == "ScopeId" || attribute.Name == "ServerUrl" || attribute.Name == "EncodedAbsUrl" || attribute.Name == "Edit" || attribute.Name == "xd_ProgId" || attribute.Name == "VirusStatus" || attribute.Name == "SelectFilename" || attribute.Name == "SelectTitle" || attribute.Name == "SummaryLinks" || attribute.Name == "SummaryLinks2" || attribute.Name == "FileDirRef" || attribute.Name == "URL" || attribute.Name == "Created_x0020_Date" || attribute.Name == "Last_x0020_Modified" || attribute.Name == "DocConcurrencyNumber")
                        {
                            flag1 = false;
                        }
                        else
                        {
                            flag1 = (attribute.Name == "LinkFilenameNoMenu" || attribute.Name == "LinkFilename" || attribute.Name == "BaseName" || attribute.Name == "ShortestThreadIndexIdLookup" || attribute.Name == "Workspace" || attribute.Name == "FromExternal" || attribute.Name == "EditUser" || attribute.Name == "RecurrenceID" ? false : !(attribute.Name == "Duration"));
                        }
                        if (flag1)
                        {
                            if ((attribute.Name == "IssueID" ? false : !(attribute.Name == "RelatedID")))
                            {
                                if (!(attribute.Name == "HasUniquePermissions"))
                                {
                                    if ((attribute.Name == "IsExternalized" || attribute.Name == "RbsId" ? false : !(attribute.Name == "BinaryAvailable")))
                                    {
                                        if ((!flag5 ? true : !(attribute.Name == "_SetupPath")))
                                        {
                                            if (!(attribute.Name == "ProgId"))
                                            {
                                                XmlAttribute xmlAttribute = XmlUtility.GetAttribute(xmlNodes, null, attribute.Name, false);
                                                if ((xmlAttribute == null ? false : !string.IsNullOrEmpty(xmlAttribute.Value)))
                                                {
                                                    bool lower = attribute.Value.ToLower() == xmlAttribute.Value.ToLower();
                                                    if ((xmlAttribute.Name == "PublishingPageLayout" ? true : xmlAttribute.Name == "PublishingPageImage"))
                                                    {
                                                        if ((itemXML.Attributes["ScopeUrl"] == null ? false : xmlNodes.Attributes["ScopeUrl"] != null))
                                                        {
                                                            string str = attribute.Value.ToLower();
                                                            string lower1 = xmlAttribute.Value.ToLower();
                                                            if (xmlAttribute.Name == "PublishingPageLayout")
                                                            {
                                                                char[] chrArray = new char[] { ',' };
                                                                string[] strArrays = str.Split(chrArray);
                                                                chrArray = new char[] { ',' };
                                                                string[] strArrays1 = lower1.Split(chrArray);
                                                                str = strArrays[0];
                                                                lower1 = strArrays1[0];
                                                            }
                                                            int num = (lower1.IndexOf(xmlNodes.Attributes["ScopeUrl"].Value.ToLower()) == -1 ? 0 : lower1.IndexOf(xmlNodes.Attributes["ScopeUrl"].Value.ToLower()));
                                                            lower1 = lower1.Remove(num, (xmlNodes.Attributes["ScopeUrl"].Value.Length == 0 ? 0 : xmlNodes.Attributes["ScopeUrl"].Value.Length + 1));
                                                            int num1 = (str.IndexOf(itemXML.Attributes["ScopeUrl"].Value.ToLower()) == -1 ? 0 : str.IndexOf(itemXML.Attributes["ScopeUrl"].Value.ToLower()));
                                                            str = str.Remove(num1, (itemXML.Attributes["ScopeUrl"].Value.Length == 0 ? 0 : itemXML.Attributes["ScopeUrl"].Value.Length + 1));
                                                            lower = lower1 == str;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    Attribute[] nameAttribute = new Attribute[] { new NameAttribute(attribute.Name) };
                                                    PropertyDescriptorCollection properties = this.GetProperties(nameAttribute);
                                                    if (!lower)
                                                    {
                                                        object value = null;
                                                        object obj = null;
                                                        if (properties.Count > 0)
                                                        {
                                                            value = properties[0].GetValue(this);
                                                            obj = properties[0].GetValue(sPListItem);
                                                        }
                                                        if ((value == null ? false : obj != null))
                                                        {
                                                            lower = (lower || value == obj ? true : value.Equals(obj));
                                                        }
                                                    }
                                                    if (!lower)
                                                    {
                                                        SPField fieldByNames = ((SPFieldCollection)this.ParentList.Fields).GetFieldByNames(null, attribute.Name);
                                                        if (!(fieldByNames == null ? true : !(fieldByNames.Type == "Note")))
                                                        {
                                                            differencesOutput.Write(string.Concat("The '", attribute.Name, "' attribute value is different - This is expected when comparing Rich Text fields between different versions of SharePoint. "), attribute.Name, DifferenceStatus.Difference, true);
                                                            continue;
                                                        }
                                                        else if (!(attribute.Name == "ID"))
                                                        {
                                                            if (!this.ParentList.IsDocumentLibrary)
                                                            {
                                                                if (attribute.Name == "FileLeafRef")
                                                                {
                                                                    differencesOutput.Write("The 'FileLeafRef' attribute value is different - This may be expected if ID preservation is not turned on. ", "FileLeafRef", DifferenceStatus.Difference, true);
                                                                    continue;
                                                                }
                                                            }
                                                            if (!(attribute.Name != "_ModerationStatus" ? true : this.ParentList.EnableModeration))
                                                            {
                                                                differencesOutput.Write("The '_ModerationStatus' attribute value is different - Since Approval is not enabled on the source list, this does not matter. ", "_ModerationStatus", DifferenceStatus.Difference, true);
                                                                continue;
                                                            }
                                                            else if ((!(attribute.Name == "_ModerationStatus") || !this.ParentList.EnableModeration ? true : this.ItemType != SPListItemType.Folder))
                                                            {
                                                                if (attribute.Name == "_ModerationStatus")
                                                                {
                                                                    differencesOutput.Write("The '_ModerationStatus' attribute value is different.", "_ModerationStatus");
                                                                    flag4 = false;
                                                                }
                                                                if (!(attribute.Name == "_FileSize" || attribute.Name == "File_x0020_Size" ? false : !(attribute.Name == "FileSizeDisplay")))
                                                                {
                                                                    differencesOutput.Write("The '_FileSize' attribute value is different - This is expected when comparing file sizes between different versions of SharePoint. ", "_FileSize", DifferenceStatus.Difference, true);
                                                                    continue;
                                                                }
                                                                else if (!(attribute.Name != "_CheckinComment" ? true : attribute.Value.Length <= 1023))
                                                                {
                                                                    differencesOutput.Write("The '_CheckinComment' attribute value is different - Note that the source value is greater than 1023 characters, and MOSS only supports 1023 characters for checkin comments.", "_CheckinComment", DifferenceStatus.Difference, true);
                                                                    continue;
                                                                }
                                                                else if (!(attribute.Name == "owshiddenversion"))
                                                                {
                                                                    if (this.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard)
                                                                    {
                                                                        if ((attribute.Name == "ThreadIndex" ? true : attribute.Name == "Order"))
                                                                        {
                                                                            continue;
                                                                        }
                                                                    }
                                                                    if (this.ParentList.BaseTemplate == ListTemplateType.Events)
                                                                    {
                                                                        if (itemXML.Attributes["fAllDayEvent"] != null)
                                                                        {
                                                                            XmlAttribute itemOf = itemXML.Attributes["fAllDayEvent"];
                                                                            nameAttribute = new Attribute[] { new NameAttribute(itemOf.Name) };
                                                                            PropertyDescriptorCollection propertyDescriptorCollections = this.GetProperties(nameAttribute);
                                                                            object value1 = propertyDescriptorCollections[0].GetValue(this);
                                                                            if (value1 != null)
                                                                            {
                                                                                bool.TryParse(value1.ToString(), out flag);
                                                                            }
                                                                            else
                                                                            {
                                                                                flag = false;
                                                                            }
                                                                            if (!flag)
                                                                            {
                                                                                flag3 = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                flag3 = (attribute.Name == "EventDate" || attribute.Name == "EndDate" ? false : !(attribute.Name == "Date"));
                                                                            }
                                                                            if (!flag3)
                                                                            {
                                                                                continue;
                                                                            }
                                                                        }
                                                                        if (attribute.Name == "UID")
                                                                        {
                                                                            if (attribute.Value.ToUpper().Replace("{", "").Replace("}", "") == xmlAttribute.Value.ToUpper().Replace("{", "").Replace("}", ""))
                                                                            {
                                                                                continue;
                                                                            }
                                                                        }
                                                                        if (attribute.Name == "TimeZone")
                                                                        {
                                                                            continue;
                                                                        }
                                                                    }
                                                                    if (this.ParentList.BaseTemplate == ListTemplateType.PictureLibrary)
                                                                    {
                                                                        if ((attribute.Name == "EncodedAbsThumbnailUrl" || attribute.Name == "EncodedAbsWebImgUrl" ? true : attribute.Name == "RequiredField"))
                                                                        {
                                                                            continue;
                                                                        }
                                                                    }
                                                                    if ((!attribute.Name.StartsWith("_") || !(attribute.Name != "_UIVersion") || !(attribute.Name != "_UIVersionString") ? true : !(attribute.Name != "_VersionString")))
                                                                    {
                                                                        if (!(this is SPListItemVersion) || ((SPListItemVersion)this).IsCurrentVersion)
                                                                        {
                                                                            flag2 = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            flag2 = (attribute.Name == "ImageWidth" ? false : !(attribute.Name == "ImageHeight"));
                                                                        }
                                                                        if (!flag2)
                                                                        {
                                                                            continue;
                                                                        }
                                                                        else if (!(attribute.Name == "Modified_x0020_By" ? false : !(attribute.Name == "Created_x0020_By")))
                                                                        {
                                                                            differencesOutput.Write(string.Concat("The '", attribute.Name, "' attribute value is different. "), attribute.Name, DifferenceStatus.Difference, true);
                                                                            continue;
                                                                        }
                                                                        else if (!(attribute.Name == "Modified_x0020_By" || attribute.Name == "Created_x0020_By" || attribute.Name == "Editor" || attribute.Name == "Author" || attribute.Name == "Modified" || attribute.Name == "Created" || attribute.Name == "Last_x0020_Modified" || attribute.Name == "Created_x0020_Date" ? false : !(attribute.Name == "Links_x0020_for_x0020_Pub")))
                                                                        {
                                                                            if (options.Level == CompareLevel.Strict)
                                                                            {
                                                                                differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. "), attribute.Name, DifferenceStatus.Difference);
                                                                                flag4 = false;
                                                                            }
                                                                            differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. "), attribute.Name, DifferenceStatus.Difference, true);
                                                                            continue;
                                                                        }
                                                                        else if ((attribute.Name == "NameOverloaded" || attribute.Name == "NameDisplay" || attribute.Name == "NameDisplayLink" ? false : !(attribute.Name == "_EditMenuTableEnd")))
                                                                        {
                                                                            differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. "), attribute.Name);
                                                                            flag4 = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            differencesOutput.Write(string.Concat("The '", attribute.Name, "' attribute value is different. "), attribute.Name, DifferenceStatus.Difference, true);
                                                                            continue;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        differencesOutput.Write(string.Concat("The '", attribute.Name, "' attribute value is different. "), attribute.Name, DifferenceStatus.Difference, true);
                                                                        continue;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                differencesOutput.Write("The '_ModerationStatus' attribute value is different - Note that approval statuses will not preserved for folders. ", "_ModerationStatus", DifferenceStatus.Difference, true);
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            differencesOutput.Write("The 'ID' attribute value is different - This may be expected behavior if the target list was not overwritten during the migration. ", "ID", DifferenceStatus.Difference, true);
                                                            continue;
                                                        }
                                                    }
                                                }
                                                else if (!(attribute.Name == "Author" ? false : !(attribute.Name == "Editor")))
                                                {
                                                    differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. This is likely caused by the target being set to not show user data."), attribute.Name, DifferenceStatus.Difference, true);
                                                    continue;
                                                }
                                                else if (attribute.Name == "CheckoutUser")
                                                {
                                                    differencesOutput.Write("The checked out state of the file differs from source to target. ", "Checked out state", DifferenceStatus.Difference, true);
                                                    continue;
                                                }
                                                else if ((!(attribute.Name == "_ModerationComments") || !this.ParentList.EnableModeration ? true : this.ItemType != SPListItemType.Folder))
                                                {
                                                    if ((this.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard ? true : this.ParentList.BaseType == Metalogix.SharePoint.ListType.DiscussionForum))
                                                    {
                                                        if ((attribute.Name == "ThreadID" || attribute.Name == "Ordering" || attribute.Name == "ShortestThreadIndexId" ? true : attribute.Name == "ShortestThreadIndexIdLookup"))
                                                        {
                                                            differencesOutput.Write(string.Concat("The '", attribute.Name, "' attribute value is missing - this may be expected if migrating between SharePoint versions. "), attribute.Name, DifferenceStatus.Missing, true);
                                                            continue;
                                                        }
                                                    }
                                                    if (this.ParentList.BaseTemplate == ListTemplateType.FormLibrary)
                                                    {
                                                        if (attribute.Name == "xd_Signature")
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    if (!(attribute.Name == "_VersionLevel" || attribute.Name == "_FileSize" ? false : !(attribute.Name == "_VersionIsCurrent")))
                                                    {
                                                        continue;
                                                    }
                                                    else if (!(attribute.Name == "ParentLeafName" || attribute.Name == "ParentVersionString" || attribute.Name == "MetaInfo" || attribute.Name == "DocIcon" || attribute.Name == "CheckedOutTitle" || attribute.Name == "VirusStatus" || attribute.Name == "IsCheckedoutToLocal" || attribute.Name == "CheckedOutUserId" || attribute.Name == "FSObjType" || attribute.Name == "FileRef" || attribute.Name == "Last_x0020_Modified" || attribute.Name == "Created_x0020_Date" || attribute.Name == "File_x0020_Type" || attribute.Name == "File_x0020_Size" || attribute.Name == "LinkFilename" || attribute.Name == "LinkFilenameNoMenu" || attribute.Name == "BaseName" || attribute.Name == "PermMask" || attribute.Name.StartsWith("_") || attribute.Name == "HTML_x0020_File_x0020_Type" || attribute.Name == "Combine" || attribute.Name == "RepairDocument" ? false : !(attribute.Name == "FileSizeDisplay")))
                                                    {
                                                        continue;
                                                    }
                                                    else if (attribute.Value == string.Empty)
                                                    {
                                                        continue;
                                                    }
                                                    else if ((attribute.Name == "Modified_x0020_By" ? false : !(attribute.Name == "Created_x0020_By")))
                                                    {
                                                        if ((attribute.Name == "_CheckinComment" ? true : attribute.Name == "_CheckinComment"))
                                                        {
                                                            if (attribute.Value == string.Empty)
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        if (attribute.Name == "WorkflowVersion")
                                                        {
                                                            continue;
                                                        }
                                                        else if (!attribute.Name.Equals("CategoryID", StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            if (this.ParentList.BaseTemplate == ListTemplateType.Links)
                                                            {
                                                                if ((attribute.Name == "URLwMenu" ? true : attribute.Name == "URLNoMenu"))
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            if (this.ParentList.BaseTemplate == ListTemplateType.Issues)
                                                            {
                                                                if ((attribute.Name == "LinkTitleVersionNoMenu" ? true : attribute.Name == "LinkIssueIDNoMenu"))
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            if ((this.ParentList.BaseTemplate != ListTemplateType.PictureLibrary ? false : options.Level == CompareLevel.Moderate))
                                                            {
                                                                if (attribute.Name == "ImageCreateDate")
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            if (attribute.Name == "ContentType")
                                                            {
                                                                SPContentType contentType = sPListItem.GetContentType();
                                                                if (contentType != null)
                                                                {
                                                                    if (attribute.Value == contentType.Name)
                                                                    {
                                                                        continue;
                                                                    }
                                                                }
                                                            }
                                                            differencesOutput.Write(string.Concat("The attribute: '", attribute.Name, "' is missing. "), attribute.Name, DifferenceStatus.Missing);
                                                            flag4 = false;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    differencesOutput.Write("The '_ModerationComments' attribute value is missing. ", "_ModerationComments", DifferenceStatus.Missing, true);
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if ((this.CanHaveVersions ? true : sPListItem.CanHaveVersions))
            {
                if (this.VersionHistory.Count != sPListItem.VersionHistory.Count)
                {
                    object[] count = new object[] { "The target item does not have the same number of versions. The source has: ", this.VersionHistory.Count, " and the target has: ", sPListItem.VersionHistory.Count, " " };
                    differencesOutput.Write(string.Concat(count), "Number of Versions");
                    flag4 = false;
                }
                if (((SPListItemVersionCollection)sPListItem.VersionHistory).HasTempVersions)
                {
                    differencesOutput.Write("The target version collection has some temporary versions which were not deleted. ", "Temporary versions", DifferenceStatus.Difference, true);
                }
                for (int i = 0; i < this.VersionHistory.Count; i++)
                {
                    SPListItemVersion item = (SPListItemVersion)this.VersionHistory[i];
                    SPListItemVersion sPListItemVersion = null;
                    if (i >= sPListItem.VersionHistory.Count)
                    {
                        differencesOutput.Write(string.Concat("Version", item.VersionString, " is missing."), item.VersionString, DifferenceStatus.Missing);
                        flag4 = false;
                    }
                    else
                    {
                        sPListItemVersion = (SPListItemVersion)sPListItem.VersionHistory[i];
                        DifferenceLog differenceLogs = new DifferenceLog();
                        if (!item.IsEqual(sPListItemVersion, differenceLogs, options))
                        {
                            versionString = new string[] { "Version ", item.VersionString, " was different: ", differenceLogs.ToString(), " " };
                            differencesOutput.Write(string.Concat(versionString), item.VersionString);
                            flag4 = false;
                        }
                        if (differenceLogs.ToString().Length > 0)
                        {
                            versionString = new string[] { "Version ", item.VersionString, " was different: ", differenceLogs.ToString(), " " };
                            differencesOutput.Write(string.Concat(versionString), item.VersionString, DifferenceStatus.Difference, true);
                            break;
                        }
                    }
                }
            }
            return flag4;
        }

        public static bool operator ==(SPListItem item1, SPListItem item2)
        {
            bool flag;
            if (object.ReferenceEquals(item1, item2))
            {
                flag = true;
            }
            else if ((object.ReferenceEquals(item1, null) ? false : !object.ReferenceEquals(item2, null)))
            {
                flag = (item1.ParentList != item2.ParentList ? false : item1.ID == item2.ID);
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public static bool operator !=(SPListItem item1, SPListItem item2)
        {
            bool flag;
            if (object.ReferenceEquals(item1, item2))
            {
                flag = false;
            }
            else if ((object.ReferenceEquals(item1, null) ? false : !object.ReferenceEquals(item2, null)))
            {
                flag = (item1.ParentList != item2.ParentList ? true : item1.ID != item2.ID);
            }
            else
            {
                flag = true;
            }
            return flag;
        }

        public virtual void ReleasePermissionsData()
        {
            lock (this.m_oLockRoleAssignments)
            {
                if (this.m_roleAssignments != null)
                {
                    this.m_roleAssignments.RoleAssignmentCollectionChanged -= new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
                    this.m_roleAssignments = null;
                }
            }
        }

        private void RoleAssignmentCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (!(this is SPListItemVersion))
            {
                if (this.m_xmlNode != null)
                {
                    if (this.ItemXML.Attributes["HasUniquePermissions"] != null)
                    {
                        this.ItemXML.Attributes["HasUniquePermissions"].Value = "True";
                    }
                }
                this.m_bHasUniquePermissions = new bool?(true);
            }
        }

        protected void SaveTerseProperties(Dictionary<string, string> terseProperties)
        {
            terseProperties.Clear();
            terseProperties.Add("FileLeafRef", this.m_sFileLeafRef);
            terseProperties.Add("FileDirRef", this.m_sFileDirRef);
            if (this.m_dateModified.HasValue)
            {
                terseProperties.Add("Modified", Metalogix.SharePoint.Adapters.Utils.FormatDateToUTC(this.m_dateModified.Value));
            }
            if (this.m_bIsExternalized.HasValue)
            {
                terseProperties.Add("IsExternalized", this.m_bIsExternalized.ToString());
            }
            if (this.m_bBinaryAvailable.HasValue)
            {
                terseProperties.Add("BinaryAvailable", this.m_bBinaryAvailable.ToString());
            }
            if (this.m_bHasUniquePermissions.HasValue)
            {
                terseProperties.Add("HasUniquePermissions", this.m_bHasUniquePermissions.ToString());
            }
            if (this.m_itemType.HasValue)
            {
                terseProperties.Add("FSObjType", this.m_itemType.Value.ToString());
            }
        }

        public void SetFullXML(XmlNode xmlNode)
        {
            this.m_itemXML = xmlNode;
            this.m_terseData = null;
            this.UpdateTersePropertiesFromXml(this.m_itemXML);
        }

        public void UpdateDocument(string sListItemXML, byte[] fileContents)
        {
            XmlNode xmlNodes = null;
            if (!this.WriteVirtually)
            {
                if (this.ParentList.Adapter.Writer == null)
                {
                    throw new Exception("The underlying SharePoint adapter does not support write operations");
                }
                string str = null;
                if (!this.ParentList.IsDocumentLibrary)
                {
                    throw new InvalidOperationException("Cannot update documents in lists which are not document libraries");
                }
                str = (fileContents != null ? this.ParentList.Adapter.Writer.UpdateDocument(this.m_parentList.ConstantID.ToString(), this.m_parentFolder.ToString(), this.ConstantFileLeafRef, sListItemXML, fileContents, new UpdateDocumentOptions()) : this.ParentList.Adapter.Writer.UpdateListItem(this.m_parentList.ConstantID.ToString(), this.m_parentFolder.ToString(), this.ConstantID, sListItemXML, null, null, new UpdateListItemOptions()));
                xmlNodes = XmlUtility.StringToXmlNode(str).SelectSingleNode(".//ListItem");
            }
            else
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
                base.SaveVirtualData(this.ItemXML, xmlNode, "XML");
                this.m_binary = fileContents;
                xmlNodes = xmlNode;
            }
            this.m_terseData = null;
            this.UpdateTersePropertiesFromXml(xmlNodes);
        }

        public void UpdateListItem(string sListItemXML, string[] attachmentNames, byte[][] attachmentContents)
        {
            XmlNode xmlNodes = null;
            if (!this.WriteVirtually)
            {
                if (this.ParentList.Adapter.Writer == null)
                {
                    throw new Exception("The underlying SharePoint adapter does not support write operations");
                }
                string str = this.ParentList.Adapter.Writer.UpdateListItem(this.m_parentList.ConstantID.ToString(), this.m_parentFolder.ToString(), this.ConstantID, sListItemXML, attachmentNames, attachmentContents, new UpdateListItemOptions());
                this.m_attachments = null;
                xmlNodes = XmlUtility.StringToXmlNode(str).SelectSingleNode(".//ListItem");
            }
            else
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
                base.SaveVirtualData(this.ItemXML, xmlNode, "XML");
                xmlNodes = xmlNode;
                if ((attachmentNames == null ? false : (int)attachmentNames.Length > 0))
                {
                    this.Attachments.UpdateAttachments(attachmentNames, attachmentContents);
                }
            }
            this.SetFullXML(xmlNodes);
        }

        public void UpdatePublishStatus(bool bPublish, bool bCheckin, bool bApprove, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            ISharePointWriter writer = base.Adapter.Writer;
            string xML = this.XML;
            string str = this.ParentList.XML;
            int d = this.ID;
            writer.UpdateListItemStatus(bPublish, bCheckin, bApprove, xML, str, d.ToString(), sCheckinComment, sPublishComment, sApprovalComment);
        }

        protected void UpdateTersePropertiesFromDictionary(Dictionary<string, string> terseProperties)
        {
            string str;
            bool flag;
            if (terseProperties != null)
            {
                if (terseProperties.TryGetValue("FileLeafRef", out str))
                {
                    this.m_sFileLeafRef = str;
                    if (this.m_sConstantFileLeafRef == null)
                    {
                        this.m_sConstantFileLeafRef = str;
                    }
                }
                if (terseProperties.TryGetValue("FileDirRef", out str))
                {
                    this.m_sFileDirRef = str;
                    if (this.m_sConstantFileDirRef == null)
                    {
                        this.m_sConstantFileDirRef = str;
                    }
                }
                if (terseProperties.TryGetValue("Modified", out str))
                {
                    this.m_dateModified = new DateTime?(Metalogix.SharePoint.Adapters.Utils.ParseDateAsUtc(str));
                }
                if (terseProperties.TryGetValue("IsExternalized", out str))
                {
                    if (!bool.TryParse(str, out flag))
                    {
                        this.m_bIsExternalized = new bool?(false);
                    }
                    else
                    {
                        this.m_bIsExternalized = new bool?(flag);
                    }
                }
                if (terseProperties.TryGetValue("BinaryAvailable", out str))
                {
                    if (!bool.TryParse(str, out flag))
                    {
                        this.m_bBinaryAvailable = new bool?(true);
                    }
                    else
                    {
                        this.m_bBinaryAvailable = new bool?(flag);
                    }
                }
                if (terseProperties.TryGetValue("HasUniquePermissions", out str))
                {
                    if (!bool.TryParse(str, out flag))
                    {
                        this.m_bHasUniquePermissions = new bool?(false);
                    }
                    else
                    {
                        this.m_bHasUniquePermissions = new bool?(flag);
                    }
                }
                if (terseProperties.TryGetValue("FSObjType", out str))
                {
                    this.m_itemType = new SPListItemType?((str != null ? (SPListItemType)Enum.Parse(typeof(SPListItemType), str) : SPListItemType.Item));
                }
                if (terseProperties.TryGetValue("UniqueId", out str))
                {
                    this._uniqueId = new Guid(str);
                }
            }
        }

        protected void UpdateTersePropertiesFromXml(XmlNode xmlNode)
        {
            string value;
            bool flag;
            if (xmlNode.Attributes["FileLeafRef"] != null)
            {
                this.m_sFileLeafRef = xmlNode.Attributes["FileLeafRef"].Value;
                if (this.m_sConstantFileLeafRef == null)
                {
                    this.m_sConstantFileLeafRef = this.m_sFileLeafRef;
                }
            }
            if (xmlNode.Attributes["FileDirRef"] != null)
            {
                this.m_sFileDirRef = xmlNode.Attributes["FileDirRef"].Value;
                if (this.m_sConstantFileDirRef == null)
                {
                    this.m_sConstantFileDirRef = this.m_sFileDirRef;
                }
            }
            if (xmlNode.Attributes["Modified"] != null)
            {
                this.m_dateModified = new DateTime?(Metalogix.SharePoint.Adapters.Utils.ParseDateAsUtc(xmlNode.Attributes["Modified"].Value));
            }
            if (xmlNode.Attributes["IsExternalized"] != null)
            {
                value = xmlNode.Attributes["IsExternalized"].Value;
                if (!bool.TryParse(value, out flag))
                {
                    this.m_bIsExternalized = new bool?(false);
                }
                else
                {
                    this.m_bIsExternalized = new bool?(flag);
                }
            }
            if (xmlNode.Attributes["BinaryAvailable"] != null)
            {
                value = xmlNode.Attributes["BinaryAvailable"].Value;
                if (!bool.TryParse(value, out flag))
                {
                    this.m_bBinaryAvailable = new bool?(false);
                }
                else
                {
                    this.m_bBinaryAvailable = new bool?(flag);
                }
            }
            if (xmlNode.Attributes["HasUniquePermissions"] != null)
            {
                value = xmlNode.Attributes["HasUniquePermissions"].Value;
                if (!bool.TryParse(value, out flag))
                {
                    this.m_bHasUniquePermissions = new bool?(false);
                }
                else
                {
                    this.m_bHasUniquePermissions = new bool?(flag);
                }
            }
            if (xmlNode.Attributes["FSObjType"] != null)
            {
                value = xmlNode.Attributes["FSObjType"].Value;
                SPListItemType sPListItemType = SPListItemType.Item;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        sPListItemType = (SPListItemType)Enum.Parse(typeof(SPListItemType), value);
                    }
                    catch (ArgumentException argumentException)
                    {
                    }
                }
                this.m_itemType = new SPListItemType?(sPListItemType);
            }
        }
    }
}