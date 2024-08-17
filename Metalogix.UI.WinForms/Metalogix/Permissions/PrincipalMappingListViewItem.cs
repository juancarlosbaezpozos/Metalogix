using System;
using System.Windows.Forms;

namespace Metalogix.Permissions
{
	internal class PrincipalMappingListViewItem : ListViewItem
	{
		public MappableSecurityPrincipal MappablePrincipal
		{
			get
			{
				if (base.Tag == null)
				{
					return null;
				}
				return base.Tag as MappableSecurityPrincipal;
			}
		}

		public PrincipalMappingListViewItem(MappableSecurityPrincipal mappablePrincipal)
		{
			this.Update(mappablePrincipal);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is PrincipalMappingListViewItem))
			{
				return false;
			}
			if ((obj as PrincipalMappingListViewItem).MappablePrincipal.PrincipalName == this.MappablePrincipal.PrincipalName)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Update(MappableSecurityPrincipal mappablePrincipal)
		{
			base.Tag = mappablePrincipal;
			base.SubItems.Clear();
			if (this.MappablePrincipal != null)
			{
				base.Text = this.MappablePrincipal.PrincipalName;
				base.SubItems.Add(this.MappablePrincipal.TargetPrincipal.PrincipalName);
			}
		}
	}
}