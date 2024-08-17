using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointFolderPermissions", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[MenuText("3:Paste Folder Objects {0-Paste} > Permissions...")]
	[SourceType(typeof(SPFolder), false)]
	[TargetType(typeof(SPFolder))]
	public class CopyFolderPermissions : CopyPermissionsRecursivelyAction
	{
		public override SharePointObjectScope Scope
		{
			get
			{
				return SharePointObjectScope.Folder;
			}
		}

		public CopyFolderPermissions()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (!typeof(SPList).IsAssignableFrom(sourceSelections.CollectionType))
			{
				return true;
			}
			return !typeof(SPList).IsAssignableFrom(targetSelections.CollectionType);
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			PropertyInfo[] properties = cmdletOptions.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.Name != "CopySitePermissions" && propertyInfo.Name != "CopyPermissionLevels" && propertyInfo.Name != "RecursiveSites" && propertyInfo.Name != "CopyListPermissions" && propertyInfo.Name != "LinkCorrectionScope")
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPOptimizationNode sPOptimizationNode;
			using (SPFolder item = source[0] as SPFolder)
			{
				base.InitializeOptimizationTable(item);
				if (base.HasPermissionsOptimization)
				{
					sPOptimizationNode = base.OptimizationTree.Find(item.ServerRelativeUrl);
				}
				else
				{
					sPOptimizationNode = null;
				}
				SPOptimizationNode sPOptimizationNode1 = sPOptimizationNode;
				foreach (SPFolder sPFolder in target)
				{
					base.CopyFolderRoleAssignments(item, sPFolder, sPOptimizationNode1, true);
				}
			}
		}
	}
}