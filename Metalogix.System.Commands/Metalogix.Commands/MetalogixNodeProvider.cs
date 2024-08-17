using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace Metalogix.Commands
{
	public abstract class MetalogixNodeProvider : NavigationCmdletProvider
	{
		private static char[] Seperators;

		static MetalogixNodeProvider()
		{
			MetalogixNodeProvider.Seperators = new char[] { '/' };
		}

		protected MetalogixNodeProvider()
		{
		}

		public Node FindNode(string sPath)
		{
			Node node = ((NodeDriveInfo)base.PSDriveInfo).Connection.Node;
			if (this.PathIsDrive(sPath))
			{
				node.FetchChildren();
			}
			else
			{
				node = node.GetNodeByPath(this.GetDriveRelativePath(sPath));
			}
			return node;
		}

		protected override void GetChildItems(string path, bool recurse)
		{
			foreach (Node child in this.FindNode(path).Children)
			{
				string str = string.Concat(path, (path.EndsWith("/") ? "" : "/"), child.Name);
				base.WriteItemObject(child, str, true);
				if (!recurse)
				{
					continue;
				}
				this.GetChildItems(str, recurse);
			}
		}

		protected override string GetChildName(string path)
		{
			Node node = this.FindNode(path);
			if (node == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("The argument specified is not valid"), "InvalidArgument", ErrorCategory.InvalidArgument, path));
			}
			return node.Name;
		}

		protected override void GetChildNames(string path, ReturnContainers returnContainers)
		{
			foreach (Node child in this.FindNode(path).Children)
			{
				base.WriteItemObject(child.Name, string.Concat(path, (path.EndsWith("/") ? "" : "/"), child.Name), true);
			}
		}

		private string GetDriveRelativePath(string sPath)
		{
			return this.NormalizeRelativePath(sPath, base.PSDriveInfo.Root);
		}

		protected override void GetItem(string path)
		{
			Node node = this.FindNode(path);
			path = path.Replace("\\", "/");
			if (node == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("The argument specified is not valid"), "InvalidArgument", ErrorCategory.InvalidArgument, path));
				return;
			}
			base.WriteItemObject(node, string.Concat(base.PSDriveInfo.Root, '/', node.Path), node.Children.Count > 0);
		}

		protected override string GetParentPath(string path, string root)
		{
			string str = path.Replace("\\", "/");
			int num = str.LastIndexOf("/");
			if (num < 0)
			{
				return "";
			}
			return str.Substring(0, num);
		}

		protected override bool HasChildItems(string path)
		{
			Node node = this.FindNode(path);
			if (node == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("The argument specified is not valid"), "InvalidArgument", ErrorCategory.InvalidArgument, path));
			}
			return node.Children.Count > 0;
		}

		protected override bool IsItemContainer(string path)
		{
			Node node = this.FindNode(path);
			if (node == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("The argument specified is not valid"), "InvalidArgument", ErrorCategory.InvalidArgument, path));
			}
			if (typeof(ListItem).IsAssignableFrom(node.GetType()))
			{
				return false;
			}
			return true;
		}

		protected override bool IsValidPath(string path)
		{
			if (path != null && path.Length > 0)
			{
				return true;
			}
			return false;
		}

		protected override bool ItemExists(string path)
		{
			return this.FindNode(path) != null;
		}

		protected override string MakePath(string parent, string child)
		{
			string str = string.Concat(parent.Replace("\\", "/").TrimEnd(MetalogixNodeProvider.Seperators), "/", child.TrimStart(MetalogixNodeProvider.Seperators));
			return str;
		}

		protected override string NormalizeRelativePath(string path, string basePath)
		{
			string str = path.Replace("\\", "/");
			string str1 = basePath.Replace("\\", "/");
			string str2 = str;
			if (str.StartsWith(str1))
			{
				str2 = str.Substring(str1.Length, str.Length - str1.Length);
			}
			str2 = str2.TrimStart(new char[] { '/' });
			return str2;
		}

		private bool PathIsDrive(string sPath)
		{
			return this.GetDriveRelativePath(sPath) == string.Empty;
		}
	}
}