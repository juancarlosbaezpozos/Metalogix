using System;
using System.Windows.Forms;

namespace Metalogix.Permissions
{
	internal class PrincipalListViewItem : ListViewItem
	{
		public SecurityPrincipal Principal
		{
			get
			{
				if (base.Tag == null)
				{
					return null;
				}
				return base.Tag as SecurityPrincipal;
			}
		}

		public PrincipalListViewItem(SecurityPrincipal principal)
		{
			this.Update(principal);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is PrincipalListViewItem))
			{
				return false;
			}
			if ((obj as PrincipalListViewItem).Principal.PrincipalName == this.Principal.PrincipalName)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Update(SecurityPrincipal principal)
		{
			base.Tag = principal;
			base.SubItems.Clear();
			if (this.Principal != null)
			{
				base.Text = this.Principal.PrincipalName;
				base.SubItems.Add(this.Principal.Type.ToString());
			}
		}
	}
}