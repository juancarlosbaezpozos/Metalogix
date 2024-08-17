using Metalogix.Explorer;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public class FolderNameSorter : IComparer<Node>
	{
		public FolderNameSorter()
		{
		}

		public int Compare(object a, object b)
		{
			return (!(a is SPFolder) || !(b is SPFolder) ? 0 : ((SPFolder)a).Name.CompareTo(((SPFolder)b).Name));
		}

		public int Compare(Node x, Node y)
		{
			return this.Compare((object)x, (object)y);
		}
	}
}