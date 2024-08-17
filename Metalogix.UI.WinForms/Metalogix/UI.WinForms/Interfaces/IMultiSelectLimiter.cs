using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Interfaces
{
	public interface IMultiSelectLimiter
	{
		bool Check(TreeNode nodeToBeSelected, ReadOnlyCollection<TreeNode> selectedNodes);
	}
}