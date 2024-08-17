using Metalogix.Widgets;
using System;
using System.Collections.Generic;

namespace Metalogix.UI.WinForms.Widgets
{
	public class TreeViewX : EnhancedTreeView
	{
		private const int WM_ERASEBACKGROUND = 20;

		private List<Type> m_nodeTypeFilterEnum = new List<Type>();

		private bool m_deferErase = true;

		public bool DeferErase
		{
			get
			{
				return this.m_deferErase;
			}
			set
			{
				this.m_deferErase = value;
			}
		}

		public List<Type> NodeTypeFilter
		{
			get
			{
				return this.m_nodeTypeFilterEnum;
			}
		}

		public TreeViewX()
		{
		}
	}
}