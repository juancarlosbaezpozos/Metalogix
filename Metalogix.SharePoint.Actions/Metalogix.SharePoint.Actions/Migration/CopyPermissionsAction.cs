using Metalogix;
using Metalogix.Actions;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointPermissions", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Permissions.ico")]
	[LaunchAsJob(true)]
	[Name("Paste Permissions")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(ISecurableObject))]
	[SubActionTypes(typeof(CopyRoleAssignmentsAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(ISecurableObject))]
	[UsesStickySettings(true)]
	public class CopyPermissionsAction : PasteAction<CopyPermissionsOptions>
	{
		public CopyPermissionsAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			if (typeof(SPWeb).IsAssignableFrom(sourceSelections.CollectionType) && typeof(SPWeb).IsAssignableFrom(targetSelections.CollectionType))
			{
				return false;
			}
			if (!typeof(SPFolder).IsAssignableFrom(sourceSelections.CollectionType))
			{
				return true;
			}
			return !typeof(SPFolder).IsAssignableFrom(targetSelections.CollectionType);
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			PropertyInfo[] properties = cmdletOptions.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.Name != "CopySitePermissions" && propertyInfo.Name != "CopyPermissionLevels" && propertyInfo.Name != "RecursiveSites" && propertyInfo.Name != "CopyListPermissions" && propertyInfo.Name != "LinkCorrectionScope" && propertyInfo.Name != "CopyFolderPermissions" && propertyInfo.Name != "RecursiveFolders" && propertyInfo.Name != "CopyItemPermissions")
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, base.SharePointOptions.ForceRefresh);
			foreach (ISecurableObject securableObject in target)
			{
				SPWeb parentWeb = null;
				if (typeof(SPWeb).IsAssignableFrom(securableObject.GetType()))
				{
					parentWeb = (SPWeb)securableObject;
				}
				else if (typeof(SPFolder).IsAssignableFrom(securableObject.GetType()))
				{
					parentWeb = ((SPFolder)securableObject).ParentList.ParentWeb;
				}
				else if (typeof(SPListItem).IsAssignableFrom(securableObject.GetType()))
				{
					parentWeb = ((SPListItem)securableObject).ParentList.ParentWeb;
				}
				foreach (ISecurableObject securableObject1 in source)
				{
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyRoleAssignmentsAction);
					object[] objArray = new object[] { securableObject1, securableObject, true };
					copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(null, parentWeb), null);
				}
			}
		}
	}
}