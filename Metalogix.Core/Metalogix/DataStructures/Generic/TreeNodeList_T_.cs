using System;
using System.Collections.Generic;

namespace Metalogix.DataStructures.Generic
{
    public class TreeNodeList<T> : List<TreeNode<T>>
    {
        public TreeNode<T> m_Parent;

        public TreeNodeList(TreeNode<T> parent)
        {
            this.m_Parent = parent;
        }

        public TreeNode<T> Add(TreeNode<T> node)
        {
            base.Add(node);
            node.Parent = this.m_Parent;
            return node;
        }

        public new TreeNode<T> Add(T value)
        {
            return this.Add(new TreeNode<T>(value));
        }

        public override string ToString()
        {
            return string.Concat("Count=", base.Count.ToString());
        }
    }
}