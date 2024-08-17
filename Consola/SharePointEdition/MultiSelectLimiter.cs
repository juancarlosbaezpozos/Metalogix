using Metalogix.SharePoint;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Metalogix.SharePointEdition
{
    public class MultiSelectLimiter : IMultiSelectLimiter
    {
        public MultiSelectLimiter()
        {
        }

        public bool Check(TreeNode treeNodeToSelect, ReadOnlyCollection<TreeNode> otherTreeNodes)
        {
            SPNode sharePointNodeFromTreeNode = GetSharePointNodeFromTreeNode(treeNodeToSelect);
            if (sharePointNodeFromTreeNode == null)
            {
                return false;
            }

            bool count = otherTreeNodes.Count > 1;
            bool parent = treeNodeToSelect.Parent == null;
            foreach (TreeNode otherTreeNode in otherTreeNodes)
            {
                if (otherTreeNode.Parent == null)
                {
                    continue;
                }

                parent = false;
                break;
            }

            SPNode sPNode = null;
            if (otherTreeNodes.Count > 0)
            {
                sPNode = GetSharePointNodeFromTreeNode(otherTreeNodes[0]);
            }

            if (sPNode == null)
            {
                return false;
            }

            if (!CompareNodeTypes(sharePointNodeFromTreeNode, sPNode, parent, count))
            {
                return false;
            }

            if (!parent && !typeof(SPServer).IsAssignableFrom(sharePointNodeFromTreeNode.GetType()))
            {
                if (IsFolder(sharePointNodeFromTreeNode))
                {
                    if (!InSameFolderCollection(sharePointNodeFromTreeNode, sPNode))
                    {
                        return false;
                    }
                }
                else if (!InSameSiteCollection(sharePointNodeFromTreeNode, sPNode))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareNodeTypes(SPNode node1, SPNode node2, bool bAllRootNodes,
            bool bTripleOrGreaterSelect)
        {
            if (bAllRootNodes && typeof(SPConnection).IsAssignableFrom(node1.GetType()) &&
                typeof(SPConnection).IsAssignableFrom(node2.GetType()))
            {
                return true;
            }

            if (bAllRootNodes && bTripleOrGreaterSelect)
            {
                return false;
            }

            if (typeof(SPServer).IsAssignableFrom(node1.GetType()))
            {
                if (!typeof(SPServer).IsAssignableFrom(node2.GetType()))
                {
                    return false;
                }
            }
            else if (typeof(SPWeb).IsAssignableFrom(node1.GetType()))
            {
                if (!typeof(SPWeb).IsAssignableFrom(node2.GetType()))
                {
                    return false;
                }
            }
            else if (typeof(SPList).IsAssignableFrom(node1.GetType()))
            {
                if (!typeof(SPList).IsAssignableFrom(node2.GetType()))
                {
                    return false;
                }
            }
            else if (typeof(SPFolder).IsAssignableFrom(node1.GetType()) &&
                     !typeof(SPFolder).IsAssignableFrom(node2.GetType()))
            {
                return false;
            }

            return true;
        }

        private static SPList GetParentListFromNode(SPNode node)
        {
            if (typeof(SPList).IsAssignableFrom(node.GetType()))
            {
                return null;
            }

            if (!typeof(SPFolder).IsAssignableFrom(node.GetType()))
            {
                return null;
            }

            return ((SPFolder)node).ParentList;
        }

        private static SPWeb GetRootWebFromNode(SPNode node)
        {
            if (typeof(SPWeb).IsAssignableFrom(node.GetType()))
            {
                return ((SPWeb)node).RootWeb;
            }

            if (typeof(SPList).IsAssignableFrom(node.GetType()))
            {
                return ((SPList)node).ParentWeb.RootWeb;
            }

            if (!typeof(SPFolder).IsAssignableFrom(node.GetType()))
            {
                return null;
            }

            return ((SPFolder)node).ParentList.ParentWeb.RootWeb;
        }

        private static SPNode GetSharePointNodeFromTreeNode(TreeNode node)
        {
            ExplorerTreeNode explorerTreeNode = node as ExplorerTreeNode;
            if (explorerTreeNode == null)
            {
                return null;
            }

            return explorerTreeNode.Node as SPNode;
        }

        private static bool InSameFolderCollection(SPNode node1, SPNode node2)
        {
            SPList parentListFromNode = GetParentListFromNode(node1);
            SPList sPList = GetParentListFromNode(node2);
            if (parentListFromNode == null)
            {
                return false;
            }

            return ReferenceEquals(parentListFromNode, sPList);
        }

        private static bool InSameSiteCollection(SPNode node1, SPNode node2)
        {
            return ReferenceEquals(GetRootWebFromNode(node1), GetRootWebFromNode(node2));
        }

        private static bool IsFolder(SPNode node)
        {
            if (typeof(SPList).IsAssignableFrom(node.GetType()))
            {
                return false;
            }

            if (typeof(SPFolder).IsAssignableFrom(node.GetType()))
            {
                return true;
            }

            return false;
        }
    }
}