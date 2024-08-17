using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Jobs;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
    [Analyzable(true)]
    [BasicModeViewAllowed(true)]
    [CmdletEnabled(true, "Copy-MLSharePointItem", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
    [Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
    [Incrementable(true, "Paste Items Incrementally")]
    [MandatoryTransformers(new Type[] { typeof(ManagedMetadataItemValueMapper), typeof(DeletionPropagator), typeof(ReferencedListItemDataUpdater), typeof(DocumentSetsApplicator), typeof(ContentTypesApplicator), typeof(ListItemColumnMapper), typeof(WorkflowDataUpdater), typeof(PublishingDataUpdater), typeof(InfopathItemContentXml) })]
    [MenuText("1:Paste Selected Item... {0-Paste}")]
    [MenuTextPlural("1:Paste Selected Items... {0-Paste}", PluralCondition.MultipleSources)]
    [Name("Paste Selected Items")]
    [Shortcut(ShortcutAction.Paste)]
    [ShowStatusDialog(true)]
    [SourceCardinality(Cardinality.OneOrMore)]
    [SourceType(typeof(SPListItem), true)]
    [SubActionTypes(new Type[] { typeof(CopyRoleAssignmentsAction), typeof(CopyContentTypesAction), typeof(CopyWebPartsAction), typeof(CopyUsersAction) })]
    [SupportsThreeStateConfiguration(true)]
    [TargetCardinality(Cardinality.OneOrMore)]
    [TargetType(typeof(SPFolder))]
    public class PasteListItemAction : PasteAction<PasteListItemOptions>
    {
        private const string DOCUMENT_SET_ID = "0x0120D520";

        private readonly object _syncPermissionsCopy = new object();

        private static TransformerDefinition<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection> s_listItemTransformerDefinition;

        public Dictionary<SPList, List<SPListItem>> DocumentSets = new Dictionary<SPList, List<SPListItem>>();

        static PasteListItemAction()
        {
            PasteListItemAction.s_listItemTransformerDefinition = new TransformerDefinition<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>("SharePoint Items", false);
        }

        public PasteListItemAction()
        {
        }

        private void AddAttachmentsToManifestItem(SPAttachmentCollection attachments, BaseManifestItemWithAttachments manifestItem, SPFolder targetFolder, IUploadManager uploadManager)
        {
            foreach (SPAttachment attachment in attachments)
            {
                byte[] numArray = ((!base.SharePointOptions.ShallowCopyExternalizedData || attachment.BinaryAvailable || !attachment.IsExternalized ? false : SharePointConfigurationVariables.AllowDBWriting) ? attachment.GetBlobRef() : attachment.Binary);
                if (numArray == null)
                {
                    throw new ArgumentException("Could not fetch document binary. This may be because the source data is externalized. To migrate externalized data, please enabled database writing");
                }
                if (targetFolder.ParentList.ParentWeb.MaximumFileSize > 0 && targetFolder.ParentList.ParentWeb.MaximumFileSize < (int)numArray.Length / 1048576)
                {
                    throw new ArgumentException("The file cannot be processed because it exceeds the maximum size specified by the SharePoint web application.");
                }
                string str = uploadManager.SaveDocument(numArray);
                ManifestAttachment manifestAttachment = new ManifestAttachment()
                {
                    Filename = attachment.FileName,
                    LocalFilename = str,
                    FileSize = (int)numArray.Length
                };
                manifestItem.Attachments.Add(manifestAttachment);
            }
        }

        private void AddDocumentSetsToCollection(SPListItem sourceItem, SPList targetList)
        {
            if (!this.DocumentSets.ContainsKey(targetList))
            {
                this.DocumentSets.Add(targetList, new List<SPListItem>()
                {
                    sourceItem
                });
                return;
            }
            List<SPListItem> item = this.DocumentSets[targetList];
            item.Add(sourceItem);
            this.DocumentSets[targetList] = item;
        }

        private void AddFieldsToManifestItem(SPListItem sourceItem, BaseManifestItem manifestItem, IUploadManager uploadManager)
        {
            string value;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceItem.XML);
            foreach (Metalogix.Office365.Field fieldName in uploadManager.FieldNames)
            {
                if (xmlNode.Attributes[fieldName.Name] != null)
                {
                    value = xmlNode.Attributes[fieldName.Name].Value;
                }
                else
                {
                    value = null;
                }
                string str = value;
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }
                if (str.Equals("SharePoint.DocumentSet") && fieldName.Name.Equals("HTML_x0020_File_x0020_Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    str = "Sharepoint.DocumentSet";
                }
                Metalogix.Office365.Field field = new Metalogix.Office365.Field()
                {
                    Name = fieldName.Name,
                    Value = str,
                    FieldId = fieldName.FieldId,
                    Access = fieldName.Access,
                    Type = fieldName.Type,
                    IsReadOnly = fieldName.IsReadOnly,
                    TaxonomyHiddenTextFieldId = fieldName.TaxonomyHiddenTextFieldId,
                    TaxonomyHiddenTextFieldName = fieldName.TaxonomyHiddenTextFieldName
                };
                Metalogix.Office365.Field manifestTaxonomyValue = field;
                string type = fieldName.Type;
                string[] strArrays = new string[] { "TaxonomyFieldType", "TaxonomyFieldTypeMulti" };
                if (type.In<string>(strArrays))
                {
                    manifestTaxonomyValue.Value = PasteListItemAction.GetManifestTaxonomyValue(str, manifestTaxonomyValue.Type);
                }
                string type1 = fieldName.Type;
                string[] strArrays1 = new string[] { "LookupMulti", "Lookup" };
                if (type1.In<string>(strArrays1))
                {
                    if (manifestTaxonomyValue.IsReadOnly)
                    {
                        continue;
                    }
                    manifestTaxonomyValue.Value = this.GetLookupValue(str, manifestTaxonomyValue.Type);
                }
                string str1 = fieldName.Type;
                string[] strArrays2 = new string[] { "User", "UserMulti" };
                if (str1.In<string>(strArrays2))
                {
                    this.MapUsersForManifestField(manifestTaxonomyValue, str, uploadManager);
                }
                manifestItem.FieldValues.Add(manifestTaxonomyValue);
                string type2 = fieldName.Type;
                string[] strArrays3 = new string[] { "TaxonomyFieldType", "TaxonomyFieldTypeMulti" };
                if (!type2.In<string>(strArrays3))
                {
                    continue;
                }
                Metalogix.Office365.Field field1 = new Metalogix.Office365.Field()
                {
                    Name = manifestTaxonomyValue.TaxonomyHiddenTextFieldName,
                    FieldId = manifestTaxonomyValue.TaxonomyHiddenTextFieldId,
                    Access = manifestTaxonomyValue.Access,
                    Type = "Note",
                    Value = str
                };
                manifestItem.FieldValues.Add(field1);
            }
        }

        private void AddItemPermissionsToAzureManifest(SPListItem sourceItem, SPList target, BaseManifestItem baseManifestItem, bool isFolder, IUploadManager uploadManager)
        {
            if (base.CheckForAbort())
            {
                return;
            }
            LogItem logItem = new LogItem(string.Format("Processing {0} permissions", (isFolder ? "folder" : "item")), sourceItem.Name, sourceItem.DisplayUrl, string.Concat(target.DisplayUrl, "/", sourceItem.Name), ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            try
            {
                CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
                copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
                base.SubActions.Add(copyRoleAssignmentsAction);
                object[] objArray = new object[] { sourceItem, target, false, uploadManager, baseManifestItem };
                copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceItem.ParentList.ParentWeb, target.ParentWeb), null);
                logItem.Status = ActionOperationStatus.Completed;
            }
            catch (Exception exception)
            {
                logItem.Exception = exception;
            }
            base.FireOperationFinished(logItem);
        }

        public override Dictionary<string, string> AnalyzeAction(Job parentJob, DateTime pivotDate)
        {
            Dictionary<string, string> strs = new Dictionary<string, string>();
            long num = (long)0;
            long num1 = (long)0;
            IXMLAbleList sourceList = parentJob.SourceList;
            if (typeof(SPNode).IsAssignableFrom(sourceList.CollectionType))
            {
                bool flag = true;
                foreach (SPNode sPNode in sourceList)
                {
                    long num2 = (long)0;
                    long num3 = (long)0;
                    flag = (!flag ? false : sPNode.AnalyzeChurn(pivotDate, false, out num3, out num2));
                    if (!flag)
                    {
                        continue;
                    }
                    num1 += num3;
                    num += num2;
                }
                if (flag)
                {
                    strs.Add("ItemsChanged", num.ToString());
                    strs.Add("BytesChanged", Format.FormatSize(new long?(num1)));
                }
            }
            return strs;
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            bool flag;
            if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
            {
                return false;
            }
            List<Metalogix.SharePoint.ListType> listTypes = new List<Metalogix.SharePoint.ListType>();
            if (!(sourceSelections[0] is SPListItem))
            {
                foreach (object sourceSelection in sourceSelections)
                {
                    SPFolder sPFolder = sourceSelection as SPFolder;
                    if (sPFolder == null)
                    {
                        continue;
                    }
                    Metalogix.SharePoint.ListType baseType = sPFolder.ParentList.BaseType;
                    if (listTypes.Contains(baseType))
                    {
                        continue;
                    }
                    listTypes.Add(baseType);
                }
            }
            else
            {
                listTypes.Add(((SPListItem)sourceSelections[0]).ParentList.BaseType);
            }
            List<Metalogix.SharePoint.ListType> listTypes1 = new List<Metalogix.SharePoint.ListType>();
            if (targetSelections[0] is SPFolder)
            {
                foreach (object targetSelection in targetSelections)
                {
                    SPFolder sPFolder1 = targetSelection as SPFolder;
                    if (sPFolder1 == null)
                    {
                        continue;
                    }
                    Metalogix.SharePoint.ListType listType = sPFolder1.ParentList.BaseType;
                    if (listTypes1.Contains(listType))
                    {
                        continue;
                    }
                    listTypes1.Add(listType);
                }
            }
            List<Metalogix.SharePoint.ListType>.Enumerator enumerator = listTypes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Metalogix.SharePoint.ListType current = enumerator.Current;
                    List<Metalogix.SharePoint.ListType>.Enumerator enumerator1 = listTypes1.GetEnumerator();
                    try
                    {
                        while (enumerator1.MoveNext())
                        {
                            if (current == enumerator1.Current)
                            {
                                continue;
                            }
                            flag = false;
                            return flag;
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator1).Dispose();
                    }
                }
                return true;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
            return flag;
        }

        private bool CheckIfGhosted(SPListItem sourceItem)
        {
            bool flag = false;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sourceItem.XML);
            XmlNode documentElement = xmlDocument.DocumentElement;
            if (documentElement.Attributes["_DocFlags"] != null)
            {
                string value = documentElement.Attributes["_DocFlags"].Value;
                if (!string.IsNullOrEmpty(value))
                {
                    int num = Convert.ToInt32(value);
                    long num1 = (long)64;
                    flag = (((long)num & num1) > (long)0 ? false : true);
                }
            }
            return flag;
        }

        // Metalogix.SharePoint.Actions.Migration.PasteListItemAction
        private long CopyDocument(SPListItem sourceItem, SPFolder targetFolder, SPListItem existingItem, out SPListItem newItem, out bool bItemNewlyCreated, out string sSourceXml, LogItem copyOperationLogItem, SPListItemCollection targetItemCollection)
        {
            newItem = null;
            bItemNewlyCreated = false;
            long num = 0L;
            sSourceXml = null;
            sSourceXml = sourceItem.XML;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sSourceXml);
            bool flag = (sourceItem.ParentList.EnableMinorVersions && xmlNode.Attributes["_UIVersionString"] != null && xmlNode.Attributes["_UIVersionString"].Value == "0.1") || (!sourceItem.ParentList.EnableMinorVersions && xmlNode.Attributes["_UIVersionString"] != null && xmlNode.Attributes["_UIVersionString"].Value == "1.0");
            string a = (xmlNode.Attributes["_Level"] == null) ? null : xmlNode.Attributes["_Level"].Value;
            if (flag && a == "255")
            {
                copyOperationLogItem.Status = ActionOperationStatus.Skipped;
                copyOperationLogItem.Information = "Document has no checked in state";
                return 0L;
            }
            bool flag2 = false;
            if (!(sourceItem.ParentList.Name == "Workflows") || sourceItem.ParentList.BaseTemplate != ListTemplateType.NoCodeWorkflows)
            {
                flag2 = sourceItem.IsWebPartPage;
            }
            AddDocumentOptions addDocumentOptions = new AddDocumentOptions();
            addDocumentOptions.PreserveID = base.SharePointOptions.PreserveDocumentIDs;
            addDocumentOptions.FileChunkSizeInMB = SharePointConfigurationVariables.FileChunkSizeInMB;
            addDocumentOptions.OverrideCheckouts = base.SharePointOptions.OverrideCheckouts;
            addDocumentOptions.AllowDBWriting = SharePointConfigurationVariables.AllowDBWriting;
            addDocumentOptions.Overwrite = false;
            addDocumentOptions.PreserveSharePointDocumentIDs = base.SharePointOptions.PreserveSharePointDocumentIDs;
            bool flag3 = base.SharePointOptions.CopyVersions && sourceItem.CanHaveVersions && targetFolder.ParentList.EnableVersioning && !(sourceItem is SPListItemVersion);
            if (flag3)
            {
                ListItemVersionCollection versionHistory = sourceItem.VersionHistory;
                flag3 = (versionHistory.Count > 0);
            }
            if (!flag3)
            {
                sourceItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sourceItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
                if (sourceItem == null)
                {
                    return num;
                }
                sSourceXml = sourceItem.XML;
                byte[] array = null;
                bool flag4 = base.SharePointOptions.ShallowCopyExternalizedData && !sourceItem.HasBinary && sourceItem.IsExternalized && SharePointConfigurationVariables.AllowDBWriting;
                addDocumentOptions.ShallowCopyExternalizedData = false;
                addDocumentOptions.SideLoadDocumentsToStoragePoint = ((!flag2 || !sourceItem.HasBinary) && base.SharePointOptions.SideLoadDocuments);
                if (this.IsInfoPathFormDocument(sourceItem) && this.CorrectLinksInInfoPathDocument(copyOperationLogItem, sourceItem, addDocumentOptions, out array))
                {
                    num += (long)array.Length;
                }
                if (array == null)
                {
                    if (flag4)
                    {
                        array = sourceItem.GetBlobRef();
                        num += sourceItem.FileSize;
                    }
                    else
                    {
                        array = sourceItem.Binary;
                        num += (long)((array != null) ? array.Length : 0);
                    }
                }
                num += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                addDocumentOptions.PreserveID = (base.SharePointOptions.PreserveDocumentIDs && SharePointConfigurationVariables.AllowDBWriting);
                if (array == null)
                {
                    throw new ArgumentException("Could not fetch document binary. This may be because the source data is externalized. To migrate externalized data, please enabled database writing");
                }
                if (targetFolder.ParentList.ParentWeb.MaximumFileSize > 0 && targetFolder.ParentList.ParentWeb.MaximumFileSize < array.Length / 1048576)
                {
                    throw new ArgumentException("The file cannot be processed because it exceeds the maximum size specified by the SharePoint web application.");
                }
                string text = sourceItem.ParentRelativePath;
                if (!string.IsNullOrEmpty(text) && base.SharePointOptions.RenameSpecificNodes)
                {
                    text = this.LinkCorrectParentFolderName(sourceItem);
                }
                this.SetTargetParentFolderName(sourceItem, targetFolder.ParentList, ref text);
                newItem = targetItemCollection.AddDocument(text, sSourceXml, addDocumentOptions, array, AddDocumentMode.Comprehensive);
                bItemNewlyCreated = (existingItem == null && newItem != null);
            }
            else
            {
                bool flag5 = false;
                if (targetFolder.ParentList.ParentWeb.WelcomePageUrl != null && targetFolder.ParentList.ParentWeb.WelcomePageUrl.Contains(sourceItem.Name) && targetFolder.ParentList.ParentWeb.WelcomePageUrl.Contains(sourceItem.FileDirRef) && sourceItem.Name.ToLower() == "default.aspx" && targetItemCollection.Count == 1 && targetItemCollection.GetItemByFileName(sourceItem.FileLeafRef) != null)
                {
                    targetItemCollection.CorrectDefaultPageVersions(sourceItem.ParentRelativePath, sSourceXml);
                    flag5 = true;
                }
                int? num2 = null;
                bool flag6 = false;
                int num3 = base.SharePointOptions.MaximumVersionCount;
                bool flag7 = base.SharePointOptions.CopyMaxVersions && !sourceItem.ParentList.IsWorkflowLibrary;
                if (sourceItem.Adapter.SharePointVersion.IsSharePointOnline && sourceItem.ParentList.IsWorkflowLibrary)
                {
                    num3 = 1;
                    flag7 = true;
                }
                try
                {
                    int i = 0;
                    while (i < sourceItem.VersionHistory.Count)
                    {
                        SPListItem sPListItem = (SPListItem)sourceItem.VersionHistory[i];
                        if (!sPListItem.IsCheckedOut)
                        {
                            goto IL_4F4;
                        }
                        XmlNode xmlNode2 = XmlUtility.StringToXmlNode(sPListItem.XML);
                        if (xmlNode2.Attributes["_Level"] == null || !(xmlNode2.Attributes["_Level"].Value == "255"))
                        {
                            goto IL_4F4;
                        }
                        IL_794:
                        i++;
                        continue;
                        IL_4F4:
                        if (base.SharePointOptions.FilterItems && !base.SharePointOptions.ItemFilterExpression.Evaluate(sPListItem, new CompareDatesInUtc()))
                        {
                            goto IL_794;
                        }
                        bool flag8 = i == sourceItem.VersionHistory.Count - 1;
                        int num4 = sourceItem.VersionHistory.Count - i - 1;
                        bool flag9 = !flag7 || num4 < num3 || (sPListItem.IsCurrentVersion && sPListItem.FileLevel == DocumentFileLevel.Published);
                        bool flag10 = this.HasNewerVersions(sPListItem, existingItem);
                        bool flag11 = flag10 && (flag8 || flag9);
                        if (!flag11)
                        {
                            goto IL_794;
                        }
                        sPListItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sPListItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
                        if (sPListItem == null)
                        {
                            goto IL_794;
                        }
                        string xML = sPListItem.XML;
                        string sItemXML = num2.HasValue ? this.UpdateItemID(xML, num2.Value) : xML;
                        bool flag12 = base.SharePointOptions.ShallowCopyExternalizedData && !sPListItem.HasBinary && sPListItem.IsExternalized && SharePointConfigurationVariables.AllowDBWriting;
                        addDocumentOptions.ShallowCopyExternalizedData = false;
                        addDocumentOptions.SideLoadDocumentsToStoragePoint = ((!flag2 || !sPListItem.HasBinary) && base.SharePointOptions.SideLoadDocuments);
                        byte[] array2 = flag12 ? sPListItem.GetBlobRef() : sPListItem.Binary;
                        if (flag12)
                        {
                            num += sPListItem.FileSize;
                        }
                        else
                        {
                            num += (long)((array2 != null) ? array2.Length : 0);
                        }
                        num += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                        if (array2 == null)
                        {
                            throw new ArgumentException("Could not fetch document binary. This may be because the source data is externalized. To migrate externalized data, please enabled database writing");
                        }
                        if (targetFolder.ParentList.ParentWeb.MaximumFileSize > 0 && targetFolder.ParentList.ParentWeb.MaximumFileSize < array2.Length / 1048576)
                        {
                            throw new ArgumentException("The file cannot be processed because it exceeds the maximum size specified by the SharePoint web application.");
                        }
                        string text2 = sPListItem.ParentRelativePath;
                        if (!string.IsNullOrEmpty(text2) && base.SharePointOptions.RenameSpecificNodes)
                        {
                            text2 = this.LinkCorrectParentFolderName(sPListItem);
                        }
                        this.SetTargetParentFolderName(sourceItem, targetFolder.ParentList, ref text2);
                        if (sPListItem is SPListItemVersion && !flag6 && (sPListItem as SPListItemVersion).IsMinorVersion && !targetFolder.ParentList.EnableMinorVersions)
                        {
                            targetFolder.ParentList.EnableMinorVersions = true;
                            flag6 = true;
                        }
                        newItem = targetItemCollection.AddDocument(text2, sItemXML, addDocumentOptions, array2, AddDocumentMode.Comprehensive);
                        bItemNewlyCreated = (existingItem == null && newItem != null);
                        if (!num2.HasValue)
                        {
                            num2 = new int?(newItem.ID);
                        }
                        if (flag5 && i == 0)
                        {
                            targetItemCollection.CorrectDefaultPageVersions(sourceItem.ParentRelativePath, sSourceXml);
                            flag5 = false;
                            goto IL_794;
                        }
                        goto IL_794;
                    }
                }
                finally
                {
                    if (flag6)
                    {
                        targetFolder.ParentList.EnableMinorVersions = false;
                        flag6 = false;
                    }
                }
            }
            if (bItemNewlyCreated && base.SharePointOptions.CopyDocumentWebParts && SPWebPartPage.IsWebPartPage(sourceItem) && (!sourceItem.IsWelcomePage || base.SharePointOptions.CopyDefaultPageWebPartsAtItemsLevel))
            {
                if (base.SharePointOptions.CopyWebPartsAtItemsLevel)
                {
                    TaskDefinition task = new TaskDefinition(new ThreadedOperationDelegate(this.CopyDocumentWebParts), new object[]
                    {
                        sourceItem,
                        newItem
                    });
                    base.ThreadManager.QueueBufferedTask(base.GetWebPartCopyBufferKey(newItem.ParentList.ParentWeb), task);
                    base.ThreadManager.QueueBufferedTask("RunActionEndReached", task);
                }
                else
                {
                    base.WebPartPagesNotCopiedAtItemsLevel.Add(sourceItem, newItem);
                }
            }
            return num;
        }

        // Metalogix.SharePoint.Actions.Migration.PasteListItemAction
        private long CopyDocumentOffice365(SPListItem sourceItem, SPFolder targetFolder, SPListItem existingItem, out SPListItem newItem, out bool bItemNewlyCreated, out string sSourceXml, LogItem copyOperationLogItem, SPListItemCollection targetItemCollection, IUploadManager uploadManager)
        {
            newItem = null;
            bItemNewlyCreated = false;
            long num = 0L;
            sSourceXml = null;
            sSourceXml = sourceItem.XML;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sSourceXml);
            bool flag = (sourceItem.ParentList.EnableMinorVersions && xmlNode.Attributes["_UIVersionString"] != null && xmlNode.Attributes["_UIVersionString"].Value == "0.1") || (!sourceItem.ParentList.EnableMinorVersions && xmlNode.Attributes["_UIVersionString"] != null && xmlNode.Attributes["_UIVersionString"].Value == "1.0");
            string a = (xmlNode.Attributes["_Level"] == null) ? null : xmlNode.Attributes["_Level"].Value;
            if (flag && a == "255")
            {
                copyOperationLogItem.Status = ActionOperationStatus.Skipped;
                copyOperationLogItem.Information = "Document has no checked in state";
                return 0L;
            }
            bool flag2 = base.SharePointOptions.CopyVersions && sourceItem.CanHaveVersions && targetFolder.ParentList.EnableVersioning && !(sourceItem is SPListItemVersion);
            if (flag2)
            {
                ListItemVersionCollection versionHistory = sourceItem.VersionHistory;
                flag2 = (versionHistory.Count > 0);
            }
            SPContentType sPContentType = sourceItem.ParentList.ContentTypes[sourceItem.ContentTypeId];
            SPContentType sPContentType2 = null;
            if (sPContentType != null)
            {
                sPContentType2 = targetFolder.ParentList.ContentTypes.GetContentTypeByName(sPContentType.Name);
            }
            if (!flag2)
            {
                this.CopyReferencedUsersForItem(sourceItem, targetFolder, copyOperationLogItem, uploadManager);
                sourceItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sourceItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
                if (sourceItem == null)
                {
                    return num;
                }
                sSourceXml = sourceItem.XML;
                byte[] array = null;
                bool flag3 = base.SharePointOptions.ShallowCopyExternalizedData && !sourceItem.HasBinary && sourceItem.IsExternalized && SharePointConfigurationVariables.AllowDBWriting;
                if (array == null)
                {
                    if (flag3)
                    {
                        array = sourceItem.GetBlobRef();
                        num += sourceItem.FileSize;
                    }
                    else
                    {
                        array = sourceItem.Binary;
                        num += (long)((array != null) ? array.Length : 0);
                    }
                }
                num += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                if (array == null)
                {
                    throw new ArgumentException("Could not fetch document binary. This may be because the source data is externalized. To migrate externalized data, please enabled database writing");
                }
                if (targetFolder.ParentList.ParentWeb.MaximumFileSize > 0 && targetFolder.ParentList.ParentWeb.MaximumFileSize < array.Length / 1048576)
                {
                    throw new ArgumentException("The file cannot be processed because it exceeds the maximum size specified by the SharePoint web application.");
                }
                string text = sourceItem.ParentRelativePath;
                if (!string.IsNullOrEmpty(text) && base.SharePointOptions.RenameSpecificNodes)
                {
                    text = this.LinkCorrectParentFolderName(sourceItem);
                }
                string text2 = uploadManager.SaveDocument(array);
                string targetParentPath = targetFolder.WebRelativeUrl + (string.IsNullOrEmpty(text) ? string.Empty : ("/" + text));
                Guid itemGuid = (existingItem != null && existingItem.UniqueId != Guid.Empty) ? existingItem.UniqueId : Guid.NewGuid();
                string moderationStatus = PasteListItemAction.GetModerationStatus(sourceItem);
                ManifestFileItem manifestFileItem = new ManifestFileItem(false)
                {
                    Filename = sourceItem.Name,
                    LocalFilename = text2,
                    FileSize = array.Length,
                    TargetParentPath = targetParentPath,
                    ListItemIntegerId = sourceItem.ID,
                    ItemGuid = itemGuid,
                    Version = sourceItem.VersionString,
                    ModerationStatus = moderationStatus,
                    TimeCreated = sourceItem.Created,
                    TimeLastModified = sourceItem.Modified,
                    ContentTypeId = ((sPContentType2 != null) ? sPContentType2.ContentTypeID : string.Empty),
                    CheckinComment = (sourceItem["_CheckinComment"] ?? string.Empty)
                };
                string principalName = base.MapPrincipal(sourceItem.CreatedBy);
                int userOrGroupIDByName = uploadManager.GetUserOrGroupIDByName(principalName);
                manifestFileItem.Author = userOrGroupIDByName;
                string principalName2 = base.MapPrincipal(sourceItem.ModifiedBy);
                int userOrGroupIDByName2 = uploadManager.GetUserOrGroupIDByName(principalName2);
                manifestFileItem.ModifiedBy = userOrGroupIDByName2;
                this.AddFieldsToManifestItem(sourceItem, manifestFileItem, uploadManager);
                if (this.IsPermissionCopyAllowed(sourceItem, manifestFileItem))
                {
                    this.AddItemPermissionsToAzureManifest(sourceItem, targetFolder.ParentList, manifestFileItem, false, uploadManager);
                }
                uploadManager.AddFileToManifest(manifestFileItem);
                copyOperationLogItem.Details = string.Format("Document:{0}, Source Url={1} saved locally as {2}", sourceItem.Name, sourceItem.Url, text2);
            }
            else
            {
                int? num2 = null;
                int maximumVersionCount = base.SharePointOptions.MaximumVersionCount;
                bool flag4 = base.SharePointOptions.CopyMaxVersions && !sourceItem.ParentList.IsWorkflowLibrary;
                ManifestFileItem manifestFileItem2 = new ManifestFileItem(true);
                Guid itemGuid2 = (existingItem != null && existingItem.UniqueId != Guid.Empty) ? existingItem.UniqueId : Guid.NewGuid();
                string text3 = sourceItem.ParentRelativePath;
                if (!string.IsNullOrEmpty(text3) && base.SharePointOptions.RenameSpecificNodes)
                {
                    text3 = this.LinkCorrectParentFolderName(sourceItem);
                }
                string targetParentPath2 = targetFolder.WebRelativeUrl + (string.IsNullOrEmpty(text3) ? string.Empty : ("/" + text3));
                int i = 0;
                while (i < sourceItem.VersionHistory.Count)
                {
                    SPListItem sPListItem = (SPListItem)sourceItem.VersionHistory[i];
                    if (!sPListItem.IsCheckedOut)
                    {
                        goto IL_573;
                    }
                    XmlNode xmlNode2 = XmlUtility.StringToXmlNode(sPListItem.XML);
                    if (xmlNode2.Attributes["_Level"] == null || !(xmlNode2.Attributes["_Level"].Value == "255"))
                    {
                        goto IL_573;
                    }
                    IL_88F:
                    i++;
                    continue;
                    IL_573:
                    if (base.SharePointOptions.FilterItems && !base.SharePointOptions.ItemFilterExpression.Evaluate(sPListItem, new CompareDatesInUtc()))
                    {
                        goto IL_88F;
                    }
                    bool flag5 = i == sourceItem.VersionHistory.Count - 1;
                    int num3 = sourceItem.VersionHistory.Count - i - 1;
                    bool flag6 = !flag4 || num3 < maximumVersionCount || (sPListItem.IsCurrentVersion && sPListItem.FileLevel == DocumentFileLevel.Published);
                    bool flag7 = flag5 || flag6;
                    if (!flag7)
                    {
                        goto IL_88F;
                    }
                    this.CopyReferencedUsersForItem(sPListItem, targetFolder, copyOperationLogItem, uploadManager);
                    sPListItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sPListItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
                    if (sPListItem == null)
                    {
                        goto IL_88F;
                    }
                    string xML = sPListItem.XML;
                    if (num2.HasValue)
                    {
                        this.UpdateItemID(xML, num2.Value);
                    }
                    bool flag8 = base.SharePointOptions.ShallowCopyExternalizedData && !sPListItem.HasBinary && sPListItem.IsExternalized && SharePointConfigurationVariables.AllowDBWriting;
                    byte[] array2 = flag8 ? sPListItem.GetBlobRef() : sPListItem.Binary;
                    if (flag8)
                    {
                        num += sPListItem.FileSize;
                    }
                    else
                    {
                        num += (long)((array2 != null) ? array2.Length : 0);
                    }
                    num += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                    if (array2 == null)
                    {
                        throw new ArgumentException("Could not fetch document binary. This may be because the source data is externalized. To migrate externalized data, please enabled database writing");
                    }
                    if (targetFolder.ParentList.ParentWeb.MaximumFileSize > 0 && targetFolder.ParentList.ParentWeb.MaximumFileSize < array2.Length / 1048576)
                    {
                        throw new ArgumentException("The file cannot be processed because it exceeds the maximum size specified by the SharePoint web application.");
                    }
                    string text4 = uploadManager.SaveDocument(array2);
                    copyOperationLogItem.Details = string.Format("Document:{0}, Source Url={1}, Version {2} saved locally as {3}", new object[]
                    {
                        sourceItem.Name,
                        sourceItem.Url,
                        sPListItem.VersionString,
                        text4
                    });
                    ManifestFileItem manifestFileItem3 = new ManifestFileItem(false)
                    {
                        Filename = sourceItem.Name,
                        LocalFilename = text4,
                        FileSize = array2.Length,
                        TargetParentPath = targetParentPath2,
                        ListItemIntegerId = sourceItem.ID,
                        ItemGuid = itemGuid2,
                        Version = sPListItem.VersionString,
                        ModerationStatus = ((sPListItem.FileLevel == DocumentFileLevel.Draft) ? "Draft" : "Approved"),
                        TimeCreated = sPListItem.Created,
                        TimeLastModified = sPListItem.Modified,
                        ContentTypeId = ((sPContentType2 != null) ? sPContentType2.ContentTypeID : string.Empty),
                        CheckinComment = (XmlUtility.StringToXmlNode(sPListItem.XML).GetAttributeValueAsString("_CheckinComment") ?? string.Empty)
                    };
                    string principalName3 = base.MapPrincipal(sPListItem.CreatedBy);
                    int userOrGroupIDByName3 = uploadManager.GetUserOrGroupIDByName(principalName3);
                    manifestFileItem3.Author = userOrGroupIDByName3;
                    string principalName4 = base.MapPrincipal(sPListItem.ModifiedBy);
                    int userOrGroupIDByName4 = uploadManager.GetUserOrGroupIDByName(principalName4);
                    manifestFileItem3.ModifiedBy = userOrGroupIDByName4;
                    this.AddFieldsToManifestItem(sPListItem, manifestFileItem3, uploadManager);
                    manifestFileItem2.Versions.Add(manifestFileItem3);
                    goto IL_88F;
                }
                manifestFileItem2.Filename = sourceItem.Name;
                manifestFileItem2.TargetParentPath = targetParentPath2;
                manifestFileItem2.Version = sourceItem.VersionString;
                manifestFileItem2.CheckinComment = sourceItem.VersionComments;
                manifestFileItem2.ItemGuid = itemGuid2;
                manifestFileItem2.ListItemIntegerId = sourceItem.ID;
                manifestFileItem2.ModerationStatus = ((sourceItem.FileLevel == DocumentFileLevel.Draft) ? "Draft" : "Approved");
                manifestFileItem2.TimeCreated = sourceItem.Created;
                manifestFileItem2.TimeLastModified = sourceItem.Modified;
                manifestFileItem2.CheckinComment = (sourceItem["_CheckinComment"] ?? string.Empty);
                manifestFileItem2.ContentTypeId = ((sPContentType2 != null) ? sPContentType2.ContentTypeID : string.Empty);
                string principalName5 = base.MapPrincipal(sourceItem.CreatedBy);
                int userOrGroupIDByName5 = uploadManager.GetUserOrGroupIDByName(principalName5);
                manifestFileItem2.Author = userOrGroupIDByName5;
                base.MapPrincipal(sourceItem.ModifiedBy);
                int userOrGroupIDByName6 = uploadManager.GetUserOrGroupIDByName(principalName5);
                manifestFileItem2.ModifiedBy = userOrGroupIDByName6;
                this.AddFieldsToManifestItem(sourceItem, manifestFileItem2, uploadManager);
                if (this.IsPermissionCopyAllowed(sourceItem, manifestFileItem2))
                {
                    this.AddItemPermissionsToAzureManifest(sourceItem, targetFolder.ParentList, manifestFileItem2, false, uploadManager);
                }
                uploadManager.AddFileToManifest(manifestFileItem2);
            }
            return num;
        }

        public void CopyDocumentSetsVersionHistory()
        {
            try
            {
                if (this.DocumentSets.Count > 0)
                {
                    List<TaskDefinition> taskDefinitions = new List<TaskDefinition>();
                    foreach (SPList key in this.DocumentSets.Keys)
                    {
                        try
                        {
                            SPListItemCollection allItems = key.AllItems;
                            List<SPListItem> item = this.DocumentSets[key];
                            taskDefinitions.AddRange(
                                from docSet in item
                                select this.ThreadManager.QueueTask(new object[] { docSet, key, allItems }, new ThreadedOperationDelegate(this.CopyDocumentSetVersionHistoryDelegate)));
                            base.ThreadManager.WaitForTasks(taskDefinitions);
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }
            }
            finally
            {
                this.DocumentSets.Clear();
            }
        }

        private void CopyDocumentSetVersionHistory(SPListItem sourceItem, SPList targetList, SPListItemCollection targetListItems)
        {
            try
            {
                LogItem logItem = null;
                try
                {
                    try
                    {
                        XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceItem.XML);
                        string attributeValueAsString = xmlNode.SelectSingleNode("//ListItem").GetAttributeValueAsString("MetaInfo");
                        if (!string.IsNullOrEmpty(attributeValueAsString))
                        {
                            string snapShotPropertyFromMetaInfo = DocumentSetUtils.GetSnapShotPropertyFromMetaInfo(attributeValueAsString);
                            if (!string.IsNullOrEmpty(snapShotPropertyFromMetaInfo))
                            {
                                SPListItem itemByFileName = targetListItems.GetItemByFileName(sourceItem.Name);
                                if (itemByFileName != null)
                                {
                                    logItem = new LogItem("Copying Document Set Version History", sourceItem.Name, sourceItem.ParentList.Url, itemByFileName.ParentList.Url, ActionOperationStatus.Running);
                                    base.FireOperationStarted(logItem);
                                    string str = DocumentSetUtils.UpdateMappingsInSnapShot(this, sourceItem.ParentList, targetListItems, itemByFileName, snapShotPropertyFromMetaInfo);
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        string str1 = DocumentSetUtils.UpdateTargetMetaInfoProperty(itemByFileName, str);
                                        if (!string.IsNullOrEmpty(str1))
                                        {
                                            ISharePointWriter writer = itemByFileName.Adapter.Writer;
                                            string title = itemByFileName.ParentList.Title;
                                            int d = itemByFileName.ID;
                                            string str2 = writer.AddDocumentSetVersions(title, d.ToString(), str1);
                                            logItem = MigrationUtils.GetLogItemDetails(logItem, str2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        if (logItem != null)
                        {
                            logItem.Exception = exception;
                            logItem.Status = ActionOperationStatus.Failed;
                            LogItem logItem1 = logItem;
                            logItem1.Details = string.Concat(logItem1.Details, exception.StackTrace);
                        }
                    }
                }
                finally
                {
                    if (logItem != null)
                    {
                        base.FireOperationFinished(logItem);
                    }
                }
            }
            catch (Exception exception3)
            {
                Exception exception2 = exception3;
                LogItem logItem2 = new LogItem("Copying Document Set Version History for list", sourceItem.ParentList.Name, sourceItem.ParentList.Url, targetList.Url, ActionOperationStatus.Failed);
                base.FireOperationStarted(logItem2);
                logItem2.Information = string.Format("An error occurred while setting document set version history for list: {0}{1} Exception: {2}", sourceItem.ParentList.Name, Environment.NewLine, exception2);
                base.FireOperationFinished(logItem2);
            }
        }

        private void CopyDocumentSetVersionHistoryDelegate(object[] oParams)
        {
            if ((int)oParams.Length > 2)
            {
                SPListItem sPListItem = oParams[0] as SPListItem;
                SPList sPList = oParams[1] as SPList;
                SPListItemCollection sPListItemCollection = oParams[2] as SPListItemCollection;
                if (sPListItem != null && sPList != null)
                {
                    this.CopyDocumentSetVersionHistory(sPListItem, sPList, sPListItemCollection);
                }
            }
        }

        private void CopyDocumentWebParts(object[] parameters)
        {
            SPListItem sPListItem = parameters[0] as SPListItem;
            SPListItem sPListItem1 = parameters[1] as SPListItem;
            LogItem logItem = new LogItem(Resources.CopyWebPartsInDocument, sPListItem.FileLeafRef, sPListItem.DisplayUrl, sPListItem1.DisplayUrl, ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            try
            {
                try
                {
                    CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction();
                    copyWebPartsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
                    copyWebPartsAction.LinkCorrector = base.LinkCorrector;
                    base.SubActions.Add(copyWebPartsAction);
                    object[] objArray = new object[] { sPListItem, sPListItem1, logItem };
                    copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(sPListItem.ParentList.ParentWeb, sPListItem1.ParentList.ParentWeb), null);
                    if (logItem.Status != ActionOperationStatus.Failed)
                    {
                        logItem.Status = ActionOperationStatus.Completed;
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    logItem.SourceContent = sPListItem.XML;
                    logItem.Status = ActionOperationStatus.Failed;
                    logItem.Exception = exception;
                }
            }
            finally
            {
                base.FireOperationFinished(logItem);
            }
        }

        private void CopyFolder(SPListItem sourceItem, SPFolder targetFolder, SPListItemCollection targetItems, bool bIsCopyRoot, out SPListItem newItem)
        {
            sourceItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sourceItem, this, sourceItem.ParentCollection, targetItems, this.Options.Transformers);
            string xML = sourceItem.XML;
            if (base.SharePointOptions.TaskCollection != null)
            {
                foreach (TransformationTask taskCollection in base.SharePointOptions.TaskCollection)
                {
                    FilterExpression applyTo = taskCollection.ApplyTo as FilterExpression;
                    if (applyTo == null || !(applyTo.Pattern == sourceItem.DisplayUrl))
                    {
                        continue;
                    }
                    xML = taskCollection.PerformTransformation(xML);
                }
            }
            AddFolderOptions addFolderOption = new AddFolderOptions();
            if (!targetFolder.ParentList.IsDocumentLibrary)
            {
                addFolderOption.Overwrite = base.SharePointOptions.PreserveItemIDs;
                addFolderOption.PreserveID = base.SharePointOptions.PreserveItemIDs;
            }
            else
            {
                addFolderOption.Overwrite = (!base.SharePointOptions.PreserveDocumentIDs ? false : SharePointConfigurationVariables.AllowDBWriting);
                addFolderOption.PreserveID = (!base.SharePointOptions.PreserveDocumentIDs ? false : SharePointConfigurationVariables.AllowDBWriting);
            }
            newItem = targetFolder.Items.AddFolder(sourceItem.ParentRelativePath, xML, addFolderOption);
            if (base.SharePointOptions.CopyItemPermissions)
            {
                Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
                string keyFor = base.PermissionsKeyFormatter.GetKeyFor(this.ParentFolderOf(newItem));
                object[] objArray = new object[] { sourceItem, newItem, bIsCopyRoot };
                threadManager.QueueBufferedTask(keyFor, objArray, new ThreadedOperationDelegate(this.CopyFolderPermissionsAsItemTaskDelegate));
            }
        }

        private void CopyFolderPermissionsAsItemTaskDelegate(object[] oParams)
        {
            lock (this._syncPermissionsCopy)
            {
                SPListItem sPListItem = oParams[0] as SPListItem;
                SPListItem sPListItem1 = oParams[1] as SPListItem;
                bool flag = (bool)oParams[2];
                if (!base.CheckForAbort() && (sPListItem.HasUniquePermissions || flag && base.SharePointOptions.CopyRootPermissions))
                {
                    CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
                    copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
                    base.SubActions.Add(copyRoleAssignmentsAction);
                    object[] objArray = new object[] { sPListItem, sPListItem1, true };
                    copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPListItem.ParentList.ParentWeb, sPListItem1.ParentList.ParentWeb), null);
                }
                base.ThreadManager.SetBufferedTasks(base.PermissionsKeyFormatter.GetKeyFor(sPListItem1), false, false);
            }
        }

        private long CopyItem(SPListItem sourceItem, SPFolder targetFolder, int? iParentItemID, SPListItem existingItem, out SPListItem newItem, out bool bItemNewlyCreated, out string sLastUpdateItemXml, LogItem copyOperationLogItem, SPListItemCollection targetItemCollection, Dictionary<int, int> itemIdMap)
        {
            string str;
            string str1;
            bool flag;
            newItem = null;
            bItemNewlyCreated = false;
            long objectTypeSize = (long)0;
            sLastUpdateItemXml = null;
            bool itemType = sourceItem.ItemType == SPListItemType.Folder;
            bool supportsIDPreservation = !targetFolder.Adapter.SupportsIDPreservation;
            AddListItemOptions addListItemOption = new AddListItemOptions()
            {
                ParentID = iParentItemID
            };
            if (!itemType || !targetFolder.ParentList.IsDocumentLibrary)
            {
                addListItemOption.PreserveID = (supportsIDPreservation ? false : base.SharePointOptions.PreserveItemIDs);
            }
            else
            {
                addListItemOption.PreserveID = (supportsIDPreservation || !base.SharePointOptions.PreserveDocumentIDs ? false : SharePointConfigurationVariables.AllowDBWriting);
            }
            addListItemOption.AllowDBWriting = SharePointConfigurationVariables.AllowDBWriting;
            addListItemOption.ShallowCopyExternalizedData = false;
            addListItemOption.InitialVersion = existingItem == null;
            addListItemOption.PreserveSharePointDocumentIDs = base.SharePointOptions.PreserveSharePointDocumentIDs;
            bool flag1 = false;
            if ((itemType || !base.SharePointOptions.CopyVersions || !sourceItem.CanHaveVersions || !targetFolder.ParentList.EnableVersioning || sourceItem is SPListItemVersion ? false : sourceItem.VersionHistory.Count > 0))
            {
                int? nullable = null;
                int maximumVersionCount = base.SharePointOptions.MaximumVersionCount;
                bool flag2 = this.IsItemVersionBeingLimited(sourceItem);
                int num = (sourceItem.IsCheckedOut ? sourceItem.VersionHistory.Count - 1 : sourceItem.VersionHistory.Count);
                for (int i = 0; i < num; i++)
                {
                    SPListItem item = (SPListItem)sourceItem.VersionHistory[i];
                    if (!base.SharePointOptions.FilterItems || base.SharePointOptions.ItemFilterExpression.Evaluate(item, new CompareDatesInUtc()))
                    {
                        bool flag3 = i == num - 1;
                        int num1 = num - i - 1;
                        bool flag4 = (!flag2 ? true : num1 < maximumVersionCount);
                        if (!this.HasNewerVersions(item, existingItem))
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (flag3 ? true : flag4);
                        }
                        if (flag)
                        {
                            newItem = this.CopyItemVersion(item, targetFolder, flag3, addListItemOption, targetItemCollection, nullable, itemIdMap, out objectTypeSize, out str1);
                            bItemNewlyCreated = (existingItem != null ? false : newItem != null);
                            sLastUpdateItemXml = str1;
                            if (!nullable.HasValue)
                            {
                                nullable = new int?(newItem.ID);
                            }
                            addListItemOption.InitialVersion = false;
                            if (newItem != null)
                            {
                                objectTypeSize += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                            }
                        }
                    }
                }
            }
            else
            {
                int? nullable1 = null;
                newItem = this.CopyItemVersion(sourceItem, targetFolder, true, addListItemOption, targetItemCollection, nullable1, itemIdMap, out objectTypeSize, out str);
                bItemNewlyCreated = (existingItem != null ? false : newItem != null);
                sLastUpdateItemXml = str;
                if (newItem != null)
                {
                    objectTypeSize += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                }
            }
            if (sourceItem.ItemType == SPListItemType.Folder && flag1)
            {
                base.LinkCorrector.AddFlattenedFolderMappings(sourceItem, newItem);
            }
            return objectTypeSize;
        }

        private long CopyItemOffice365(SPListItem sourceItem, SPFolder targetFolder, int? iParentItemID, SPListItem existingItem, LogItem copyOperationLogItem, SPListItemCollection targetItemCollection, Dictionary<int, int> itemIdMap, IUploadManager uploadManager)
        {
            ManifestListItem manifestListItem;
            ManifestListItem manifestDiscussionItem;
            long objectTypeSize = (long)0;
            bool itemType = sourceItem.ItemType == SPListItemType.Folder;
            bool supportsIDPreservation = !targetFolder.Adapter.SupportsIDPreservation;
            AddListItemOptions addListItemOption = new AddListItemOptions()
            {
                ParentID = iParentItemID
            };
            if (!itemType || !targetFolder.ParentList.IsDocumentLibrary)
            {
                addListItemOption.PreserveID = (supportsIDPreservation ? false : base.SharePointOptions.PreserveItemIDs);
            }
            else
            {
                addListItemOption.PreserveID = (supportsIDPreservation || !base.SharePointOptions.PreserveDocumentIDs ? false : SharePointConfigurationVariables.AllowDBWriting);
            }
            addListItemOption.AllowDBWriting = SharePointConfigurationVariables.AllowDBWriting;
            addListItemOption.ShallowCopyExternalizedData = false;
            addListItemOption.InitialVersion = existingItem == null;
            bool flag = false;
            bool flag1 = ((!itemType || sourceItem.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard) && base.SharePointOptions.CopyVersions && sourceItem.CanHaveVersions && targetFolder.ParentList.EnableVersioning && !(sourceItem is SPListItemVersion) ? sourceItem.VersionHistory.Count > 0 : false);
            this.CopyReferencedUsersForItem(sourceItem, targetFolder, copyOperationLogItem, uploadManager);
            sourceItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sourceItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
            objectTypeSize = (long)0;
            if (sourceItem == null)
            {
                return objectTypeSize;
            }
            string xML = sourceItem.XML;
            string parentRelativePath = sourceItem.ParentRelativePath;
            if (!string.IsNullOrEmpty(parentRelativePath) && base.SharePointOptions.RenameSpecificNodes)
            {
                parentRelativePath = this.LinkCorrectParentFolderName(sourceItem);
            }
            if (sourceItem.ItemType == SPListItemType.Folder)
            {
                SPUtils.MapBaseNameToEmptyTitle(ref xML);
                if (base.SharePointOptions.RenameSpecificNodes)
                {
                    xML = this.RenameFolder(xML, sourceItem, out flag);
                }
                string str = string.Concat(targetFolder.WebRelativeUrl, (string.IsNullOrEmpty(parentRelativePath) ? string.Empty : string.Concat("/", parentRelativePath)));
                Guid guid = (!(existingItem != null) || !(existingItem.UniqueId != Guid.Empty) ? Guid.NewGuid() : existingItem.UniqueId);
                ManifestFolderItem manifestFolderItem = new ManifestFolderItem((sourceItem.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard ? false : sourceItem.ParentList.EnableVersioning))
                {
                    Foldername = sourceItem.FileLeafRef,
                    TargetParentPath = str,
                    ListItemIntegerId = sourceItem.ID,
                    ItemGuid = guid,
                    TimeCreated = sourceItem.Created,
                    TimeLastModified = sourceItem.Modified
                };
                ManifestFolderItem contentTypeID = manifestFolderItem;
                string moderationStatus = PasteListItemAction.GetModerationStatus(sourceItem);
                SPContentType item = sourceItem.ParentList.ContentTypes[sourceItem.ContentTypeId];
                SPContentType contentTypeByName = null;
                if (item != null)
                {
                    contentTypeByName = targetFolder.ParentList.ContentTypes.GetContentTypeByName(item.Name);
                }
                if (contentTypeByName != null && sourceItem.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard && contentTypeByName.ContentTypeID.ToLowerInvariant().StartsWith("0x0120d520"))
                {
                    contentTypeID.ContentTypeId = contentTypeByName.ContentTypeID;
                }
                if (sourceItem.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard)
                {
                    contentTypeID.Version = sourceItem.VersionString;
                    contentTypeID.ModerationStatus = moderationStatus;
                    contentTypeID.CheckinComment = sourceItem["_CheckinComment"] ?? string.Empty;
                    contentTypeID.ContentTypeId = (contentTypeByName != null ? contentTypeByName.ContentTypeID : string.Empty);
                }
                string str1 = base.MapPrincipal(sourceItem.CreatedBy);
                contentTypeID.Author = uploadManager.GetUserOrGroupIDByName(str1);
                string str2 = base.MapPrincipal(sourceItem.ModifiedBy);
                contentTypeID.ModifiedBy = uploadManager.GetUserOrGroupIDByName(str2);
                if (this.IsPermissionCopyAllowed(sourceItem, contentTypeID))
                {
                    this.AddItemPermissionsToAzureManifest(sourceItem, targetFolder.ParentList, contentTypeID, true, uploadManager);
                }
                this.AddFieldsToManifestItem(sourceItem, contentTypeID, uploadManager);
                this.AddAttachmentsToManifestItem(sourceItem.Attachments, contentTypeID, targetFolder, uploadManager);
                if (flag1)
                {
                    int maximumVersionCount = base.SharePointOptions.MaximumVersionCount;
                    bool flag2 = this.IsItemVersionBeingLimited(sourceItem);
                    int num = (sourceItem.IsCheckedOut ? sourceItem.VersionHistory.Count - 1 : sourceItem.VersionHistory.Count);
                    for (int i = 0; i < num; i++)
                    {
                        SPListItem sPListItem = (SPListItem)sourceItem.VersionHistory[i];
                        if (!base.SharePointOptions.FilterItems || base.SharePointOptions.ItemFilterExpression.Evaluate(sPListItem, new CompareDatesInUtc()))
                        {
                            bool flag3 = i == num - 1;
                            int num1 = num - i - 1;
                            if ((flag3 ? true : (!flag2 ? true : num1 < maximumVersionCount)))
                            {
                                string moderationStatus1 = PasteListItemAction.GetModerationStatus(sPListItem);
                                this.CopyReferencedUsersForItem(sPListItem, targetFolder, copyOperationLogItem, uploadManager);
                                sPListItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sPListItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
                                if (sPListItem != null)
                                {
                                    ManifestFolderItem manifestFolderItem1 = new ManifestFolderItem(false)
                                    {
                                        TargetParentPath = str,
                                        ListItemIntegerId = sPListItem.ID,
                                        ItemGuid = guid,
                                        Version = sPListItem.VersionString,
                                        TimeCreated = sPListItem.Created,
                                        TimeLastModified = sPListItem.Modified,
                                        CheckinComment = sPListItem["_CheckinComment"] ?? string.Empty,
                                        ModerationStatus = moderationStatus1,
                                        ContentTypeId = (contentTypeByName != null ? contentTypeByName.ContentTypeID : string.Empty)
                                    };
                                    ManifestFolderItem userOrGroupIDByName = manifestFolderItem1;
                                    string str3 = base.MapPrincipal(sPListItem.CreatedBy);
                                    userOrGroupIDByName.Author = uploadManager.GetUserOrGroupIDByName(str3);
                                    string str4 = base.MapPrincipal(sPListItem.ModifiedBy);
                                    userOrGroupIDByName.ModifiedBy = uploadManager.GetUserOrGroupIDByName(str4);
                                    this.AddFieldsToManifestItem(sPListItem, userOrGroupIDByName, uploadManager);
                                    contentTypeID.Versions.Add(userOrGroupIDByName);
                                    copyOperationLogItem.Details = string.Format("Folder:{0}, Source Url={1}, Version {2}", sourceItem.Name, sourceItem.Url, sPListItem.VersionString);
                                }
                            }
                        }
                    }
                }
                uploadManager.AddFolderToManifest(contentTypeID);
                copyOperationLogItem.Details = string.Format("Folder:{0}, SourceUrl={1}", sourceItem.Name, sourceItem.Url);
                if (sourceItem.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard)
                {
                    SPDiscussionItemCollection discussionItems = ((SPDiscussionItem)sourceItem).DiscussionItems;
                    if (discussionItems.Any<Node>())
                    {
                        this.CopyListItemsOffice365(discussionItems, targetFolder, new int?(sourceItem.ID), false, ref targetItemCollection, uploadManager);
                    }
                }
            }
            else if (sourceItem.ItemType == SPListItemType.Item)
            {
                SPUtils.MapBaseNameToEmptyTitle(ref xML);
                string moderationStatus2 = PasteListItemAction.GetModerationStatus(sourceItem);
                string str5 = string.Concat(targetFolder.WebRelativeUrl, (string.IsNullOrEmpty(parentRelativePath) ? string.Empty : string.Concat("/", parentRelativePath)));
                Guid guid1 = (!(existingItem != null) || !(existingItem.UniqueId != Guid.Empty) ? Guid.NewGuid() : existingItem.UniqueId);
                SPContentType sPContentType = sourceItem.ParentList.ContentTypes[sourceItem.ContentTypeId];
                SPContentType contentTypeByName1 = null;
                if (sPContentType != null)
                {
                    contentTypeByName1 = targetFolder.ParentList.ContentTypes.GetContentTypeByName(sPContentType.Name);
                }
                if (targetFolder.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard)
                {
                    manifestListItem = new ManifestListItem(flag1);
                }
                else
                {
                    manifestListItem = new ManifestDiscussionItem(flag1);
                }
                manifestListItem.TargetParentPath = str5;
                manifestListItem.ListItemIntegerId = sourceItem.ID;
                manifestListItem.ItemGuid = guid1;
                manifestListItem.Version = sourceItem.VersionString;
                manifestListItem.TimeCreated = sourceItem.Created;
                manifestListItem.TimeLastModified = sourceItem.Modified;
                manifestListItem.FileSize = sourceItem.FileSize;
                manifestListItem.Filename = sourceItem.FileLeafRef;
                manifestListItem.ModerationStatus = moderationStatus2;
                manifestListItem.CheckinComment = sourceItem["_CheckinComment"] ?? string.Empty;
                manifestListItem.ContentTypeId = (contentTypeByName1 != null ? contentTypeByName1.ContentTypeID : string.Empty);
                if (manifestListItem.ObjectType == ManifestObjectType.DiscussionItem)
                {
                    string threadIndex = ((SPDiscussionItem)sourceItem).ThreadIndex;
                    if (!string.IsNullOrEmpty(threadIndex))
                    {
                        ((ManifestDiscussionItem)manifestListItem).ThreadIndex = threadIndex;
                    }
                }
                string str6 = base.MapPrincipal(sourceItem.CreatedBy);
                manifestListItem.Author = uploadManager.GetUserOrGroupIDByName(str6);
                string str7 = base.MapPrincipal(sourceItem.ModifiedBy);
                manifestListItem.ModifiedBy = uploadManager.GetUserOrGroupIDByName(str7);
                this.AddFieldsToManifestItem(sourceItem, manifestListItem, uploadManager);
                this.AddAttachmentsToManifestItem(sourceItem.Attachments, manifestListItem, targetFolder, uploadManager);
                if (flag1)
                {
                    int maximumVersionCount1 = base.SharePointOptions.MaximumVersionCount;
                    bool flag4 = this.IsItemVersionBeingLimited(sourceItem);
                    int num2 = (sourceItem.IsCheckedOut ? sourceItem.VersionHistory.Count - 1 : sourceItem.VersionHistory.Count);
                    for (int j = 0; j < num2; j++)
                    {
                        SPListItem item1 = (SPListItem)sourceItem.VersionHistory[j];
                        if (!base.SharePointOptions.FilterItems || base.SharePointOptions.ItemFilterExpression.Evaluate(item1, new CompareDatesInUtc()))
                        {
                            bool flag5 = j == num2 - 1;
                            int num3 = num2 - j - 1;
                            if ((flag5 ? true : (!flag4 ? true : num3 < maximumVersionCount1)))
                            {
                                string moderationStatus3 = PasteListItemAction.GetModerationStatus(item1);
                                this.CopyReferencedUsersForItem(item1, targetFolder, copyOperationLogItem, uploadManager);
                                item1 = PasteListItemAction.s_listItemTransformerDefinition.Transform(item1, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
                                if (item1 != null)
                                {
                                    if (targetFolder.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard)
                                    {
                                        manifestDiscussionItem = new ManifestListItem(false);
                                    }
                                    else
                                    {
                                        manifestDiscussionItem = new ManifestDiscussionItem(false);
                                    }
                                    manifestDiscussionItem.TargetParentPath = str5;
                                    manifestDiscussionItem.ListItemIntegerId = item1.ID;
                                    manifestDiscussionItem.ItemGuid = guid1;
                                    manifestDiscussionItem.Version = item1.VersionString;
                                    manifestDiscussionItem.TimeCreated = item1.Created;
                                    manifestDiscussionItem.TimeLastModified = item1.Modified;
                                    manifestDiscussionItem.FileSize = item1.FileSize;
                                    manifestDiscussionItem.Filename = item1.FileLeafRef;
                                    manifestDiscussionItem.ModerationStatus = moderationStatus3;
                                    manifestDiscussionItem.ContentTypeId = (contentTypeByName1 != null ? contentTypeByName1.ContentTypeID : string.Empty);
                                    if (manifestListItem.ObjectType == ManifestObjectType.DiscussionItem)
                                    {
                                        string threadIndex1 = ((SPDiscussionItem)sourceItem).ThreadIndex;
                                        if (!string.IsNullOrEmpty(threadIndex1))
                                        {
                                            ((ManifestDiscussionItem)manifestListItem).ThreadIndex = threadIndex1;
                                        }
                                    }
                                    string str8 = base.MapPrincipal(item1.CreatedBy);
                                    manifestDiscussionItem.Author = uploadManager.GetUserOrGroupIDByName(str8);
                                    string str9 = base.MapPrincipal(item1.ModifiedBy);
                                    manifestDiscussionItem.ModifiedBy = uploadManager.GetUserOrGroupIDByName(str9);
                                    this.AddFieldsToManifestItem(item1, manifestDiscussionItem, uploadManager);
                                    manifestListItem.Versions.Add(manifestDiscussionItem);
                                    objectTypeSize += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                                    copyOperationLogItem.Details = string.Format("Item:{0}, Source Url={1}, Version {2}", sourceItem.Name, sourceItem.Url, item1.VersionString);
                                }
                            }
                        }
                    }
                }
                if (this.IsPermissionCopyAllowed(sourceItem, manifestListItem))
                {
                    this.AddItemPermissionsToAzureManifest(sourceItem, targetFolder.ParentList, manifestListItem, false, uploadManager);
                }
                if (manifestListItem.ObjectType != ManifestObjectType.DiscussionItem)
                {
                    uploadManager.AddListItemToManifest(manifestListItem);
                }
                else
                {
                    uploadManager.AddDiscussionItemToManifest(manifestListItem as ManifestDiscussionItem);
                }
                objectTypeSize += SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
                copyOperationLogItem.Details = string.Format("List Item:{0}, Source Url={1}", sourceItem.Name, sourceItem.Url);
                if (sourceItem.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard)
                {
                    SPDiscussionItemCollection sPDiscussionItemCollection = ((SPDiscussionItem)sourceItem).DiscussionItems;
                    if (sPDiscussionItemCollection.Any<Node>())
                    {
                        this.CopyListItemsOffice365(sPDiscussionItemCollection, targetFolder, new int?(sourceItem.ID), false, ref targetItemCollection, uploadManager);
                    }
                }
            }
            return objectTypeSize;
        }

        private void CopyItemPermissions(SPListItem sourceItem, SPListItem targetItem, bool bIsFolder)
        {
            if (base.CheckForAbort())
            {
                return;
            }
            LogItem logItem = new LogItem(string.Format("Copying {0} permissions", (bIsFolder ? "folder" : "item")), sourceItem.Name, sourceItem.DisplayUrl, targetItem.DisplayUrl, ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            try
            {
                CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
                copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
                base.SubActions.Add(copyRoleAssignmentsAction);
                object[] objArray = new object[] { sourceItem, targetItem, true };
                copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceItem.ParentList.ParentWeb, targetItem.ParentList.ParentWeb), null);
                logItem.Status = ActionOperationStatus.Completed;
            }
            catch (Exception exception)
            {
                logItem.Exception = exception;
            }
            base.FireOperationFinished(logItem);
        }

        internal void CopyItemPermissionsTaskDelegate(object[] oParams)
        {
            lock (this._syncPermissionsCopy)
            {
                SPListItem sPListItem = oParams[0] as SPListItem;
                SPListItem sPListItem1 = oParams[1] as SPListItem;
                bool flag = (bool)oParams[2];
                if ((bool)oParams[3])
                {
                    this.CopyItemPermissions(sPListItem, sPListItem1, flag);
                }
                if (flag)
                {
                    base.ThreadManager.SetBufferedTasks(base.PermissionsKeyFormatter.GetKeyFor(sPListItem1), false, false);
                }
            }
        }

        private SPListItem CopyItemVersion(SPListItem sourceItem, SPFolder targetFolder, bool bIsLastVersion, AddListItemOptions addItemOptions, SPListItemCollection targetItemCollection, int? iNewItemId, Dictionary<int, int> itemIdMap, out long lBytes, out string sItemXml)
        {
            bool flag;
            byte[][] fileContents;
            string[] fileNames;
            sourceItem = PasteListItemAction.s_listItemTransformerDefinition.Transform(sourceItem, this, sourceItem.ParentCollection, targetItemCollection, this.Options.Transformers);
            lBytes = (long)0;
            if (sourceItem == null)
            {
                sItemXml = null;
                return null;
            }
            if (!bIsLastVersion)
            {
                fileContents = null;
            }
            else if (sourceItem.HasAttachments)
            {
                fileContents = sourceItem.Attachments.GetFileContents(base.SharePointOptions.ShallowCopyExternalizedData, out lBytes);
            }
            else
            {
                fileContents = null;
            }
            byte[][] numArray = fileContents;
            if (!bIsLastVersion)
            {
                fileNames = null;
            }
            else if (sourceItem.HasAttachments)
            {
                fileNames = sourceItem.Attachments.GetFileNames();
            }
            else
            {
                fileNames = null;
            }
            string[] strArrays = fileNames;
            sItemXml = (iNewItemId.HasValue ? this.UpdateItemID(sourceItem.XML, iNewItemId.Value) : sourceItem.XML);
            if (sourceItem.ParentList.BaseTemplate == ListTemplateType.KpiList && sourceItem.Adapter.SharePointVersion.IsSharePoint2007OrEarlier && targetFolder.Adapter.SharePointVersion.IsSharePoint2010OrLater)
            {
                sItemXml = this.UpdateKpiListItemXml(sItemXml, sourceItem, targetFolder);
            }
            if (sourceItem.ParentList.BaseTemplate == ListTemplateType.Events || sourceItem.ParentList.BaseTemplate == ListTemplateType.Tasks || sourceItem.ParentList.BaseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy)
            {
                sItemXml = this.UpdateIdsAccordingToItemIdMap(sItemXml, itemIdMap);
            }
            string parentRelativePath = sourceItem.ParentRelativePath;
            if (!string.IsNullOrEmpty(parentRelativePath) && base.SharePointOptions.RenameSpecificNodes)
            {
                parentRelativePath = this.LinkCorrectParentFolderName(sourceItem);
            }
            if (sourceItem.ItemType == SPListItemType.Folder)
            {
                SPUtils.MapBaseNameToEmptyTitle(ref sItemXml);
                if (base.SharePointOptions.RenameSpecificNodes)
                {
                    sItemXml = this.RenameFolder(sItemXml, sourceItem, out flag);
                }
            }
            if (targetItemCollection.ParentSPList.BaseTemplate == ListTemplateType.BlogPosts)
            {
                this.LinkCorrectAttachments(sourceItem, targetItemCollection.ParentSPList, ref sItemXml);
            }
            SPListItem sPListItem = targetItemCollection.AddItem(parentRelativePath, sItemXml, strArrays, numArray, addItemOptions);
            if (sPListItem.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard)
            {
                string str = this.LinkCorrectHtmlParts(sourceItem, sPListItem);
                if (str != sItemXml)
                {
                    sItemXml = str;
                    UpdateListItemOptions updateListItemOption = new UpdateListItemOptions()
                    {
                        PreserveSharePointDocumentIDs = addItemOptions.PreserveSharePointDocumentIDs
                    };
                    sPListItem.ParentList.Adapter.Writer.UpdateListItem(sPListItem.ParentList.ConstantID.ToString(), parentRelativePath, sPListItem.ID, sItemXml, strArrays, numArray, updateListItemOption);
                }
            }
            return sPListItem;
        }

        // Metalogix.SharePoint.Actions.Migration.PasteListItemAction
        private void CopyListItem(SPListItem sourceItem, int iItemIndex, int? iParentItemID, SPFolder targetFolder, SPListItemCollection sourceItems, ref SPListItemCollection targetItems, bool bIsFolder, bool bPreservingItems, bool bIsNwsTarget, bool bIsCopyRoot, ListTemplateType targetListTemplate, ref Dictionary<int, int> itemIdMap, List<string> filteredFolders)
        {
            if (sourceItem.ParentList != null && sourceItem.ParentList.ParentWeb.HasNintextFeature && PasteListItemAction.IsNintexItem(sourceItem, targetFolder, bIsFolder))
            {
                return;
            }
            SPListItem sPListItem = null;
            if (SPUtils.IsOneNoteItem(sourceItem) || (sourceItem.ParentList.BaseTemplate == ListTemplateType.DiscussionBoard && targetFolder.Adapter.IsClientOM))
            {
                bPreservingItems = false;
            }
            LogItem logItem = null;
            string str = bIsFolder ? "Folder" : (sourceItems.ParentSPList.IsDocumentLibrary ? (SPUtils.IsOneNoteItem(sourceItem) ? "OneNote Item" : "Document") : "Item");
            if (sourceItem.ParentList.BaseTemplate == ListTemplateType.DesignCatalog)
            {
                str = "Theme";
            }
            try
            {
                if (!base.CheckForAbort())
                {
                    string parentRelativePath = sourceItem.ParentRelativePath;
                    string fileLeafRef = sourceItem.FileLeafRef;
                    if (!base.SharePointOptions.CopyWorkflowInstanceData && (sourceItem.ParentList.BaseTemplate == ListTemplateType.Tasks || sourceItem.ParentList.BaseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy) && sourceItem.Adapter.SharePointVersion.IsSharePoint2007OrLater && sourceItem["ContentTypeId"].StartsWith("0x01080100"))
                    {
                        if (base.SharePointOptions.LogSkippedItems)
                        {
                            logItem = new LogItem(bIsFolder ? "Skipping Folder" : "Skipping Item", bIsFolder ? sourceItem.FileLeafRef : ("Item ID='" + sourceItem.ID + "'"), sourceItem.DisplayUrl, "", ActionOperationStatus.Skipped);
                            logItem.Information = "Filtered out because it is a workflow task and workflow instances are not being migrated.";
                            base.FireOperationStarted(logItem);
                            base.FireOperationFinished(logItem);
                        }
                        if (bIsFolder)
                        {
                            filteredFolders.Add(UrlUtils.ConcatUrls(new string[]
                            {
                                parentRelativePath,
                                fileLeafRef
                            }));
                        }
                    }
                    else if (sourceItem.ParentList.BaseTemplate == ListTemplateType.WorkFlowHistory && sourceItem.Adapter.SharePointVersion.IsSharePoint2013OrLater)
                    {
                        if (base.SharePointOptions.LogSkippedItems)
                        {
                            logItem = new LogItem(bIsFolder ? "Skipping Folder" : "Skipping Item", bIsFolder ? sourceItem.FileLeafRef : ("Item ID='" + sourceItem.ID + "'"), sourceItem.DisplayUrl, "", ActionOperationStatus.Skipped);
                            logItem.Information = "Filtered out because workflow history cannot be maintained on target.";
                            base.FireOperationStarted(logItem);
                            base.FireOperationFinished(logItem);
                        }
                        if (bIsFolder)
                        {
                            filteredFolders.Add(UrlUtils.ConcatUrls(new string[]
                            {
                                parentRelativePath,
                                fileLeafRef
                            }));
                        }
                    }
                    else
                    {
                        bool anticipatedFileLeafRefAndPath = this.GetAnticipatedFileLeafRefAndPath(sourceItem, ref fileLeafRef, ref parentRelativePath, bIsFolder, targetFolder);
                        if ((bIsFolder && targetFolder.Adapter.SharePointVersion.IsSharePoint2010OrLater && sourceItem.FileLeafRef.Contains("_vti_")) || (!bIsFolder && base.SharePointOptions.FilterItems && !base.SharePointOptions.ItemFilterExpression.Evaluate(sourceItem, new CompareDatesInUtc())))
                        {
                            if (base.SharePointOptions.LogSkippedItems)
                            {
                                logItem = new LogItem(bIsFolder ? "Skipping Folder" : "Skipping Item", bIsFolder ? sourceItem.FileLeafRef : ("Item ID='" + sourceItem.ID + "'"), sourceItem.DisplayUrl, "", ActionOperationStatus.Skipped);
                                logItem.Information = "Filtered out by " + (bIsFolder ? "folder" : "item") + " filters.";
                                base.FireOperationStarted(logItem);
                                base.FireOperationFinished(logItem);
                            }
                            if (bIsFolder)
                            {
                                filteredFolders.Add(UrlUtils.ConcatUrls(new string[]
                                {
                                    parentRelativePath,
                                    fileLeafRef
                                }));
                            }
                        }
                        else
                        {
                            bool flag = sourceItem.ParentList.ParentWeb.WebTemplateID == 62 && sourceItem.IsWebPartPage && string.Equals(sourceItem.ParentList.Name, "SitePages", StringComparison.OrdinalIgnoreCase) && string.Equals(sourceItem.FileLeafRef, "Category.aspx", StringComparison.OrdinalIgnoreCase);
                            if (flag && (targetFolder.Adapter.IsNws || targetFolder.Adapter.IsClientOM))
                            {
                                bool flag2 = targetItems.GetMatchingItem(sourceItem, fileLeafRef, parentRelativePath) != null;
                                if (flag2)
                                {
                                    logItem = new LogItem("Skipping Item", sourceItem.FileLeafRef, sourceItem.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Skipped);
                                    logItem.Information = "WebParts can not be migrated for category pages in community site using remote target adapter.";
                                    base.FireOperationStarted(logItem);
                                    base.FireOperationFinished(logItem);
                                    return;
                                }
                            }
                            bool flag3 = sourceItem.Adapter.IsDB && sourceItem.Adapter.SharePointVersion.IsSharePoint2007OrLater;
                            if (flag3)
                            {
                                bool flag4 = sourceItem.FileDirRef.EndsWith("Style Library/XSL Style Sheets") && this.CheckIfGhosted(sourceItem);
                                if (flag4)
                                {
                                    bool flag5 = targetItems.GetMatchingItem(sourceItem, fileLeafRef, parentRelativePath) != null;
                                    if (flag5)
                                    {
                                        logItem = new LogItem("Skipping Item", sourceItem.FileLeafRef, sourceItem.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Skipped);
                                        logItem.Information = "Source document is Ghosted.";
                                        base.FireOperationStarted(logItem);
                                        base.FireOperationFinished(logItem);
                                        return;
                                    }
                                }
                            }
                            if (!iParentItemID.HasValue && filteredFolders.Contains(parentRelativePath))
                            {
                                if (base.SharePointOptions.LogSkippedItems)
                                {
                                    logItem = new LogItem(bIsFolder ? "Skipping Folder" : "Skipping Item", bIsFolder ? sourceItem.FileLeafRef : ("Item ID='" + sourceItem.ID + "'"), sourceItem.DisplayUrl, "", ActionOperationStatus.Skipped);
                                    logItem.Information = "Skipped due to non-existence of necessary folder structure. This is likely caused by filtering out a necessary parent";
                                    base.FireOperationStarted(logItem);
                                    base.FireOperationFinished(logItem);
                                }
                                if (bIsFolder)
                                {
                                    filteredFolders.Add(UrlUtils.ConcatUrls(new string[]
                                    {
                                        parentRelativePath,
                                        fileLeafRef
                                    }));
                                }
                            }
                            else
                            {
                                SPListItem sPListItem2 = null;
                                if (bPreservingItems || sourceItem.ItemType == SPListItemType.Folder)
                                {
                                    sPListItem2 = targetItems.GetMatchingItem(sourceItem, fileLeafRef, parentRelativePath);
                                    bool flag6 = base.SharePointOptions.UpdateItems && base.SharePointOptions.UpdateItemOptionsBitField > 0;
                                    bool flag7 = sPListItem2 != null;
                                    bool flag8 = !flag7 || !base.SharePointOptions.CheckModifiedDatesForItemsDocuments || sourceItem.Modified > sPListItem2.Modified;
                                    bool flag9 = false;
                                    if (flag7 && MigrationUtils.IsListWithDefaultItems(sPListItem2.ParentList))
                                    {
                                        ListTemplateType baseTemplate = sourceItem.ParentList.BaseTemplate;
                                        int iD = sourceItem.ID;
                                        if ((baseTemplate == ListTemplateType.BlogCategories && (iD == 1 || iD == 2 || iD == 3)) || ((baseTemplate == ListTemplateType.BlogPosts || baseTemplate == ListTemplateType.CommunityCategories || baseTemplate == ListTemplateType.CommunityMembers) && iD == 1))
                                        {
                                            flag9 = this.IsAlwaysUpdateOOBBlogandCommunityListItems(flag6, flag8);
                                        }
                                    }
                                    if (flag7 && (!flag8 || (!flag6 && !sourceItem.ParentList.EnableVersioning)) && !flag9)
                                    {
                                        if (base.SharePointOptions.LogSkippedItems)
                                        {
                                            logItem = new LogItem(bIsFolder ? "Skipping Folder" : "Skipping Item", bIsFolder ? sourceItem.FileLeafRef : ("Item ID='" + sourceItem.ID + "'"), sourceItem.DisplayUrl, sPListItem2.DisplayUrl, ActionOperationStatus.Skipped);
                                            logItem.Information = (bIsFolder ? "Folder" : "Item") + " skipped because it already exists, and the operation is not overwriting or updating.";
                                            base.FireOperationStarted(logItem);
                                            base.FireOperationFinished(logItem);
                                        }
                                        if (anticipatedFileLeafRefAndPath)
                                        {
                                            base.LinkCorrector.AddFlattenedFolderMappings(sourceItem, sPListItem2);
                                        }
                                        if (base.SharePointOptions.CopyItemPermissions)
                                        {
                                            bool flag10 = base.SharePointOptions.UpdateItems && (base.SharePointOptions.UpdateItemOptionsBitField & 2) > 0 && ((bIsCopyRoot && base.SharePointOptions.CopyRootPermissions) || sourceItem.HasUniquePermissions);
                                            if (flag10 || bIsFolder)
                                            {
                                                base.ThreadManager.QueueBufferedTask(base.PermissionsKeyFormatter.GetKeyFor(this.ParentFolderOf(sPListItem2)), new object[]
                                                {
                                                    sourceItem,
                                                    sPListItem2,
                                                    bIsFolder,
                                                    flag10
                                                }, new ThreadedOperationDelegate(this.CopyItemPermissionsTaskDelegate));
                                            }
                                        }
                                        if (sPListItem2 != null)
                                        {
                                            itemIdMap.Add(sourceItem.ID, sPListItem2.ID);
                                        }
                                        if (sourceItems.ParentSPList.BaseTemplate == ListTemplateType.DiscussionBoard)
                                        {
                                            this.CopyListItems(((SPDiscussionItem)sourceItem).DiscussionItems, targetFolder, new int?(sPListItem2.ID), false, ref targetItems);
                                            if (iParentItemID == 0)
                                            {
                                                sPListItem2.UpdateListItem(sPListItem2.XML, null, null);
                                            }
                                        }
                                        return;
                                    }
                                }
                                else if (base.SharePointOptions.ItemCopyingMode == ListItemCopyMode.Overwrite)
                                {
                                    sPListItem2 = targetItems.GetMatchingItem(sourceItem, fileLeafRef, parentRelativePath);
                                    if (sPListItem2 != null)
                                    {
                                        bool flag11 = (!bIsNwsTarget && base.SharePointOptions.PreserveItemIDs) || targetFolder.ParentList.IsDocumentLibrary;
                                        if (flag11 && !sPListItem2.IsWelcomePage)
                                        {
                                            targetItems.DeleteItem(sPListItem2);
                                        }
                                    }
                                    sPListItem2 = null;
                                }
                                if (targetListTemplate != ListTemplateType.MeetingAttendees || this.FilterAttendeesListAdd(sourceItem, targetFolder.ParentList.ParentWeb, sourceItems))
                                {
                                    string sourceContent = null;
                                    string text = null;
                                    long? num = null;
                                    try
                                    {
                                        logItem = new LogItem("Copying " + str, sourceItem.FileLeafRef, sourceItem.DisplayUrl, SPUtils.IsOneNoteItem(sourceItem) ? SPUtils.OneNoteFolderDisplayName(targetFolder, true) : targetFolder.DisplayUrl, ActionOperationStatus.Running);
                                        base.FireOperationStarted(logItem);
                                        sourceItems.FetchDataAt(iItemIndex);
                                        sourceContent = sourceItem.XML;
                                        bool flag12 = false;
                                        if (sourceItem != null)
                                        {
                                            if (this.IsItemVersionBeingLimited(sourceItem))
                                            {
                                                sourceItem.NoOfLatestVersionsToGet = base.SharePointOptions.MaximumVersionCount;
                                            }
                                            if (sourceItems.ParentSPList.IsDocumentLibrary && sourceItem.ItemType != SPListItemType.Folder)
                                            {
                                                num = new long?(this.CopyDocument(sourceItem, targetFolder, sPListItem2, out sPListItem, out flag12, out sourceContent, logItem, targetItems));
                                            }
                                            else
                                            {
                                                num = new long?(this.CopyItem(sourceItem, targetFolder, iParentItemID, sPListItem2, out sPListItem, out flag12, out text, logItem, targetItems, itemIdMap));
                                            }
                                        }
                                        if (sPListItem != null)
                                        {
                                            if (sourceItem.Adapter.SharePointVersion.IsSharePointOnline && sPListItem.Adapter.SharePointVersion.IsSharePointOnline && sourceItem.ItemType == SPListItemType.Folder && SPUtils.IsDefaultOneNoteFolder(sourceItem) && !base.GuidMappings.ContainsKey(sourceItem.UniqueId))
                                            {
                                                base.AddGuidMappings(sourceItem.UniqueId, sPListItem.UniqueId);
                                            }
                                            if (base.SharePointOptions.CopyItemPermissions)
                                            {
                                                bool flag13 = (flag12 || (!flag12 && base.SharePointOptions.UpdateItems && (base.SharePointOptions.UpdateItemOptionsBitField & 2) > 0)) && ((bIsCopyRoot && base.SharePointOptions.CopyRootPermissions) || sourceItem.HasUniquePermissions);
                                                if (flag13 || bIsFolder)
                                                {
                                                    base.ThreadManager.QueueBufferedTask(base.PermissionsKeyFormatter.GetKeyFor(this.ParentFolderOf(sPListItem)), new object[]
                                                    {
                                                        sourceItem,
                                                        sPListItem,
                                                        bIsFolder,
                                                        flag13
                                                    }, new ThreadedOperationDelegate(this.CopyItemPermissionsTaskDelegate));
                                                }
                                            }
                                            if (DocumentSetUtils.IsDocSetVersionHistoryMigrationSupported(this, sourceItem.ParentList, targetFolder, sourceItem))
                                            {
                                                this.AddDocumentSetsToCollection(sourceItem, targetFolder.ParentList);
                                            }
                                            if (logItem != null)
                                            {
                                                if (base.SharePointOptions.CheckResults)
                                                {
                                                    base.CompareNodes(sourceItem, sPListItem, logItem);
                                                }
                                                else
                                                {
                                                    logItem.Status = SPUtils.EvaluateLog(logItem);
                                                }
                                                if (bIsFolder)
                                                {
                                                    logItem.AddCompletionDetail(Resources.Migration_Detail_FoldersCopied, 1L);
                                                }
                                                else
                                                {
                                                    logItem.AddCompletionDetail(Resources.Migration_Detail_ItemsCopied, 1L);
                                                }
                                                if (num.HasValue)
                                                {
                                                    logItem.LicenseDataUsed = num.Value;
                                                }
                                                if (base.SharePointOptions.CopyWorkflowInstanceData && (sourceItem.HasWorkflows || sourceItem.ParentList.BaseTemplate == ListTemplateType.Tasks || sourceItem.ParentList.BaseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy) && !base.WorkflowItemMappings.ContainsKey(sourceItem.GUID.ToString()))
                                                {
                                                    base.WorkflowItemMappings.Add(sourceItem.GUID.ToString(), sPListItem.GUID.ToString());
                                                }
                                                if (base.SharePointOptions.Verbose)
                                                {
                                                    logItem.SourceContent = (base.SharePointOptions.CopyVersions ? sourceItem.XMLWithVersions : sourceItem.XML);
                                                    logItem.TargetContent = (base.SharePointOptions.CopyVersions ? sPListItem.XMLWithVersions : sPListItem.XML);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logItem.Status = ActionOperationStatus.Skipped;
                                            if (base.SharePointOptions.Verbose)
                                            {
                                                logItem.SourceContent = (base.SharePointOptions.CopyVersions ? sourceItem.XMLWithVersions : sourceItem.XML);
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        if (logItem != null)
                                        {
                                            logItem.Exception = exception;
                                            logItem.SourceContent = sourceContent;
                                        }
                                        return;
                                    }
                                    finally
                                    {
                                        if (logItem != null)
                                        {
                                            if (logItem.Status == ActionOperationStatus.Running)
                                            {
                                                logItem.Status = ActionOperationStatus.Completed;
                                            }
                                            base.FireOperationFinished(logItem);
                                        }
                                    }
                                    if (sPListItem != null && !string.IsNullOrEmpty(text) && sourceItems.ParentSPList.BaseTemplate == ListTemplateType.DiscussionBoard)
                                    {
                                        this.CopyListItems(((SPDiscussionItem)sourceItem).DiscussionItems, targetFolder, new int?(sPListItem.ID), false, ref targetItems);
                                        if (!iParentItemID.HasValue || iParentItemID == 0)
                                        {
                                            sPListItem.UpdateListItem(text, null, null);
                                        }
                                    }
                                    if (sPListItem != null)
                                    {
                                        itemIdMap.Add(sourceItem.ID, sPListItem.ID);
                                    }
                                    else if (sPListItem2 != null)
                                    {
                                        itemIdMap.Add(sourceItem.ID, sPListItem2.ID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                if (logItem == null)
                {
                    logItem = new LogItem("Copying " + str, sourceItem.FileLeafRef, sourceItem.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
                }
                logItem.Exception = exception2;
                base.FireOperationStarted(logItem);
                base.FireOperationFinished(logItem);
            }
            finally
            {
                if (sPListItem != null)
                {
                    sPListItem.Dispose();
                }
            }
        }

        private void CopyListItemOffice365(SPListItem sourceItem, int iItemIndex, int? iParentItemID, SPFolder targetFolder, SPListItemCollection sourceItems, ref SPListItemCollection targetItems, bool bIsFolder, bool bPreservingItems, bool bIsNwsTarget, bool bIsCopyRoot, ListTemplateType targetListTemplate, ref Dictionary<int, int> itemIdMap, List<string> filteredFolders, IUploadManager uploadManager)
        {
            string str;
            SPListItem matchingItem;
            bool flag;
            SPListItem sPListItem = null;
            LogItem logItem = null;
            if (bIsFolder)
            {
                str = "Folder";
            }
            else if (sourceItems.ParentSPList.IsDocumentLibrary)
            {
                str = (SPUtils.IsOneNoteItem(sourceItem) ? "OneNote Item" : "Document");
            }
            else
            {
                str = "Item";
            }
            string str1 = str;
            try
            {
                try
                {
                    if (!base.CheckForAbort())
                    {
                        string parentRelativePath = sourceItem.ParentRelativePath;
                        string fileLeafRef = sourceItem.FileLeafRef;
                        bool anticipatedFileLeafRefAndPath = this.GetAnticipatedFileLeafRefAndPath(sourceItem, ref fileLeafRef, ref parentRelativePath, bIsFolder, targetFolder);
                        string xML = null;
                        if (targetItems.Count > 0)
                        {
                            matchingItem = targetItems.GetMatchingItem(sourceItem, fileLeafRef, parentRelativePath);
                        }
                        else
                        {
                            matchingItem = null;
                        }
                        SPListItem sPListItem1 = matchingItem;
                        bool flag1 = (!base.SharePointOptions.UpdateItems ? false : base.SharePointOptions.UpdateItemOptionsBitField > 0);
                        bool flag2 = sPListItem1 != null;
                        if (!flag2)
                        {
                            flag = true;
                        }
                        else if (bIsFolder)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (!base.SharePointOptions.CheckModifiedDatesForItemsDocuments ? true : sourceItem.Modified > sPListItem1.Modified);
                        }
                        bool flag3 = flag;
                        if (flag2 && bPreservingItems && (bIsFolder || !flag3 || !flag1 && !sourceItem.ParentList.EnableVersioning))
                        {
                            if (!bIsFolder && base.SharePointOptions.LogSkippedItems)
                            {
                                logItem = new LogItem("Skipping Item", string.Format("Item ID='{0}'", sourceItem.ID), sourceItem.DisplayUrl, sPListItem1.DisplayUrl, ActionOperationStatus.Skipped)
                                {
                                    Information = "Item skipped because it already exists, and the operation is not overwriting or updating."
                                };
                                base.FireOperationStarted(logItem);
                                base.FireOperationFinished(logItem);
                            }
                            if (anticipatedFileLeafRefAndPath)
                            {
                                base.LinkCorrector.AddFlattenedFolderMappings(sourceItem, sPListItem1);
                            }
                            if (sPListItem1 != null)
                            {
                                itemIdMap.Add(sourceItem.ID, sPListItem1.ID);
                            }
                            if (!bIsFolder)
                            {
                                return;
                            }
                        }
                        try
                        {
                            try
                            {
                                logItem = new LogItem(string.Concat("Processing ", str1), sourceItem.FileLeafRef, sourceItem.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
                                base.FireOperationStarted(logItem);
                                sourceItems.FetchDataAt(iItemIndex);
                                xML = sourceItem.XML;
                                bool flag4 = false;
                                if (sourceItem != null)
                                {
                                    if (this.IsItemVersionBeingLimited(sourceItem))
                                    {
                                        sourceItem.NoOfLatestVersionsToGet = base.SharePointOptions.MaximumVersionCount;
                                    }
                                    if (!sourceItems.ParentSPList.IsDocumentLibrary || sourceItem.ItemType == SPListItemType.Folder)
                                    {
                                        this.CopyItemOffice365(sourceItem, targetFolder, iParentItemID, sPListItem1, logItem, targetItems, itemIdMap, uploadManager);
                                    }
                                    else
                                    {
                                        this.CopyDocumentOffice365(sourceItem, targetFolder, sPListItem1, out sPListItem, out flag4, out xML, logItem, targetItems, uploadManager);
                                    }
                                    if (DocumentSetUtils.IsDocSetVersionHistoryMigrationSupported(this, sourceItem.ParentList, targetFolder, sourceItem))
                                    {
                                        this.AddDocumentSetsToCollection(sourceItem, targetFolder.ParentList);
                                    }
                                }
                            }
                            catch (Exception exception1)
                            {
                                Exception exception = exception1;
                                if (logItem != null)
                                {
                                    logItem.Exception = exception;
                                    logItem.SourceContent = xML;
                                }
                            }
                        }
                        finally
                        {
                            if (logItem != null)
                            {
                                logItem.LicenseDataUsed = (long)0;
                                if (logItem.Status == ActionOperationStatus.Running)
                                {
                                    logItem.Status = ActionOperationStatus.Completed;
                                }
                                base.FireOperationFinished(logItem);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception exception3)
                {
                    Exception exception2 = exception3;
                    if (logItem == null)
                    {
                        logItem = new LogItem(string.Concat("Copying ", str1), sourceItem.FileLeafRef, sourceItem.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
                    }
                    logItem.Exception = exception2;
                    base.FireOperationStarted(logItem);
                    base.FireOperationFinished(logItem);
                }
            }
            finally
            {
                if (sPListItem != null)
                {
                    sPListItem.Dispose();
                }
            }
        }

        private void CopyListItems(SPListItemCollection sourceItems, SPFolder targetFolder, int? iParentItemID, bool bIsCopyRoot, IUploadManager uploadManager = null)
        {
            SPListItemCollection sPListItemCollection = null;
            if (!targetFolder.Adapter.SharePointVersion.IsSharePointOnline || uploadManager == null || !base.SharePointOptions.UseAzureOffice365Upload || !sourceItems.ParentSPList.IsMigrationPipelineSupported || !targetFolder.ParentList.IsMigrationPipelineSupportedForTarget)
            {
                this.CopyListItems(sourceItems, targetFolder, iParentItemID, bIsCopyRoot, ref sPListItemCollection);
                return;
            }
            this.CopyListItemsOffice365(sourceItems, targetFolder, iParentItemID, bIsCopyRoot, ref sPListItemCollection, uploadManager);
        }

        private void CopyListItems(SPListItemCollection sourceItems, SPFolder targetFolder, int? parentItemId, bool bIsCopyRoot, ref SPListItemCollection targetItems)
        {
            if (sourceItems == null)
            {
                throw new Exception("Source item collection cannot be null");
            }
            if (targetFolder == null)
            {
                throw new Exception("Target folder cannot be null");
            }
            if (base.SharePointOptions.CorrectingLinks)
            {
                LogItem logItem = null;
                try
                {
                    try
                    {
                        base.LinkCorrector.Scope = base.SharePointOptions.LinkCorrectionScope;
                        base.LinkCorrector.PopulateForItemCopy(sourceItems, targetFolder);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        logItem = new LogItem("Initialize Link Corrector for Item Copy", null, null, null, ActionOperationStatus.Running);
                        base.FireOperationStarted(logItem);
                        logItem.Exception = exception;
                        logItem.Details = exception.StackTrace;
                    }
                }
                finally
                {
                    if (logItem != null)
                    {
                        base.FireOperationFinished(logItem);
                    }
                }
            }
            ListTemplateType baseTemplate = targetFolder.ParentList.BaseTemplate;
            if (baseTemplate == ListTemplateType.MeetingAttendees && targetFolder.ParentList.ParentWeb.MeetingInstances != null && targetFolder.ParentList.ParentWeb.MeetingInstances.ContainsRecurringMeeting)
            {
                try
                {
                    targetFolder.ParentList.ParentWeb.GetMeetingInstances(true);
                }
                catch
                {
                }
            }
            XmlNode xmlNodes = null;
            List<string> strs = null;
            if (targetFolder.Adapter.SharePointVersion.IsSharePoint2010OrLater)
            {
                this.DisableLookupRelationshipEnforcementOnList(targetFolder.ParentList, out xmlNodes, out strs);
            }
            List<string> strs1 = new List<string>();
            SPListItemCollection oneNoteItems = null;
            try
            {
                bool flag = targetFolder.Adapter.AdapterShortName.Equals("NW", StringComparison.OrdinalIgnoreCase);
                LogItem logItem1 = new LogItem("Fetching target items", "", "", "", ActionOperationStatus.Running)
                {
                    WriteToJobDatabase = false
                };
                base.FireOperationStarted(logItem1);
                bool flag1 = (base.SharePointOptions.CopySubFolders ? true : targetFolder.ParentList is SPDiscussionList);
                ListItemQueryType listItemQueryType = ListItemQueryType.ListItem;
                if (flag1)
                {
                    listItemQueryType |= ListItemQueryType.Folder;
                }
                targetItems = targetFolder.GetItems(flag1, listItemQueryType, null);
                if (!parentItemId.HasValue)
                {
                    PasteListItemAction.s_listItemTransformerDefinition.BeginTransformation(this, sourceItems, targetItems, this.Options.Transformers);
                }
                sourceItems.SortItemsToEnsureFolderExistence();
                bool flag2 = PasteAction<PasteListItemOptions>.CheckPreservingItems(sourceItems.ParentSPList, base.SharePointOptions);
                if (!flag2 && MigrationUtils.IsListWithDefaultItems(targetFolder.ParentList) && (base.SharePointOptions.MigrationMode != MigrationMode.Custom || base.SharePointOptions.ItemCopyingMode != ListItemCopyMode.Preserve))
                {
                    MigrationUtils.ChangeItemOptionsToUpdate(base.SharePointOptions, false);
                    flag2 = true;
                }
                bool flag3 = SPUtils.IsOneNoteFeatureEnabled(sourceItems.ParentSPList);
                bool flag4 = SPUtils.IsOneNoteFeatureEnabled(targetFolder);
                if (flag3 && this.OneNoteFolderExists(targetFolder))
                {
                    oneNoteItems = this.GetOneNoteItems(targetFolder, ref targetItems);
                }
                List<string> strs2 = new List<string>();
                Dictionary<int, int> nums = new Dictionary<int, int>(sourceItems.Count);
                for (int i = 0; i < sourceItems.Count; i++)
                {
                    SPListItem item = (SPListItem)sourceItems[i];
                    if (!flag4 && (item.ItemType == SPListItemType.Folder && SPUtils.IsDefaultOneNoteFolder(item) || item.ItemType == SPListItemType.Item && SPUtils.IsDefaultOneNoteItem(item)))
                    {
                        LogItem logItem2 = new LogItem("Skipping Item", item.FileLeafRef, item.DisplayUrl, SPUtils.OneNoteFolderDisplayName(targetFolder, true), ActionOperationStatus.Skipped)
                        {
                            Information = "Skipping Item because Site Notebook Feature is not activated at Target. To migrate the item, kindly activate Site Notebook feature."
                        };
                        LogItem logItem3 = logItem2;
                        base.FireOperationStarted(logItem3);
                        base.FireOperationFinished(logItem3);
                    }
                    else if (!flag3 || !this.SkipListItem(item, targetFolder))
                    {
                        try
                        {
                            bool itemType = item.ItemType == SPListItemType.Folder;
                            this.CopyListItem(item, i, parentItemId, targetFolder, sourceItems, ref targetItems, itemType, flag2, flag, bIsCopyRoot, baseTemplate, ref nums, strs2);
                            if (flag3 && SPUtils.IsOneNoteItem(item))
                            {
                                strs1.Add(item.Name);
                            }
                            if (base.CheckForAbort())
                            {
                                return;
                            }
                        }
                        finally
                        {
                            if (item != null)
                            {
                                item.Dispose();
                            }
                        }
                    }
                }
                this.DeleteOneNoteItems(strs1, oneNoteItems);
                this.CopyDocumentSetsVersionHistory();
                if (!parentItemId.HasValue)
                {
                    PasteListItemAction.s_listItemTransformerDefinition.EndTransformation(this, sourceItems, targetItems, this.Options.Transformers);
                }
                if (bIsCopyRoot)
                {
                    base.ThreadManager.SetBufferedTasks(base.PermissionsKeyFormatter.GetKeyFor(targetFolder), false, false);
                }
            }
            finally
            {
                base.ThreadManager.SetBufferedTasks(base.GetListItemCopyCompletedBufferKey(targetFolder.ParentList), false, false);
                if (xmlNodes != null)
                {
                    ThreadedOperationDelegate threadedOperationDelegate = new ThreadedOperationDelegate(this.RestoreOriginalListSettings);
                    object[] parentList = new object[] { targetFolder.ParentList, xmlNodes };
                    TaskDefinition taskDefinition = new TaskDefinition(threadedOperationDelegate, parentList, false);
                    base.ThreadManager.QueueBufferedTask(strs, taskDefinition);
                    base.ThreadManager.QueueBufferedTask("RunActionEndReached", taskDefinition);
                }
            }
        }

        private void CopyListItemsOffice365(SPListItemCollection sourceItems, SPFolder targetFolder, int? iParentItemID, bool bIsCopyRoot, ref SPListItemCollection targetItems, IUploadManager uploadManager)
        {
            if (sourceItems == null)
            {
                throw new Exception("Source item collection cannot be null");
            }
            if (targetFolder == null)
            {
                throw new Exception("Target folder cannot be null");
            }
            if (base.SharePointOptions.CorrectingLinks)
            {
                LogItem logItem = null;
                try
                {
                    try
                    {
                        base.LinkCorrector.Scope = base.SharePointOptions.LinkCorrectionScope;
                        base.LinkCorrector.PopulateForItemCopy(sourceItems, targetFolder);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        logItem = new LogItem("Initialize Link Corrector for Item Copy", null, null, null, ActionOperationStatus.Running);
                        base.FireOperationStarted(logItem);
                        logItem.Exception = exception;
                        logItem.Details = exception.StackTrace;
                    }
                }
                finally
                {
                    if (logItem != null)
                    {
                        base.FireOperationFinished(logItem);
                    }
                }
            }
            ListTemplateType baseTemplate = targetFolder.ParentList.BaseTemplate;
            XmlNode xmlNodes = null;
            List<string> strs = null;
            if (targetFolder.Adapter.SharePointVersion.IsSharePoint2010OrLater)
            {
                this.DisableLookupRelationshipEnforcementOnList(targetFolder.ParentList, out xmlNodes, out strs);
            }
            List<string> strs1 = new List<string>();
            try
            {
                bool flag = targetFolder.Adapter.AdapterShortName.Equals("NW", StringComparison.OrdinalIgnoreCase);
                LogItem logItem1 = new LogItem("Fetching target items", "", "", "", ActionOperationStatus.Running)
                {
                    WriteToJobDatabase = false
                };
                base.FireOperationStarted(logItem1);
                bool flag1 = (base.SharePointOptions.CopySubFolders ? true : targetFolder.ParentList is SPDiscussionList);
                ListItemQueryType listItemQueryType = ListItemQueryType.ListItem;
                if (flag1)
                {
                    listItemQueryType |= ListItemQueryType.Folder;
                }
                targetItems = targetFolder.GetItems(flag1, listItemQueryType, null);
                if (!iParentItemID.HasValue)
                {
                    PasteListItemAction.s_listItemTransformerDefinition.BeginTransformation(this, sourceItems, targetItems, this.Options.Transformers);
                }
                sourceItems.SortItemsToEnsureFolderExistence();
                bool flag2 = PasteAction<PasteListItemOptions>.CheckPreservingItems(sourceItems.ParentSPList, base.SharePointOptions);
                int count = sourceItems.Count / 2;
                List<string> strs2 = new List<string>();
                Dictionary<int, int> nums = new Dictionary<int, int>(sourceItems.Count);
                for (int i = 0; i < sourceItems.Count; i++)
                {
                    SPListItem item = (SPListItem)sourceItems[i];
                    try
                    {
                        bool itemType = item.ItemType == SPListItemType.Folder;
                        this.CopyListItemOffice365(item, i, iParentItemID, targetFolder, sourceItems, ref targetItems, itemType, flag2, flag, bIsCopyRoot, baseTemplate, ref nums, strs2, uploadManager);
                        if (base.CheckForAbort())
                        {
                            return;
                        }
                    }
                    finally
                    {
                        if (item != null)
                        {
                            item.Dispose();
                        }
                    }
                }
                if (!iParentItemID.HasValue)
                {
                    PasteListItemAction.s_listItemTransformerDefinition.EndTransformation(this, sourceItems, targetItems, this.Options.Transformers);
                }
            }
            finally
            {
                base.ThreadManager.SetBufferedTasks(base.GetListItemCopyCompletedBufferKey(targetFolder.ParentList), false, false);
                if (xmlNodes != null)
                {
                    ThreadedOperationDelegate threadedOperationDelegate = new ThreadedOperationDelegate(this.RestoreOriginalListSettings);
                    object[] parentList = new object[] { targetFolder.ParentList, xmlNodes };
                    TaskDefinition taskDefinition = new TaskDefinition(threadedOperationDelegate, parentList, false);
                    base.ThreadManager.QueueBufferedTask(strs, taskDefinition);
                    base.ThreadManager.QueueBufferedTask("RunActionEndReached", taskDefinition);
                }
            }
        }

        private void CopyReferencedUsersForItem(SPListItem sourceItem, SPFolder targetFolder, LogItem copyOperationLogItem, IUploadManager uploadManager)
        {
            SecurityPrincipalCollection referencedPrincipals = sourceItem.GetReferencedPrincipals();
            List<SPUser> sPUsers = new List<SPUser>();
            List<SPGroup> sPGroups = new List<SPGroup>();
            foreach (SecurityPrincipal referencedPrincipal in (IEnumerable<SecurityPrincipal>)referencedPrincipals)
            {
                if (!(referencedPrincipal is SPUser))
                {
                    if (!(referencedPrincipal is SPGroup))
                    {
                        continue;
                    }
                    SPGroup owner = (SPGroup)referencedPrincipal;
                    sPGroups.Add(owner);
                    while (owner.Owner != null && (owner.OwnerIsUser && !sPUsers.Contains((SPUser)owner.Owner) || !owner.OwnerIsUser && !sPGroups.Contains((SPGroup)owner.Owner)))
                    {
                        if (!owner.OwnerIsUser)
                        {
                            owner = (SPGroup)owner.Owner;
                            sPGroups.Add(owner);
                        }
                        else
                        {
                            sPUsers.Add((SPUser)owner.Owner);
                        }
                    }
                }
                else
                {
                    sPUsers.Add((SPUser)referencedPrincipal);
                }
            }
            base.EnsurePrincipalExistence(sPUsers.ToArray(), sPGroups.ToArray(), targetFolder.ParentList.ParentWeb, uploadManager, null);
        }

        private bool CorrectLinksInInfoPathDocument(LogItem operation, SPListItem document, AddDocumentOptions options, out byte[] binary)
        {
            string str;
            string url;
            if (!document.BinaryAvailable)
            {
                operation.Operation = "InfoPath document cannot be link corrected because the document binary is not available.";
                operation.Status = ActionOperationStatus.Warning;
                binary = null;
                return false;
            }
            binary = document.Binary;
            if (binary == null)
            {
                return false;
            }
            if (options.ShallowCopyExternalizedData)
            {
                operation.Information = "Shallow copying of externalized data has been disabled in order to correct links in infopath document.";
                options.ShallowCopyExternalizedData = false;
            }
            if (document.ParentList.LinkableUrl == null)
            {
                operation.Information = "InfoPath document cannot be link corrected because the host name was not specified while connecting to database";
                operation.Status = ActionOperationStatus.Warning;
                return false;
            }
            if (document.Adapter.IsDB)
            {
                string[] linkableUrl = new string[] { document.ParentList.LinkableUrl, document.Name };
                url = UrlUtils.ConcatUrls(linkableUrl);
            }
            else
            {
                url = document.Url;
            }
            InfoPathTemplate infoPathTemplate = new InfoPathTemplate(binary, url);
            binary = infoPathTemplate.GetReLinkedTemplate(base.LinkCorrector, out str);
            return true;
        }

        private void DeleteOneNoteItems(List<string> sourceItemsMigrated, SPListItemCollection targetItemsPreserved)
        {
            if (sourceItemsMigrated.Count > 0 && targetItemsPreserved != null && targetItemsPreserved.Count > 0)
            {
                List<SPListItem> sPListItems = new List<SPListItem>();
                foreach (SPListItem sPListItem in targetItemsPreserved)
                {
                    if (sourceItemsMigrated.Exists((string itemName) => itemName == sPListItem.Name) || !SPUtils.IsOneNoteItem(sPListItem))
                    {
                        continue;
                    }
                    sPListItems.Add(sPListItem);
                }
                foreach (SPListItem sPListItem1 in sPListItems)
                {
                    try
                    {
                        targetItemsPreserved.DeleteItem(sPListItem1);
                    }
                    catch
                    {
                        LogItem logItem = new LogItem("Deleting OneNote Item", sPListItem1.FileLeafRef, sPListItem1.DisplayUrl, SPUtils.OneNoteFolderDisplayName(sPListItem1.ParentFolder, true), ActionOperationStatus.Warning)
                        {
                            Information = "Failed to delete old OneNote Item on target."
                        };
                        base.FireOperationStarted(logItem);
                        base.FireOperationFinished(logItem);
                    }
                }
            }
        }

        private void DisableLookupRelationshipEnforcementOnList(SPList targetList, out XmlNode originalTargetListXml, out List<string> listWaitKeys)
        {
            originalTargetListXml = targetList.GetListXML(false).CloneNode(true);
            XmlNode xmlNodes = originalTargetListXml.CloneNode(true);
            XmlNodeList xmlNodeLists = xmlNodes.SelectNodes(".//Fields/Field[@Type=\"Lookup\" or @Type=\"LookupMulti\"]");
            listWaitKeys = new List<string>(xmlNodeLists.Count);
            foreach (XmlNode xmlNodes1 in xmlNodeLists)
            {
                XmlAttribute itemOf = xmlNodes1.Attributes["RelationshipDeleteBehavior"];
                if (itemOf == null || string.Equals(itemOf.Value, "none", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string attributeValueAsString = xmlNodes1.GetAttributeValueAsString("List");
                if (!Utils.IsGuid(attributeValueAsString))
                {
                    continue;
                }
                Guid guid = new Guid(attributeValueAsString);
                SPList listByGuid = targetList.ParentWeb.Lists.GetListByGuid(guid.ToString());
                if (listByGuid == null)
                {
                    continue;
                }
                string listItemCopyCompletedBufferKey = base.GetListItemCopyCompletedBufferKey(listByGuid);
                if (!listWaitKeys.Contains(listItemCopyCompletedBufferKey))
                {
                    listWaitKeys.Add(listItemCopyCompletedBufferKey);
                }
                itemOf.Value = "None";
            }
            if (listWaitKeys.Count <= 0)
            {
                originalTargetListXml = null;
            }
            else
            {
                LogItem logItem = new LogItem("Disabling lookup field relationship enforcement", targetList.Title, "", targetList.DisplayUrl, ActionOperationStatus.Running);
                base.FireOperationStarted(logItem);
                try
                {
                    try
                    {
                        targetList.UpdateList(xmlNodes.OuterXml, true, false);
                    }
                    catch (Exception exception)
                    {
                        logItem.Exception = exception;
                    }
                }
                finally
                {
                    if (logItem.Status == ActionOperationStatus.Running)
                    {
                        logItem.Status = ActionOperationStatus.Completed;
                    }
                    base.FireOperationFinished(logItem);
                }
            }
        }

        private bool FilterAttendeesListAdd(SPListItem item, SPWeb targetWeb, SPListItemCollection items)
        {
            bool flag;
            try
            {
                if (targetWeb.MeetingInstances == null || targetWeb.MeetingInstances.Count == 0 || item.GetNodeXML().Attributes["InstanceID"] == null)
                {
                    flag = true;
                }
                else if (item["InstanceID"] != "0")
                {
                    int num = int.Parse(item["InstanceID"]);
                    foreach (SPMeetingInstance meetingInstance in targetWeb.MeetingInstances)
                    {
                        if (num != meetingInstance.InstanceID)
                        {
                            continue;
                        }
                        flag = true;
                        return flag;
                    }
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        public override long GetAnalysisCost(Dictionary<string, string> analysisProperties)
        {
            string item;
            string str;
            if (analysisProperties == null || analysisProperties.Count == 0)
            {
                return (long)0;
            }
            if (analysisProperties.ContainsKey("ItemsChanged"))
            {
                item = analysisProperties["ItemsChanged"];
            }
            else
            {
                item = null;
            }
            string str1 = item;
            if (analysisProperties.ContainsKey("BytesChanged"))
            {
                str = analysisProperties["BytesChanged"];
            }
            else
            {
                str = null;
            }
            string str2 = str;
            long num = (long)0;
            if (str1 != null)
            {
                long.TryParse(str1, out num);
            }
            long num1 = (str2 != null ? Format.ParseFormattedSize(str2) : (long)0);
            float single = 2.5f;
            float single1 = single / (1f + single);
            float single2 = single1 * (float)num1 + (1f - single1) * (float)num;
            return (long)((int)single2);
        }

        protected bool GetAnticipatedFileLeafRefAndPath(SPListItem sourceItem, ref string sFileLeafRef, ref string sParentFolderPath, bool bIsFolder, SPFolder targetFolder)
        {
            bool flag = false;
            if (base.SharePointOptions.RenameSpecificNodes)
            {
                if (!string.IsNullOrEmpty(sParentFolderPath))
                {
                    SPFolder parentFolder = sourceItem.ParentFolder;
                    if (parentFolder != null)
                    {
                        string serverRelativeUrl = parentFolder.ServerRelativeUrl;
                        sParentFolderPath = base.LinkCorrector.CorrectUrl(string.Concat(serverRelativeUrl, "/", sParentFolderPath));
                        serverRelativeUrl = base.LinkCorrector.CorrectUrl(serverRelativeUrl);
                        sParentFolderPath = sParentFolderPath.Substring(serverRelativeUrl.Length + 1);
                        sParentFolderPath = base.LinkCorrector.DecodeURL(sParentFolderPath);
                    }
                }
                if (sourceItem.ItemType == SPListItemType.Folder)
                {
                    foreach (TransformationTask transformationTask in base.SharePointOptions.TaskCollection.TransformationTasks)
                    {
                        if (!transformationTask.ChangeOperations.ContainsKey("FileLeafRef") || !transformationTask.ApplyTo.Evaluate(sourceItem, new CompareDatesInUtc()))
                        {
                            continue;
                        }
                        sFileLeafRef = transformationTask.ChangeOperations["FileLeafRef"];
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private int GetIdFromXml(string itemXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(itemXml);
            XmlNode firstChild = xmlDocument.FirstChild;
            return Convert.ToInt32(firstChild.Attributes["ID"].Value);
        }

        private string GetLookupValue(string fieldValue, string fieldType)
        {
            string empty = string.Empty;
            if (string.IsNullOrEmpty(fieldValue))
            {
                return empty;
            }
            if (!fieldType.Equals("LookupMulti", StringComparison.InvariantCultureIgnoreCase))
            {
                Guid guid = Guid.Empty;
                empty = string.Format("{0};{1}", fieldValue, guid.ToString("D"));
            }
            else
            {
                string[] strArrays = fieldValue.Split(new char[] { '#' });
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    char[] chrArray = new char[] { ';' };
                    empty = string.Concat(empty, str.Trim(chrArray), ";# ;#");
                }
            }
            return empty;
        }

        private static string GetManifestTaxonomyValue(string fieldValue, string fieldType)
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder("-1;#");
            if (!string.Equals(fieldType, "TaxonomyFieldTypeMulti", StringComparison.OrdinalIgnoreCase))
            {
                stringBuilder.Append(fieldValue);
            }
            else
            {
                string[] strArrays = fieldValue.Split(new char[] { ';' });
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    stringBuilder.Append(strArrays[i]);
                    if (i < (int)strArrays.Length - 1)
                    {
                        stringBuilder.Append(";#-1;#");
                    }
                }
            }
            return stringBuilder.ToString();
        }

        private static string GetModerationStatus(SPListItem spListItem)
        {
            string str;
            string item = spListItem["_ModerationStatus"];
            string str1 = item;
            if (item != null)
            {
                if (str1 == "0")
                {
                    str = "Approved";
                    return str;
                }
                else if (str1 == "1")
                {
                    str = "Denied";
                    return str;
                }
                else
                {
                    if (str1 != "2")
                    {
                        goto Label1;
                    }
                    str = "Pending";
                    return str;
                }
            }
            str = "Draft";
            return str;
            Label1:
            if (str1 != "3")
            {
                str = "Draft";
                return str;
            }
            else
            {
                str = "Draft";
                return str;
            }
        }

        private SPListItemCollection GetOneNoteItems(SPFolder folder, ref SPListItemCollection items)
        {
            if (folder.IsOneNoteFolder)
            {
                return items;
            }
            return this.GetOneNoteItems(folder as SPList);
        }

        private SPListItemCollection GetOneNoteItems(SPList list)
        {
            SPListItemCollection items = null;
            if (list != null)
            {
                string updatedOneNoteFolderName = SPUtils.GetUpdatedOneNoteFolderName(list.ParentWeb);
                if (updatedOneNoteFolderName != null)
                {
                    SPFolder subFolderByPath = list.GetSubFolderByPath(updatedOneNoteFolderName);
                    if (subFolderByPath != null)
                    {
                        items = subFolderByPath.GetItems(false, ListItemQueryType.ListItem, null);
                    }
                }
            }
            return items;
        }

        protected override List<ITransformerDefinition> GetSupportedDefinitions()
        {
            List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
            supportedDefinitions.Add(PasteListItemAction.s_listItemTransformerDefinition);
            return supportedDefinitions;
        }

        private bool HasAttachments(string itemXml, SPAttachmentCollection attachments)
        {
            bool flag;
            IEnumerator enumerator = attachments.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SPAttachment current = (SPAttachment)enumerator.Current;
                    if (itemXml.IndexOf(HttpUtility.UrlPathEncode(string.Concat(current.DirName, "/", current.FileName)), StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        continue;
                    }
                    flag = true;
                    return flag;
                }
                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return flag;
        }

        private bool HasNewerVersions(SPListItem sourceItem, SPListItem targetItem)
        {
            if (sourceItem == null)
            {
                return false;
            }
            if (targetItem == null)
            {
                return true;
            }
            bool flag = false;
            string versionString = sourceItem.VersionString;
            string str = targetItem.VersionString;
            if (!string.IsNullOrEmpty(versionString) && !string.IsNullOrEmpty(str))
            {
                Version version = new Version(versionString);
                Version version1 = new Version(str);
                flag = ((!sourceItem.ParentList.IsDocumentLibrary ? false : targetItem.IsCheckedOut) ? version >= version1 : version > version1);
            }
            return flag;
        }

        private bool HasOneNoteFolder(SPList list)
        {
            if (list != null)
            {
                string updatedOneNoteFolderName = SPUtils.GetUpdatedOneNoteFolderName(list.ParentWeb);
                if (updatedOneNoteFolderName != null && list.GetSubFolderByPath(updatedOneNoteFolderName) != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsAlwaysUpdateOOBBlogandCommunityListItems(bool isUpdatingItems, bool isSourceItemNewer)
        {
            if (base.SharePointOptions.MigrationMode == MigrationMode.Incremental)
            {
                return true;
            }
            if (base.SharePointOptions.MigrationMode == MigrationMode.Custom && isUpdatingItems)
            {
                return isSourceItemNewer;
            }
            return false;
        }

        private bool IsInfoPathFormDocument(SPListItem document)
        {
            if (string.IsNullOrEmpty(document.Name))
            {
                return false;
            }
            return document.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsItemVersionBeingLimited(SPListItem sourceItem)
        {
            if (!base.SharePointOptions.CopyMaxVersions)
            {
                return false;
            }
            return !sourceItem.ParentList.IsWorkflowLibrary;
        }

        private static bool IsNintexItem(SPListItem sourceItem, SPFolder targetFolder, bool bIsFolder)
        {
            bool flag;
            try
            {
                if (targetFolder.Adapter.SharePointVersion.IsSharePointOnline)
                {
                    if (bIsFolder && SPUtils.IsNintexWorkflow(sourceItem.ParentList, sourceItem.Name))
                    {
                        flag = true;
                        return flag;
                    }
                    else if (!bIsFolder && PasteListItemAction.IsNintexWorkflowItem(sourceItem.Name))
                    {
                        SPListCollection lists = sourceItem.ParentList.ParentWeb.Lists;
                        if (lists.Count > 0)
                        {
                            SPList item = lists["NintexWorkflows"];
                            string fileName = Path.GetFileName(sourceItem.ParentRelativePath);
                            if (item != null && !string.IsNullOrEmpty(fileName) && item.SubFolders[fileName] != null)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Format("An error occured while skipping xoml files inside nintex workflow folder for web: {0}. Exception: {1}", sourceItem.ParentList.ParentWeb.Url, exception));
                flag = true;
            }
            return flag;
        }

        private static bool IsNintexWorkflowItem(string sourceItemName)
        {
            if (sourceItemName.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase) || sourceItemName.EndsWith(".xoml", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return sourceItemName.EndsWith(".xoml.rules", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsPermissionCopyAllowed(SPListItem sourceItem, BaseManifestItem manifestFileItem)
        {
            bool flag;
            manifestFileItem.HasUniquePermissions = sourceItem.HasUniquePermissions;
            bool flag1 = false;
            bool flag2 = true;
            bool flag3 = false;
            if (base.SharePointOptions.CopyItemPermissions)
            {
                if (flag2 || !flag2 && base.SharePointOptions.UpdateItems && (base.SharePointOptions.UpdateItemOptionsBitField & 2) > 0)
                {
                    flag = (!flag3 || !base.SharePointOptions.CopyRootPermissions ? sourceItem.HasUniquePermissions : true);
                }
                else
                {
                    flag = false;
                }
                flag1 = flag;
            }
            return flag1;
        }

        private void LinkCorrectAttachments(SPListItem sourceItem, SPList targetList, ref string itemXml)
        {
            if (!this.HasAttachments(itemXml, sourceItem.Attachments))
            {
                return;
            }
            string serverRelativeUrl = sourceItem.ParentList.ServerRelativeUrl;
            char[] chrArray = new char[] { '/' };
            string str = HttpUtility.UrlPathEncode(serverRelativeUrl.Trim(chrArray));
            string serverRelativeUrl1 = targetList.ServerRelativeUrl;
            char[] chrArray1 = new char[] { '/' };
            string str1 = HttpUtility.UrlPathEncode(serverRelativeUrl1.Trim(chrArray1));
            itemXml = itemXml.Replace(str, str1);
        }

        private string LinkCorrectHtmlParts(SPListItem sourceItem, SPListItem targetItem)
        {
            if (sourceItem.HasAttachments)
            {
                base.LinkCorrector.AddMapping(sourceItem.Attachments[0].DirName, targetItem.Attachments[0].DirName);
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(targetItem.XML);
            XmlNode firstChild = xmlDocument.FirstChild;
            foreach (XmlAttribute attribute in firstChild.Attributes)
            {
                firstChild.Attributes[attribute.Name].Value = base.LinkCorrector.CorrectHtml(attribute.Value);
            }
            if (sourceItem.HasAttachments)
            {
                base.LinkCorrector.RemoveMapping(sourceItem.Attachments[0].DirName);
            }
            return firstChild.OuterXml;
        }

        private string LinkCorrectParentFolderName(SPListItem sourceItem)
        {
            string str = base.LinkCorrector.CorrectUrl(sourceItem.ServerRelativeFolderLeafRef);
            char[] chrArray = new char[] { '/' };
            str = str.Trim(chrArray).Substring(0, str.LastIndexOf("/"));
            string str1 = base.LinkCorrector.CorrectUrl(sourceItem.ParentFolder.ServerRelativeUrl);
            str1 = str1.Trim(new char[] { '/' });
            if (str == str1)
            {
                return "";
            }
            char[] chrArray1 = new char[] { '/' };
            str = str.Trim(chrArray1).Substring(str1.Length + 1, str.Length - (str1.Length + 1));
            return base.LinkCorrector.DecodeURL(str);
        }

        private void MapUsersForManifestField(Metalogix.Office365.Field manifestField, string fieldValue, IUploadManager uploadManager)
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                manifestField.Value = string.Empty;
                manifestField.ListItemOverrideValue = string.Empty;
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            char[] chrArray = new char[] { ',' };
            List<string> list = fieldValue.Split(chrArray, StringSplitOptions.RemoveEmptyEntries).Distinct<string>().ToList<string>();
            bool flag = string.Equals(manifestField.Type, "UserMulti", StringComparison.OrdinalIgnoreCase);
            List<string>.Enumerator enumerator = list.GetEnumerator();
            try
            {
                do
                {
                    Label0:
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    int userOrGroupIDByName = uploadManager.GetUserOrGroupIDByName(enumerator.Current);
                    if (userOrGroupIDByName != 0)
                    {
                        if (stringBuilder.Length > 0 && flag)
                        {
                            stringBuilder.Append(";#");
                        }
                        stringBuilder.Append(string.Format("{0}{1}", userOrGroupIDByName, (flag ? ";# " : string.Empty)));
                    }
                    else
                    {
                        goto Label0;
                    }
                }
                while (flag);
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
            manifestField.Value = stringBuilder.ToString();
            manifestField.ListItemOverrideValue = (stringBuilder.Length > 0 ? string.Concat(stringBuilder.ToString(), ";UserInfo") : string.Empty);
        }

        private bool OneNoteFolderExists(SPFolder folder)
        {
            if (!SPUtils.IsOneNoteFeatureEnabled(folder))
            {
                return false;
            }
            if (folder.IsOneNoteFolder)
            {
                return true;
            }
            return this.HasOneNoteFolder(folder as SPList);
        }

        private SPFolder ParentFolderOf(SPListItem item)
        {
            return item.ParentFolder;
        }

        private string RenameFolder(string sSourceXml, SPListItem sourceItem, out bool bRenamed)
        {
            TransformationTask task = base.SharePointOptions.TaskCollection.GetTask(sourceItem, new CompareDatesInUtc());
            if (task == null)
            {
                bRenamed = false;
                return sSourceXml;
            }
            bRenamed = true;
            return task.PerformTransformation(sSourceXml);
        }

        private void RestoreOriginalListSettings(object[] inputs)
        {
            SPList sPList = inputs[0] as SPList;
            XmlNode xmlNodes = inputs[1] as XmlNode;
            LogItem logItem = new LogItem("Restoring lookup field relationship enforcement", sPList.Title, "", sPList.DisplayUrl, ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            try
            {
                try
                {
                    lock (sPList)
                    {
                        sPList.UpdateList(xmlNodes.OuterXml, true, false);
                    }
                }
                catch (Exception exception)
                {
                    logItem.Exception = exception;
                }
            }
            finally
            {
                if (logItem.Status == ActionOperationStatus.Running)
                {
                    logItem.Status = ActionOperationStatus.Completed;
                }
                base.FireOperationFinished(logItem);
            }
            sPList.Dispose();
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            foreach (SPFolder sPFolder in target)
            {
                Node[] nodeArray = new Node[] { sPFolder };
                this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
                bool parserEnabled = true;
                try
                {
                    if (base.SharePointOptions.DisableDocumentParsing)
                    {
                        parserEnabled = sPFolder.ParentList.ParentWeb.ParserEnabled;
                        if (parserEnabled)
                        {
                            sPFolder.ParentList.ParentWeb.SetDocumentParsing(false);
                        }
                    }
                    if (!typeof(ListItemCollection).IsAssignableFrom(source.GetType()))
                    {
                        if (source.Count > 0)
                        {
                            base.RunPreCopyListUpdate(((SPListItem)source[0]).ParentList, sPFolder, base.SharePointOptions);
                        }
                        foreach (Node node in source)
                        {
                            SPListItem sPListItem = node as SPListItem;
                            if (sPListItem == null)
                            {
                                continue;
                            }
                            SPList parentList = sPListItem.ParentList;
                            SPFolder parentFolder = sPListItem.ParentFolder;
                            Node[] nodeArray1 = new Node[] { sPListItem };
                            this.RunListItemCollectionCopy(new SPListItemCollection(parentList, parentFolder, nodeArray1), sPFolder);
                        }
                    }
                    else
                    {
                        SPListItemCollection sPListItemCollection = new SPListItemCollection((ListItemCollection)source);
                        base.RunPreCopyListUpdate(sPListItemCollection.ParentSPList, sPFolder, base.SharePointOptions);
                        this.RunListItemCollectionCopy(sPListItemCollection, sPFolder);
                    }
                    sPFolder.UpdateCurrentNode();
                    if (!sPFolder.HasEventClients)
                    {
                        sPFolder.Dispose();
                    }
                }
                finally
                {
                    if (base.SharePointOptions.DisableDocumentParsing)
                    {
                        if (parserEnabled)
                        {
                            sPFolder.ParentList.ParentWeb.SetDocumentParsing(true);
                        }
                        sPFolder.ParentList.ParentWeb.Dispose();
                    }
                }
            }
        }

        private void RunListItemCollectionCopy(SPListItemCollection sourceItems, SPFolder targetFolder)
        {
            base.InitializeAudienceMappings(sourceItems.ParentSPList, targetFolder);
            this.CopyListItems(sourceItems, targetFolder, null, true, (IUploadManager)null);
        }

        protected override void RunOperation(object[] oParams)
        {
            if (oParams == null || (int)oParams.Length < 3)
            {
                throw new Exception(string.Format("{0} is missing parameters", this.Name));
            }
            bool flag = false;
            if ((int)oParams.Length >= 4 && oParams[3] != null && oParams[3] is bool)
            {
                flag = (bool)oParams[3];
            }
            IUploadManager uploadManager = null;
            if ((int)oParams.Length >= 5)
            {
                uploadManager = oParams[4] as IUploadManager;
            }
            this.CopyListItems(oParams[0] as SPListItemCollection, oParams[1] as SPFolder, (int?)(oParams[2] as int?), flag, uploadManager);
        }

        private void SetTargetParentFolderName(SPListItem sourceItem, SPList targetList, ref string targetParentfolder)
        {
            if (SPUtils.IsDefaultOneNoteItem(sourceItem))
            {
                targetParentfolder = (string.IsNullOrEmpty(sourceItem.ParentFolder.FolderPath) ? SPUtils.GetUpdatedOneNoteFolderName(targetList.ParentWeb) : string.Empty);
            }
        }

        private bool SkipItem(SPListItem sourceItem, SPFolder targetFolder)
        {
            if (sourceItem.ItemType == SPListItemType.Folder || SPUtils.IsOneNoteItem(sourceItem) || !targetFolder.IsOneNoteFolder)
            {
                return false;
            }
            if (base.SharePointOptions.LogSkippedItems)
            {
                LogItem logItem = new LogItem("Skipping Item", sourceItem.FileLeafRef, sourceItem.DisplayUrl, SPUtils.OneNoteFolderDisplayName(targetFolder, true), ActionOperationStatus.Skipped)
                {
                    Information = "Skipping because item cannot be migrated to OneNote folder on target."
                };
                base.FireOperationStarted(logItem);
                base.FireOperationFinished(logItem);
            }
            return true;
        }

        private bool SkipListItem(SPListItem sourceItem, SPFolder targetFolder)
        {
            bool itemType = sourceItem.ItemType == SPListItemType.Folder;
            if (itemType && this.SkipOneNoteFolder(sourceItem, targetFolder))
            {
                return true;
            }
            if (itemType)
            {
                return false;
            }
            return this.SkipItem(sourceItem, targetFolder);
        }

        private bool SkipOneNoteFolder(SPListItem sourceItem, SPFolder targetFolder)
        {
            bool flag = this.OneNoteFolderExists(targetFolder);
            if (sourceItem.ItemType != SPListItemType.Folder || !flag || !sourceItem.IsOneNoteFolder || !(sourceItem.FileLeafRef == SPUtils.GetUpdatedOneNoteFolderName(targetFolder.ParentList.ParentWeb)))
            {
                return false;
            }
            if (!base.GuidMappings.ContainsKey(sourceItem.UniqueId))
            {
                SPListItem item = targetFolder.Items[sourceItem.FileLeafRef] as SPListItem;
                if (item != null)
                {
                    base.AddGuidMappings(sourceItem.UniqueId, item.UniqueId);
                }
            }
            if (base.SharePointOptions.LogSkippedItems)
            {
                LogItem logItem = new LogItem("Skipping OneNote Folder", sourceItem.FileLeafRef, sourceItem.DisplayUrl, SPUtils.OneNoteFolderDisplayName(targetFolder, true), ActionOperationStatus.Skipped)
                {
                    Information = "OneNote folder is never migrated, only its content is migrated if OneNote folder exists on target."
                };
                base.FireOperationStarted(logItem);
                base.FireOperationFinished(logItem);
            }
            return true;
        }

        private static void UpdateContentTypeForKpiListItem(SPListItem sourceListItem, SPFolder targetFolder, XmlNode itemNode)
        {
            SPContentType item = sourceListItem.ParentList.ContentTypes[itemNode.Attributes["ContentTypeId"].Value];
            if (item != null)
            {
                SPContentType parentContentType = item.ParentContentType;
                SPContentType sPContentType = null;
                if (parentContentType != null)
                {
                    sPContentType = targetFolder.ParentList.ParentWeb.ContentTypes[parentContentType.ContentTypeID] ?? targetFolder.ParentList.ParentWeb.ContentTypes.GetContentTypeByName(parentContentType.Name);
                }
                if (sPContentType != null)
                {
                    itemNode.Attributes["ContentType"].Value = sPContentType.Name;
                    itemNode.Attributes["ContentTypeId"].Value = sPContentType.ContentTypeID;
                }
            }
        }

        private void UpdateDataSourceForKpiListItem(SPListItem sourceListItem, SPFolder targetFolder, XmlNode itemNode)
        {
            if (itemNode.Attributes != null)
            {
                string attributeValue = XmlUtility.GetAttributeValue(itemNode, "DataSource");
                string[] strArrays = attributeValue.Split(new char[] { ',' });
                if (itemNode.Attributes["ViewGuid"] != null)
                {
                    string str = XmlUtility.GetAttributeValue(itemNode, "ViewGuid");
                    string empty = string.Empty;
                    SPList sPList = null;
                    string str1 = HttpUtility.UrlDecode(strArrays[0]).Trim();
                    foreach (SPList list in targetFolder.ParentList.ParentWeb.Lists)
                    {
                        string displayUrl = list.DisplayUrl;
                        char[] chrArray = new char[] { '/' };
                        if (!str1.StartsWith(string.Format("{0}/", displayUrl.TrimEnd(chrArray)), StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        empty = list.Title;
                        sPList = list;
                        break;
                    }
                    if (sPList != null)
                    {
                        this.UpdateViewAttributesForKpiListItem(sourceListItem, empty, str, itemNode, sPList);
                        return;
                    }
                    LogItem logItem = new LogItem("Searching Referenced List", sourceListItem.Title, str1, targetFolder.ParentList.DisplayUrl, ActionOperationStatus.Warning);
                    base.FireOperationStarted(logItem);
                    logItem.Information = string.Format("Referenced List '{0}' not found at target '{1}'. So, current indicator may not work properly.", str1, targetFolder.ParentList.DisplayUrl);
                    base.FireOperationFinished(logItem);
                }
            }
        }

        private string UpdateIdsAccordingToItemIdMap(string sItemXml, Dictionary<int, int> itemIdMap)
        {
            int num;
            int num1;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sItemXml);
            XmlNode firstChild = xmlDocument.FirstChild;
            if (firstChild.Attributes != null)
            {
                XmlAttribute itemOf = firstChild.Attributes["MasterSeriesItemID"];
                if (itemOf != null && int.TryParse(itemOf.Value, out num) && itemIdMap.TryGetValue(num, out num1))
                {
                    itemOf.Value = num1.ToString();
                }
                XmlAttribute xmlAttribute = firstChild.Attributes["RelatedItems"];
                if (xmlAttribute != null)
                {
                    Match match = Regex.Match(xmlAttribute.Value, "^\\[\\{([^\\]]*)\\}\\]$", RegexOptions.Compiled);
                    if (match.Success)
                    {
                        string value = match.Groups[1].Value;
                        string[] strArrays = new string[] { "},{" };
                        string[] strArrays1 = value.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < (int)strArrays1.Length; i++)
                        {
                            string str = strArrays1[i];
                            string[] strArrays2 = new string[] { ",\"" };
                            string[] strArrays3 = str.Split(strArrays2, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < (int)strArrays3.Length; j++)
                            {
                                Match match1 = Regex.Match(strArrays3[j], "(?<Key>[^\"]*)\":\"?(?<Value>[^\"]*)\"?$", RegexOptions.Compiled);
                                if (match1.Success)
                                {
                                    string value1 = match1.Groups["Key"].Value;
                                    string str1 = match1.Groups["Value"].Value;
                                    if (value1 == "WebId" || value1 == "ListId")
                                    {
                                        try
                                        {
                                            Guid guid = new Guid(str1);
                                            if (base.GuidMappings.ContainsKey(guid))
                                            {
                                                string value2 = xmlAttribute.Value;
                                                Guid item = base.GuidMappings[guid];
                                                xmlAttribute.Value = value2.Replace(str1, item.ToString());
                                            }
                                        }
                                        catch (Exception exception1)
                                        {
                                            Exception exception = exception1;
                                            LogItem logItem = new LogItem(string.Format("Updating Related Item {0}", value1), firstChild.Attributes["Title"].Value, firstChild.Attributes["FileRef"].Value, "", ActionOperationStatus.Warning);
                                            LogItem logItem1 = logItem;
                                            logItem1.Information = string.Concat(logItem1.Information, string.Format("Fail to map {0}:{1} - {2}", value1, str1, exception.Message));
                                            LogItem logItem2 = logItem;
                                            logItem2.Details = string.Concat(logItem2.Details, exception.StackTrace);
                                            base.FireOperationStarted(logItem);
                                            base.FireOperationFinished(logItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return firstChild.OuterXml;
        }

        private string UpdateItemID(string sOriginalItemXml, int iNewItemId)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sOriginalItemXml);
            XmlNode firstChild = xmlDocument.FirstChild;
            XmlNode str = xmlDocument.CreateNode(XmlNodeType.Attribute, "ID", null);
            str.Value = iNewItemId.ToString();
            firstChild.Attributes.SetNamedItem(str);
            if (firstChild.Attributes["IssueID"] != null)
            {
                firstChild.Attributes["IssueID"].Value = iNewItemId.ToString();
            }
            return firstChild.OuterXml;
        }

        private string UpdateKpiListItemXml(string itemXml, SPListItem sourceListItem, SPFolder targetFolder)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(itemXml);
            XmlNode firstChild = xmlDocument.FirstChild;
            bool flag = false;
            if (firstChild.Attributes != null && firstChild.Attributes["ContentTypeId"] != null)
            {
                if (firstChild.Attributes["ContentTypeId"].Value.StartsWith("0x00A7470EADF4194E2E9ED1031B61DA088402"))
                {
                    flag = true;
                    if (firstChild.Attributes["DataSource"] != null)
                    {
                        this.UpdateDataSourceForKpiListItem(sourceListItem, targetFolder, firstChild);
                    }
                }
                if (firstChild.Attributes["ContentTypeId"].Value.StartsWith("0x00A7470EADF4194E2E9ED1031B61DA088404") || firstChild.Attributes["ContentTypeId"].Value.StartsWith("0x00A7470EADF4194E2E9ED1031B61DA088403") || firstChild.Attributes["ContentTypeId"].Value.StartsWith("0x00A7470EADF4194E2E9ED1031B61DA088401"))
                {
                    flag = true;
                }
                if (flag)
                {
                    PasteListItemAction.UpdateContentTypeForKpiListItem(sourceListItem, targetFolder, firstChild);
                }
            }
            return firstChild.OuterXml;
        }

        private void UpdateViewAttributesForKpiListItem(SPListItem sourceListItem, string listName, string sourceViewGuid, XmlNode itemNode, SPList targetDataSourceList)
        {
            SPList listByTitle = sourceListItem.ParentList.ParentWeb.Lists.GetListByTitle(listName);
            SPView item = listByTitle.Views[string.Concat("{", sourceViewGuid.ToUpper(), "}")];
            if (item != null)
            {
                string displayName = item.DisplayName;
                if (itemNode.Attributes["ViewName"] != null)
                {
                    itemNode.Attributes["ViewName"].Value = displayName;
                }
                string str = base.LinkCorrector.MapGuid(sourceViewGuid);
                if (string.IsNullOrEmpty(str))
                {
                    SPView viewByDisplayName = targetDataSourceList.Views.GetViewByDisplayName(displayName);
                    if (viewByDisplayName != null)
                    {
                        string name = viewByDisplayName.Name;
                        char[] chrArray = new char[] { '{' };
                        str = name.TrimStart(chrArray).TrimEnd(new char[] { '}' });
                    }
                    else
                    {
                        string name1 = targetDataSourceList.Views.DefaultView.Name;
                        char[] chrArray1 = new char[] { '{' };
                        str = name1.TrimStart(chrArray1).TrimEnd(new char[] { '}' });
                    }
                    itemNode.Attributes["ViewGuid"].Value = str;
                    base.LinkCorrector.AddGuidMapping(sourceViewGuid, str);
                    return;
                }
                itemNode.Attributes["ViewGuid"].Value = str;
            }
        }
    }
}