using Metalogix.Explorer;
using System;

namespace Metalogix.SharePoint
{
	public class SPDiscussionItemCollection : SPListItemCollection
	{
		private int _parentId;

		public int SubjectID
		{
			get
			{
				return this._parentId;
			}
		}

		public SPDiscussionItemCollection(SPList parentList, SPFolder parentFolder, Node[] items, int parentId) : base(parentList, parentFolder, items)
		{
			this._parentId = parentId;
		}
	}
}