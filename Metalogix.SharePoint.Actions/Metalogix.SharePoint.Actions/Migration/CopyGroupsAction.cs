using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointGroups", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Group.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste Site Objects {0-Paste} > Groups...")]
	[Name("Paste Groups")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SubActionTypes(typeof(CopyUsersAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class CopyGroupsAction : PasteAction<CopyGroupsOptions>
	{
		protected static TransformerDefinition<SPGroup, CopyGroupsAction, SPGroupCollection, SPGroupCollection> s_definition;

		static CopyGroupsAction()
		{
			CopyGroupsAction.s_definition = new TransformerDefinition<SPGroup, CopyGroupsAction, SPGroupCollection, SPGroupCollection>("SharePoint Groups", false);
		}

		public CopyGroupsAction()
		{
		}

		private void AddGroupToAzureManifest(SPGroup mappedGroup, SPWeb targetWeb, IUploadManager uploadManager, Dictionary<ManifestGroup, string> azureGroupOwnerMappings)
		{
			int num = 0;
			if (uploadManager.GetUserOrGroupIDByName(mappedGroup.PrincipalName) == 0)
			{
				ManifestGroup manifestGroup = new ManifestGroup()
				{
					Name = mappedGroup.PrincipalName,
					Description = mappedGroup.Description,
					OwnerIsUser = mappedGroup.OwnerIsUser,
					RequestToJoinLeaveEmailSetting = mappedGroup.RequestToJoinLeaveEmailSetting,
					OnlyAllowMembersViewMembership = mappedGroup.OnlyAllowMembersViewMembership
				};
				foreach (SPUser user in (IEnumerable<SecurityPrincipal>)mappedGroup.Users)
				{
					ManifestGroupMember manifestGroupMember = new ManifestGroupMember();
					int userOrGroupIDByName = 0;
					string str = base.MapPrincipal(user.PrincipalName);
					userOrGroupIDByName = uploadManager.GetUserOrGroupIDByName(str);
					if (userOrGroupIDByName == 0)
					{
						continue;
					}
					manifestGroupMember.UserId = userOrGroupIDByName;
					manifestGroup.GroupMembers.Add(manifestGroupMember);
				}
				num = uploadManager.AddGroup(manifestGroup);
				base.PrincipalMappings.AddSafe(mappedGroup.PrincipalName, manifestGroup.Name);
				azureGroupOwnerMappings.Add(manifestGroup, mappedGroup.OwnerName);
			}
		}

		public void CopyGroups(SPGroupCollection sourceGroups, SPWeb targetWeb, SPWeb sourceWeb = null)
		{
			if (sourceGroups == null)
			{
				throw new Exception("Source group collection cannot be null");
			}
			SPGroup[] item = new SPGroup[sourceGroups.Count];
			for (int i = 0; i < sourceGroups.Count; i++)
			{
				item[i] = (SPGroup)sourceGroups[i];
			}
			this.CopyGroups(item, targetWeb, null, sourceWeb);
		}

		public void CopyGroups(SPGroup[] sourceGroups, SPWeb targetWeb, IUploadManager uploadManager = null, SPWeb sourceWeb = null)
		{
			if (sourceGroups == null)
			{
				throw new Exception("Source group collection cannot be null");
			}
			if (targetWeb == null)
			{
				throw new Exception("Target web cannot be null");
			}
			Dictionary<SPGroup, string> sPGroups = new Dictionary<SPGroup, string>();
			Dictionary<ManifestGroup, string> manifestGroups = new Dictionary<ManifestGroup, string>();
			SPGroupCollection groupsForMigration = this.GetGroupsForMigration(sourceGroups);
			CopyGroupsAction.s_definition.BeginTransformation(this, groupsForMigration, targetWeb.Groups, this.Options.Transformers);
			bool isRootWeb = sourceGroups[0].ParentWeb.IsRootWeb;
			foreach (SPGroup sPGroup in (IEnumerable<SecurityPrincipal>)groupsForMigration)
			{
				if (base.CheckForAbort())
				{
					break;
				}
				if (sPGroup == null || base.PrincipalMappings.ContainsKey(sPGroup.Name) && uploadManager == null)
				{
					continue;
				}
				if (sourceWeb == null || isRootWeb || this.IsGroupContainsPermissions(sPGroup, sourceWeb))
				{
					LogItem logItem = null;
					string xML = "";
					try
					{
						try
						{
							SPGroup item = null;
							float similarity = 0f;
							SPGroup mappableGroup = null;
							SPGroup sPGroup1 = null;
							bool flag = false;
							try
							{
								logItem = new LogItem("Adding SharePoint group", sPGroup.Name, sPGroup.Name, targetWeb.DisplayUrl, ActionOperationStatus.Running);
								if (uploadManager != null)
								{
									logItem.Operation = "Processing SharePoint group";
								}
								xML = sPGroup.XML;
								sPGroup1 = CopyGroupsAction.s_definition.Transform(sPGroup, this, groupsForMigration, targetWeb.Groups, this.Options.Transformers);
								if (sPGroup1 != null)
								{
									mappableGroup = this.GetMappableGroup(sPGroup1, targetWeb, uploadManager);
									SPGroup item1 = targetWeb.Groups[mappableGroup.PrincipalName] as SPGroup;
									if (item1 != null)
									{
										similarity = item1.GetSimilarity(mappableGroup);
										if (base.SharePointOptions.MapGroupsByName || similarity == 1f)
										{
											lock (base.PrincipalMappings)
											{
												if (!base.PrincipalMappings.ContainsKey(sPGroup.Name))
												{
													base.PrincipalMappings.Add(sPGroup.Name, item1.Name);
												}
											}
											item = item1;
										}
									}
									if (item == null && !base.SharePointOptions.MapGroupsByName)
									{
										foreach (SPGroup group in (IEnumerable<SecurityPrincipal>)targetWeb.Groups)
										{
											similarity = group.GetSimilarity(mappableGroup);
											if (similarity != 1f)
											{
												continue;
											}
											item = group;
											lock (base.PrincipalMappings)
											{
												if (!base.PrincipalMappings.ContainsKey(sPGroup.Name))
												{
													base.PrincipalMappings.Add(sPGroup.Name, item.Name);
												}
												break;
											}
										}
									}
								}
								else
								{
									flag = true;
									continue;
								}
							}
							finally
							{
								if (flag || item != null && (!base.SharePointOptions.MapGroupsByName || !base.SharePointOptions.OverwriteGroups || (double)similarity >= 1))
								{
									if (flag)
									{
										logItem.Information = "Group skipped due to the result of a transformation";
									}
									logItem.Status = ActionOperationStatus.Skipped;
								}
								else
								{
									base.FireOperationStarted(logItem);
								}
							}
							if (item == null || base.SharePointOptions.OverwriteGroups && (double)similarity < 1)
							{
								string name = mappableGroup.Name;
								if (!base.SharePointOptions.OverwriteGroups)
								{
									item = (SPGroup)targetWeb.Groups[mappableGroup.Name];
									int num = 1;
									while (item != null)
									{
										mappableGroup.SetName(string.Concat(name, "(", num.ToString(), ")"));
										item = (SPGroup)targetWeb.Groups[mappableGroup.Name];
										num++;
									}
								}
								lock (base.PrincipalMappings)
								{
									if (!base.PrincipalMappings.ContainsKey(name))
									{
										base.PrincipalMappings.Add(name, mappableGroup.Name);
									}
								}
								if (mappableGroup.Name != name)
								{
									logItem.ItemName = mappableGroup.Name;
								}
								if (uploadManager == null)
								{
									XmlNode xmlNodes = null;
									SPGroup sPGroup2 = targetWeb.Groups.AddOrUpdateGroup(mappableGroup, out xmlNodes);
									LogItem licenseDataUsed = logItem;
									licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + SPObjectSizes.GetObjectSize(sPGroup2);
									logItem.AddCompletionDetail(Resources.Migration_Detail_GroupsCopied, (long)1);
									if (!mappableGroup.OwnerIsUser && sPGroup2.OwnerName != sPGroup1.OwnerName)
									{
										sPGroups.Add(sPGroup2, sPGroup1.OwnerName);
									}
									if (sPGroup.Owner == null)
									{
										logItem.Status = ActionOperationStatus.Different;
										logItem.Information = "The owner could not be found on the source. This is likely because this group is owned by a group which the migrating user does not have the rights to view.\nThe group's owner has been set to itself.";
									}
									else
									{
										logItem.Status = ActionOperationStatus.Completed;
									}
									if (xmlNodes != null && xmlNodes.Attributes["Failures"] != null && xmlNodes.Attributes["Failures"].Value != "0")
									{
										logItem.Status = ActionOperationStatus.Warning;
										logItem.Information = string.Concat("Some users could not be copied. See the Details tab for more information.\n", logItem.Information);
										StringBuilder stringBuilder = new StringBuilder();
										foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes(".//Failure"))
										{
											string value = xmlNodes1.Attributes["LoginName"].Value;
											stringBuilder.Append(string.Concat("Could not add user: ", value, "\n"));
										}
										logItem.Details = stringBuilder.ToString();
									}
									if (base.SharePointOptions.Verbose)
									{
										logItem.SourceContent = xML;
										logItem.TargetContent = sPGroup2.XML;
									}
								}
								else
								{
									this.AddGroupToAzureManifest(mappableGroup, targetWeb, uploadManager, manifestGroups);
									logItem.Status = ActionOperationStatus.Completed;
									continue;
								}
							}
							else
							{
								if (uploadManager == null)
								{
									logItem.Status = ActionOperationStatus.Skipped;
								}
								else
								{
									this.AddGroupToAzureManifest(mappableGroup, targetWeb, uploadManager, manifestGroups);
									logItem.Status = ActionOperationStatus.Completed;
								}
								continue;
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							logItem.Exception = exception;
							if (base.SharePointOptions.Verbose)
							{
								logItem.SourceContent = xML;
							}
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Information = string.Concat("Exception thrown: ", exception.Message);
							logItem.Details = exception.StackTrace;
						}
					}
					finally
					{
						if (logItem != null && logItem.Status != ActionOperationStatus.Skipped)
						{
							base.FireOperationFinished(logItem);
						}
					}
				}
				else
				{
					LogItem logItem1 = new LogItem("Adding SharePoint group", sPGroup.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Skipped)
					{
						Information = string.Concat("Skipped group '", sPGroup.Name, "' as it does not have any permission on source site")
					};
					base.FireOperationStarted(logItem1);
					base.FireOperationFinished(logItem1);
				}
			}
			if (uploadManager == null)
			{
				this.SetGroupOwner(sPGroups);
			}
			else
			{
				this.SetAzureGroupOwner(manifestGroups, uploadManager);
			}
			CopyGroupsAction.s_definition.EndTransformation(this, groupsForMigration, targetWeb.Groups, this.Options.Transformers);
		}

		private SPGroupCollection GetGroupsForMigration(SPGroup[] originalGroups)
		{
			if (base.SharePointOptions.GroupExclusions == null)
			{
				return new SPGroupCollection(originalGroups);
			}
			List<SPGroup> sPGroups = new List<SPGroup>();
			SPGroup[] sPGroupArray = originalGroups;
			for (int i = 0; i < (int)sPGroupArray.Length; i++)
			{
				SPGroup sPGroup = sPGroupArray[i];
				XmlNode xmlNodes = base.SharePointOptions.GroupExclusions.SelectSingleNode(string.Format(".//Group[@Name='{0}']", sPGroup.Name));
				if (xmlNodes == null || xmlNodes.Attributes["Excluded"] == null)
				{
					sPGroups.Add(sPGroup);
				}
				else if (!bool.Parse(xmlNodes.Attributes["Excluded"].Value))
				{
					List<SPUser> sPUsers = new List<SPUser>();
					foreach (SPUser user in (IEnumerable<SecurityPrincipal>)sPGroup.Users)
					{
						XmlNode xmlNodes1 = base.SharePointOptions.GroupExclusions.SelectSingleNode(string.Format(".//Group[@Name='{0}']/User[@Name='{1}']", sPGroup.Name, user.LoginName));
						if (xmlNodes1 == null || xmlNodes1.Attributes["Excluded"] == null)
						{
							sPUsers.Add(user);
						}
						else
						{
							if (bool.Parse(xmlNodes1.Attributes["Excluded"].Value))
							{
								continue;
							}
							sPUsers.Add(user);
						}
					}
					sPGroups.Add(new SPGroup(sPGroup, sPUsers.ToArray()));
				}
			}
			return new SPGroupCollection(sPGroups.ToArray());
		}

		private SPGroup GetMappableGroup(SPGroup sourceGroup, SPWeb targetWeb, IUploadManager uploadManager)
		{
			CopyUsersAction copyUsersAction = new CopyUsersAction();
			copyUsersAction.Options.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(copyUsersAction);
			object[] users = new object[] { sourceGroup.Users, targetWeb, uploadManager };
			copyUsersAction.RunAsSubAction(users, new ActionContext(null, targetWeb), null);
			List<SPUser> sPUsers = new List<SPUser>();
			foreach (SPUser user in (IEnumerable<SecurityPrincipal>)sourceGroup.Users)
			{
				if (!base.PrincipalMappings.ContainsKey(user.PrincipalName))
				{
					continue;
				}
				SPUser byLoginName = targetWeb.SiteUsers.GetByLoginName(base.PrincipalMappings[user.PrincipalName]);
				if (byLoginName == null)
				{
					continue;
				}
				sPUsers.Add(byLoginName);
			}
			SPGroup sPGroup = new SPGroup(sourceGroup, sPUsers.ToArray());
			if (!string.IsNullOrEmpty(sourceGroup.OwnerName))
			{
				sPGroup.SetOwner(base.MapPrincipal(sourceGroup.OwnerName), sourceGroup.OwnerIsUser, false);
			}
			return sPGroup;
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(CopyGroupsAction.s_definition);
			return supportedDefinitions;
		}

		private bool IsGroupContainsPermissions(SPGroup group, SPWeb sourceWeb)
		{
			bool flag;
			using (IEnumerator<RoleAssignment> enumerator = sourceWeb.RoleAssignments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RoleAssignment current = enumerator.Current;
					if (!current.Principal.PrincipalName.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPGroupCollection sPGroupCollection;
			SPWeb item = (SPWeb)target[0];
			if (!typeof(SPWeb).IsAssignableFrom(source.CollectionType))
			{
				if (!(source is SPGroupCollection))
				{
					if (!typeof(SecurityPrincipalCollection).IsAssignableFrom(source.GetType()))
					{
						throw new ArgumentException("Source is not a security principal collection");
					}
					SecurityPrincipalCollection securityPrincipalCollection = (SecurityPrincipalCollection)source;
					List<SPGroup> sPGroups = new List<SPGroup>();
					foreach (SecurityPrincipal securityPrincipal in (IEnumerable<SecurityPrincipal>)securityPrincipalCollection)
					{
						SPGroup sPGroup = (SPGroup)PrincipalConverter.ConvertPrincipal(securityPrincipal, typeof(SPGroup));
						if (sPGroup == null)
						{
							continue;
						}
						sPGroups.Add(sPGroup);
					}
					sPGroupCollection = new SPGroupCollection(sPGroups.ToArray());
				}
				else
				{
					sPGroupCollection = (SPGroupCollection)source;
				}
				this.CopyGroups(sPGroupCollection, item, null);
			}
			else
			{
				foreach (SPWeb sPWeb in source)
				{
					this.CopyGroups(sPWeb.Groups, item, sPWeb);
					this.UpdateGroupQuickLaunch(sPWeb, item);
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			SPWeb parentWeb;
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			SPGroup[] item = oParams[0] as SPGroup[];
			if (item == null)
			{
				SPGroup sPGroup = oParams[0] as SPGroup;
				if (sPGroup == null)
				{
					SPGroupCollection sPGroupCollection = oParams[0] as SPGroupCollection;
					if (sPGroupCollection != null)
					{
						item = new SPGroup[sPGroupCollection.Count];
						for (int i = 0; i < sPGroupCollection.Count; i++)
						{
							item[i] = (SPGroup)sPGroupCollection[i];
						}
					}
				}
				else
				{
					item = new SPGroup[] { sPGroup };
				}
			}
			IUploadManager uploadManager = null;
			if ((int)oParams.Length >= 3)
			{
				uploadManager = oParams[2] as IUploadManager;
			}
			if ((int)oParams.Length >= 4)
			{
				this.CopyGroups(item, oParams[1] as SPWeb, uploadManager, oParams[3] as SPWeb);
				return;
			}
			SPGroup[] sPGroupArray = item;
			SPWeb sPWeb = oParams[1] as SPWeb;
			IUploadManager uploadManager1 = uploadManager;
			if (item == null)
			{
				parentWeb = null;
			}
			else
			{
				parentWeb = item[0].ParentWeb;
			}
			this.CopyGroups(sPGroupArray, sPWeb, uploadManager1, parentWeb);
		}

		private void SetAzureGroupOwner(Dictionary<ManifestGroup, string> azureGroupOwnerMappings, IUploadManager uploadManager)
		{
			foreach (ManifestGroup key in azureGroupOwnerMappings.Keys)
			{
				string item = azureGroupOwnerMappings[key];
				int userOrGroupIDByName = 0;
				userOrGroupIDByName = uploadManager.GetUserOrGroupIDByName(base.MapPrincipal(item));
				if (userOrGroupIDByName == 0)
				{
					continue;
				}
				uploadManager.SetGroupOwner(key.GroupId, userOrGroupIDByName);
			}
		}

		private void SetGroupOwner(Dictionary<SPGroup, string> groupOriginalOwnerMappings)
		{
			foreach (SPGroup key in groupOriginalOwnerMappings.Keys)
			{
				string item = groupOriginalOwnerMappings[key];
				if (!base.PrincipalMappings.ContainsKey(item))
				{
					continue;
				}
				string str = base.PrincipalMappings[item];
				if (key.OwnerName == str)
				{
					continue;
				}
				key.SetOwner(str, false, true);
			}
		}
        
	    public void UpdateGroupQuickLaunch(SPWeb sourceWeb, SPWeb targetWeb)
	    {
	        XmlNode xmlNode = sourceWeb.ToXML();
	        XmlNode xmlNode2 = targetWeb.ToXML();
	        string text = (xmlNode.Attributes["AssociateGroups"] != null) ? xmlNode.Attributes["AssociateGroups"].Value : string.Empty;
	        string text2 = (xmlNode2.Attributes["AssociateGroups"] != null) ? xmlNode2.Attributes["AssociateGroups"].Value : string.Empty;
	        if (string.IsNullOrEmpty(text))
	        {
	            return;
	        }
	        string[] array = text.Trim().Split(new char[]
	        {
	            ';'
	        });
	        string text3 = string.Empty;
	        XmlUtility.StringToXmlNode(targetWeb.Groups.ToXML());
	        string[] array2 = array;
	        for (int i = 0; i < array2.Length; i++)
	        {
	            string text4 = array2[i];
	            string text5 = text4.Trim();
	            if (base.PrincipalMappings.ContainsKey(text5))
	            {
	                string text6 = string.Empty;
	                text6 = base.PrincipalMappings[text5];
	                text5 = text6;
	            }
	            IList<string> list = text2.Trim().Split(new char[]
	            {
	                ';'
	            });
	            if (!list.Contains(text5))
	            {
	                text3 = text3 + text5 + ';';
	            }
	        }
	        if (!string.IsNullOrEmpty(text3))
	        {
	            text3 = text2 + text3.TrimEnd(new char[]
	            {
	                ';'
	            });
	            targetWeb.UpdateGroupQuickLaunch(XmlUtility.EncodeIllegalCharactersInXmlAttributeValue(text3));
	        }
	    }

    }
}