using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Data
{
	public class FilterNode
	{
		public List<FilterNode> Children
		{
			get;
			set;
		}

		public FilterNode Parent
		{
			get;
			set;
		}

		public FilterRow Row
		{
			get;
			set;
		}

		public FilterNode()
		{
		}

		public static void AddNewRow(ref FilterNode filterTree, FilterRow newRow)
		{
			if (filterTree == null)
			{
				FilterNode filterNode = new FilterNode()
				{
					Parent = null,
					Row = newRow,
					Children = null
				};
				filterTree = filterNode;
				return;
			}
			FilterNode filterNode1 = new FilterNode()
			{
				Parent = null,
				Row = newRow,
				Children = null
			};
			FilterNode filterNode2 = filterNode1;
			if (!filterTree.IsRowNode())
			{
				filterNode2.Parent = filterTree;
				filterTree.Children.Add(filterNode2);
				return;
			}
			FilterNode filterNode3 = new FilterNode()
			{
				Parent = null,
				Row = null,
				Children = new List<FilterNode>()
			};
			FilterNode filterNode4 = filterNode3;
			FilterNode filterNode5 = filterTree;
			filterNode5.Parent = filterNode4;
			filterNode4.Children.Add(filterNode5);
			filterNode2.Parent = filterNode4;
			filterNode4.Children.Add(filterNode2);
			filterTree = filterNode4;
		}

		public static bool DoesTreeMatchListEntirely(FilterNode filterTree, IEnumerable<FilterRow> list)
		{
			bool flag;
			if (filterTree == null || list == null || list.Count<FilterRow>() < 1)
			{
				return false;
			}
			if (filterTree.IsRowNode())
			{
				return list.Contains<FilterRow>(filterTree.Row);
			}
			List<FilterRow> filterRows = new List<FilterRow>(list);
			List<FilterNode>.Enumerator enumerator = filterTree.Children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (FilterNode.DoesTreeMatchListEntirelyRecursive(enumerator.Current, filterRows))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return filterRows.Count == 0;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private static bool DoesTreeMatchListEntirelyRecursive(FilterNode node, List<FilterRow> modifiableList)
		{
			bool flag;
			if (node.IsRowNode())
			{
				return modifiableList.Remove(node.Row);
			}
			List<FilterNode>.Enumerator enumerator = node.Children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (FilterNode.DoesTreeMatchListEntirelyRecursive(enumerator.Current, modifiableList))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static FilterNode FindRow(FilterNode filterTree, FilterRow row)
		{
			FilterNode filterNode;
			if (filterTree == null)
			{
				return null;
			}
			FilterNode filterNode1 = null;
			if (filterTree.IsRowNode())
			{
				if (filterTree.Row == row)
				{
					filterNode1 = filterTree;
				}
				return filterNode1;
			}
			List<FilterNode>.Enumerator enumerator = filterTree.Children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					filterNode1 = FilterNode.FindRow(enumerator.Current, row);
					if (filterNode1 == null)
					{
						continue;
					}
					filterNode = filterNode1;
					return filterNode;
				}
				return filterNode1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return filterNode;
		}

		public static List<FilterNode[]> GetGroupGrid(FilterNode filterTree, IEnumerable<FilterRow> allRows)
		{
			List<FilterNode[]> filterNodeArrays = new List<FilterNode[]>();
			if (filterTree == null || filterTree.IsRowNode())
			{
				return filterNodeArrays;
			}
			List<FilterRow> list = allRows.ToList<FilterRow>();
			for (int i = 0; i < list.Count; i++)
			{
				FilterNode filterNode = FilterNode.FindRow(filterTree, list[i]);
				if (filterNode == null)
				{
					return null;
				}
				List<FilterNode> filterNodes = new List<FilterNode>();
				for (FilterNode j = filterNode.Parent; j != null; j = j.Parent)
				{
					filterNodes.Add(j);
				}
				filterNodes.Reverse();
				for (int k = filterNodeArrays.Count; k < filterNodes.Count; k++)
				{
					filterNodeArrays.Add(new FilterNode[list.Count]);
				}
				for (int l = 0; l < filterNodes.Count; l++)
				{
					filterNodeArrays[l][i] = filterNodes[l];
				}
			}
			return filterNodeArrays;
		}

		public static void GroupNodes(ref FilterNode filterTree, IEnumerable<FilterRow> selectedRows)
		{
			if (!FilterNode.IsCurrentLevelGroupable(filterTree, selectedRows))
			{
				List<FilterNode> list = (
					from child in filterTree.Children
					where child.IsGroupNode()
					select child).ToList<FilterNode>();
				list.ForEach((FilterNode child) => FilterNode.GroupNodes(ref child, selectedRows));
				return;
			}
			List<FilterNode> filterNodes = (
				from child in filterTree.Children
				where FilterNode.IsTreeContainedInList(child, selectedRows)
				select child).ToList<FilterNode>();
			FilterNode filterNode = new FilterNode()
			{
				Parent = filterTree,
				Row = null,
				Children = new List<FilterNode>()
			};
			FilterNode filterNode1 = filterNode;
			int num = filterTree.Children.IndexOf(filterNodes.First<FilterNode>());
			foreach (FilterNode filterNode2 in filterNodes)
			{
				filterTree.Children.Remove(filterNode2);
				filterNode2.Parent = filterNode1;
				filterNode1.Children.Add(filterNode2);
			}
			filterTree.Children.Insert(num, filterNode1);
		}

		private static bool IsCurrentLevelGroupable(FilterNode filterTree, IEnumerable<FilterRow> selectedRows)
		{
			List<bool> list = (
				from child in filterTree.Children
				select FilterNode.IsTreeContainedInList(child, selectedRows)).ToList<bool>();
			if (list.All<bool>((bool b) => b))
			{
				return false;
			}
			List<bool> flags = list.SkipWhile<bool>((bool b) => !b).Reverse<bool>().SkipWhile<bool>((bool b) => !b).ToList<bool>();
			if (flags.Count<bool>() > 1 && !flags.Contains(false))
			{
				return true;
			}
			return false;
		}

		private static bool IsCurrentLevelUngroupable(FilterNode filterTree, IEnumerable<FilterRow> selectedRows)
		{
			return (
				from child in filterTree.Children
				where child.IsGroupNode()
				select child into node
				select FilterNode.DoesTreeMatchListEntirely(node, selectedRows)).Contains<bool>(true);
		}

		public static bool IsGroupable(FilterNode filterTree, IEnumerable<FilterRow> selectedRows)
		{
			if (filterTree == null || filterTree.IsRowNode() || selectedRows.Count<FilterRow>() < 2)
			{
				return false;
			}
			if (FilterNode.IsCurrentLevelGroupable(filterTree, selectedRows))
			{
				return true;
			}
			bool flag = false;
			foreach (FilterNode filterNode in 
				from child in filterTree.Children
				where child.IsGroupNode()
				select child)
			{
				flag |= FilterNode.IsGroupable(filterNode, selectedRows);
			}
			return flag;
		}

		public bool IsGroupNode()
		{
			return this.Children != null;
		}

		public bool IsRowNode()
		{
			return this.Children == null;
		}

		public static bool IsTreeContainedInList(FilterNode filterTree, IEnumerable<FilterRow> list)
		{
			bool flag;
			if (filterTree == null || list == null || list.Count<FilterRow>() < 1)
			{
				return false;
			}
			if (filterTree.IsRowNode())
			{
				return list.Contains<FilterRow>(filterTree.Row);
			}
			List<FilterNode>.Enumerator enumerator = filterTree.Children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (FilterNode.IsTreeContainedInList(enumerator.Current, list))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static bool IsUngroupable(FilterNode filterTree, IEnumerable<FilterRow> selectedRows)
		{
			bool flag;
			if (filterTree == null || filterTree.IsRowNode() || selectedRows.Count<FilterRow>() < 2)
			{
				return false;
			}
			if (FilterNode.IsCurrentLevelUngroupable(filterTree, selectedRows))
			{
				return true;
			}
			using (IEnumerator<FilterNode> enumerator = (
				from child in filterTree.Children
				where child.IsGroupNode()
				select child).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!FilterNode.IsUngroupable(enumerator.Current, selectedRows))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static void RemoveGroup(ref FilterNode filterTree, IEnumerable<FilterRow> selectedRows)
		{
			if (filterTree.IsRowNode())
			{
				return;
			}
			if (FilterNode.IsCurrentLevelUngroupable(filterTree, selectedRows))
			{
				FilterNode filterNode = filterTree.Children.Single<FilterNode>((FilterNode child) => FilterNode.DoesTreeMatchListEntirely(child, selectedRows));
				int num = filterTree.Children.IndexOf(filterNode);
				foreach (FilterNode filterNode1 in filterNode.Children)
				{
					filterNode1.Parent = filterTree;
				}
				filterTree.Children.Remove(filterNode);
				filterTree.Children.InsertRange(num, filterNode.Children);
				filterNode.Children.Clear();
			}
			List<FilterNode> list = (
				from child in filterTree.Children
				where child.IsGroupNode()
				select child).ToList<FilterNode>();
			list.ForEach((FilterNode child) => FilterNode.RemoveGroup(ref child, selectedRows));
		}

		public static void RemoveRow(ref FilterNode filterTree, FilterRow removingRow)
		{
			if (filterTree == null)
			{
				return;
			}
			if (filterTree.IsRowNode())
			{
				filterTree = null;
				return;
			}
			FilterNode filterNode = FilterNode.FindRow(filterTree, removingRow);
			if (filterNode != null)
			{
				FilterNode parent = filterNode.Parent;
				parent.Children.Remove(filterNode);
				if (parent.Children.Count == 1)
				{
					FilterNode filterNode1 = parent.Children.Single<FilterNode>();
					FilterNode parent1 = parent.Parent;
					parent.Children.Clear();
					parent.Children = null;
					if (parent1 == null)
					{
						filterNode1.Parent = null;
						filterTree = filterNode1;
						return;
					}
					int num = parent1.Children.IndexOf(parent);
					parent1.Children[num] = filterNode1;
					filterNode1.Parent = parent1;
				}
			}
		}
	}
}