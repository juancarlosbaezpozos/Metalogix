using System;
using System.Collections.Generic;

namespace Metalogix.DataStructures.Generic
{
    public class TreeNode<T>
    {
        private TreeNode<T> m_Parent;

        private TreeNodeList<T> m_Children;

        private T m_Value;

        public TreeNodeList<T> Children
        {
            get { return this.m_Children; }
            private set { this.m_Children = value; }
        }

        public int Height
        {
            get
            {
                int num = 0;
                if (this.Children.Count > 0)
                {
                    int num1 = 0;
                    foreach (TreeNode<T> child in this.Children)
                    {
                        int height = child.Height;
                        if (height <= num1)
                        {
                            continue;
                        }

                        num1 = height;
                    }

                    num = 1 + num1;
                }

                return num;
            }
        }

        public int NumLeafNodes
        {
            get
            {
                int numLeafNodes = 0;
                if (this.Children.Count != 0)
                {
                    foreach (TreeNode<T> child in this.Children)
                    {
                        numLeafNodes += child.NumLeafNodes;
                    }
                }
                else
                {
                    numLeafNodes++;
                }

                return numLeafNodes;
            }
        }

        public TreeNode<T> Parent
        {
            get { return this.m_Parent; }
            set
            {
                if (value == this.m_Parent)
                {
                    return;
                }

                if (this.m_Parent != null)
                {
                    this.m_Parent.Children.Remove(this);
                }

                if (value != null && !value.Children.Contains(this))
                {
                    value.Children.Add(this);
                }

                this.m_Parent = value;
            }
        }

        public TreeNode<T> Root
        {
            get
            {
                TreeNode<T> parent = this;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                return parent;
            }
        }

        public T Value
        {
            get { return this.m_Value; }
            set { this.m_Value = value; }
        }

        public TreeNode(T value)
        {
            this.Value = value;
            this.Parent = null;
            this.Children = new TreeNodeList<T>(this);
        }

        public TreeNode(T value, TreeNode<T> parent)
        {
            this.Value = value;
            this.Parent = parent;
            this.Children = new TreeNodeList<T>(this);
        }

        public TreeNode<T> Clone()
        {
            TreeNode<T> treeNode = new TreeNode<T>(this.Value);
            foreach (TreeNode<T> child in this.Children)
            {
                if (child.Children.Count != 0)
                {
                    if (child.Children.Count <= 0)
                    {
                        continue;
                    }

                    treeNode.Children.Add(child.Clone());
                }
                else
                {
                    treeNode.Children.Add(child.Value);
                }
            }

            return treeNode;
        }

        public bool Contains(TreeNode<T> targetNode)
        {
            bool flag;
            if (this == targetNode)
            {
                return true;
            }

            if (this.m_Children.Count > 0)
            {
                List<TreeNode<T>>.Enumerator enumerator = this.m_Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.Contains(targetNode))
                        {
                            continue;
                        }

                        flag = true;
                        return flag;
                    }

                    return false;
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                return flag;
            }

            return false;
        }

        public bool Contains(T value)
        {
            bool flag;
            if (this.m_Value != null && this.m_Value.Equals(value))
            {
                return true;
            }

            if (this.m_Children.Count > 0)
            {
                List<TreeNode<T>>.Enumerator enumerator = this.m_Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.Contains(value))
                        {
                            continue;
                        }

                        flag = true;
                        return flag;
                    }

                    return false;
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                return flag;
            }

            return false;
        }

        public TreeNode<T> FindNodeByValue(T value)
        {
            TreeNode<T> treeNode;
            if (this.m_Value.Equals(value))
            {
                return this;
            }

            if (this.m_Children.Count > 0)
            {
                List<TreeNode<T>>.Enumerator enumerator = this.m_Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        TreeNode<T> treeNode1 = enumerator.Current.FindNodeByValue(value);
                        if (treeNode1 == null)
                        {
                            continue;
                        }

                        treeNode = treeNode1;
                        return treeNode;
                    }

                    return null;
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                return treeNode;
            }

            return null;
        }
    }
}