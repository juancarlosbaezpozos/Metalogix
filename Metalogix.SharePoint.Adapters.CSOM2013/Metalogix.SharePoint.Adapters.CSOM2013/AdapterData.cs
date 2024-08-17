using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public class AdapterData
	{
		private readonly static Dictionary<Guid, AdapterData> s_CloneableData;

		private readonly static ReaderWriterLockSlim s_dictionaryLock;

		private readonly ReaderWriterLockSlim _lockUsers = new ReaderWriterLockSlim();

		private readonly ReaderWriterLockSlim _lockGroups = new ReaderWriterLockSlim();

		private readonly Dictionary<string, int> _userMap = new Dictionary<string, int>();

		private readonly Dictionary<string, int> _groupMap = new Dictionary<string, int>();

		private readonly Dictionary<int, string> _userIdMap = new Dictionary<int, string>();

		private readonly Dictionary<int, string> _groupIdMap = new Dictionary<int, string>();

		private readonly CookieManager _cookieManager;

		static AdapterData()
		{
			AdapterData.s_CloneableData = new Dictionary<Guid, AdapterData>();
			AdapterData.s_dictionaryLock = new ReaderWriterLockSlim();
		}

		private AdapterData(CSOMAdapter adapter)
		{
			this.Load(adapter);
		}

		internal void AddGroupMapping(string title, int id)
		{
			string lowerInvariant = title.ToLowerInvariant();
			bool item = false;
			this._lockGroups.EnterReadLock();
			try
			{
				if (this._groupMap.ContainsKey(lowerInvariant))
				{
					item = this._groupMap[lowerInvariant] == id;
				}
			}
			finally
			{
				this._lockGroups.ExitReadLock();
			}
			if (!item)
			{
				this._lockGroups.EnterWriteLock();
				try
				{
					if (this._groupMap.ContainsKey(lowerInvariant))
					{
						this._groupMap[lowerInvariant] = id;
					}
					else
					{
						this._groupMap.Add(lowerInvariant, id);
						this._groupIdMap.Add(id, title);
					}
				}
				finally
				{
					this._lockGroups.ExitWriteLock();
				}
			}
		}

		internal void AddUserMapping(string loginName, int id)
		{
			string lowerInvariant = loginName.ToLowerInvariant();
			bool flag = false;
			this._lockUsers.EnterReadLock();
			try
			{
				flag = this._userMap.ContainsKey(lowerInvariant);
			}
			finally
			{
				this._lockUsers.ExitReadLock();
			}
			if (!flag)
			{
				this._lockUsers.EnterWriteLock();
				try
				{
					if (!this._userMap.ContainsKey(lowerInvariant))
					{
						this._userMap.Add(lowerInvariant, id);
						this._userIdMap.Add(id, lowerInvariant);
					}
				}
				finally
				{
					this._lockUsers.ExitWriteLock();
				}
			}
		}

		internal static AdapterData GetAdapterData(CSOMAdapter adapter)
		{
			Guid siteID = adapter.SiteID;
			AdapterData item = null;
			AdapterData.s_dictionaryLock.EnterReadLock();
			try
			{
				if (AdapterData.s_CloneableData.ContainsKey(siteID))
				{
					item = AdapterData.s_CloneableData[siteID];
				}
			}
			finally
			{
				AdapterData.s_dictionaryLock.ExitReadLock();
			}
			if (item == null)
			{
				AdapterData.s_dictionaryLock.EnterWriteLock();
				try
				{
					if (!AdapterData.s_CloneableData.ContainsKey(siteID))
					{
						item = new AdapterData(adapter);
						AdapterData.s_CloneableData.Add(siteID, item);
					}
					else
					{
						item = AdapterData.s_CloneableData[siteID];
					}
				}
				finally
				{
					AdapterData.s_dictionaryLock.ExitWriteLock();
				}
			}
			return item;
		}

		internal string GetGroupFromID(int id)
		{
			string str;
			string str1;
			this._lockGroups.EnterReadLock();
			try
			{
				if (!this._groupIdMap.TryGetValue(id, out str))
				{
					return null;
				}
				else
				{
					str1 = str;
				}
			}
			finally
			{
				this._lockGroups.ExitReadLock();
			}
			return str1;
		}

		internal int GetIDFromGroup(string sGroup)
		{
			int num;
			this._lockGroups.EnterReadLock();
			try
			{
				int num1 = -1;
				string lowerInvariant = sGroup.ToLowerInvariant();
				if (!this._groupMap.TryGetValue(lowerInvariant, out num1))
				{
					return -1;
				}
				else
				{
					num = num1;
				}
			}
			finally
			{
				this._lockGroups.ExitReadLock();
			}
			return num;
		}

		internal int GetIDFromUser(string sUser)
		{
			int num;
			int num1 = -1;
			string lowerInvariant = Utils.ConvertWinOrFormsUserToClaimString(sUser).ToLowerInvariant();
			string str = Utils.ConvertClaimStringUserToWinOrFormsUser(sUser).ToLowerInvariant();
			this._lockUsers.EnterReadLock();
			try
			{
				if (this._userMap.TryGetValue(lowerInvariant, out num1))
				{
					num = num1;
				}
				else if (!this._userMap.TryGetValue(str, out num1))
				{
					return -1;
				}
				else
				{
					num = num1;
				}
			}
			finally
			{
				this._lockUsers.ExitReadLock();
			}
			return num;
		}

		internal string GetUserFromID(int id)
		{
			string str;
			string str1;
			this._lockUsers.EnterReadLock();
			try
			{
				if (!this._userIdMap.TryGetValue(id, out str))
				{
					return null;
				}
				else
				{
					str1 = str;
				}
			}
			finally
			{
				this._lockUsers.ExitReadLock();
			}
			return str1;
		}

		private void Load(CSOMAdapter adapter)
		{
			UserCollection userCollection;
			GroupCollection groupCollection;
			adapter.GetPrincipalCollectionsForMaps(out userCollection, out groupCollection);
			this.UpdateUserMap(userCollection);
			this.UpdateGroupMap(groupCollection);
		}

		internal void RemoveGroupMapping(string title)
		{
			int num;
			string lowerInvariant = title.ToLowerInvariant();
			bool flag = false;
			this._lockGroups.EnterReadLock();
			try
			{
				flag = this._groupMap.ContainsKey(lowerInvariant);
			}
			finally
			{
				this._lockGroups.ExitReadLock();
			}
			if (flag)
			{
				this._lockGroups.EnterWriteLock();
				try
				{
					if (this._groupMap.TryGetValue(lowerInvariant, out num))
					{
						this._groupMap.Remove(lowerInvariant);
						this._groupIdMap.Remove(num);
					}
				}
				finally
				{
					this._lockGroups.ExitWriteLock();
				}
			}
		}

		internal void UpdateGroupMap(GroupCollection groups)
		{
			this._lockGroups.EnterWriteLock();
			try
			{
				this._groupMap.Clear();
				this._groupIdMap.Clear();
				foreach (Group group in groups)
				{
					this._groupMap.Add(group.Title.ToLowerInvariant(), group.Id);
					this._groupIdMap.Add(group.Id, group.Title);
				}
			}
			finally
			{
				this._lockGroups.ExitWriteLock();
			}
		}

		internal void UpdateUserMap(UserCollection users)
		{
			this._lockUsers.EnterWriteLock();
			try
			{
				this._userMap.Clear();
				this._userIdMap.Clear();
				foreach (User user in users)
				{
					this._userMap.Add(user.LoginName.ToLowerInvariant(), user.Id);
					this._userIdMap.Add(user.Id, user.LoginName);
				}
			}
			finally
			{
				this._lockUsers.ExitWriteLock();
			}
		}
	}
}