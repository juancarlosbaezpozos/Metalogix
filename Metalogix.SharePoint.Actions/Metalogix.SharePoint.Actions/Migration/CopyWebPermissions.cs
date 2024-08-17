using Metalogix.Actions;
using Metalogix.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointWebPermissions", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[MenuText("3:Paste Site Objects {0-Paste} > Permissions...")]
	[SourceType(typeof(SPWeb))]
	[TargetType(typeof(SPWeb))]
	public class CopyWebPermissions : CopyPermissionsRecursivelyAction
	{
		public override SharePointObjectScope Scope
		{
			get
			{
				return SharePointObjectScope.Site;
			}
		}

		public CopyWebPermissions()
		{
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			PropertyInfo[] properties = cmdletOptions.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.Name != "LinkCorrectionScope" && propertyInfo.Name != "RecursiveFolders")
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPOptimizationNode sPOptimizationNode;
			using (SPWeb item = source[0] as SPWeb)
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
				foreach (SPWeb sPWeb in target)
				{
					base.CopyWebRoleAssignments(item, sPWeb, sPOptimizationNode1, true);
				}
			}
		}
	}
}