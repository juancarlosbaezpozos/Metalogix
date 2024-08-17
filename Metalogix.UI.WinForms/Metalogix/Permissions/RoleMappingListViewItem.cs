using System;
using System.Windows.Forms;

namespace Metalogix.Permissions
{
	internal class RoleMappingListViewItem : ListViewItem
	{
		public Metalogix.Permissions.MappableRole MappableRole
		{
			get
			{
				if (base.Tag == null)
				{
					return null;
				}
				return base.Tag as Metalogix.Permissions.MappableRole;
			}
		}

		public RoleMappingListViewItem(Metalogix.Permissions.MappableRole mappableRole)
		{
			this.Update(mappableRole);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is RoleMappingListViewItem))
			{
				return false;
			}
			if ((obj as RoleMappingListViewItem).MappableRole.RoleName == this.MappableRole.RoleName)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Update(Metalogix.Permissions.MappableRole mappableRole)
		{
			base.Tag = mappableRole;
			base.SubItems.Clear();
			if (this.MappableRole != null)
			{
				base.Text = this.MappableRole.RoleName;
				base.SubItems.Add(this.MappableRole.TargetRole.RoleName);
			}
		}
	}
}