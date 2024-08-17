using Metalogix;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPGlobalMappings
	{
		private const string SPOClaim = "i:0#.f|membership|";

		private const string WindowsClaim = "i:0#.w|";

		private readonly static string USER_MAPPINGS_FILE_NAME;

		private readonly static string DOMAIN_MAPPINGS_FILE_NAME;

		private readonly static string URL_MAPPINGS_FILE_NAME;

		private readonly static string GUID_MAPPINGS_FILE_NAME;

		private static MappingsCollection m_globalUserMappings;

		private static MappingsCollection m_globalDomainMappings;

		private static SerializableTable<string, string> m_globalUrlMappings;

		private static SerializableTable<Guid, Guid> m_globalGuidMappings;

		public static MappingsCollection GlobalDomainMappings
		{
			get
			{
				if (SPGlobalMappings.m_globalDomainMappings == null)
				{
					SPGlobalMappings.Load();
				}
				return SPGlobalMappings.m_globalDomainMappings;
			}
			set
			{
				SPGlobalMappings.m_globalDomainMappings = value;
			}
		}

		public static SerializableTable<Guid, Guid> GlobalGuidMappings
		{
			get
			{
				if (SPGlobalMappings.m_globalGuidMappings == null)
				{
					SPGlobalMappings.Load();
				}
				return SPGlobalMappings.m_globalGuidMappings;
			}
			set
			{
				SPGlobalMappings.m_globalGuidMappings = value;
			}
		}

		public static SerializableTable<string, string> GlobalUrlMappings
		{
			get
			{
				if (SPGlobalMappings.m_globalUrlMappings == null)
				{
					SPGlobalMappings.Load();
				}
				return SPGlobalMappings.m_globalUrlMappings;
			}
			set
			{
				SPGlobalMappings.m_globalUrlMappings = value;
			}
		}

		public static MappingsCollection GlobalUserMappings
		{
			get
			{
				if (SPGlobalMappings.m_globalUserMappings == null)
				{
					SPGlobalMappings.Load();
				}
				return SPGlobalMappings.m_globalUserMappings;
			}
			set
			{
				SPGlobalMappings.m_globalUserMappings = value;
			}
		}

		static SPGlobalMappings()
		{
			SPGlobalMappings.USER_MAPPINGS_FILE_NAME = "UserMappings.mls";
			SPGlobalMappings.DOMAIN_MAPPINGS_FILE_NAME = "DomainMappings.mls";
			SPGlobalMappings.URL_MAPPINGS_FILE_NAME = "UrlMappings.mls";
			SPGlobalMappings.GUID_MAPPINGS_FILE_NAME = "GuidMappings.mls";
			SPGlobalMappings.m_globalUserMappings = null;
			SPGlobalMappings.m_globalDomainMappings = null;
			SPGlobalMappings.m_globalUrlMappings = null;
			SPGlobalMappings.m_globalGuidMappings = null;
			ApplicationData.MainAssemblyChanged += new EventHandler(SPGlobalMappings.MainAssemblyChanged);
		}

		public SPGlobalMappings()
		{
		}

		private static void Clear()
		{
			SPGlobalMappings.m_globalDomainMappings = null;
			SPGlobalMappings.m_globalGuidMappings = null;
			SPGlobalMappings.m_globalUrlMappings = null;
			SPGlobalMappings.m_globalUserMappings = null;
		}

		public static string ExtractDomain(string loginName, bool isTargetSharePointOnline)
		{
			string str;
			string str1 = loginName;
			try
			{
				str1 = str1.Substring(0, str1.IndexOf("\\", StringComparison.Ordinal));
				str = (isTargetSharePointOnline ? SPUtils.RemoveClaimString(str1) : str1);
			}
			catch
			{
				str = str1;
			}
			return str;
		}

		private static string GetAutoMappedUser(string loginName, bool isTargetSharePointOnline)
		{
			string str;
			try
			{
				string sPOMappedUser = SPGlobalMappings.GetSPOMappedUser(loginName, isTargetSharePointOnline, null);
				if (!string.IsNullOrEmpty(sPOMappedUser))
				{
					str = sPOMappedUser;
					return str;
				}
			}
			catch (Exception exception)
			{
			}
			str = loginName;
			return str;
		}

		private static string GetDomainMappedUser(string loginName, bool isTargetSharePointOnline, ListSummaryItem domainMappingFromUsername)
		{
			string str;
			try
			{
				string sPOMappedUser = SPGlobalMappings.GetSPOMappedUser(loginName, isTargetSharePointOnline, SPUtils.RemoveClaimString(domainMappingFromUsername.Target.Target));
				str = (!string.IsNullOrEmpty(sPOMappedUser) ? sPOMappedUser : loginName.ToLowerInvariant().Replace(domainMappingFromUsername.Source.Target.ToLowerInvariant(), domainMappingFromUsername.Target.Target.ToUpperInvariant()));
				return str;
			}
			catch (Exception exception)
			{
			}
			str = loginName;
			return str;
		}

		public static ListSummaryItem GetMapListSummaryItem(string loginName, bool isTargetSharePointOnline)
		{
			string str = SPGlobalMappings.Map(loginName, isTargetSharePointOnline);
			ListSummaryItem listSummaryItem = new ListSummaryItem()
			{
				Source = SPUtils.CreateUserItem(loginName),
				Target = SPUtils.CreateUserItem(str)
			};
			return listSummaryItem;
		}

		private static string GetSPOMappedUser(string loginName, bool isTargetSharePointOnline, string domain = null)
		{
			string empty;
			bool flag;
			if (!isTargetSharePointOnline || SPGlobalMappings.IsSharePointDefaultUser(loginName) || loginName.IndexOf("\\", StringComparison.Ordinal) <= -1)
			{
				flag = true;
			}
			else
			{
				flag = (loginName.IndexOf('|') == -1 ? false : !loginName.StartsWith("i:0#.w|", StringComparison.Ordinal));
			}
			if (flag)
			{
				empty = string.Empty;
			}
			else
			{
				string[] strArrays = SPUtils.RemoveClaimString(loginName).ToLower().Split(new char[] { '\\' });
				empty = string.Format("{0}{1}@{2}.com", "i:0#.f|membership|", strArrays[1], (string.IsNullOrEmpty(domain) ? strArrays[0] : domain));
			}
			return empty;
		}

		private static bool IsSharePointDefaultUser(string loginName)
		{
			string[] strArrays = new string[] { "NT AUTHORITY\\", "SHAREPOINT\\", "Built-in\\", "c:0" };
			bool flag = (
				from item in strArrays
				select SPUtils.RemoveClaimString(loginName.ToLowerInvariant()).StartsWith(item.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase)).Any<bool>((bool result) => result);
			return flag;
		}

		private static void Load()
		{
			XmlDocument xmlDocument;
			Exception exception;
			try
			{
				if (!File.Exists(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.USER_MAPPINGS_FILE_NAME)))
				{
					SPGlobalMappings.m_globalUserMappings = new MappingsCollection();
				}
				else
				{
					xmlDocument = new XmlDocument();
					xmlDocument.Load(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.USER_MAPPINGS_FILE_NAME));
					SPGlobalMappings.m_globalUserMappings = new MappingsCollection(xmlDocument.FirstChild);
				}
			}
			catch (Exception exception1)
			{
				exception = exception1;
			}
			try
			{
				if (!File.Exists(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.DOMAIN_MAPPINGS_FILE_NAME)))
				{
					SPGlobalMappings.m_globalDomainMappings = new MappingsCollection();
				}
				else
				{
					xmlDocument = new XmlDocument();
					xmlDocument.Load(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.DOMAIN_MAPPINGS_FILE_NAME));
					SPGlobalMappings.m_globalDomainMappings = new MappingsCollection(xmlDocument.FirstChild);
				}
			}
			catch (Exception exception2)
			{
				exception = exception2;
			}
			try
			{
				if (!File.Exists(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.URL_MAPPINGS_FILE_NAME)))
				{
					SPGlobalMappings.m_globalUrlMappings = new CommonSerializableTable<string, string>();
				}
				else
				{
					xmlDocument = new XmlDocument();
					xmlDocument.Load(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.URL_MAPPINGS_FILE_NAME));
					SPGlobalMappings.m_globalUrlMappings = new CommonSerializableTable<string, string>(xmlDocument.FirstChild);
				}
			}
			catch (Exception exception3)
			{
				exception = exception3;
			}
			try
			{
				if (!File.Exists(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.GUID_MAPPINGS_FILE_NAME)))
				{
					SPGlobalMappings.m_globalGuidMappings = new CommonSerializableTable<Guid, Guid>();
				}
				else
				{
					xmlDocument = new XmlDocument();
					xmlDocument.Load(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.GUID_MAPPINGS_FILE_NAME));
					SPGlobalMappings.m_globalGuidMappings = new CommonSerializableTable<Guid, Guid>(xmlDocument.FirstChild);
				}
			}
			catch (Exception exception4)
			{
				exception = exception4;
				SPGlobalMappings.m_globalGuidMappings = new CommonSerializableTable<Guid, Guid>();
			}
		}

		private static void MainAssemblyChanged(object sender, EventArgs e)
		{
			SPGlobalMappings.Save();
			SPGlobalMappings.Clear();
		}

		public static SPUserCollection Map(SPUserCollection users, SPWeb targetWeb)
		{
			SPUserCollection sPUserCollection;
			try
			{
				SPUser[] tag = new SPUser[users.Count];
				int num = 0;
				foreach (SPUser user in (IEnumerable<SecurityPrincipal>)users)
				{
					ListSummaryItem item = SPGlobalMappings.GlobalUserMappings[user.LoginName];
					if (item != null)
					{
						SecurityPrincipal securityPrincipal = targetWeb.SiteUsers[item.Target.Target];
						if (securityPrincipal != null)
						{
							tag[num] = (SPUser)securityPrincipal;
						}
						else
						{
							tag[num] = (SPUser)item.Target.Tag;
						}
					}
					else
					{
						ListSummaryItem listSummaryItem = SPGlobalMappings.GlobalDomainMappings[SPGlobalMappings.ExtractDomain(user.LoginName, false)];
						if (listSummaryItem != null)
						{
							tag[num] = (SPUser)SPGlobalMappings.MapObjectViaDomainMapping(user, listSummaryItem, false);
						}
						else
						{
							tag[num] = user;
						}
					}
					num++;
				}
				sPUserCollection = new SPUserCollection(tag);
			}
			catch (Exception exception)
			{
				sPUserCollection = users;
			}
			return sPUserCollection;
		}

		public static string Map(string loginName, bool isTargetSharePointOnline)
		{
			string str;
			try
			{
				ListSummaryItem item = SPGlobalMappings.GlobalUserMappings[loginName];
				if (item != null)
				{
					str = (!(item.Target.Tag is SPUser) ? loginName : ((SPUser)item.Target.Tag).LoginName);
				}
				else
				{
					ListSummaryItem listSummaryItem = SPGlobalMappings.GlobalDomainMappings[SPGlobalMappings.ExtractDomain(loginName, isTargetSharePointOnline)];
					str = (listSummaryItem != null ? (string)SPGlobalMappings.MapObjectViaDomainMapping(loginName, listSummaryItem, isTargetSharePointOnline) : SPGlobalMappings.GetAutoMappedUser(loginName, isTargetSharePointOnline));
				}
			}
			catch (Exception exception)
			{
				str = loginName;
			}
			return str;
		}

		public static SecurityPrincipal Map(SecurityPrincipal principal, bool isTargetSharePointOnline)
		{
			SecurityPrincipal tag;
			if (!(principal is SPUser))
			{
				tag = principal;
			}
			else
			{
				ListSummaryItem item = SPGlobalMappings.GlobalUserMappings[principal.PrincipalName];
				if (item != null)
				{
					tag = (SPUser)item.Target.Tag;
				}
				else
				{
					ListSummaryItem listSummaryItem = SPGlobalMappings.GlobalDomainMappings[SPGlobalMappings.ExtractDomain(principal.PrincipalName, isTargetSharePointOnline)];
					tag = (listSummaryItem != null ? (SPUser)SPGlobalMappings.MapObjectViaDomainMapping(principal, listSummaryItem, isTargetSharePointOnline) : SPUtils.GetSPUserObject(SPGlobalMappings.GetAutoMappedUser(principal.PrincipalName, isTargetSharePointOnline)));
				}
			}
			return tag;
		}

		public static object MapObjectViaDomainMapping(object input, ListSummaryItem domainMappingFromUsername, bool isTargetSharePointOnline)
		{
			object sPUserObject;
			if (input is SPUser)
			{
				try
				{
					string domainMappedUser = SPGlobalMappings.GetDomainMappedUser(((SPUser)input).LoginName, isTargetSharePointOnline, domainMappingFromUsername);
					sPUserObject = SPUtils.GetSPUserObject(domainMappedUser);
				}
				catch
				{
					sPUserObject = input;
				}
			}
			else if (!(input is string))
			{
				sPUserObject = input;
			}
			else
			{
				sPUserObject = SPGlobalMappings.GetDomainMappedUser((string)input, isTargetSharePointOnline, domainMappingFromUsername);
			}
			return sPUserObject;
		}

		public static void Save()
		{
			Exception exception;
			try
			{
				File.WriteAllText(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.USER_MAPPINGS_FILE_NAME), SPGlobalMappings.GlobalUserMappings.ToXML());
			}
			catch (Exception exception1)
			{
				exception = exception1;
			}
			try
			{
				File.WriteAllText(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.DOMAIN_MAPPINGS_FILE_NAME), SPGlobalMappings.GlobalDomainMappings.ToXML());
			}
			catch (Exception exception2)
			{
				exception = exception2;
			}
			try
			{
				File.WriteAllText(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.URL_MAPPINGS_FILE_NAME), SPGlobalMappings.GlobalUrlMappings.ToXML());
			}
			catch (Exception exception3)
			{
				exception = exception3;
			}
			try
			{
				File.WriteAllText(Path.Combine(ApplicationData.ApplicationPath, SPGlobalMappings.GUID_MAPPINGS_FILE_NAME), SPGlobalMappings.GlobalGuidMappings.ToXML());
			}
			catch (Exception exception4)
			{
				exception = exception4;
			}
		}
	}
}