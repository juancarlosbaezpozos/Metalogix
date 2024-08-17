using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Widgets;
using Metalogix.Widgets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Explorer
{
	[DesignTimeVisible(false)]
	public class ApplicationControl : HasSelectableObjects
	{
		public virtual Metalogix.Actions.Action[] Actions
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override System.Windows.Forms.ContextMenuStrip ContextMenuStrip
		{
			get
			{
				return base.ContextMenuStrip;
			}
			set
			{
				base.ContextMenuStrip = value;
			}
		}

		public virtual NodeCollection DataSource
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool ExplorerMultiSelectEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual EnhancedTreeView.AllowSelectionDelegate ExplorerMultiSelectLimitationMethod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[Browsable(false)]
		public virtual IDataConverter<object, string> ItemsViewDataConverter
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual List<Type> NodeTypeFilter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExplorerTreeNode SelectedNode
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ReadOnlyCollection<ExplorerTreeNode> SelectedNodes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override IXMLAbleList SelectedObjects
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool ShowExplorerCheckBoxes
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ApplicationControl()
		{
		}

		protected void FireSelectedNodeChanged(Node node)
		{
			if (this.OnSelectedNodeChanged != null)
			{
				this.OnSelectedNodeChanged(node);
			}
		}

		public virtual void NavigateToLocation(Metalogix.Explorer.Location location)
		{
			throw new NotImplementedException();
		}

		public virtual void NavigateToLocation(IEnumerable<Metalogix.Explorer.Location> locations)
		{
			throw new NotImplementedException();
		}

		public virtual void NavigateToNode(Node node)
		{
			throw new NotImplementedException();
		}

		public virtual void NavigateToNode(IEnumerable<Node> nodes)
		{
			throw new NotImplementedException();
		}

		public virtual void Redraw()
		{
		}

		public event LocationBar.SelectedNodeChangedHandler OnSelectedNodeChanged;
	}
}