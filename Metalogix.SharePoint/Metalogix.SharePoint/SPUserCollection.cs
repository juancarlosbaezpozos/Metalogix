using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPUserCollection : SecurityPrincipalCollection
	{
		private SPWeb m_parentWeb;

		private object m_oLockWritingUsers = new object();

		public override Type CollectionType
		{
			get
			{
				return typeof(SPUser);
			}
		}

		public override object this[int index]
		{
			get
			{
				object obj;
				int num = 0;
				if (index < base.Count)
				{
					foreach (SPUser sPUser in (IEnumerable<SecurityPrincipal>)this)
					{
						if (num != index)
						{
							num++;
						}
						else
						{
							obj = sPUser;
							return obj;
						}
					}
				}
				obj = null;
				return obj;
			}
		}

		public override SecurityPrincipal this[SecurityPrincipal key]
		{
			get
			{
				return this.GetByLoginName(key.PrincipalName);
			}
		}

		public override SecurityPrincipal this[string sPrincipalName]
		{
			get
			{
				return this.GetByLoginName(sPrincipalName);
			}
		}

		public SPUserCollection()
		{
		}

		public SPUserCollection(XmlNode node) : base(node)
		{
		}

		public SPUserCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
		}

		public SPUserCollection(SPUser[] users)
		{
			lock (this.m_oLockWritingUsers)
			{
				this.ClearCollection();
				SPUser[] sPUserArray = users;
				for (int i = 0; i < (int)sPUserArray.Length; i++)
				{
					SPUser sPUser = sPUserArray[i];
					if (sPUser != null)
					{
						base.AddToCollection(sPUser);
					}
				}
			}
		}

		public override void Add(SecurityPrincipal item)
		{
			if (item is SPUser)
			{
				base.Add((SPUser)item, false);
			}
			else if (this.AddPrincipal(item) == null)
			{
				throw new Exception("Can't add node because it can't be converted to an SPUser");
			}
		}

		public override SecurityPrincipal AddPrincipal(SecurityPrincipal principal)
		{
			SecurityPrincipal securityPrincipal;
			SPUser sPUser = (SPUser)PrincipalConverter.ConvertPrincipal(principal, typeof(SPUser));
			if (sPUser == null)
			{
				securityPrincipal = null;
			}
			else
			{
				securityPrincipal = this.AddPrincipal(sPUser, false);
			}
			return securityPrincipal;
		}

		public SecurityPrincipal AddPrincipal(SecurityPrincipal principal, bool bAllowDBWrite)
		{
			SecurityPrincipal securityPrincipal;
			SPUser sPUser = (SPUser)PrincipalConverter.ConvertPrincipal(principal, typeof(SPUser));
			if (sPUser == null)
			{
				securityPrincipal = null;
			}
			else
			{
				securityPrincipal = this.AddUser(sPUser, bAllowDBWrite);
			}
			return securityPrincipal;
		}

		public SPUser AddUser(SPUser newUser, bool bAllowDBWrite)
		{
			SPUser sPUser;
			lock (this.m_oLockWritingUsers)
			{
				SPUser byLoginName = this.GetByLoginName(newUser.LoginName);
				if (byLoginName == null)
				{
					SPUser sPUser1 = null;
					if ((this.m_parentWeb == null ? true : !this.m_parentWeb.WriteVirtually))
					{
						AddUserOptions addUserOption = new AddUserOptions()
						{
							AllowDBWrite = bAllowDBWrite,
							AllowDBWriteEnvironment = SharePointConfigurationVariables.AllowDBWriting
						};
						string str = (this.m_parentWeb != null ? this.m_parentWeb.Adapter.Writer.AddSiteUser(newUser.XML, addUserOption) : newUser.XML);
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(str);
						sPUser1 = new SPUser(xmlDocument.FirstChild);
						base.AddToCollection(sPUser1);
					}
					else
					{
						string xML = base.ToXML();
						sPUser1 = new SPUser(XmlUtility.StringToXmlNode(newUser.XML));
						base.AddToCollection(sPUser1);
						string xML1 = base.ToXML();
						this.m_parentWeb.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(xML1), "Users");
					}
					sPUser = sPUser1;
				}
				else
				{
					sPUser = byLoginName;
				}
			}
			return sPUser;
		}

		public bool Contains(string sLoginName)
		{
			return this.GetByLoginName(sLoginName) != null;
		}

		public override void DeletePrincipal(SecurityPrincipal principal)
		{
		}

		public void FetchData()
		{
			lock (this.m_oLockWritingUsers)
			{
				this.ClearCollection();
				string siteUsers = this.m_parentWeb.Adapter.Reader.GetSiteUsers();
				if (siteUsers != null)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(siteUsers);
					this.m_parentWeb.AttachVirtualData(xmlDocument.DocumentElement, "Users");
					foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//User"))
					{
						if (xmlNodes.Attributes.Count != 0)
						{
							base.AddToCollection(new SPUser(xmlNodes));
						}
					}
				}
				else
				{
					return;
				}
			}
		}

		public override void FromXML(XmlNode xmlNode)
		{
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//User"))
			{
				base.AddToCollection(new SPUser(xmlNodes));
			}
		}

		public SPUser GetByLoginName(string sLoginName)
		{
			string str;
			SPUser item;
			sLoginName = sLoginName.Trim().ToLowerInvariant();
			if ((this.m_parentWeb == null ? false : !this.m_parentWeb.Adapter.SharePointVersion.IsSharePoint2007OrEarlier))
			{
				bool flag = Utils.SwitchUserNameFormat(sLoginName, out str);
				SPUser sPUser = null;
				SPUser item1 = base[sLoginName] as SPUser;
				if (item1 == null)
				{
					if (flag)
					{
						sPUser = base[str] as SPUser;
					}
					if (sPUser == null)
					{
						item = null;
					}
					else
					{
						item = sPUser;
					}
				}
				else
				{
					item = item1;
				}
			}
			else
			{
				item = base[sLoginName] as SPUser;
			}
			return item;
		}

		public string[] GetSiteAdminLoginNames()
		{
			List<string> strs = new List<string>();
			foreach (SPUser sPUser in (IEnumerable<SecurityPrincipal>)this)
			{
				if (sPUser.IsSiteAdmin)
				{
					strs.Add(sPUser.LoginName);
				}
			}
			return strs.ToArray();
		}

		public IEnumerable<SPUser> GetSiteAdmins()
		{
			List<SPUser> sPUsers = new List<SPUser>();
			foreach (SPUser sPUser in (IEnumerable<SecurityPrincipal>)this)
			{
				if (sPUser.IsSiteAdmin)
				{
					sPUsers.Add(sPUser);
				}
			}
			return sPUsers;
		}

		public SPUser GetUserByID(string id)
		{
			SPUser sPUser;
			foreach (SPUser sPUser1 in (IEnumerable<SecurityPrincipal>)this)
			{
				if ((sPUser1.ID == null ? false : sPUser1.ID == id))
				{
					sPUser = sPUser1;
					return sPUser;
				}
			}
			sPUser = null;
			return sPUser;
		}

		public override bool Remove(SecurityPrincipal item)
		{
			throw new NotImplementedException();
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Users");
			foreach (SPUser sPUser in (IEnumerable<SecurityPrincipal>)this)
			{
				xmlWriter.WriteRaw(sPUser.XML);
			}
			xmlWriter.WriteEndElement();
		}
	}
}