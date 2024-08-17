using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointListAsFolder", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[MenuText("1:Paste List as Folder... {0-Paste}")]
	[MenuTextPlural("1:Paste Lists as Folders... {0-Paste}", PluralCondition.MultipleSources)]
	[Name("Paste List as Folder")]
	[Shortcut(ShortcutAction.Paste)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPList))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder), true)]
	public class PasteListAsFolderAction : PasteFolderAction
	{
		public PasteListAsFolderAction()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!this.AppliesTo(sourceSelections, targetSelections) || !base.EnabledOn(sourceSelections, targetSelections))
			{
				return false;
			}
			if (!this.HasDiscussionList(sourceSelections))
			{
				return true;
			}
			return !this.HasDiscussionList(targetSelections);
		}

		private bool HasDiscussionList(IXMLAbleList selections)
		{
			bool flag;
			if (selections != null && selections.Count > 0)
			{
				IEnumerator enumerator = selections.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						if (!(current is SPDiscussionList) && (!(current is SPFolder) || !((current as SPFolder).ParentList is SPDiscussionList)))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
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
			return false;
		}
	}
}