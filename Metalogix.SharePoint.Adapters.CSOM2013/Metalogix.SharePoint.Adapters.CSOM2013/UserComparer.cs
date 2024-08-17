using Metalogix.SharePoint.Adapters;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public class UserComparer : IEqualityComparer<User>
	{
		public UserComparer()
		{
		}

		public bool Equals(User x, User y)
		{
			string loginName = x.LoginName;
			string claimString = y.LoginName;
			if (!x.LoginName.Contains("|"))
			{
				loginName = Utils.ConvertWinOrFormsUserToClaimString(x.LoginName);
			}
			if (!y.LoginName.Contains("|"))
			{
				claimString = Utils.ConvertWinOrFormsUserToClaimString(y.LoginName);
			}
			if (loginName == claimString)
			{
				return true;
			}
			return false;
		}

		public int GetHashCode(User obj)
		{
			return Utils.ConvertWinOrFormsUserToClaimString(obj.LoginName).GetHashCode();
		}
	}
}