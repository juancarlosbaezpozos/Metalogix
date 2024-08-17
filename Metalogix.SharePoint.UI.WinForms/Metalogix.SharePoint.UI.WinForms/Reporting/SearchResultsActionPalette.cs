using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.UI.WinForms.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
	public class SearchResultsActionPalette : ActionPaletteControl
	{
		private SPSearchResultsCollection m_searchResults;

		public SPSearchResultsCollection MonitoredSearchResults
		{
			get
			{
				return this.m_searchResults;
			}
			set
			{
				if (this.m_searchResults != null)
				{
					this.m_searchResults.OnNodeCollectionChanged -= new NodeCollectionChangedHandler(this.MonitoredSearchResultsChanged);
					foreach (SPSearchResult mSearchResult in this.m_searchResults)
					{
						mSearchResult.Dispose();
					}
				}
				this.m_searchResults = value;
				if (this.m_searchResults != null)
				{
					this.m_searchResults.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.MonitoredSearchResultsChanged);
				}
			}
		}

		protected override IXMLAbleList Target
		{
			get
			{
				IXMLAbleList xMLAbleLists;
				bool flag = true;
				SPSearchResultsCollection target = base.Target as SPSearchResultsCollection;
				if (target == null)
				{
					return base.Target;
				}
				Node[] node = new Node[target.Count];
				int num = 0;
				using (IEnumerator<Node> enumerator = target.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SPSearchResult current = (SPSearchResult)enumerator.Current;
						if (current.HasNode)
						{
							node[num] = current.Node;
							num++;
						}
						else if (!current.FetchFailed)
						{
							flag = false;
							break;
						}
						else
						{
							xMLAbleLists = new XMLAbleList();
							return xMLAbleLists;
						}
					}
					if (!flag)
					{
						return target;
					}
					return new NodeCollection(node);
				}
				return xMLAbleLists;
			}
		}

		public SearchResultsActionPalette(IContainer container) : base(container, null)
		{
		}

	    // Metalogix.SharePoint.UI.WinForms.Reporting.SearchResultsActionPalette
	    public override void BuildActionMenu()
	    {
	        IXMLAbleList target = this.Target;
	        if (target is SPSearchResultsCollection)
	        {
	            this.m_UsedShortcutKeys.Clear();
	            this.Items.Clear();
	            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Fetching item data...");
	            toolStripMenuItem.Enabled = false;
	            toolStripMenuItem.Font = new System.Drawing.Font(toolStripMenuItem.Font, System.Drawing.FontStyle.Italic);
	            this.Items.Add(toolStripMenuItem);
	        }
	        else
	        {
	            try
	            {
	                if (target != null)
	                {
	                    if (target.Count > 0 && target.CollectionType != null)
	                    {
	                        base.BuildActionMenu();
	                        ActionCollection actionCollection = new ActionCollection();
	                        actionCollection.Add(new NavigateSearchResult());
	                        base.AddMenus(this.Items, actionCollection, false);
	                    }
	                    else
	                    {
	                        base.SuspendLayout();
	                        this.Items.Clear();
	                        this.m_UsedShortcutKeys.Clear();
	                        ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Not a navigatable item");
	                        toolStripMenuItem2.Enabled = false;
	                        toolStripMenuItem2.Font = new System.Drawing.Font(toolStripMenuItem2.Font, System.Drawing.FontStyle.Italic);
	                        this.Items.Add(toolStripMenuItem2);
	                        base.ResumeLayout();
	                    }
	                }
	            }
	            catch
	            {
	            }
	        }
	        if (this.Items.Count > 0)
	        {
	            this.SetDisplayedItems();
	            return;
	        }
	        base.Close();
	    }


        private void MonitoredSearchResultsChanged(NodeCollectionChangeType changeType, Node changedNode)
		{
			this.UpdateMenu();
		}

		private void UpdateMenu()
		{
			if (!base.InvokeRequired)
			{
				this.BuildActionMenu();
				return;
			}
			base.Invoke(new SearchResultsActionPalette.UpdateMenuDelegate(this.UpdateMenu));
		}

		private delegate void UpdateMenuDelegate();
	}
}