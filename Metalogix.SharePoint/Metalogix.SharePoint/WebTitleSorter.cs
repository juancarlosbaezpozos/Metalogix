using Metalogix.Explorer;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public class WebTitleSorter : IComparer<Node>
	{
		public WebTitleSorter()
		{
		}

		public int Compare(object a, object b)
		{
			return ((SPWeb)a).Title.CompareTo(((SPWeb)b).Title);
		}

		public int Compare(Node x, Node y)
		{
			return this.Compare((object)x, (object)y);
		}
	}
}