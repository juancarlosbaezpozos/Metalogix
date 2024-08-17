using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metalogix.Explorer
{
    public class NodeCollectionExtended : NodeCollection
    {
        private readonly NodeCollection _dataSource = new NodeCollection();

        private string _filterValue;

        private IEnumerable<Type> _filterTypes;

        private IEnumerable<ConnectionStatus> _filterStatuses;

        private NodeCollectionSortBy _sortBy;

        public NodeCollection DataSource
        {
            get { return this._dataSource; }
        }

        public NodeCollectionExtended(NodeCollection nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            base.AddRange(nodes.ToArray());
            this.ApplyFilterSort();
            nodes.OnNodeCollectionChanged +=
                new NodeCollectionChangedHandler(this.DataSourceRawOnNodeCollectionChanged);
        }

        public void ApplyFilterSort(string filterValue, IEnumerable<Type> filterTypes,
            IEnumerable<ConnectionStatus> filterStatuses, NodeCollectionSortBy sortBy)
        {
            this._filterValue = filterValue;
            this._filterTypes = filterTypes;
            this._filterStatuses = filterStatuses;
            this._sortBy = sortBy;
            this.ApplyFilterSort();
        }

        public void ApplyFilterSort()
        {
            this._dataSource.Clear();
            foreach (Node node in this)
            {
                if (!string.IsNullOrEmpty(this._filterValue) && (string.IsNullOrEmpty(node.DisplayName) ||
                                                                 node.DisplayName.IndexOf(this._filterValue,
                                                                     StringComparison.OrdinalIgnoreCase) < 0) ||
                    this._filterTypes != null && !this._filterTypes.Contains<Type>(node.GetType()) ||
                    this._filterStatuses != null &&
                    !this._filterStatuses.Contains<ConnectionStatus>(node.Connection.Status))
                {
                    continue;
                }

                this._dataSource.Add(node);
            }

            this._dataSource.Sort(new NodeCollectionComparer(this._dataSource, this._sortBy));
            this._dataSource.FireNodeCollectionChanged(NodeCollectionChangeType.FullReset, null);
        }

        private void DataSourceRawOnNodeCollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
        {
            try
            {
                try
                {
                    switch (changeType)
                    {
                        case NodeCollectionChangeType.NodeAdded:
                        {
                            base.Add(changedNode);
                            this._dataSource.Add(changedNode);
                            break;
                        }
                        case NodeCollectionChangeType.NodeRemoved:
                        {
                            base.Remove(changedNode);
                            if (!this._dataSource.Contains(changedNode))
                            {
                                break;
                            }

                            this._dataSource.Remove(changedNode);
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    GlobalServices.ErrorHandler.HandleException(exception);
                }
            }
            finally
            {
                this._dataSource.FireNodeCollectionChanged(changeType, changedNode);
            }
        }

        public void ResetFilterSort()
        {
            this._filterValue = null;
            this._filterTypes = null;
            this._filterStatuses = null;
            this._sortBy = NodeCollectionSortBy.Default;
        }
    }
}