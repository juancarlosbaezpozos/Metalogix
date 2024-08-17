using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Metalogix.Actions.AllowsSameSourceTarget(true)]
	[CmdletEnabled(true, "Copy-MLSharePointMySites", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Incrementable(true, "Paste MySites Incrementally")]
	[MenuText("1:Paste MySites...{0-Paste} > Admin Mode")]
	[Name("Paste MySites")]
	[Shortcut(ShortcutAction.Paste)]
	[SourceType(typeof(SPBaseServer), true)]
	[SubActionTypes(typeof(PasteNavigationAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetType(typeof(SPBaseServer), true)]
	public class PasteMySitesAction : PasteSiteCollectionBaseAction<PasteMySiteOptions>
	{
		private List<string> m_userProfilePropertiesToIgnore = new List<string>()
		{
			"UserProfile_GUID",
			"AccountName",
			"PersonalSpace",
			"UserName",
			"SID",
			"ADGuid",
			"SPS-ProxyAddresses",
			"SPS-ResourceSID",
			"SPS-SavedSID",
			"SPS-PersonalSiteInstantiationState",
			"SPS-LastKeywordAdded",
			"SPS-FeedIdentifier"
		};

		private List<string> m_MappingProfilePropertiesToIgnore = new List<string>()
		{
			"WorkPhone",
			"Office",
			"Manager",
			"Department",
			"PublicSiteRedirect",
			"CellPhone",
			"HomePhone",
			"Assistant"
		};

		public PasteMySitesAction()
		{
		}

		protected override void AppendPowerShellParameters(StringBuilder sb, object cmdletOptions)
		{
			if (base.SharePointOptions.ExclusionsExist)
			{
				sb.Append(" -MySitesToExclude ");
				IEnumerator<string> enumerator = base.SharePointOptions.MySitesToExclude.GetEnumerator();
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

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!(sourceSelections is MySitesServerNodeCollection) || !base.AppliesTo(sourceSelections, targetSelections) || sourceSelections.Count <= 0 || !(sourceSelections[0] is SPBaseServer) || targetSelections.Count <= 0)
			{
				return false;
			}
			if (targetSelections[0] is SPServer)
			{
				return true;
			}
			return targetSelections[0] is SPTenantMySiteHost;
		}

		private string ChangeWebTemplateConfigForSP2016(string sourceXML)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sourceXML);
			XmlAttribute itemOf = xmlDocument.DocumentElement.Attributes["WebTemplateConfig"];
			if (itemOf != null && itemOf.Value.Equals("0", StringComparison.OrdinalIgnoreCase))
			{
				itemOf.Value = "2";
			}
			return xmlDocument.OuterXml;
		}

		public void CopyMySite(SPWeb sourceWeb, SPBaseServer targetServer)
		{
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				return;
			}
			LogItem logItem = null;
			SPSite sPSite = null;
			string siteXML = null;
			logItem = new LogItem("Copy MySite", sourceWeb.Name, sourceWeb.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					base.SharePointOptions.LanguageCode = sourceWeb.Language.ToString();
					try
					{
						siteXML = base.GetSiteXML(sourceWeb, targetServer);
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
						return;
					}
					if (siteXML != null)
					{
						string str = this.ExtractUserName(sourceWeb);
						string empty = string.Empty;
						string email = string.Empty;
						string loginName = string.Empty;
						foreach (SPUser siteUser in (IEnumerable<SecurityPrincipal>)sourceWeb.SiteUsers)
						{
							if (siteUser.LoginName.Substring(siteUser.LoginName.ToLower().LastIndexOf("\\") + 1, siteUser.LoginName.Length - (siteUser.LoginName.LastIndexOf("\\") + 1)) != str.ToLower())
							{
								continue;
							}
							empty = siteUser.LoginName;
							break;
						}
						string str1 = "Username_CollisionDomain";
						string str2 = null;
						SPWebTemplateCollection templates = null;
						this.ExtractMySiteData(out str1, out str2);
						if (targetServer.Languages[base.SharePointOptions.LanguageCode] != null)
						{
							templates = targetServer.Languages[sourceWeb.Language.ToString()].Templates;
							siteXML = base.MapWebTemplate(siteXML, sourceWeb.Template.Name, templates, true);
							SPUser tag = null;
							bool flag = false;
							ListSummaryItem mapListSummaryItem = SPGlobalMappings.GetMapListSummaryItem(empty, targetServer.Adapter.SharePointVersion.IsSharePointOnline);
							if (mapListSummaryItem != null && mapListSummaryItem.Target != null && mapListSummaryItem.Target.Tag != null)
							{
								tag = mapListSummaryItem.Target.Tag as SPUser;
								if (tag != null)
								{
									loginName = tag.LoginName;
									if (!string.IsNullOrEmpty(loginName) && !loginName.Equals(empty, StringComparison.InvariantCultureIgnoreCase))
									{
										if (loginName.LastIndexOf("\\", StringComparison.Ordinal) != -1)
										{
											email = loginName.Substring(loginName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
											flag = !string.IsNullOrEmpty(email);
										}
										if (targetServer is SPTenant)
										{
											email = tag.Email;
											flag = !string.IsNullOrEmpty(email);
										}
									}
								}
							}
							if (!flag)
							{
								email = str;
								loginName = empty;
							}
							siteXML = this.MapMySiteName(siteXML, tag, email, loginName, str1, targetServer);
							if (!base.CheckForAbort())
							{
								if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2007 && targetServer.Adapter.SharePointVersion.IsSharePoint2010OrLater && (base.SharePointOptions.CopyListOOBWorkflowAssociations || base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations))
								{
									siteXML = base.AddLegacyFeaturesToSiteCollection(siteXML);
								}
								AddSiteCollectionOptions addSiteCollectionOption = new AddSiteCollectionOptions()
								{
									Overwrite = base.SharePointOptions.OverwriteSites,
									SelfServiceCreateMode = base.SharePointOptions.SelfServiceCreateMode,
									MergeFeatures = base.SharePointOptions.MergeSiteFeatures,
									CopyFeatures = base.SharePointOptions.CopySiteFeatures,
									TenantPersonalSiteCreationRetryCount = SharePointConfigurationVariables.TenantPersonalSiteCreationRetryCount,
									TenantPersonalSiteCreationWaitInterval = SharePointConfigurationVariables.TenantPersonalSiteCreationWaitInterval
								};
								if (targetServer.Adapter.SharePointVersion.IsSharePoint2016)
								{
									siteXML = this.ChangeWebTemplateConfigForSP2016(siteXML);
								}
								sPSite = targetServer.Sites.AddSiteCollection(base.SharePointOptions.WebApplicationName, siteXML, addSiteCollectionOption);
								try
								{
									base.LinkCorrector.AddMapping(sourceWeb.DisplayUrl, sPSite.DisplayUrl);
									base.LinkCorrector.AddMapping(sourceWeb.ServerRelativeUrl, sPSite.ServerRelativeUrl);
								}
								catch (Exception exception2)
								{
									Exception exception1 = exception2;
									LogItem logItem1 = logItem;
									logItem1.Details = string.Concat(logItem1.Details, "\nWarning: Link correction was not enabled due to error: ", exception1.Message);
								}
								string empty1 = string.Empty;
								LogItem logItem2 = new LogItem("Copying User Profile Properties", sourceWeb.Name, sourceWeb.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem2);
								try
								{
									try
									{
										if (base.SharePointOptions.WebApplicationName != str2)
										{
											logItem2.Status = ActionOperationStatus.Skipped;
											logItem2.Information = "User profile copying has been skipped as the target Web Application does not match the MySite Web Application on that server.";
										}
										else
										{
											string userProfiles = sourceWeb.Adapter.Reader.GetUserProfiles(sourceWeb.DisplayUrl, str, out empty1);
											if (string.IsNullOrEmpty(empty1))
											{
												try
												{
													userProfiles = this.LinkCorrectProfileXml(userProfiles, tag, targetServer.Adapter.SharePointVersion.IsSharePointOnline);
												}
												catch (Exception exception4)
												{
													Exception exception3 = exception4;
													empty1 = string.Format("Unable to Link Correct Profile XML: {0}{1}", exception3.Message, Environment.NewLine);
												}
												if (string.IsNullOrEmpty(empty1))
												{
													empty1 = (!(targetServer is SPTenant) ? targetServer.Adapter.Writer.SetUserProfile(sPSite.DisplayUrl, email, userProfiles, true) : targetServer.Adapter.Writer.SetUserProfile(sPSite.DisplayUrl, loginName, userProfiles, true));
												}
											}
											logItem2.Information = (string.IsNullOrEmpty(empty1) ? string.Empty : "Issues encountered in copying User Profile. See Details.");
											logItem2.Details = (string.IsNullOrEmpty(empty1) ? string.Empty : empty1);
											logItem2.Status = (string.IsNullOrEmpty(empty1) ? ActionOperationStatus.Completed : ActionOperationStatus.Warning);
										}
									}
									catch (Exception exception5)
									{
										logItem2.Exception = exception5;
										logItem2.Status = ActionOperationStatus.Failed;
									}
								}
								finally
								{
									base.FireOperationFinished(logItem2);
								}
								base.CompareNodes(sourceWeb, sPSite, logItem);
								if (base.SharePointOptions.Verbose)
								{
									logItem.SourceContent = sourceWeb.XML;
									logItem.TargetContent = sPSite.XML;
								}
								if (base.SharePointOptions.CopyReferencedManagedMetadata)
								{
									base.EnsureManagedMetadataExistence(sourceWeb.AvailableColumns, sourceWeb, sPSite, base.SharePointOptions.ResolveManagedMetadataByName);
								}
								base.CopySiteCollectionInformation(sourceWeb, sPSite, true, true);
							}
							else
							{
								sourceWeb.Dispose();
								return;
							}
						}
						else
						{
							object[] objArray = new object[] { sourceWeb.Language.ToString(), sourceWeb.Node.Url, targetServer.Name, Environment.NewLine };
							string str3 = string.Format("The target personal site cannot be created as the language '{0}' from the source site '{1}' could not be found on the target farm '{2}'{3}", objArray);
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Information = str3;
							return;
						}
					}
					else
					{
						logItem.Operation = "Skipping MySite";
						logItem.Status = ActionOperationStatus.Skipped;
						logItem.Information = "MySite skipped by transformation";
						return;
					}
				}
				catch (Exception exception6)
				{
					logItem.Exception = exception6;
					if (base.SharePointOptions.Verbose)
					{
						logItem.SourceContent = siteXML;
					}
					return;
				}
				if (!base.CheckForAbort())
				{
					if (base.SharePointOptions.RunNavigationStructureCopy)
					{
						base.SourceTargetNavCopyMap.Add(sourceWeb, sPSite);
					}
					base.CopySubSiteAndListContent(sourceWeb, sPSite, true, true);
					base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyRoleAssignmentsAction).Name, sourceWeb.Name), false, false);
					return;
				}
				sourceWeb.Dispose();
				sPSite.Dispose();
				return;
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		public void ExtractMySiteData(out string sMySiteNamingConvention, out string sMySiteHostWebApp)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(base.SharePointOptions.MySiteData);
				XmlNode xmlNodes = xmlDocument.FirstChild.SelectSingleNode("//MySiteData");
				if (xmlNodes == null)
				{
					sMySiteNamingConvention = "Username_CollisionDomain";
					sMySiteHostWebApp = "";
				}
				else
				{
					if (xmlNodes.Attributes["PersonalSiteFormat"] == null)
					{
						sMySiteNamingConvention = "Username_CollisionDomain";
					}
					else
					{
						sMySiteNamingConvention = xmlNodes.Attributes["PersonalSiteFormat"].Value;
					}
					if (xmlNodes.Attributes["MySiteHostUrl"] == null)
					{
						sMySiteHostWebApp = "";
					}
					else
					{
						sMySiteHostWebApp = xmlNodes.Attributes["MySiteHostUrl"].Value;
					}
				}
			}
			catch
			{
				sMySiteNamingConvention = "Username_CollisionDomain";
				sMySiteHostWebApp = "";
			}
		}

		public string ExtractUserName(SPWeb sourceWeb)
		{
			string str;
			string displayUrl = sourceWeb.DisplayUrl;
			try
			{
				int num = displayUrl.LastIndexOf("/") + 1;
				string userFromProfile = displayUrl.Substring(num, displayUrl.Length - num);
				if (userFromProfile.Contains("_"))
				{
					userFromProfile = sourceWeb.Adapter.Reader.GetUserFromProfile() ?? userFromProfile.Substring(userFromProfile.LastIndexOf("_") + 1, userFromProfile.Length - (userFromProfile.LastIndexOf("_") + 1));
				}
				str = userFromProfile;
			}
			catch
			{
				str = null;
			}
			return str;
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			List<string> strs = new List<string>()
			{
				"CopySubSiteAndListContent",
				"LanguageCode",
				"Name",
				"Description",
				"OwnerLogin",
				"SecondaryOwnerLogin",
				"ContentDatabaseName",
				"CopyMasterPageGallery",
				"CopyListTemplateGallery",
				"CopyMasterPages",
				"CopyPageLayouts",
				"CopyOtherResources",
				"CopySiteAdmins",
				"CopySiteQuota",
				"QuotaMaximum",
				"QuotaWarning",
				"StorageQuota",
				"ResourceQuota",
				"SiteCollectionAdministrators",
				"QuotaID",
				"WebTemplateName",
				"ExperienceVersion",
				"IsHostHeader",
				"HostHeaderURL"
			};
			PropertyInfo[] optionParameters = base.GetOptionParameters(cmdletOptions);
			for (int i = 0; i < (int)optionParameters.Length; i++)
			{
				PropertyInfo propertyInfo = optionParameters[i];
				if (!strs.Contains(propertyInfo.Name))
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		public static void InitializeMappings(PasteSiteOptions options, SPBaseServer sourceServer, SPWebTemplateCollection targetTemplates)
		{
			SPWebTemplate sPWebTemplate = null;
			foreach (SPWebTemplate template in sourceServer.Languages[0].Templates)
			{
				if (options.WebTemplateMappingTable.ContainsKey(template.Name) || template.ID == -1)
				{
					continue;
				}
				sPWebTemplate = targetTemplates.MapWebTemplate(template) ?? targetTemplates.Find(1, 1);
				options.WebTemplateMappingTable.Add(template.Name, sPWebTemplate.Name);
			}
		}

		public string LinkCorrectProfileXml(string sProfileXml, SPUser targetUser, bool isTargetSharePointOnline)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sProfileXml);
			XmlNode firstChild = xmlDocument.FirstChild;
			if (targetUser != null)
			{
				string[] strArrays = targetUser.Name.Split(new char[] { ' ' });
				if ((int)strArrays.Length > 0)
				{
					string str = strArrays[(int)strArrays.Length - 1];
					string str1 = strArrays[0];
					string email = targetUser.Email;
					if (firstChild.SelectSingleNode("./FirstName") != null)
					{
						firstChild.SelectSingleNode("./FirstName").FirstChild.InnerText = str1;
					}
					if (firstChild.SelectSingleNode("./LastName") != null)
					{
						firstChild.SelectSingleNode("./LastName").FirstChild.InnerText = str;
					}
					if (firstChild.SelectSingleNode("./PreferredName") != null)
					{
						firstChild.SelectSingleNode("./PreferredName").FirstChild.InnerText = targetUser.Name;
					}
					if (firstChild.SelectSingleNode("./WorkEmail") != null)
					{
						firstChild.SelectSingleNode("./WorkEmail").FirstChild.InnerText = email;
					}
				}
			}
			foreach (string mUserProfilePropertiesToIgnore in this.m_userProfilePropertiesToIgnore)
			{
				XmlNode xmlNodes = firstChild.SelectSingleNode(string.Concat("./", mUserProfilePropertiesToIgnore));
				if (xmlNodes == null)
				{
					continue;
				}
				firstChild.RemoveChild(xmlNodes);
			}
			XmlNode loginName = firstChild.SelectSingleNode(".//Assistant");
			if (loginName != null)
			{
				string innerText = loginName.InnerText;
				if (!string.IsNullOrEmpty(innerText))
				{
					SPUser tag = null;
					foreach (ListSummaryItem globalUserMapping in SPGlobalMappings.GlobalUserMappings)
					{
						if (globalUserMapping == null || !Utils.ConvertClaimStringUserToWinOrFormsUser((globalUserMapping.Source.Tag as SPUser).LoginName).Equals(Utils.ConvertClaimStringUserToWinOrFormsUser(innerText), StringComparison.InvariantCultureIgnoreCase))
						{
							continue;
						}
						tag = globalUserMapping.Target.Tag as SPUser;
						loginName.InnerText = tag.LoginName;
						break;
					}
				}
			}
			foreach (XmlNode xmlNodes1 in firstChild.SelectNodes("//*[@Type='URL']"))
			{
				foreach (XmlNode childNode in xmlNodes1.ChildNodes)
				{
					string innerText1 = childNode.InnerText;
					childNode.InnerText = base.LinkCorrector.CorrectUrl(innerText1);
				}
			}
			foreach (XmlNode xmlNodes2 in firstChild.SelectNodes("//*[@Type='HTML']"))
			{
				foreach (XmlNode childNode1 in xmlNodes2.ChildNodes)
				{
					string innerText2 = childNode1.InnerText;
					childNode1.InnerText = base.LinkCorrector.CorrectHtml(innerText2);
				}
			}
			foreach (XmlNode xmlNodes3 in firstChild.SelectNodes(".//QuickLink"))
			{
				xmlNodes3.Attributes["Url"].Value = base.LinkCorrector.CorrectHtml(xmlNodes3.Attributes["Url"].Value);
			}
			return firstChild.OuterXml;
		}

		public string MapMySiteName(string siteXML, SPUser targetUser, string sMySiteUser, string sMySiteUserWithDomain, string sMySiteNamingConvention, SPBaseServer targetServer)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(siteXML);
			XmlAttribute itemOf = xmlDocument.FirstChild.Attributes["ServerRelativeUrl"];
			if (targetUser != null)
			{
				string name = targetUser.Name;
				if (xmlDocument.FirstChild != null && xmlDocument.FirstChild.Attributes["Owner"] != null)
				{
					xmlDocument.FirstChild.Attributes["Owner"].Value = sMySiteUserWithDomain;
				}
				if (xmlDocument.FirstChild != null && xmlDocument.FirstChild.Attributes["Name"] != null)
				{
					xmlDocument.FirstChild.Attributes["Name"].Value = sMySiteUser;
				}
				if (xmlDocument.FirstChild != null && xmlDocument.FirstChild.Attributes["Name"] != null)
				{
					xmlDocument.FirstChild.Attributes["Title"].Value = name;
				}
			}
			sMySiteUserWithDomain = sMySiteUserWithDomain.Replace(".", "_");
			sMySiteUser = sMySiteUser.Replace(".", "_");
			if (targetServer is SPTenant)
			{
				sMySiteUser = sMySiteUser.Replace("@", "_");
				if (xmlDocument.FirstChild != null && xmlDocument.FirstChild.Attributes["IsMySite"] == null)
				{
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("IsMySite");
					xmlAttribute.Value = "True";
					xmlDocument.FirstChild.Attributes.Append(xmlAttribute);
				}
			}
			string str = sMySiteNamingConvention;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "Domain_Username")
				{
					int num = itemOf.Value.LastIndexOf("/") + 1;
					string str2 = itemOf.Value.Substring(num, itemOf.Value.Length - num);
					itemOf.Value = itemOf.Value.Replace(str2, sMySiteUserWithDomain.Replace("\\", "_"));
				}
				else if (str1 == "UNKNOWN")
				{
					int num1 = itemOf.Value.LastIndexOf("/") + 1;
					string str3 = itemOf.Value.Substring(num1, itemOf.Value.Length - num1);
					itemOf.Value = itemOf.Value.Replace(str3, sMySiteUserWithDomain.Replace("\\", "_"));
				}
				else if (str1 == "Username_CollisionDomain")
				{
					bool flag = false;
					foreach (SPSite site in targetServer.Sites)
					{
						if (!site.IsMySiteTemplate || !(site.ServerRelativeUrl.Substring(site.ServerRelativeUrl.LastIndexOf("/") + 1, site.ServerRelativeUrl.Length - (site.ServerRelativeUrl.LastIndexOf("/") + 1)) == sMySiteUser))
						{
							continue;
						}
						int num2 = itemOf.Value.LastIndexOf("/") + 1;
						string str4 = itemOf.Value.Substring(num2, itemOf.Value.Length - num2);
						itemOf.Value = itemOf.Value.Replace(str4, sMySiteUserWithDomain.Replace("\\", "_"));
						flag = true;
						break;
					}
					if (!flag)
					{
						int num3 = itemOf.Value.LastIndexOf("/") + 1;
						string str5 = itemOf.Value.Substring(num3, itemOf.Value.Length - num3);
						itemOf.Value = itemOf.Value.Replace(str5, sMySiteUser);
					}
				}
				else if (str1 == "Username_CollisionError")
				{
					int num4 = itemOf.Value.LastIndexOf("/") + 1;
					string str6 = itemOf.Value.Substring(num4, itemOf.Value.Length - num4);
					itemOf.Value = itemOf.Value.Replace(str6, sMySiteUser);
				}
			}
			return xmlDocument.OuterXml;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, false);
			SPBaseServer item = target[0] as SPBaseServer;
			SPBaseServer sPBaseServer = source[0] as SPBaseServer;
			if (item == null)
			{
				throw new ArgumentException("Target is not an SPBaseServer");
			}
			try
			{
				base.SharePointOptions.MySiteData = item.Adapter.Reader.GetMySiteData(item.DisplayUrl);
			}
			catch
			{
			}
			base.InitializeWorkflow();
			if (base.SharePointOptions.MySitesToInclude == null)
			{
				base.SharePointOptions.MySitesToInclude = new List<SPSite>();
				for (int i = 0; i < sPBaseServer.Sites.Count; i++)
				{
					try
					{
						SPSite sPSite = (SPSite)sPBaseServer.Sites[i];
						if (sPSite.IsMySiteTemplate && !base.SharePointOptions.MySitesToExclude.Contains(sPSite.DisplayUrl))
						{
							base.SharePointOptions.MySitesToInclude.Add(sPSite);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string str = string.Format("Failed to retrieve site: {0}. Exception: {1}", ((SPSite)sPBaseServer.Sites[i]).DisplayUrl, exception.Message);
						Trace.WriteLine(str);
					}
				}
			}
			PasteSiteCollectionAction pasteSiteCollectionAction = new PasteSiteCollectionAction();
			pasteSiteCollectionAction.Options.SetFromOptions(this.Options);
			base.SubActions.Add(pasteSiteCollectionAction);
			SPWebCollection sPAdHocWebCollection = new SPAdHocWebCollection(null, base.SharePointOptions.MySitesToInclude.ToArray());
			base.SiteTransformerDefinition.BeginTransformation(pasteSiteCollectionAction, sPAdHocWebCollection, item.Sites, pasteSiteCollectionAction.Options.Transformers);
			foreach (SPWeb mySitesToInclude in base.SharePointOptions.MySitesToInclude)
			{
				if (base.SharePointOptions.ForceRefresh)
				{
					base.RefreshSiteCollection(mySitesToInclude, item);
				}
				if (base.SharePointOptions.LanguageCode == null)
				{
					SPLanguage sPLanguage = item.Languages[mySitesToInclude.Language.ToString()];
					if (sPLanguage == null && item.Languages.Count > 0)
					{
						sPLanguage = item.Languages[0];
					}
					if (sPLanguage != null)
					{
						base.SharePointOptions.LanguageCode = sPLanguage.LCID.ToString();
					}
				}
				this.CopyMySite(mySitesToInclude, item);
			}
			base.SiteTransformerDefinition.EndTransformation(pasteSiteCollectionAction, sPAdHocWebCollection, item.Sites, pasteSiteCollectionAction.Options.Transformers);
			if (base.SharePointOptions.RunNavigationStructureCopy)
			{
				PasteNavigationAction pasteNavigationAction = new PasteNavigationAction();
				pasteNavigationAction.SharePointOptions.CopyCurrentNavigation = base.SharePointOptions.CopyCurrentNavigation;
				pasteNavigationAction.SharePointOptions.CopyGlobalNavigation = base.SharePointOptions.CopyGlobalNavigation;
				pasteNavigationAction.SharePointOptions.Recursive = false;
				pasteNavigationAction.SharePointOptions.TaskCollection = base.SharePointOptions.TaskCollection;
				base.SubActions.Add(pasteNavigationAction);
				object[] sourceTargetNavCopyMap = new object[] { base.SourceTargetNavCopyMap, false };
				pasteNavigationAction.RunAsSubAction(sourceTargetNavCopyMap, new ActionContext(null, item), null);
			}
			base.StartCommonWorkflowBufferedTasks();
			base.ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
			base.ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
		}
	}
}