using Metalogix.Explorer;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public class ListTitleSorter : IComparer<Node>
	{
		public ListTitleSorter()
		{
		}

		public int Compare(object a, object b)
		{
			return (!(a is SPList) || !(b is SPList) ? 0 : ((SPList)a).Title.CompareTo(((SPList)b).Title));
		}

		public int Compare(Node x, Node y)
		{
			return this.Compare((object)x, (object)y);
		}
	}
}