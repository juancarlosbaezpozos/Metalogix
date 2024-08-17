using System;
using System.Windows.Forms;

namespace Metalogix.Permissions
{
	internal class RoleListViewItem : ListViewItem
	{
		public Metalogix.Permissions.Role Role
		{
			get
			{
				if (base.Tag == null)
				{
					return null;
				}
				return base.Tag as Metalogix.Permissions.Role;
			}
		}

		public RoleListViewItem(Metalogix.Permissions.Role role)
		{
			this.Update(role);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is RoleListViewItem))
			{
				return false;
			}
			if ((obj as RoleListViewItem).Role.RoleName == this.Role.RoleName)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Update(Metalogix.Permissions.Role role)
		{
			base.Tag = role;
			if (this.Role != null)
			{
				base.Text = this.Role.RoleName;
			}
		}
	}
}