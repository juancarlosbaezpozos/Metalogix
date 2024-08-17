using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Refresh.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Refresh Node {4-Refresh}")]
	[MenuTextPlural("Refresh Nodes {4-Refresh}", PluralCondition.MultipleTargets)]
	[Name("Refresh Node")]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.Refresh)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(ExplorerNode))]
	public class RefreshAction : Metalogix.Actions.Action
	{
		public RefreshAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPConnection current = enumerator.Current as SPConnection;
					if (current == null || current.Status != ConnectionStatus.Invalid)
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

		public void RefreshNode(IXMLAbleList target)
		{
			foreach (ExplorerNode explorerNode in target)
			{
				explorerNode.Refresh();
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.RefreshNode(target);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 1)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.RefreshNode(oParams[0] as IXMLAbleList);
		}
	}
}