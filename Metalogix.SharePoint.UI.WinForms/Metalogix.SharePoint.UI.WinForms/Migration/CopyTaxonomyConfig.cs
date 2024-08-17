using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyTaxonomyAction) })]
	public class CopyTaxonomyConfig : IActionConfig
	{
		public CopyTaxonomyConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			if (!context.GetActionOptions<PasteTaxonomyOptions>().TaxonomyConfigured)
			{
				context.GetActionOptions<PasteTaxonomyOptions>().TermstoreNameMappingTable.Clear();
			}
			using (CopyTaxonomyDialog copyTaxonomyDialog = new CopyTaxonomyDialog())
			{
				copyTaxonomyDialog.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
				copyTaxonomyDialog.TargetNodes = context.ActionContext.GetTargetsAsNodeCollection();
				copyTaxonomyDialog.Context = context.ActionContext;
				copyTaxonomyDialog.Action = context.Action;
				copyTaxonomyDialog.Options = context.GetActionOptions<PasteTaxonomyOptions>();
				copyTaxonomyDialog.ShowDialog();
				configurationResult = copyTaxonomyDialog.ConfigurationResult;
			}
			return configurationResult;
		}
	}
}