using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Azure;
using Metalogix.Core.OperationLog;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointUsers", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Users.ico")]
	[LaunchAsJob(true)]
	[MandatoryTransformers(new Type[] { typeof(MapUsers) })]
	[MenuText("3:Paste Site Objects {0-Paste} > Users...")]
	[Name("Paste Users")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class CopyUsersAction : PasteAction<CopyUserOptions>
	{
		protected static TransformerDefinition<SPUser, CopyUsersAction, SPUserCollection, SPUserCollection> s_definition;

		public bool IsTargetSharePointOnline
		{
			get;
			set;
		}

		static CopyUsersAction()
		{
			CopyUsersAction.s_definition = new TransformerDefinition<SPUser, CopyUsersAction, SPUserCollection, SPUserCollection>("SharePoint Users", false);
		}

		public CopyUsersAction()
		{
		}

		private void AddUserToAzureManifest(SPUser mappedUser, string sourcePrincipalName, SPWeb targetWeb, IUploadManager uploadManager, bool mapMissingUsers, string mapMissingLoginName, bool isValidMapMissingUser)
		{
			LogItem logItem = null;
			if (!string.IsNullOrEmpty(base.TransformationRepository.GetValueForKey("ADUnmappedUsers", mappedUser.LoginName)))
			{
				return;
			}
			try
			{
				try
				{
					logItem = new LogItem("Processing referenced user", string.Empty, sourcePrincipalName, mappedUser.LoginName, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					bool flag = false;
					bool flag1 = false;
					if (!targetWeb.SiteUsers.Contains(mappedUser.LoginName))
					{
						string loginName = mappedUser.LoginName;
						string str = loginName.Substring(loginName.LastIndexOf('|') + 1);
						ADGraphUserResponse aDGraphUserResponse = this.ValidateADUser(str, targetWeb);
						if (aDGraphUserResponse.Success)
						{
							logItem.Details = aDGraphUserResponse.Details;
						}
						else
						{
							bool flag2 = false;
							StringBuilder stringBuilder = new StringBuilder();
							if (aDGraphUserResponse.BadRequestStatus)
							{
								stringBuilder.AppendLine(string.Format("Unable to add user '{0}' due to a BadRequestStatus", loginName));
								stringBuilder.AppendLine(Metalogix.Actions.Properties.Resources.AzureAdGraphCredentialsNotFoundMessage);
							}
							if (aDGraphUserResponse.UserNotFoundStatus)
							{
								stringBuilder.AppendLine(string.Format("User '{0}' not found", loginName));
							}
							if (mapMissingUsers)
							{
								if (!isValidMapMissingUser)
								{
									stringBuilder.AppendLine(string.Format("Unable to map user '{0}' as the specified Map Missing User to '{1}' is not a valid user", loginName, mapMissingLoginName));
								}
								else
								{
									if (!targetWeb.SiteUsers.Contains(mapMissingLoginName))
									{
										string str1 = string.Format("<User LoginName='{0}' />", SecurityElement.Escape(mapMissingLoginName));
										mappedUser = new SPUser(XmlUtility.StringToXmlNode(str1));
									}
									else
									{
										mappedUser = targetWeb.SiteUsers.GetByLoginName(mapMissingLoginName);
									}
									stringBuilder.AppendLine(string.Format("Mapping missing user '{0}' to '{1}'", loginName, mappedUser.LoginName));
									flag2 = true;
								}
							}
							stringBuilder.AppendLine();
							stringBuilder.AppendLine(aDGraphUserResponse.Details);
							logItem.Information = "Please see details";
							logItem.Target = mappedUser.LoginName;
							logItem.Details = stringBuilder.ToString();
							if (!flag2)
							{
								if (!base.SharePointOptions.AllowDBUserWriting)
								{
									base.TransformationRepository.Add("ADUnmappedUsers", mappedUser.LoginName, "0");
									logItem.Status = ActionOperationStatus.Warning;
									return;
								}
								else
								{
									flag1 = true;
									stringBuilder.AppendLine(string.Format("Mapping user '{0}' to auto-mapped user '{1}'", sourcePrincipalName, mappedUser.PrincipalName));
									logItem.Details = stringBuilder.ToString();
								}
							}
						}
					}
					else
					{
						mappedUser = targetWeb.SiteUsers.GetByLoginName(mappedUser.LoginName);
						flag = true;
					}
					ManifestUser manifestUser = CopyUsersAction.CreateManifestUser(mappedUser, targetWeb.IsMySiteTemplate, flag, flag1);
					uploadManager.AddUser(manifestUser);
					base.PrincipalMappings.AddSafe(sourcePrincipalName, mappedUser.PrincipalName);
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
					logItem.Status = ActionOperationStatus.Warning;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CopyUsers(SPUserCollection sourceUsers, SPWeb targetWeb, IUploadManager uploadManager = null)
		{
			if (sourceUsers == null)
			{
				throw new Exception("Source user collection cannot be null");
			}
			if (targetWeb == null)
			{
				throw new Exception("Target web cannot be null");
			}
			this.IsTargetSharePointOnline = targetWeb.Adapter.SharePointVersion.IsSharePointOnline;
			CopyUsersAction.s_definition.BeginTransformation(this, sourceUsers, targetWeb.SiteUsers, this.Options.Transformers);
			bool mapMissingUsers = ((SharePointActionOptions)this.Options).MapMissingUsers;
			string mapMissingUsersToLoginName = ((SharePointActionOptions)this.Options).MapMissingUsersToLoginName;
			bool flag = false;
			bool isMySiteTemplate = targetWeb.IsMySiteTemplate;
			bool success = false;
			if (uploadManager != null && mapMissingUsers && !string.IsNullOrEmpty(mapMissingUsersToLoginName))
			{
				string valueForKey = base.TransformationRepository.GetValueForKey("MapMissingUser", "IsValid");
				if (!string.IsNullOrEmpty(valueForKey))
				{
					success = Convert.ToBoolean(valueForKey);
				}
				else
				{
					string str = mapMissingUsersToLoginName.Substring(mapMissingUsersToLoginName.LastIndexOf('|') + 1);
					ADGraphUserResponse aDGraphUserResponse = this.ValidateADUser(str, targetWeb);
					success = aDGraphUserResponse.Success;
					base.TransformationRepository.Add("MapMissingUser", "IsValid", aDGraphUserResponse.Success.ToString());
					if (!success)
					{
						StringBuilder stringBuilder = new StringBuilder();
						LogItem logItem = new LogItem("Invalid Mapped Missing User", str, string.Empty, targetWeb.DisplayUrl, ActionOperationStatus.Failed);
						base.FireOperationStarted(logItem);
						logItem.Information = string.Format("Unable to verify validity of user '{0}'", mapMissingUsersToLoginName);
						if (aDGraphUserResponse.BadRequestStatus)
						{
							stringBuilder.AppendLine(string.Format("Unable to add user '{0}' due to a BadRequestStatus", mapMissingUsersToLoginName));
							stringBuilder.AppendLine(Metalogix.Actions.Properties.Resources.AzureAdGraphCredentialsInvalidMessage);
						}
						stringBuilder.AppendLine(aDGraphUserResponse.Details);
						logItem.Details = stringBuilder.ToString();
						base.FireOperationFinished(logItem);
					}
				}
			}
			foreach (SPUser sourceUser in (IEnumerable<SecurityPrincipal>)sourceUsers)
			{
				if (base.CheckForAbort())
				{
					break;
				}
				if (base.PrincipalMappings.ContainsKey(sourceUser.LoginName) && uploadManager == null && !isMySiteTemplate)
				{
					continue;
				}
				SPUser sPUser = CopyUsersAction.s_definition.Transform(sourceUser, this, sourceUsers, targetWeb.SiteUsers, this.Options.Transformers);
				if (sPUser == null)
				{
					LogItem logItem1 = new LogItem("Skipping referenced user", sourceUser.LoginName, null, targetWeb.DisplayUrl, ActionOperationStatus.Skipped);
					base.FireOperationStarted(logItem1);
					logItem1.Information = "User skipped due to the result of a transformation";
					base.FireOperationFinished(logItem1);
				}
				else if (uploadManager != null)
				{
					if (uploadManager.GetUserOrGroupIDByName(base.MapPrincipal(sourceUser.PrincipalName)) != 0)
					{
						continue;
					}
					this.AddUserToAzureManifest(sPUser, sourceUser.PrincipalName, targetWeb, uploadManager, mapMissingUsers, mapMissingUsersToLoginName, success);
				}
				else if (targetWeb.SiteUsers.Contains(sPUser.LoginName))
				{
					base.PrincipalMappings.AddSafe(sourceUser.PrincipalName, sPUser.PrincipalName);
				}
				else
				{
					LogItem xML = null;
					string xML1 = null;
					try
					{
						try
						{
							xML = new LogItem("Adding referenced user", sPUser.LoginName, sPUser.Name, targetWeb.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(xML);
							xML1 = sPUser.XML;
							SPUser sPUser1 = targetWeb.SiteUsers.AddUser(sPUser, base.SharePointOptions.AllowDBUserWriting);
							base.PrincipalMappings.AddSafe(sourceUser.PrincipalName, sPUser1.PrincipalName);
							LogItem licenseDataUsed = xML;
							licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + SPObjectSizes.GetObjectSize(sPUser1);
							xML.AddCompletionDetail(Metalogix.SharePoint.Actions.Properties.Resources.Migration_Detail_UsersCopied, (long)1);
							base.CompareNodes(sourceUser, sPUser1, xML);
							if (base.SharePointOptions.Verbose)
							{
								xML.SourceContent = sourceUser.XML;
								xML.TargetContent = sPUser1.XML;
							}
						}
						catch (Exception exception3)
						{
							Exception exception = exception3;
							string loginName = sPUser.LoginName;
							bool flag1 = false;
							if (mapMissingUsers && !string.IsNullOrEmpty(mapMissingUsersToLoginName) && !sPUser.LoginName.Equals(mapMissingUsersToLoginName, StringComparison.OrdinalIgnoreCase))
							{
								string str1 = null;
								if (!base.PrincipalMappings.TryGetValue(mapMissingUsersToLoginName, out str1))
								{
									SPUser byLoginName = null;
									if (!targetWeb.SiteUsers.Contains(mapMissingUsersToLoginName))
									{
										LogItem stackTrace = null;
										string str2 = string.Format("<User LoginName='{0}' />", SecurityElement.Escape(mapMissingUsersToLoginName));
										try
										{
											try
											{
												stackTrace = new LogItem("Adding referenced user", mapMissingUsersToLoginName, mapMissingUsersToLoginName, targetWeb.DisplayUrl, ActionOperationStatus.Running);
												base.FireOperationStarted(stackTrace);
												SPUser sPUser2 = new SPUser(XmlUtility.StringToXmlNode(str2));
												byLoginName = targetWeb.SiteUsers.AddUser(sPUser2, base.SharePointOptions.AllowDBUserWriting);
												stackTrace.Status = ActionOperationStatus.Completed;
											}
											catch (Exception exception2)
											{
												Exception exception1 = exception2;
												stackTrace.SourceContent = str2;
												stackTrace.Exception = exception1;
												stackTrace.Status = ActionOperationStatus.Warning;
												stackTrace.Information = string.Concat("Exception thrown: ", exception1.Message);
												stackTrace.Details = exception1.StackTrace;
												flag = true;
											}
										}
										finally
										{
											base.FireOperationFinished(stackTrace);
										}
									}
									else
									{
										byLoginName = targetWeb.SiteUsers.GetByLoginName(mapMissingUsersToLoginName);
									}
									base.PrincipalMappings.AddSafe(mapMissingUsersToLoginName, (byLoginName != null ? byLoginName.LoginName : mapMissingUsersToLoginName));
									if (byLoginName == null)
									{
										flag = true;
									}
									else
									{
										loginName = byLoginName.LoginName;
										flag1 = true;
									}
								}
								else if (!targetWeb.SiteUsers.Contains(str1))
								{
									loginName = sPUser.LoginName;
									flag1 = false;
								}
								else
								{
									loginName = str1;
									flag1 = true;
								}
							}
							if (flag)
							{
								loginName = sPUser.LoginName;
								flag1 = false;
							}
							base.PrincipalMappings.AddSafe(sourceUser.PrincipalName, loginName);
							if (!flag1)
							{
								xML.SourceContent = xML1;
								xML.Exception = exception;
								xML.Status = ActionOperationStatus.Warning;
								xML.Information = string.Concat("Exception thrown: ", exception.Message);
								xML.Details = exception.StackTrace;
							}
							else
							{
								xML.SourceContent = xML1;
								xML.Status = ActionOperationStatus.Warning;
								xML.Information = string.Format("Could not add {0}. Mapping to {1}.", sPUser.LoginName, loginName);
							}
						}
					}
					finally
					{
						base.FireOperationFinished(xML);
					}
				}
			}
			CopyUsersAction.s_definition.EndTransformation(this, sourceUsers, targetWeb.SiteUsers, this.Options.Transformers);
		}

		private static ManifestUser CreateManifestUser(SPUser mappedUser, bool isTargetMySite, bool isUserAvailable, bool useSourceUser)
		{
			ManifestUser manifestUser = new ManifestUser()
			{
				Login = mappedUser.LoginName,
				IsSiteAdmin = (!isTargetMySite || isUserAvailable ? mappedUser.IsSiteAdmin : false),
				Name = mappedUser.Name,
				Email = mappedUser.Email
			};
			ManifestUser base64String = manifestUser;
			if (useSourceUser)
			{
				string str = Guid.NewGuid().ToString("N");
				byte[] bytes = Encoding.UTF8.GetBytes(str);
				base64String.SystemId = Convert.ToBase64String(bytes);
				base64String.IsDeleted = true;
			}
			return base64String;
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(CopyUsersAction.s_definition);
			return supportedDefinitions;
		}

		private ADGraphUserResponse ResolvePrincipals(IMigrationPipeline migrationPipeline, string principal)
		{
			ADGraphUserResponse aDGraphUserResponse = new ADGraphUserResponse();
			OperationReportingResult operationReportingResult = new OperationReportingResult(migrationPipeline.ResolvePrincipals(principal));
			if (operationReportingResult.ErrorOccured)
			{
				aDGraphUserResponse.Success = false;
				aDGraphUserResponse.Details = operationReportingResult.GetAllErrorsAsString;
			}
			else if (!operationReportingResult.WarningOccured)
			{
				aDGraphUserResponse.Success = true;
			}
			else
			{
				aDGraphUserResponse.Success = false;
				aDGraphUserResponse.UserNotFoundStatus = true;
				aDGraphUserResponse.Details = operationReportingResult.GetAllWarningsAsString;
			}
			return aDGraphUserResponse;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (base.SharePointOptions.SourceUsers == null)
			{
				if (!(source is SPUserCollection))
				{
					if (!typeof(SecurityPrincipalCollection).IsAssignableFrom(source.GetType()))
					{
						throw new ArgumentException("Source is not a security principal collection");
					}
					SecurityPrincipalCollection securityPrincipalCollection = (SecurityPrincipalCollection)source;
					List<SPUser> sPUsers = new List<SPUser>();
					foreach (SecurityPrincipal securityPrincipal in (IEnumerable<SecurityPrincipal>)securityPrincipalCollection)
					{
						SPUser sPUser = (SPUser)PrincipalConverter.ConvertPrincipal(securityPrincipal, typeof(SPUser));
						if (sPUser == null)
						{
							continue;
						}
						sPUsers.Add(sPUser);
					}
					base.SharePointOptions.SourceUsers = new SPUserCollection(sPUsers.ToArray());
				}
				else
				{
					base.SharePointOptions.SourceUsers = (SPUserCollection)source;
				}
			}
			SPWeb item = (SPWeb)target[0];
			this.CopyUsers(base.SharePointOptions.SourceUsers, item, null);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			IUploadManager uploadManager = null;
			if ((int)oParams.Length >= 3)
			{
				uploadManager = oParams[2] as IUploadManager;
			}
			this.CopyUsers(oParams[0] as SPUserCollection, oParams[1] as SPWeb, uploadManager);
		}

		private ADGraphUserResponse ValidateADUser(string principal, SPWeb targetWeb)
		{
			ResolvePrincipalsMethod enumValue = SharePointConfigurationVariables.ResolvePrincipalsMethod.ToEnumValue<ResolvePrincipalsMethod>(ResolvePrincipalsMethod.People);
			ADGraphUserResponse aDGraphUserResponse = null;
			if (enumValue != ResolvePrincipalsMethod.Graph)
			{
				IMigrationPipeline writer = targetWeb.Adapter.Writer as IMigrationPipeline;
				aDGraphUserResponse = this.ResolvePrincipals(writer, principal);
			}
			else
			{
				string userName = targetWeb.Connection.Node.Credentials.UserName;
				string str = userName.Substring(userName.LastIndexOf('@') + 1);
				aDGraphUserResponse = AzureGraphAPIHelper.ValidateADUser(principal, targetWeb.Adapter.AzureAdGraphCredentials.AppClientId, targetWeb.Adapter.AzureAdGraphCredentials.AppSecret, str);
			}
			return aDGraphUserResponse;
		}
	}
}