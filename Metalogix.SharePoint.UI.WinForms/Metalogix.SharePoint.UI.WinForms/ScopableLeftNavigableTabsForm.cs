using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint.Options;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms
{
	public class ScopableLeftNavigableTabsForm : TemplatableTabsForm
	{
		private SharePointObjectScope m_scope;

		private NodeCollection m_sourceNodes;

		private NodeCollection m_targetNodes;

		private bool m_bMultiSelectUI;

		private IContainer components;

		public override bool CanShowBasicModeButton => true;

		public virtual bool MultiSelectUI
		{
			get
			{
				return m_bMultiSelectUI;
			}
			set
			{
				m_bMultiSelectUI = value;
				foreach (ScopableTabbableControl scopableTab in GetScopableTabs())
				{
					scopableTab.MultiSelectUI = value;
				}
			}
		}

		public SharePointObjectScope Scope
		{
			get
			{
				return m_scope;
			}
			set
			{
				m_scope = value;
				foreach (ScopableTabbableControl scopableTab in GetScopableTabs())
				{
					scopableTab.Scope = value;
				}
			}
		}

		public virtual NodeCollection SourceNodes
		{
			get
			{
				return m_sourceNodes;
			}
			set
			{
				m_sourceNodes = value;
				foreach (ScopableTabbableControl scopableTab in GetScopableTabs())
				{
					scopableTab.SourceNodes = value;
				}
			}
		}

		public virtual NodeCollection TargetNodes
		{
			get
			{
				return m_targetNodes;
			}
			set
			{
				m_targetNodes = value;
				foreach (ScopableTabbableControl scopableTab in GetScopableTabs())
				{
					scopableTab.TargetNodes = value;
				}
			}
		}

		public ScopableLeftNavigableTabsForm()
		{
			InitializeComponent();
			if (Action != null && Action.IsBasicModeAllowed)
			{
				SPUIUtils.LogTelemetry(Action.Name, CanShowBasicModeButton);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private List<ScopableTabbableControl> GetScopableTabs()
		{
			List<ScopableTabbableControl> list = new List<ScopableTabbableControl>();
			foreach (TabbableControl tab in base.Tabs)
			{
				if (tab is ScopableTabbableControl item)
				{
					list.Add(item);
				}
			}
			return list;
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.ScopableLeftNavigableTabsForm));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("ScopableLeftNavigableTabsForm.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "ScopableLeftNavigableTabsForm";
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadTabs()
		{
			base.LoadTabs();
			foreach (ScopableTabbableControl scopableTab in GetScopableTabs())
			{
				scopableTab.AvailabilityChanged += On_TabControl_AvailabilityChanged;
			}
		}

		protected override void LoadUI(Action action)
		{
			if (base.TransformerTab != null)
			{
				base.TransformerTab.PersistentTransformerCollection = action.Options.Transformers;
			}
			base.LoadUI(action);
		}

		private void On_TabControl_AvailabilityChanged(AvailabilityChangedEventArgs args, object sender)
		{
			PropertyInfo propertyInfo = null;
			switch (args.ObjectType)
			{
			case SharePointObjectScope.SiteCollection:
				propertyInfo = typeof(ScopableTabbableControl).GetProperty("SiteCollectionsAvailable");
				break;
			case SharePointObjectScope.Site:
				propertyInfo = typeof(ScopableTabbableControl).GetProperty("SitesAvailable");
				break;
			case SharePointObjectScope.List:
				propertyInfo = typeof(ScopableTabbableControl).GetProperty("ListsAvailable");
				break;
			case SharePointObjectScope.Folder:
				propertyInfo = typeof(ScopableTabbableControl).GetProperty("FoldersAvailable");
				break;
			case SharePointObjectScope.Item:
				propertyInfo = typeof(ScopableTabbableControl).GetProperty("ItemsAvailable");
				break;
			}
			foreach (ScopableTabbableControl scopableTab in GetScopableTabs())
			{
				if (scopableTab != sender && (bool)propertyInfo.GetValue(scopableTab, null) != args.ObjectAvailable)
				{
					propertyInfo.SetValue(scopableTab, args.ObjectAvailable, null);
				}
			}
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			if (action != null && action.Options != null && base.TransformerTab != null)
			{
				action.Options.Transformers = base.TransformerTab.OutputTransformerCollection;
			}
			return true;
		}

		protected override void SwitchView(Action action, bool isFromAdvancedView)
		{
			if (action != null && action.Options != null)
			{
				base.IsModeSwitched = true;
				SaveUI(action);
				(action.Options as SharePointActionOptions).IsFromAdvancedMode = isFromAdvancedView;
				base.ConfigurationResult = ConfigurationResult.Switch;
				Close();
			}
		}
	}
}
