using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("Group")]
	[PluralName("Groups")]
	public class SPGroup : SecurityPrincipal, Metalogix.DataStructures.IComparable
	{
		private SPWeb m_parentWeb = null;

		private SPUserCollection m_users = null;

		public bool AllowMembersEditMembership
		{
			get
			{
				bool flag = false;
				if (this.m_XML.Attributes["AllowMembersEditMembership"] != null)
				{
					flag = bool.Parse((string.IsNullOrEmpty(this.m_XML.Attributes["AllowMembersEditMembership"].Value) ? "false" : this.m_XML.Attributes["AllowMembersEditMembership"].Value));
				}
				return flag;
			}
		}

		public bool AllowRequestToJoinLeave
		{
			get
			{
				bool flag = false;
				if (this.m_XML.Attributes["AllowRequestToJoinLeave"] != null)
				{
					flag = bool.Parse((string.IsNullOrEmpty(this.m_XML.Attributes["AllowRequestToJoinLeave"].Value) ? "false" : this.m_XML.Attributes["AllowRequestToJoinLeave"].Value));
				}
				return flag;
			}
		}

		public bool AutoAcceptRequestToJoinLeave
		{
			get
			{
				bool flag = false;
				if (this.m_XML.Attributes["AutoAcceptRequestToJoinLeave"] != null)
				{
					flag = bool.Parse((string.IsNullOrEmpty(this.m_XML.Attributes["AutoAcceptRequestToJoinLeave"].Value) ? "false" : this.m_XML.Attributes["AutoAcceptRequestToJoinLeave"].Value));
				}
				return flag;
			}
		}

		public string Description
		{
			get
			{
				return this.m_XML.Attributes["Description"].Value;
			}
		}

		public string ID
		{
			get
			{
				string value;
				if (this.m_XML.Attributes["ID"] != null)
				{
					value = this.m_XML.Attributes["ID"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_XML.Attributes["Name"].Value;
			}
		}

		public bool OnlyAllowMembersViewMembership
		{
			get
			{
				bool flag = true;
				if (this.m_XML.Attributes["OnlyAllowMembersViewMembership"] != null)
				{
					flag = bool.Parse((string.IsNullOrEmpty(this.m_XML.Attributes["OnlyAllowMembersViewMembership"].Value) ? "true" : this.m_XML.Attributes["OnlyAllowMembersViewMembership"].Value));
				}
				return flag;
			}
		}

		public SecurityPrincipal Owner
		{
			get
			{
				SecurityPrincipal item;
				if (this.ParentWeb == null)
				{
					item = null;
				}
				else if (!this.OwnerIsUser)
				{
					item = this.ParentWeb.Groups[this.m_XML.Attributes["Owner"].Value];
				}
				else
				{
					item = this.ParentWeb.SiteUsers.GetByLoginName(this.m_XML.Attributes["Owner"].Value);
				}
				return item;
			}
		}

		public bool OwnerIsUser
		{
			get
			{
				bool flag = bool.Parse(this.m_XML.Attributes["OwnerIsUser"].Value);
				return flag;
			}
		}

		public string OwnerName
		{
			get
			{
				return this.m_XML.Attributes["Owner"].Value;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public override string PrincipalName
		{
			get
			{
				return this.Name;
			}
		}

		public string RequestToJoinLeaveEmailSetting
		{
			get
			{
				string empty = string.Empty;
				if (this.m_XML.Attributes["OnlyAllowMembersViewMembership"] != null)
				{
					empty = (string.IsNullOrEmpty(this.m_XML.Attributes["RequestToJoinLeaveEmailSetting"].Value) ? string.Empty : this.m_XML.Attributes["RequestToJoinLeaveEmailSetting"].Value);
				}
				return empty;
			}
		}

		public override SecurityPrincipalType Type
		{
			get
			{
				return SecurityPrincipalType.Group;
			}
		}

		public int UserCount
		{
			get
			{
				int num;
				num = (this.m_users == null ? this.m_XML.SelectNodes("./Member").Count : this.m_users.Count);
				return num;
			}
		}

		public SPUserCollection Users
		{
			get
			{
				if (this.m_users == null)
				{
					if (this.ParentWeb == null)
					{
						throw new ArgumentNullException("ParentWeb cannot be null.");
					}
					List<SPUser> sPUsers = new List<SPUser>();
					foreach (XmlNode xmlNodes in this.m_XML.SelectNodes("./Member"))
					{
						SPUser byLoginName = this.ParentWeb.SiteUsers.GetByLoginName(xmlNodes.Attributes["Login"].Value);
						if (byLoginName != null)
						{
							sPUsers.Add(byLoginName);
						}
					}
					this.m_users = new SPUserCollection(sPUsers.ToArray());
				}
				return this.m_users;
			}
		}

		public SPGroup(XmlNode xmlNode) : base(xmlNode)
		{
		}

		public SPGroup(XmlNode xmlNode, SPWeb parentWeb) : base(xmlNode)
		{
			this.m_parentWeb = parentWeb;
		}

		public SPGroup(string sName, string sDescription, SecurityPrincipal owner, SPUser[] users) : base(null)
		{
			if (owner == null)
			{
				this.m_XML = SPGroup.BuildGroupXml(sName, sDescription, true, null, users);
				this.m_users = new SPUserCollection(users);
			}
			else if (!(owner is SPGroup))
			{
				if (!(owner is SPUser))
				{
					throw new ArgumentException("Owner is not a SharePoint security principal");
				}
				this.m_XML = SPGroup.BuildGroupXml(sName, sDescription, true, owner, users);
				this.m_users = new SPUserCollection(users);
			}
			else
			{
				this.m_XML = SPGroup.BuildGroupXml(sName, sDescription, false, owner, users);
				this.m_users = new SPUserCollection(users);
			}
		}

		public SPGroup(SPGroup sourceGroup, SPUser[] users) : base(null)
		{
			SecurityPrincipal owner = sourceGroup.Owner;
			if (owner == null)
			{
				this.m_XML = SPGroup.BuildGroupXmlWithMetadata(sourceGroup, true, null, users);
				this.m_users = new SPUserCollection(users);
			}
			else if (!(owner is SPGroup))
			{
				if (!(owner is SPUser))
				{
					throw new ArgumentException("Owner is not a SharePoint security principal");
				}
				this.m_XML = SPGroup.BuildGroupXmlWithMetadata(sourceGroup, true, owner, users);
				this.m_users = new SPUserCollection(users);
			}
			else
			{
				this.m_XML = SPGroup.BuildGroupXmlWithMetadata(sourceGroup, false, owner, users);
				this.m_users = new SPUserCollection(users);
			}
		}

		public static XmlNode BuildGroupXml(string sName, string sDescription, bool bOwnerIsUser, SecurityPrincipal owner, SPUser[] users)
		{
			sName = SPUtils.SanitizeInput(sName);
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Group");
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Name");
			XmlAttribute xmlAttribute1 = xmlDocument.CreateAttribute("Description");
			XmlAttribute str = xmlDocument.CreateAttribute("OwnerIsUser");
			XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("Owner");
			xmlAttribute.Value = sName;
			xmlAttribute1.Value = sDescription;
			str.Value = bOwnerIsUser.ToString();
			xmlAttribute2.Value = (owner != null ? owner.PrincipalName : sName);
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute1);
			xmlElement.Attributes.Append(str);
			xmlElement.Attributes.Append(xmlAttribute2);
			if (users != null)
			{
				SPUser[] sPUserArray = users;
				for (int i = 0; i < (int)sPUserArray.Length; i++)
				{
					SPUser sPUser = sPUserArray[i];
					XmlElement xmlElement1 = xmlDocument.CreateElement("Member");
					XmlAttribute loginName = xmlDocument.CreateAttribute("Login");
					loginName.Value = sPUser.LoginName;
					xmlElement1.Attributes.Append(loginName);
					xmlElement.AppendChild(xmlElement1);
				}
			}
			return xmlElement;
		}

		public static XmlNode BuildGroupXmlWithMetadata(SPGroup sourceGroup, bool bOwnerIsUser, SecurityPrincipal owner, SPUser[] users)
		{
			XmlNode xmlNodes = SPGroup.BuildGroupXml(sourceGroup.Name, sourceGroup.Description, sourceGroup.OwnerIsUser, sourceGroup.Owner, users);
			XmlAttribute str = xmlNodes.OwnerDocument.CreateAttribute("AllowRequestToJoinLeave");
			XmlAttribute xmlAttribute = xmlNodes.OwnerDocument.CreateAttribute("AutoAcceptRequestToJoinLeave");
			XmlAttribute str1 = xmlNodes.OwnerDocument.CreateAttribute("AllowMembersEditMembership");
			XmlAttribute xmlAttribute1 = xmlNodes.OwnerDocument.CreateAttribute("OnlyAllowMembersViewMembership");
			XmlAttribute requestToJoinLeaveEmailSetting = xmlNodes.OwnerDocument.CreateAttribute("RequestToJoinLeaveEmailSetting");
			str.Value = sourceGroup.AllowRequestToJoinLeave.ToString();
			xmlAttribute.Value = sourceGroup.AutoAcceptRequestToJoinLeave.ToString();
			str1.Value = sourceGroup.AllowMembersEditMembership.ToString();
			xmlAttribute1.Value = sourceGroup.OnlyAllowMembersViewMembership.ToString();
			requestToJoinLeaveEmailSetting.Value = sourceGroup.RequestToJoinLeaveEmailSetting;
			xmlNodes.Attributes.Append(str);
			xmlNodes.Attributes.Append(xmlAttribute);
			xmlNodes.Attributes.Append(str1);
			xmlNodes.Attributes.Append(xmlAttribute1);
			xmlNodes.Attributes.Append(requestToJoinLeaveEmailSetting);
			return xmlNodes;
		}

		private void CommitChanges()
		{
			if (this.ParentWeb != null)
			{
				string str = this.ParentWeb.Adapter.Writer.AddOrUpdateGroup(this.XML);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(str);
				this.m_XML = XmlUtility.StringToXmlNode(xmlDocument.DocumentElement.SelectSingleNode(".//Group").OuterXml);
				this.m_users = null;
			}
		}

		public override float GetSimilarity(SecurityPrincipal principal)
		{
			float single;
			if (principal is SPGroup)
			{
				SPGroup sPGroup = (SPGroup)principal;
				int num = 0;
				int count = 0;
				SPUserCollection users = sPGroup.Users;
				SPUserCollection sPUserCollection = this.Users;
				if ((users.Count != 0 ? true : sPUserCollection.Count != 0))
				{
					foreach (SPUser user in (IEnumerable<SecurityPrincipal>)users)
					{
						if (sPUserCollection.Contains(user.LoginName))
						{
							num += 2;
						}
					}
					count = users.Count * 2 + sPUserCollection.Count * 2 - num;
					single = (float)num / (float)count;
				}
				else
				{
					single = 1f;
				}
			}
			else
			{
				single = 0f;
			}
			return single;
		}

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			if (targetComparable == null)
			{
				throw new Exception("Cannot compare an SPGroup to a null node'");
			}
			if (!(targetComparable is SPGroup))
			{
				throw new Exception(string.Concat("Cannot compare an SPGroup to a: '", targetComparable.GetType().ToString(), "'"));
			}
			SPGroup sPGroup = (SPGroup)targetComparable;
			foreach (SPUser user in (IEnumerable<SecurityPrincipal>)this.Users)
			{
				if (sPGroup.Users[user.LoginName] == null)
				{
					if (sPGroup.ParentWeb.SiteUsers[user.LoginName] == null)
					{
						differencesOutput.Write(string.Concat("The user ", user.LoginName, " is not included, but is unavailable. This is expected when a user no longer exists in Active Directory"), user.LoginName, DifferenceStatus.Difference, true);
					}
					else
					{
						differencesOutput.Write(string.Concat("The user ", user.LoginName, " is available, but not included in the group"), user.LoginName);
						flag = false;
						return flag;
					}
				}
			}
			if (this.Owner == null)
			{
				if (sPGroup.Owner != null)
				{
					differencesOutput.Write("The source owner is null and the target isn't", "Owner");
					flag = false;
					return flag;
				}
			}
			if (this.OwnerIsUser != sPGroup.OwnerIsUser)
			{
				differencesOutput.Write("The owner is of a different type", "Owner type");
				flag = false;
			}
			else if ((!this.OwnerIsUser ? true : !(this.OwnerName != sPGroup.OwnerName)))
			{
				if (!this.OwnerIsUser)
				{
					DifferenceLog differenceLogs = new DifferenceLog();
					if (!((SPGroup)this.Owner).IsEqual((SPGroup)sPGroup.Owner, differenceLogs, options))
					{
						differencesOutput.Write(string.Concat("The owning group is different: ", differenceLogs.ToString()), differenceLogs.ToString());
						flag = false;
						return flag;
					}
				}
				if (this.Name != sPGroup.Name)
				{
					differencesOutput.Write("The name attribute is different", "Name");
					flag = false;
				}
				else if (!(this.Description != sPGroup.Description))
				{
					flag = true;
				}
				else
				{
					differencesOutput.Write("The description attribute is different", "Description");
					flag = false;
				}
			}
			else
			{
				differencesOutput.Write("The owner is a different user", "Owner");
				flag = false;
			}
			return flag;
		}

		public void SetName(string sNewName)
		{
			this.SetName(sNewName, true);
		}

		public void SetName(string sNewName, bool bCommitChanges)
		{
			this.m_XML.Attributes["Name"].Value = sNewName;
			if (bCommitChanges)
			{
				this.CommitChanges();
			}
		}

		public void SetOwner(SecurityPrincipal owner)
		{
			this.SetOwner(owner, true);
		}

		public void SetOwner(SecurityPrincipal owner, bool bCommitChanges)
		{
			if (!(owner is SPGroup))
			{
				this.SetOwner(owner.PrincipalName, true, bCommitChanges);
			}
			else
			{
				this.SetOwner(owner.PrincipalName, false, bCommitChanges);
			}
		}

		public void SetOwner(string sOwnerName, bool bOwnerIsUser)
		{
			this.SetOwner(sOwnerName, bOwnerIsUser, true);
		}

		public void SetOwner(string sOwnerName, bool bOwnerIsUser, bool bCommitChanges)
		{
			SecurityPrincipal item;
			if (this.ParentWeb != null)
			{
				if (!bOwnerIsUser)
				{
					item = this.ParentWeb.Groups[sOwnerName];
				}
				else
				{
					item = this.ParentWeb.SiteUsers.GetByLoginName(sOwnerName);
				}
				if (item == null)
				{
					throw new ArgumentOutOfRangeException("The new owner is not a valid security principal on this web");
				}
			}
			this.m_XML.Attributes["Owner"].Value = sOwnerName;
			this.m_XML.Attributes["OwnerIsUser"].Value = bOwnerIsUser.ToString();
			if (bCommitChanges)
			{
				this.CommitChanges();
			}
		}

		public void UpdateMembership(IEnumerable<SPUser> spUsers, bool ownerIsUser, SecurityPrincipal owner)
		{
			List<SPUser> list = this.Users.Cast<SPUser>().ToList<SPUser>();
			list.AddRange(spUsers);
			XmlNode xmlNodes = SPGroup.BuildGroupXmlWithMetadata(this, ownerIsUser, owner, list.ToArray());
			this.m_XML = xmlNodes;
			this.CommitChanges();
		}
	}
}