using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Explorer
{
    public class ListItemCollection : NodeCollection
    {
        protected List _parentList;

        protected Folder _parentContainer;

        public override object this[int index]
        {
            get { return (ListItem)base[index]; }
        }

        public virtual Folder ParentFolder
        {
            get { return this._parentContainer; }
        }

        public virtual List ParentList
        {
            get
            {
                if (this._parentList == null)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (this[i] is ListItem && ((ListItem)this[i]).Parent is List)
                        {
                            List parent = (List)((ListItem)this[i]).Parent;
                            List list = parent;
                            this._parentList = parent;
                            return list;
                        }
                    }
                }

                return this._parentList;
            }
        }

        public ListItemCollection(XmlNode node) : base(node)
        {
        }

        public ListItemCollection(List parentList, Folder parentContainer, ICollection<Node> items) : base(items)
        {
            this._parentList = parentList;
            this._parentContainer = parentContainer;
        }

        public new ListItem[] ToArray()
        {
            List<ListItem> listItems = new List<ListItem>();
            foreach (Node node in this)
            {
                if (!(node is ListItem))
                {
                    throw new Exception(string.Concat(
                        "A list item collection contains a node that is not a list item. The node has XML:\n",
                        node.XML));
                }

                listItems.Add(node as ListItem);
            }

            return listItems.ToArray();
        }
    }
}