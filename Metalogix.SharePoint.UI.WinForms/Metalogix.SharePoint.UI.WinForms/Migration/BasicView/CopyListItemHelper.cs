using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class CopyListItemHelper
	{
		protected CopyListItemHelper()
		{
		}

		public static ScopableLeftNavigableTabsForm GetListItemDialog(NodeCollection spSourceNodes, NodeCollection spTargetNodes, bool targetIsCSOM, ActionConfigContext configContext)
		{
			ScopableLeftNavigableTabsForm copyListItemActionDialogBasicView;
			bool flag = SPUIUtils.ShouldShowAdvancedMode(configContext.ActionOptions);
			if (!flag)
			{
				copyListItemActionDialogBasicView = new CopyListItemActionDialogBasicView();
			}
			else
			{
				copyListItemActionDialogBasicView = new CopyListItemActionDialog(targetIsCSOM);
			}
			if (configContext.ActionContext.Targets.Count > 1)
			{
				copyListItemActionDialogBasicView.MultiSelectUI = true;
			}
			copyListItemActionDialogBasicView.SourceNodes = spSourceNodes;
			copyListItemActionDialogBasicView.TargetNodes = spTargetNodes;
			if (!flag)
			{
				copyListItemActionDialogBasicView.Action.JobID = configContext.Action.JobID;
				((CopyListItemActionDialogBasicView)copyListItemActionDialogBasicView).Options = configContext.GetActionOptions<PasteListItemOptions>();
			}
			else
			{
				((CopyListItemActionDialog)copyListItemActionDialogBasicView).Options = configContext.GetActionOptions<PasteListItemOptions>();
				copyListItemActionDialogBasicView.EnableTransformerConfiguration(configContext.Action, configContext.ActionContext, configContext.Action.Options.Transformers);
				copyListItemActionDialogBasicView.MinimumSize = new Size(792, copyListItemActionDialogBasicView.Height);
			}
			return copyListItemActionDialogBasicView;
		}
	}
}