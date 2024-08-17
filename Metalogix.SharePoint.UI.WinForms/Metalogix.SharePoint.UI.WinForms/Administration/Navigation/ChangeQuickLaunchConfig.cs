using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration.Navigation;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
    [ActionConfig(new Type[] { typeof(ChangeQuickLaunchAction) })]
    public class ChangeQuickLaunchConfig : IActionConfig
    {
        public ConfigurationResult Configure(ActionConfigContext context)
        {
            SPWeb web = context.ActionContext.Targets[0] as SPWeb;
            ChangeQuickLaunchDialog changeQuickLaunchDialog = new ChangeQuickLaunchDialog
            {
                Options = context.GetAction<ChangeQuickLaunchAction>().GetWebNavigationSettings(web)
            };
            changeQuickLaunchDialog.ShowDialog();
            if (changeQuickLaunchDialog.DialogResult != DialogResult.OK)
            {
                return ConfigurationResult.Cancel;
            }
            context.ActionOptions = changeQuickLaunchDialog.Options;
            return ConfigurationResult.Run;
        }
    }
}