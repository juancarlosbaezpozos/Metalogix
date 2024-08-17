using Metalogix.Actions;
using Metalogix.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointListPermissions", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[MenuText("3:Paste List Objects {0-Paste} > Permissions...")]
	[SourceType(typeof(SPList))]
	[TargetType(typeof(SPFolder))]
	public class CopyListPermissions : CopyPermissionsRecursivelyAction
	{
		public override SharePointObjectScope Scope
		{
			get
			{
				return SharePointObjectScope.List;
			}
		}

		public CopyListPermissions()
		{
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			PropertyInfo[] properties = cmdletOptions.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.Name != "CopySitePermissions" && propertyInfo.Name != "CopyPermissionLevels" && propertyInfo.Name != "RecursiveSites" && propertyInfo.Name != "LinkCorrectionScope" && propertyInfo.Name != "RecursiveFolders")
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPOptimizationNode sPOptimizationNode;
			using (SPList item = source[0] as SPList)
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
					base.CopyListRoleAssignments(item, sPFolder, sPOptimizationNode1, true);
				}
			}
		}
	}
}