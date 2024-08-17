using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;

namespace Metalogix.Explorer
{
    public class NodeCollectionComparer : IComparer<Node>
    {
        private readonly NodeCollection _collection;

        private readonly NodeCollectionSortBy _sortBy;

        public NodeCollectionComparer(NodeCollection originalCollection, NodeCollectionSortBy sortBy)
        {
            this._collection = originalCollection;
            this._sortBy = sortBy;
        }

        public int Compare(Node x, Node y)
        {
            switch (this._sortBy)
            {
                case NodeCollectionSortBy.Type:
                {
                    return string.Compare(x.Connection.GetType().ToString(), y.Connection.GetType().ToString(),
                        StringComparison.InvariantCultureIgnoreCase);
                }
                case NodeCollectionSortBy.Url:
                {
                    return string.Compare(x.Url, y.Url, StringComparison.InvariantCultureIgnoreCase);
                }
                default:
                {
                    int num = this._collection.IndexOf(x);
                    return num.CompareTo(this._collection.IndexOf(y));
                }
            }
        }
    }
}