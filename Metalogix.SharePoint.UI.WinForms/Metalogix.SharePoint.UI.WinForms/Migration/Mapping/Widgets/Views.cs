using Metalogix.Data.Mapping;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class Views
	{
		public Views()
		{
		}

		[Default]
		public class RoleNameView : ListView<SPRole>
		{
			public override string Name
			{
				get
				{
					return "Role Name";
				}
			}

			public RoleNameView()
			{
			}

			public override string Render(SPRole item)
			{
				return item.RoleName;
			}
		}

		[Default]
		public class SecurityPrincipalComparer : ListPickerComparer<SecurityPrincipal, SecurityPrincipal>
		{
			public SecurityPrincipalComparer()
			{
			}

			public override int Compare(SecurityPrincipal source, SecurityPrincipal target)
			{
				if (source.PrincipalName.Equals(target.PrincipalName, StringComparison.InvariantCultureIgnoreCase))
				{
					return 0;
				}
				return -1;
			}
		}

		public class SecurityPrincipalDisplayNameView : ListView<SecurityPrincipal>
		{
			public override string Name
			{
				get
				{
					return "Display Name";
				}
			}

			public SecurityPrincipalDisplayNameView()
			{
			}

			public override string Render(SecurityPrincipal item)
			{
				if (!(item is SPUser))
				{
					return item.PrincipalName;
				}
				return ((SPUser)item).Name;
			}
		}

		[Default]
		public class SecurityPrincipalView : ListView<SecurityPrincipal>
		{
			public override string Name
			{
				get
				{
					return "Login Name";
				}
			}

			public SecurityPrincipalView()
			{
			}

			public override string Render(SecurityPrincipal item)
			{
				return item.PrincipalName;
			}
		}

		[Default]
		public class SPRoleComparer : ListPickerComparer<SPRole, SPRole>
		{
			public SPRoleComparer()
			{
			}

			public override int Compare(SPRole source, SPRole target)
			{
				if (source.RoleName.Equals(target.RoleName, StringComparison.InvariantCultureIgnoreCase))
				{
					return 0;
				}
				return -1;
			}
		}

		public class SPUserComparer : ListPickerComparer<SPUser, SPUser>
		{
			public SPUserComparer()
			{
			}

			public override int Compare(SPUser source, SPUser target)
			{
				if (source.LoginName.Equals(target.LoginName, StringComparison.InvariantCultureIgnoreCase))
				{
					return 0;
				}
				return -1;
			}
		}
	}
}