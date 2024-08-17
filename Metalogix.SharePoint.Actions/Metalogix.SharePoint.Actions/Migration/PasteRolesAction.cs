using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointPermissionLevels", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.PermissionLevels.ico")]
	[MenuText("3:Paste Site Objects {0-Paste} > Permission Levels...")]
	[Name("Paste Permission Levels")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteRolesAction : PasteAction<PasteRolesOptions>
	{
		public PasteRolesAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = sourceSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (((SPWeb)enumerator.Current).Adapter.SharePointVersion.IsSharePoint2007OrLater)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
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

		private void CopyRoles(SPRoleCollection sourceRoles, SPRoleCollection targetRoles)
		{
			foreach (SPRole sourceRole in sourceRoles)
			{
				if (!base.CheckForAbort())
				{
					if (sourceRole.Hidden)
					{
						continue;
					}
					SPRole item = targetRoles[sourceRole.RoleName] as SPRole;
					string xML = null;
					LogItem logItem = null;
					try
					{
						try
						{
							if (item == null || sourceRole.GetSimilarity(item) < 1f)
							{
								logItem = new LogItem(string.Concat((item != null ? "Updating" : "Adding"), " Permission Level"), sourceRole.RoleName, sourceRoles.ParentWeb.DisplayUrl, targetRoles.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem);
								SPRole sPRole = targetRoles.AddOrUpdateRole(sourceRole) as SPRole;
								xML = sPRole.XML;
								if (!base.SharePointOptions.CheckResults)
								{
									logItem.Status = ActionOperationStatus.Completed;
								}
								else if (sourceRole.GetSimilarity(sPRole) == 1f)
								{
									logItem.Status = ActionOperationStatus.Completed;
								}
								else
								{
									logItem.Status = ActionOperationStatus.Different;
								}
								if (base.SharePointOptions.Verbose)
								{
									logItem.SourceContent = sourceRole.XML;
									logItem.TargetContent = xML;
								}
							}
						}
						catch (Exception exception)
						{
							logItem.Exception = exception;
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
				else
				{
					return;
				}
			}
		}

		private void CopyRoles(SPWeb sourceWeb, SPWeb targetWeb, bool bIsRootCopy)
		{
			LogItem logItem = new LogItem("Initializing Permission Levels", sourceWeb.DisplayUrl, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					using (sourceWeb)
					{
						if (sourceWeb.HasUniqueRoles || bIsRootCopy && base.SharePointOptions.CopyRootPermissions)
						{
							SPRoleCollection roles = sourceWeb.Roles as SPRoleCollection;
							SPRoleCollection sPRoleCollection = targetWeb.Roles as SPRoleCollection;
							logItem.Status = ActionOperationStatus.Completed;
							this.CopyRoles(roles, sPRoleCollection);
						}
						else
						{
							logItem.Information = "Skipping because this web inherits permission levels";
							logItem.Status = ActionOperationStatus.Skipped;
						}
						targetWeb.Dispose();
					}
					if (base.CheckForAbort())
					{
						return;
					}
					else if (base.SharePointOptions.RecursivelyCopyPermissionLevels)
					{
						foreach (SPWeb subWeb in sourceWeb.SubWebs)
						{
							if (!base.CheckForAbort())
							{
								SPWeb item = (SPWeb)targetWeb.SubWebs[subWeb.Name];
								if (item != null)
								{
									this.CopyRoles(subWeb, item, false);
								}
								else
								{
									LogItem logItem1 = new LogItem("Copying Permission Levels", subWeb.Name, subWeb.DisplayUrl, null, ActionOperationStatus.MissingOnTarget)
									{
										Information = string.Concat("The site: '", subWeb.Name, "' does not exist on the target")
									};
									base.FireOperationStarted(logItem1);
								}
							}
							else
							{
								return;
							}
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
					logItem.Status = ActionOperationStatus.Failed;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			List<string> strs = new List<string>()
			{
				"CopyRootPermissions",
				"MapAudiences",
				"LogSkippedItems",
				"CheckResults",
				"ForceRefresh",
				"ComparisonLevel",
				"CorrectingLinks",
				"LinkCorrectTextFields",
				"LinkCorrectionScope",
				"UseComprehensiveLinkCorrection",
				"AllowDBUserWriting",
				"MapGroupsByName",
				"OverwriteGroups",
				"OverrideCheckouts",
				"MapMissingUsersToLoginName"
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

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPWeb sPWeb in source)
			{
				foreach (SPWeb sPWeb1 in target)
				{
					using (sPWeb1)
					{
						this.CopyRoles(sPWeb, sPWeb1, true);
					}
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.CopyRoles(oParams[0] as SPRoleCollection, oParams[1] as SPRoleCollection);
		}
	}
}