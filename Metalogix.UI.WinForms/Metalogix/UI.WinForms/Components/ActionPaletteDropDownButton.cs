using Metalogix.Actions;
using Metalogix.UI.WinForms.Actions;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class ActionPaletteDropDownButton : ToolStripDropDownButton
	{
		private static ActionPaletteControl actionPalette;

		private string m_sActionNameFilter;

		private Type m_actionTypeFilter;

		private Control m_hostingControl;

		private IXMLAbleList m_selectedSources = new XMLAbleList();

		private IXMLAbleList m_selectedTargets = new XMLAbleList();

		private ActionContext m_context;

		public string ActionNameFilter
		{
			get
			{
				return this.m_sActionNameFilter;
			}
			set
			{
				this.m_sActionNameFilter = value;
			}
		}

		public Type ActionTypeFilter
		{
			get
			{
				return this.m_actionTypeFilter;
			}
			set
			{
				this.m_actionTypeFilter = value;
			}
		}

		public ActionContext Context
		{
			get
			{
				if (this.m_context == null)
				{
					this.m_context = new ActionContext(this.Source, this.Target);
				}
				return this.m_context;
			}
		}

		protected ActionCollection FilteredActions
		{
			get
			{
				ActionCollection actions = ActionPaletteDropDownButton.actionPalette.Actions;
				if (this.ActionTypeFilter != null)
				{
					actions = actions.GetActions(this.ActionTypeFilter);
				}
				if (this.ActionNameFilter != null)
				{
					actions = actions.GetActions(this.ActionNameFilter);
				}
				return actions;
			}
		}

		public Control HostingControl
		{
			get
			{
				if (this.m_hostingControl != null || base.Parent == null)
				{
					return this.m_hostingControl;
				}
				return base.Parent.Parent;
			}
			set
			{
				this.m_hostingControl = value;
			}
		}

		protected IXMLAbleList Source
		{
			get
			{
				try
				{
					IXMLAbleList dataObject = (IXMLAbleList)ClipBoard.GetDataObject();
					if (dataObject != null)
					{
						return dataObject;
					}
				}
				catch (Exception exception)
				{
				}
				return new XMLAbleList();
			}
		}

		protected virtual IXMLAbleList Target
		{
			get
			{
				if (this.HostingControl == null || this.HostingControl.GetType().GetInterface(typeof(IHasSelectableObjects).FullName) == null)
				{
					return new XMLAbleList();
				}
				IXMLAbleList selectedObjects = ((IHasSelectableObjects)this.HostingControl).SelectedObjects;
				if (selectedObjects != null)
				{
					return selectedObjects;
				}
				return new XMLAbleList();
			}
		}

		static ActionPaletteDropDownButton()
		{
			ActionPaletteDropDownButton.actionPalette = new ActionPaletteControl();
		}

		public ActionPaletteDropDownButton()
		{
		}

		protected virtual void LoadDynamicActions()
		{
			base.DropDownItems.Clear();
			ActionCollection applicableActions = this.FilteredActions.GetApplicableActions(this.Source, this.Target);
			ActionPaletteDropDownButton.actionPalette.AddMenus(base.DropDownItems, applicableActions, this.Context, true);
		}

		protected override void OnDropDownShow(EventArgs e)
		{
			this.LoadDynamicActions();
			base.OnDropDownShow(e);
		}
	}
}