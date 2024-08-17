using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyMasterPageGalleryDialog : ScopableLeftNavigableTabsForm
	{
		private TCMasterPageGalleryOptions w_tcMasterPageGallery = new TCMasterPageGalleryOptions();

		private TCListContentOptions w_tcListContent = new TCListContentOptions();

		private TCMappingOptions w_tcMapping = new TCMappingOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private TCWebPartsOptions w_tcWebParts = new TCWebPartsOptions();

		private TCTaxonomyOptions w_tcTaxonomy = new TCTaxonomyOptions();

		private TCMigrationModeOptions w_tcMigrationMode = new TCMigrationModeOptions();

		private bool m_bTabsInitialized;

		private IContainer components;

		public PasteMasterPageGalleryOptions Options
		{
			get
			{
				return Action.Options as PasteMasterPageGalleryOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public override NodeCollection SourceNodes
		{
			get
			{
				return base.SourceNodes;
			}
			set
			{
				base.SourceNodes = value;
				if (value != null)
				{
					LoadDialogTabs();
				}
			}
		}

		public CopyMasterPageGalleryDialog()
		{
			InitializeComponent();
			Text = "Configure Master Page Gallery Copying Options";
			LoadDialogTabs();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.Name = "CopyMasterPageGalleryDialog";
			base.ShowIcon = true;
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		private void LoadDialogTabs()
		{
			bool flag = BCSHelper.HasExternalListsOnly(SourceNodes);
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigrationMode };
			if (!flag)
			{
				list.Add(w_tcMasterPageGallery);
				list.Add(w_tcListContent);
				list.Add(w_tcTaxonomy);
			}
			list.Add(w_tcWebParts);
			list.Add(w_tcMapping);
			list.Add(w_tcGeneral);
			foreach (ScopableTabbableControl item in list)
			{
				if (!m_bTabsInitialized)
				{
					item.Scope = SharePointObjectScope.List;
				}
			}
			base.Tabs = list;
			m_bTabsInitialized = true;
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcMapping.Options = sPMappingOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneral.Options = sPGeneralOptions;
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			w_tcMigrationMode.Options = sPMigrationModeOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			w_tcWebParts.Options = sPWebPartOptions;
			if (!BCSHelper.HasExternalListsOnly(SourceNodes))
			{
				SPMasterPageGalleryOptions sPMasterPageGalleryOptions = new SPMasterPageGalleryOptions();
				sPMasterPageGalleryOptions.SetFromOptions(action.Options);
				w_tcMasterPageGallery.Options = sPMasterPageGalleryOptions;
				SPListContentOptions sPListContentOptions = new SPListContentOptions();
				sPListContentOptions.SetFromOptions(action.Options);
				w_tcListContent.Options = sPListContentOptions;
				SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
				sPTaxonomyOptions.SetFromOptions(action.Options);
				w_tcTaxonomy.Options = sPTaxonomyOptions;
			}
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcGeneral.Options);
			action.Options.SetFromOptions(w_tcMapping.Options);
			action.Options.SetFromOptions(w_tcMigrationMode.Options);
			action.Options.SetFromOptions(w_tcWebParts.Options);
			if (BCSHelper.HasExternalListsOnly(SourceNodes))
			{
				action.Options.SetFromOptions(new SPMasterPageGalleryOptions());
				action.Options.SetFromOptions(new SPListContentOptions());
				action.Options.SetFromOptions(new SPTaxonomyOptions());
			}
			else
			{
				action.Options.SetFromOptions(w_tcListContent.Options);
				action.Options.SetFromOptions(w_tcMasterPageGallery.Options);
				action.Options.SetFromOptions(w_tcTaxonomy.Options);
			}
			return true;
		}
	}
}
