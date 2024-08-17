using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[CmdletEnabled(false, "Remove-MLSharePointFolder", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Delete.ico")]
	[LaunchAsJob(false)]
	[MenuText("Delete Folder {2-Delete}")]
	[MenuTextPlural("Delete Folders {2-Delete}", PluralCondition.MultipleTargets)]
	[Name("Delete Folder")]
	[RequiresWriteAccess(true)]
	[Shortcut(ShortcutAction.Delete)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder), false)]
	public class DeleteFolder : DeleteBase
	{
		public DeleteFolder()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (base.GetType() == typeof(DeleteFolder))
			{
				IEnumerator enumerator = targetSelections.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SPFolder current = (SPFolder)enumerator.Current;
						if (current.ParentFolder != null && current.ParentFolder != current)
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
			return true;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPList parentList = null;
			foreach (SPFolder sPFolder in target)
			{
				if (parentList == null && sPFolder.ParentList != null)
				{
					parentList = sPFolder.ParentList;
				}
				sPFolder.ParentFolder.SubFolders.DeleteFolder(sPFolder.Name);
			}
			if (parentList != null)
			{
				parentList.Refresh();
			}
		}
	}
}