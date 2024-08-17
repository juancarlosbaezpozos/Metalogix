using System;

namespace Metalogix.Explorer
{
    public class ListItemVersionCollection : ListItemCollection
    {
        protected ListItem _parentItem;

        public virtual ListItem ParentItem
        {
            get { return this._parentItem; }
        }

        public ListItemVersionCollection() : base(null)
        {
        }

        public ListItemVersionCollection(ListItem parentItem, List parentList, Folder parentContainer, Node[] items) :
            base(parentList, parentContainer, items)
        {
            this._parentItem = parentItem;
        }
    }
}