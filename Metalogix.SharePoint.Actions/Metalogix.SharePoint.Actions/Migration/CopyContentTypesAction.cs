using Metalogix;
using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCPublicFolder | ProductFlags.CMWebComponents)]
	[MandatoryTransformers(new Type[] { typeof(ContentTypeFieldMapper) })]
	[Name("Paste Content Types")]
	[ShowInMenus(false)]
	public class CopyContentTypesAction : PasteAction<PasteContentTypesOptions>
	{
		private TransformerDefinition<SPContentType, CopyContentTypesAction, SPContentTypeCollection, SPContentTypeCollection> s_ctTransformer = new TransformerDefinition<SPContentType, CopyContentTypesAction, SPContentTypeCollection, SPContentTypeCollection>("SharePoint Content Types", false);

		public CopyContentTypesAction()
		{
		}

		protected override void AppendPowerShellParameters(StringBuilder sb, object cmdletOptions)
		{
			if (base.SharePointOptions.FilterCTs)
			{
				sb.Append(" -FilteredCTCollection ");
				IEnumerator<string> enumerator = base.SharePointOptions.FilteredCTCollection.GetEnumerator();
				enumerator.MoveNext();
				while (enumerator.Current != null)
				{
					sb.Append(base.EncodePowerShellParameterValue(enumerator.Current));
					enumerator.MoveNext();
					if (enumerator.Current == null)
					{
						continue;
					}
					sb.Append(", ");
				}
			}
			base.AppendPowerShellParameters(sb, cmdletOptions);
		}

		private void ApplyNewContentTypes(SPList targetList, ContentTypeApplicationOptionsCollection applicationOptions)
		{
			List<string> strs = new List<string>();
			foreach (ContentTypeApplicationOptions datum in applicationOptions.Data)
			{
				LogItem logItem = null;
				SPContentType sPContentType = null;
				strs.Add(datum.ContentTypeName);
				try
				{
					try
					{
						foreach (SPContentType contentType in targetList.ParentWeb.ContentTypes)
						{
							if (contentType.Name != datum.ContentTypeName)
							{
								continue;
							}
							sPContentType = contentType;
							break;
						}
						if (sPContentType != null)
						{
							logItem = new LogItem("Applying Content Type", datum.ContentTypeName, targetList.ParentWeb.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							XmlNode xmlNodes = null;
							targetList.ContentTypes.AddOrUpdateContentType(sPContentType.Name, sPContentType.XML, sPContentType.Name, datum.MakeDefault, out xmlNodes);
							logItem.Status = ActionOperationStatus.Completed;
						}
						else
						{
							continue;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem == null)
						{
							logItem = new LogItem("Applying Content Type", datum.ContentTypeName, targetList.ParentWeb.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
						}
						logItem.Exception = exception;
						logItem.SourceContent = (sPContentType == null ? "" : sPContentType.XML);
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
			if (applicationOptions.RemoveAllOtherContentTypes)
			{
				List<SPContentType> sPContentTypes = new List<SPContentType>();
				foreach (SPContentType contentType1 in targetList.ContentTypes)
				{
					if (strs.Contains(contentType1.Name))
					{
						continue;
					}
					sPContentTypes.Add(contentType1);
				}
				if (sPContentTypes.Count > 0)
				{
					LogItem stackTrace = new LogItem("Removing all other content types from list", targetList.Title, null, targetList.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(stackTrace);
					try
					{
						try
						{
							targetList.ContentTypes.DeleteContentTypes(sPContentTypes);
							stackTrace.Status = ActionOperationStatus.Completed;
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							stackTrace.Exception = exception2;
							stackTrace.Details = exception2.StackTrace;
						}
					}
					finally
					{
						base.FireOperationFinished(stackTrace);
					}
				}
			}
			targetList.RefreshFields(true);
		}

		private Dictionary<string, SPContentType> BuildParentContentTypeIDToContentTypeMap(SPContentTypeCollection collection)
		{
			Dictionary<string, SPContentType> strs = new Dictionary<string, SPContentType>(collection.Count);
			foreach (SPContentType sPContentType in collection)
			{
				string parentContentTypeID = sPContentType.ParentContentTypeID;
				parentContentTypeID = (string.IsNullOrEmpty(parentContentTypeID) ? parentContentTypeID : parentContentTypeID.ToLower());
				if (string.IsNullOrEmpty(parentContentTypeID) || strs.ContainsKey(parentContentTypeID))
				{
					continue;
				}
				strs.Add(parentContentTypeID, sPContentType);
			}
			return strs;
		}

		private void CopyContentTypeFormWebParts(object[] inputs)
		{
			SPFile sPFile = inputs[0] as SPFile;
			SPFile sPFile1 = inputs[1] as SPFile;
			LogItem logItem = new LogItem("Copying Web Parts on Form Page", sPFile.Name, sPFile.DisplayUrl, sPFile1.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					SPWebPartPage sPWebPartPage = inputs[2] as SPWebPartPage;
					SPWebPartPage sPWebPartPage1 = inputs[3] as SPWebPartPage;
					CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction()
					{
						LinkCorrector = base.LinkCorrector
					};
					copyWebPartsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					if (!sPWebPartPage.FileLeafRef.Equals("docsethomepage.aspx", StringComparison.Ordinal))
					{
						copyWebPartsAction.SharePointOptions.ExistingWebPartsAction = ExistingWebPartsProtocol.Preserve;
					}
					base.SubActions.Add(copyWebPartsAction);
					object[] objArray = new object[] { sPWebPartPage, sPWebPartPage1, logItem };
					copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(sPWebPartPage.ParentWeb, sPWebPartPage1.ParentWeb), null);
					if (logItem.Status != ActionOperationStatus.Failed)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.SourceContent = sPFile.XML;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Exception = exception;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CopyContentTypeInfoPathForm(SPFile sourceFile, SPContentType sourceCT, SPContentType targetCT, SPFolderBasic targetResFolder)
		{
			ActionOperationStatus actionOperationStatu;
			if (string.IsNullOrEmpty(sourceCT.ResourceFolderWebRelativeUrl) || string.IsNullOrEmpty(targetCT.ResourceFolderWebRelativeUrl))
			{
				return;
			}
			string[] displayUrl = new string[] { sourceCT.ParentCollection.ParentWeb.DisplayUrl, sourceCT.ResourceFolderWebRelativeUrl };
			string str = UrlUtils.ConcatUrls(displayUrl);
			string[] strArrays = new string[] { targetCT.ParentCollection.ParentWeb.DisplayUrl, targetCT.ResourceFolderWebRelativeUrl };
			string str1 = UrlUtils.ConcatUrls(strArrays);
			string str2 = string.Format(Resources.FS_CopyingInfoPathTemplate, string.Format("{0}:{1}", Resources.ContentType, sourceCT.Name));
			if (!targetCT.ParentCollection.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				LogItem logItem = new LogItem(str2, sourceFile.Name, str, str1, ActionOperationStatus.Failed)
				{
					Information = Resources.InfoPathMigrationNotSupported
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
			}
			else
			{
				LogItem successReviewDetails = new LogItem(str2, sourceFile.Name, str, str1, ActionOperationStatus.Running);
				base.FireOperationStarted(successReviewDetails);
				try
				{
					try
					{
						byte[] content = sourceFile.GetContent();
						OperationReportingResult operationReportingResult = null;
						StringBuilder stringBuilder = new StringBuilder();
						string[] webRelativeUrl = new string[] { (sourceFile.Web.Adapter.IsDB ? sourceFile.Web.LinkableUrl : sourceFile.Web.DisplayUrl), sourceFile.WebRelativeUrl };
						InfoPathTemplate infoPathTemplate = new InfoPathTemplate(content, UrlUtils.ConcatUrls(webRelativeUrl));
						string str3 = "";
						SPList parentList = targetCT.ParentCollection.ParentList;
						if (parentList == null)
						{
							content = infoPathTemplate.GetReLinkedTemplate(base.LinkCorrector, out str3);
							if (!UrlUtils.Equal(sourceCT.DocumentTemplateServerRelativeUrl, sourceFile.ServerRelativeUrl))
							{
								targetResFolder.Files.Add(sourceFile, content);
							}
							else
							{
								targetCT.AddDocumentTemplate(content, sourceFile.Name);
							}
						}
						else
						{
							content = infoPathTemplate.GetReLinkedTemplate(base.LinkCorrector, out str3, parentList, targetCT);
							operationReportingResult = targetCT.AddFormTemplate(content, str3);
						}
						if (operationReportingResult == null)
						{
							actionOperationStatu = ActionOperationStatus.Completed;
						}
						else
						{
							if (!operationReportingResult.ErrorOccured)
							{
								actionOperationStatu = (operationReportingResult.WarningOccured || operationReportingResult.HasInformation ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
							}
							else
							{
								actionOperationStatu = ActionOperationStatus.Failed;
							}
							if (operationReportingResult.ErrorOccured)
							{
								stringBuilder.AppendLine("ERRORS:");
								stringBuilder.AppendLine(operationReportingResult.GetAllErrorsAsString);
							}
							if (operationReportingResult.WarningOccured)
							{
								stringBuilder.AppendLine("WARNINGS:");
								stringBuilder.AppendLine(operationReportingResult.GetAllWarningsAsString);
							}
							if (operationReportingResult.HasInformation)
							{
								stringBuilder.AppendLine("INFORMATION:");
								stringBuilder.AppendLine(operationReportingResult.AllInformationAsString);
							}
						}
						if (string.Equals(sourceCT.ContentTypeXML.Attributes.GetAttributeValueAsString("RequireClientRenderingOnNew"), "false", StringComparison.OrdinalIgnoreCase) && !targetCT.ParentCollection.ParentWeb.HasFormsServiceFeature)
						{
							if (actionOperationStatu != ActionOperationStatus.Failed)
							{
								actionOperationStatu = ActionOperationStatus.Warning;
							}
							stringBuilder.AppendLine(string.Format("WARNING: Migration of browser activated template settings for form template is not supported on target {0}.", targetCT.ParentCollection.ParentWeb.Url));
						}
						if (!string.IsNullOrEmpty(str3))
						{
							stringBuilder.AppendLine("CORRECTED REFERENCES:");
							stringBuilder.AppendLine(str3);
						}
						switch (actionOperationStatu)
						{
							case ActionOperationStatus.Warning:
							{
								successReviewDetails.Information = "Warnings encountered, please see details";
								break;
							}
							case ActionOperationStatus.Failed:
							{
								successReviewDetails.Information = "Issues encountered, please see details";
								break;
							}
							default:
							{
								if (stringBuilder.Length <= 0)
								{
									break;
								}
								successReviewDetails.Information = Resources.SuccessReviewDetails;
								break;
							}
						}
						successReviewDetails.Status = actionOperationStatu;
						successReviewDetails.Details = stringBuilder.ToString();
					}
					catch (Exception exception)
					{
						successReviewDetails.Exception = exception;
					}
				}
				finally
				{
					base.FireOperationFinished(successReviewDetails);
				}
			}
		}

		private void CopyContentTypeResourceFolderContent(object[] args)
		{
			SPContentType sPContentType = (SPContentType)args[0];
			SPContentType sPContentType1 = (SPContentType)args[1];
			if ((int)args.Length < 3)
			{
				this.CopyContentTypeResourceFolderContent(sPContentType, sPContentType1, null);
				return;
			}
			this.CopyContentTypeResourceFolderContent(sPContentType, sPContentType1, args[2] as SPFolderBasic);
		}

		protected void CopyContentTypeResourceFolderContent(SPContentType sourceCT, SPContentType targetCT, SPFolderBasic sourceResFolder = null)
		{
			bool flag;
			if (base.CheckForAbort())
			{
				return;
			}
			if (sourceResFolder == null)
			{
				sourceResFolder = sourceCT.GetResourceFolder();
			}
			SPFolderBasic resourceFolder = targetCT.GetResourceFolder();
			if (sourceResFolder == null || resourceFolder == null || sourceResFolder.Files.Count == 0)
			{
				return;
			}
			LogItem logItem = null;
			string[] displayUrl = new string[] { sourceCT.ParentCollection.ParentWeb.DisplayUrl, sourceResFolder.WebRelativeUrl };
			string str = UrlUtils.ConcatUrls(displayUrl);
			string[] strArrays = new string[] { targetCT.ParentCollection.ParentWeb.DisplayUrl, resourceFolder.WebRelativeUrl };
			string str1 = UrlUtils.ConcatUrls(strArrays);
			if (targetCT.ParentCollection.ParentList != null)
			{
				logItem = new LogItem(Resources.CopyingContentTypeResourceFolderForList, sourceCT.Name, str, str1, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
			}
			try
			{
				try
				{
					string documentTemplateServerRelativeUrl = sourceCT.DocumentTemplateServerRelativeUrl;
					foreach (SPFile file in sourceResFolder.Files)
					{
						if (!base.CheckForAbort())
						{
							try
							{
								bool item = resourceFolder.Files[file.Name] != null;
								bool flag1 = (string.IsNullOrEmpty(file.ModifiedBy) ? true : file.CustomizedPageStatus == SPCustomizedPageStatus.Uncustomized);
								if (!sourceCT.ParentCollection.ParentWeb.Adapter.IsDB && !item || !flag1)
								{
									if (sourceResFolder.Web.Adapter.SharePointVersion.MajorVersion == resourceFolder.Web.Adapter.SharePointVersion.MajorVersion)
									{
										flag = false;
									}
									else
									{
										flag = (file.Name.EndsWith(".master", StringComparison.OrdinalIgnoreCase) ? true : file.Name.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase));
									}
									if (!item || !flag)
									{
										LogItem logItem1 = null;
										try
										{
											try
											{
												if (!file.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
												{
													logItem1 = new LogItem(Resources.CopyingContentTypeResourceFile, file.Name, str, str1, ActionOperationStatus.Running);
													base.FireOperationStarted(logItem1);
													if (!UrlUtils.Equal(documentTemplateServerRelativeUrl, file.ServerRelativeUrl))
													{
														resourceFolder.Files.Add(file);
													}
													else
													{
														targetCT.AddDocumentTemplate(file.GetContent(), file.Name);
													}
													logItem1.Status = ActionOperationStatus.Completed;
												}
												else
												{
													this.CopyContentTypeInfoPathForm(file, sourceCT, targetCT, resourceFolder);
												}
											}
											catch (Exception exception1)
											{
												Exception exception = exception1;
												if (logItem1 == null)
												{
													logItem1 = new LogItem("Copying Content Type Resource File", file.Name, str, str1, ActionOperationStatus.Failed);
													base.FireOperationStarted(logItem1);
												}
												logItem1.Exception = exception;
											}
										}
										finally
										{
											if (logItem1 != null)
											{
												base.FireOperationFinished(logItem1);
											}
										}
									}
								}
							}
							finally
							{
								file.ReleaseContent();
							}
						}
						else
						{
							return;
						}
					}
					if (base.SharePointOptions.CopyFormWebParts)
					{
						resourceFolder = targetCT.GetResourceFolder();
						this.CopyWebPartsOnPage(sourceResFolder, resourceFolder);
					}
					if (logItem != null)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					if (logItem == null)
					{
						logItem = new LogItem("Copying Content Type Resource Folder", sourceCT.Name, str, str1, ActionOperationStatus.Failed);
						base.FireOperationStarted(logItem);
					}
					logItem.Exception = exception2;
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

		private void CopyListContentTypes(SPList sourceList, SPList targetList, SPContentTypeCollection targetWebContentTypes)
		{
			SPContentType sPContentType;
			List<string> strs = new List<string>();
			this.s_ctTransformer.BeginTransformation(this, sourceList.ContentTypes, targetList.ContentTypes, this.Options.Transformers);
			List<SPContentType> sPContentTypes = new List<SPContentType>();
			foreach (SPContentType contentType in sourceList.ContentTypes)
			{
				int num = 0;
				if (contentType.IsFromFeature)
				{
					continue;
				}
				foreach (SPContentType sPContentType1 in sPContentTypes)
				{
					if (sPContentType1.ContentTypeID.Length > contentType.ContentTypeID.Length)
					{
						break;
					}
					num++;
				}
				sPContentTypes.Insert(num, contentType);
			}
			List<string> strs1 = new List<string>();
			Dictionary<string, SPContentType> contentTypeMap = this.BuildParentContentTypeIDToContentTypeMap(targetList.ContentTypes);
			foreach (SPContentType sPContentType2 in sPContentTypes)
			{
				if (sourceList.BaseTemplate == ListTemplateType.NoCodeWorkflows && sourceList.Name == "Workflows" && (sPContentType2.Name == "User Workflow Document" || sPContentType2.Name == "Folder"))
				{
					continue;
				}
				LogItem logItem = null;
				SPContentType sPContentType3 = null;
				string name = sPContentType2.Name;
				try
				{
					try
					{
						strs.Add(name);
						sPContentType3 = this.s_ctTransformer.Transform(sPContentType2, this, sourceList.ContentTypes, targetList.ContentTypes, this.Options.Transformers);
						if (sPContentType3 != null)
						{
							SPContentType item = null;
							string str = null;
							bool flag = (!sourceList.Adapter.SharePointVersion.IsSharePoint2010 || !targetList.Adapter.SharePointVersion.IsSharePoint2013OrLater ? false : sPContentType3.ParentContentTypeID.StartsWith("0x0101009148F5A04DDD49CBA7127AADA5FB792B00291D173ECE694D56B19D111489C4369D"));
							string parentContentTypeID = sPContentType3.ParentContentTypeID;
							if (flag)
							{
								parentContentTypeID = "0x0120D520A808";
							}
							parentContentTypeID = (string.IsNullOrEmpty(parentContentTypeID) ? parentContentTypeID : parentContentTypeID.ToLower());
							if (!contentTypeMap.TryGetValue(parentContentTypeID, out sPContentType))
							{
								item = targetWebContentTypes[parentContentTypeID];
								if (item != null)
								{
									str = item.Name;
								}
							}
							else
							{
								item = sPContentType.ParentContentType;
								if (item != null)
								{
									str = item.Name;
								}
								else
								{
									if (base.SharePointOptions.LogSkippedItems)
									{
										logItem = new LogItem("Applying Content Type", name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
										base.FireOperationStarted(logItem);
										logItem.Status = ActionOperationStatus.Skipped;
										logItem.Information = "The content type is already applied to the target list but the parent cannot be found on the target.";
									}
									continue;
								}
							}
							if (item == null)
							{
								SPContentType parentContentType = sPContentType3.ParentContentType;
								str = (parentContentType != null ? parentContentType.Name : sPContentType3.Name);
								item = targetWebContentTypes.GetContentTypeByName(str);
								if (item == null && parentContentType != null && parentContentType.Name != sPContentType3.Name)
								{
									str = sPContentType3.Name;
									if (strs1.Contains(str))
									{
										if (base.SharePointOptions.LogSkippedItems)
										{
											logItem = new LogItem("Applying Content Type", name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
											base.FireOperationStarted(logItem);
											logItem.Status = ActionOperationStatus.Skipped;
											logItem.Information = "Closest parent content type match has already been applied to the target list.";
										}
										continue;
									}
									else
									{
										item = targetWebContentTypes.GetContentTypeByName(str);
									}
								}
								if (item == null && parentContentType == null)
								{
									parentContentType = sPContentType3.GetClosestParent();
									str = (parentContentType != null ? parentContentType.Name : sPContentType3.Name);
									if (strs1.Contains(str))
									{
										if (base.SharePointOptions.LogSkippedItems)
										{
											logItem = new LogItem("Applying Content Type", name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
											base.FireOperationStarted(logItem);
											logItem.Status = ActionOperationStatus.Skipped;
											logItem.Information = "Closest parent content type match has already been applied to the target list.";
										}
										continue;
									}
									else
									{
										item = targetWebContentTypes.GetContentTypeByName(str);
									}
								}
							}
							if (item != null)
							{
								XmlNode xmlNodes = null;
								item = targetList.ContentTypes.AddOrUpdateContentType(name, sPContentType3.XML, str, false, out xmlNodes);
								strs1.Add(str);
								SPFolderBasic resourceFolder = sPContentType2.GetResourceFolder();
								if (resourceFolder != null && resourceFolder.Files.Count > 0)
								{
									XmlAttribute d = item.ContentTypeXML.OwnerDocument.CreateAttribute("ListID");
									d.Value = targetList.ID;
									item.ContentTypeXML.Attributes.Append(d);
									base.LinkCorrector.AddStringMapping(sPContentType3.ContentTypeID, item.ContentTypeID);
									Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
									object[] objArray = new object[] { sPContentType2, item, resourceFolder };
									threadManager.QueueBufferedTask("CopyDocumentTemplatesforContentTypes", objArray, new ThreadedOperationDelegate(this.CopyContentTypeResourceFolderContent));
								}
								if ((base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations) && !base.WorkflowMappings.ContainsKey(sPContentType2.ContentTypeID.ToString()))
								{
									base.WorkflowMappings.Add(sPContentType2.ContentTypeID.ToString(), item.ContentTypeID.ToString());
								}
							}
							else if (base.SharePointOptions.LogSkippedItems)
							{
								logItem = new LogItem("Applying Content Type", name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem);
								logItem.Status = ActionOperationStatus.Skipped;
								logItem.Information = "Content type is not available on the target site";
							}
						}
						else
						{
							continue;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem == null)
						{
							logItem = new LogItem("Applying Content Type", name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
						}
						logItem.Exception = exception;
						logItem.SourceContent = (sPContentType3 != null ? sPContentType3.XML : string.Empty);
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
			if (!targetList.Adapter.IsNws && !(sourceList is SPDiscussionList))
			{
				List<string> contentTypesOrder = sourceList.ContentTypesOrder;
				List<string> contentTypesOrder1 = targetList.ContentTypesOrder;
				if (contentTypesOrder.Count <= 0)
				{
					this.ReorderContentTypes(sourceList, targetList, strs, false);
				}
				else
				{
					foreach (string str1 in contentTypesOrder1)
					{
						if (strs.Contains(str1))
						{
							continue;
						}
						contentTypesOrder.Add(str1);
					}
					this.ReorderContentTypes(sourceList, targetList, contentTypesOrder, true);
				}
			}
			if (sPContentTypes.Count > 0)
			{
				this.RemoveExtraContentTypes(sourceList, targetList, sPContentTypes);
			}
			this.s_ctTransformer.EndTransformation(this, sourceList.ContentTypes, targetList.ContentTypes, this.Options.Transformers);
		}

		protected void CopyReferencedSiteColumns(SPWeb sourceWeb, SPWeb targetWeb)
		{
			try
			{
				CopySiteColumnsAction copySiteColumnsAction = new CopySiteColumnsAction();
				copySiteColumnsAction.Options.SetFromOptions(this.Options);
				this.ConnectSubaction(copySiteColumnsAction);
				object[] referencedFields = new object[] { sourceWeb.ContentTypes.GetReferencedFields(), null, targetWeb.GetAvailableColumns(false), null, true };
				copySiteColumnsAction.RunAsSubAction(referencedFields, new ActionContext(sourceWeb, targetWeb), null);
			}
			catch
			{
			}
		}

		protected void CopyWebContentTypes(SPContentTypeCollection sourceWebContentTypes, SPContentTypeCollection targetWebContentTypes)
		{
			SPContentType sPContentType;
			string name;
			string str;
			if (targetWebContentTypes.Count == 0)
			{
				return;
			}
			CommonSerializableList<string> filteredCTCollection = base.SharePointOptions.FilteredCTCollection;
			if (base.SharePointOptions.FilterCTs && (filteredCTCollection == null || filteredCTCollection.Count == 0))
			{
				return;
			}
			List<SPContentType> sPContentTypes = new List<SPContentType>();
			this.s_ctTransformer.BeginTransformation(this, sourceWebContentTypes, targetWebContentTypes, this.Options.Transformers);
			foreach (SPContentType sourceWebContentType in sourceWebContentTypes)
			{
				if (base.SharePointOptions.FilterCTs && !filteredCTCollection.Contains(sourceWebContentType.Name))
				{
					continue;
				}
				int num = 0;
				if (sourceWebContentType.IsFromFeature)
				{
					continue;
				}
				foreach (SPContentType sPContentType1 in sPContentTypes)
				{
					if (sPContentType1.ContentTypeID.Length > sourceWebContentType.ContentTypeID.Length)
					{
						break;
					}
					num++;
				}
				sPContentTypes.Insert(num, sourceWebContentType);
			}
			List<string> strs = new List<string>();
			foreach (SPContentType sPContentType2 in sPContentTypes)
			{
				SPContentType targetContentTypeBySourceContentTypeForWebLevel = this.GetTargetContentTypeBySourceContentTypeForWebLevel(sPContentType2, targetWebContentTypes);
				bool flag = targetContentTypeBySourceContentTypeForWebLevel != null;
				LogItem logItem = null;
				SPContentType sPContentType3 = null;
				try
				{
					try
					{
						if (base.CheckForAbort())
						{
							return;
						}
						else if (sPContentType2.IsFromFeature)
						{
							continue;
						}
						else if (targetWebContentTypes.ParentWeb.Parent == null || targetWebContentTypes.ParentWeb.Parent is SPBaseServer || targetContentTypeBySourceContentTypeForWebLevel == null || this.GetTargetContentTypeBySourceContentTypeForWebLevel(sPContentType2, ((SPWeb)targetWebContentTypes.ParentWeb.Parent).ContentTypes) == null)
						{
							logItem = new LogItem(string.Concat((flag ? "Updating" : "Adding"), " Content Type"), sPContentType2.Name, sourceWebContentTypes.ParentWeb.DisplayUrl, targetWebContentTypes.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							sPContentType3 = this.s_ctTransformer.Transform(sPContentType2, this, sourceWebContentTypes, targetWebContentTypes, this.Options.Transformers);
							if (sPContentType3 != null)
							{
								SPContentType sPContentType4 = (targetContentTypeBySourceContentTypeForWebLevel != null ? targetContentTypeBySourceContentTypeForWebLevel : sPContentType3);
								sPContentType = (targetContentTypeBySourceContentTypeForWebLevel == null ? this.GetTargetContentTypeBySourceContentTypeForWebLevel(sPContentType3.ParentContentType, targetWebContentTypes) : targetContentTypeBySourceContentTypeForWebLevel.ParentContentType);
								string name1 = sPContentType4.Name;
								if (sPContentType == null)
								{
									name = null;
								}
								else
								{
									name = sPContentType.Name;
								}
								string str1 = name;
								string xML = sPContentType3.XML;
								if (sPContentType2.HasDocumentTemplateInsideResourceFolder)
								{
									XmlNode xmlNode = XmlUtility.StringToXmlNode(xML);
									XmlNode xmlNodes = xmlNode.SelectSingleNode("//DocumentTemplate");
									if (xmlNodes != null)
									{
										xmlNodes.ParentNode.RemoveChild(xmlNodes);
										xML = xmlNode.OuterXml;
									}
								}
								XmlNode xmlNodes1 = null;
								SPContentType sPContentType5 = targetWebContentTypes.AddOrUpdateContentType(name1, xML, str1, false, out xmlNodes1);
								if (xmlNodes1 != null && xmlNodes1.ChildNodes.Count > 0)
								{
									foreach (XmlNode childNode in xmlNodes1.ChildNodes)
									{
										strs.Add(childNode.Attributes["ContentTypeName"].Value);
									}
								}
								this.CopyContentTypeResourceFolderContent(sPContentType2, sPContentType5, null);
								if ((base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations) && !base.WorkflowMappings.ContainsKey(sPContentType2.ContentTypeID.ToString()))
								{
									base.WorkflowMappings.Add(sPContentType3.ContentTypeID.ToString(), sPContentType5.ContentTypeID.ToString());
								}
								if (!base.SharePointOptions.CheckResults)
								{
									logItem.Status = ActionOperationStatus.Completed;
								}
								else
								{
									base.CompareNodes(sPContentType3, sPContentType5, logItem);
								}
								if (base.SharePointOptions.Verbose)
								{
									logItem.SourceContent = sPContentType2.XML;
									logItem.TargetContent = sPContentType5.XML;
								}
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
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (!exception.Message.Contains("ML-SPCT-ROS"))
						{
							logItem.Exception = exception;
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Details = exception.StackTrace;
							logItem.Information = string.Concat("Exception thrown: ", exception.Message);
							logItem.SourceContent = (sPContentType3 != null ? sPContentType3.XML : string.Empty);
						}
						else
						{
							logItem.Exception = exception;
							logItem.Status = ActionOperationStatus.Warning;
							logItem.Details = exception.StackTrace;
							logItem.Information = string.Concat("Warning thrown: ", exception.Message);
							logItem.SourceContent = (sPContentType3 != null ? sPContentType3.XML : string.Empty);
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
			if (strs.Count > 0)
			{
				foreach (SPContentType sPContentType6 in sPContentTypes)
				{
					LogItem stackTrace = null;
					try
					{
						try
						{
							foreach (string str2 in strs)
							{
								if (sPContentType6.Name != str2)
								{
									continue;
								}
								stackTrace = new LogItem("Updating Content Type", sPContentType6.Name, sourceWebContentTypes.ParentWeb.DisplayUrl, targetWebContentTypes.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(stackTrace);
								XmlNode xmlNodes2 = null;
								SPContentTypeCollection sPContentTypeCollections = targetWebContentTypes;
								string name2 = sPContentType6.Name;
								string xML1 = sPContentType6.XML;
								if (sPContentType6.ParentContentType == null)
								{
									str = null;
								}
								else
								{
									str = sPContentType6.ParentContentType.Name;
								}
								SPContentType sPContentType7 = sPContentTypeCollections.AddOrUpdateContentType(name2, xML1, str, false, out xmlNodes2);
								strs.Remove(str2);
								stackTrace.Status = ActionOperationStatus.Completed;
								if (!base.SharePointOptions.Verbose)
								{
									break;
								}
								stackTrace.SourceContent = sPContentType6.XML;
								stackTrace.TargetContent = sPContentType7.XML;
								break;
							}
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							if (!exception2.Message.Contains("ML-SPCT-ROS"))
							{
								stackTrace.Exception = exception2;
								stackTrace.Status = ActionOperationStatus.Failed;
								stackTrace.Details = exception2.StackTrace;
								stackTrace.Information = string.Concat("Exception thrown: ", exception2.Message);
								stackTrace.SourceContent = sPContentType6.XML;
							}
							else
							{
								stackTrace.Exception = exception2;
								stackTrace.Status = ActionOperationStatus.Warning;
								stackTrace.Details = exception2.StackTrace;
								stackTrace.Information = string.Concat("Warning thrown: ", exception2.Message);
								stackTrace.SourceContent = sPContentType6.XML;
							}
						}
					}
					finally
					{
						if (stackTrace != null)
						{
							base.FireOperationFinished(stackTrace);
						}
					}
				}
			}
			this.s_ctTransformer.EndTransformation(this, sourceWebContentTypes, targetWebContentTypes, this.Options.Transformers);
		}

		private void CopyWebPartsOnPage(SPFolderBasic sourceResFolder, SPFolderBasic targetResFolder)
		{
			foreach (SPFile file in sourceResFolder.Files)
			{
				if (file.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				SPWebPartPage webPartPage = (new SPWebPartPage()).GetWebPartPage(file.Web, file.ServerRelativeUrl, this);
				if (webPartPage.WebParts.Count <= 1)
				{
					continue;
				}
				SPFile item = targetResFolder.Files[file.Name];
				SPWebPartPage sPWebPartPage = (new SPWebPartPage()).GetWebPartPage(item.Web, item.ServerRelativeUrl, this);
				ThreadedOperationDelegate threadedOperationDelegate = new ThreadedOperationDelegate(this.CopyContentTypeFormWebParts);
				object[] objArray = new object[] { file, item, webPartPage, sPWebPartPage };
				TaskDefinition taskDefinition = new TaskDefinition(threadedOperationDelegate, objArray);
				base.ThreadManager.QueueBufferedTask(base.GetWebPartCopyBufferKey(targetResFolder.Web), taskDefinition);
				base.ThreadManager.QueueBufferedTask("RunActionEndReached", taskDefinition);
			}
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(this.s_ctTransformer);
			return supportedDefinitions;
		}

		private SPContentType GetTargetContentTypeBySourceContentTypeForWebLevel(SPContentType sourceContentType, SPContentTypeCollection targetContentTypes)
		{
			if (sourceContentType == null)
			{
				return null;
			}
			SPContentType item = targetContentTypes[sourceContentType.ContentTypeID] ?? targetContentTypes.GetContentTypeByName(sourceContentType.Name);
			return item;
		}

		private bool IsExtraContentTypesRemovalAllowed(SPList sourceList, SPList targetList)
		{
			if (sourceList.BaseTemplate != targetList.BaseTemplate)
			{
				return false;
			}
			if (this.IsSameVersionsMigration(sourceList, targetList))
			{
				return true;
			}
			return this.IsSameExperienceMigration(sourceList, targetList);
		}

		private bool IsSameExperienceMigration(SPList sourceList, SPList targetList)
		{
			if (!sourceList.Adapter.SharePointVersion.IsSharePoint2010 || !targetList.Adapter.SharePointVersion.IsSharePoint2013)
			{
				return false;
			}
			return targetList.ParentWeb.ExperienceVersion == SharePoint2013ExperienceVersion.SP2010;
		}

		private bool IsSameVersionsMigration(SPList sourceList, SPList targetList)
		{
			SharePointVersion sharePointVersion = sourceList.Adapter.SharePointVersion;
			SharePointVersion sharePointVersion1 = targetList.Adapter.SharePointVersion;
			if (!(sharePointVersion == sharePointVersion1) && (!sharePointVersion.IsSharePoint2013OrLater || !sharePointVersion1.IsSharePoint2013OrLater))
			{
				return false;
			}
			return sourceList.ParentWeb.ExperienceVersion == targetList.ParentWeb.ExperienceVersion;
		}

		private void RemoveExtraContentTypes(SPList sourceList, SPList targetList, List<SPContentType> sourceContentTypes)
		{
			LogItem logItem = null;
			try
			{
				try
				{
					if (this.IsExtraContentTypesRemovalAllowed(sourceList, targetList))
					{
						List<SPContentType> sPContentTypes = new List<SPContentType>();
						foreach (SPContentType contentType in targetList.ContentTypes)
						{
							if (contentType.ContentTypeXML.IsReadOnly || contentType.ContentTypeXML.GetAttributeValueAsBoolean("Hidden"))
							{
								continue;
							}
							if (sourceContentTypes.Any<SPContentType>((SPContentType item) => item.Name.Equals(contentType.Name, StringComparison.InvariantCultureIgnoreCase)))
							{
								continue;
							}
							sPContentTypes.Add(contentType);
						}
						if (sPContentTypes.Count > 0)
						{
							string str = string.Join(", ", (
								from ct in sPContentTypes
								select ct.Name).ToArray<string>());
							logItem = new LogItem("Remove Extra Content Types", str, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							targetList.ContentTypes.DeleteContentTypes(sPContentTypes);
							logItem.Information = string.Format("Removed below extra content types from target:{0}{1}", Environment.NewLine, str);
							logItem.Status = ActionOperationStatus.Completed;
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (logItem != null)
					{
						logItem.Exception = exception;
						logItem.Details = exception.StackTrace;
						logItem.Information = string.Format("Error occurred while removing extra content types from target list. Error: {0}", exception.Message);
						logItem.Status = ActionOperationStatus.Failed;
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

		private void ReorderContentTypes(SPList sourceList, SPList targetList, List<string> sContentTypeNames, bool isVisibleContentTypesFetched)
		{
			string[] strArrays;
			LogItem logItem = null;
			try
			{
				try
				{
					strArrays = (!isVisibleContentTypesFetched ? (
						from contentType in (IEnumerable<SPContentType>)sourceList.ContentTypes.ToArray()
						where sContentTypeNames.Contains(contentType.Name)
						select contentType.Name).ToArray<string>() : sContentTypeNames.ToArray());
					if ((int)strArrays.Length > 1)
					{
						logItem = new LogItem("Setting Content Type Order", "ContentTypes", sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						targetList.ReorderContentTypes(strArrays);
						logItem.Status = ActionOperationStatus.Completed;
					}
				}
				catch (ArgumentException argumentException1)
				{
					ArgumentException argumentException = argumentException1;
					if (logItem != null)
					{
						logItem.Information = argumentException.Message;
						logItem.Status = ActionOperationStatus.Warning;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (logItem != null)
					{
						logItem.Exception = exception;
						logItem.Details = exception.StackTrace;
						logItem.Information = string.Concat("Exception thrown: ", exception.Message);
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

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.CopyWebContentTypes(oParams[0] as SPContentTypeCollection, oParams[1] as SPContentTypeCollection);
		}
	}
}