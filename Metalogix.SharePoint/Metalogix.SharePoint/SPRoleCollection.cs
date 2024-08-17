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
	public class SPRoleCollection : RoleCollection
	{
		private SharePointVersion m_SharePointVersion = new SharePointVersion();

		private bool m_bIsPortal = false;

		private bool m_bIgnoreList = false;

		private SPWeb m_parentWeb = null;

		private SPList m_parentList = null;

		public override Type CollectionType
		{
			get
			{
				Type type;
				IEnumerator<Role> enumerator = base.GetEnumerator();
				try
				{
					if (enumerator.MoveNext())
					{
						type = ((SPRole)enumerator.Current).GetType();
						return type;
					}
				}
				finally
				{
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				type = typeof(SPRole);
				return type;
			}
		}

		internal bool IgnoreList
		{
			get
			{
				return this.m_bIgnoreList;
			}
			set
			{
				this.m_bIgnoreList = value;
			}
		}

		public override Role this[string sRoleName]
		{
			get
			{
				Role role;
				foreach (SPRole sPRole in this)
				{
					if (sPRole.RoleName == sRoleName)
					{
						role = sPRole;
						return role;
					}
				}
				role = null;
				return role;
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
					foreach (SPRole sPRole in this)
					{
						if (num != index)
						{
							num++;
						}
						else
						{
							obj = sPRole;
							return obj;
						}
					}
				}
				obj = null;
				return obj;
			}
		}

		public SPList ParentList
		{
			get
			{
				return this.m_parentList;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public SPRoleCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
		}

		public SPRoleCollection(SPList parentList, SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
			this.m_parentList = parentList;
		}

		public override void Add(Role item)
		{
			this.AddOrUpdateRole(item);
		}

		public override Role AddOrUpdateRole(Role role)
		{
			string xML;
			XmlDocument xmlDocument;
			SPRole item;
			Role role1;
			SPNode mParentList;
			SPRole sPRole2007;
			if (this.m_parentList != null)
			{
				mParentList = this.m_parentList;
			}
			else
			{
				mParentList = this.m_parentWeb;
			}
			SPNode sPNode = mParentList;
			if (!sPNode.WriteVirtually)
			{
				if (this.m_parentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				if (this.CollectionType != typeof(SPRole2007))
				{
					throw new Exception("Write access is not allowed for this type of role collection");
				}
				SPRole2007 sPRole20071 = (SPRole2007)RoleConverter.ConvertRole(role, typeof(SPRole2007));
				if (sPRole20071 == null)
				{
					object[] fullName = new object[] { role.GetType().FullName, typeof(SPRole2007).FullName };
					throw new ArgumentException(string.Format("The underlying adapter only supports adding roles of type {1}.\nNo conversion is defined between roles of type {0} and {1}", fullName));
				}
				xML = this.m_parentWeb.Adapter.Writer.AddOrUpdateRole(sPRole20071.RoleName, sPRole20071.Description, sPRole20071.PermissionMask);
				xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xML);
				item = (SPRole)this[sPRole20071.RoleName];
				if (item != null)
				{
					base.RemoveFromCollection(item);
				}
				sPRole20071 = new SPRole2007(xmlDocument.DocumentElement);
				base.AddToCollection(sPRole20071);
				role1 = sPRole20071;
			}
			else
			{
				if (this.CollectionType != role.GetType())
				{
					throw new Exception(string.Format("The given role does not match the type of the collection. The role is {0}, the collection is {1}", this.CollectionType.Name, role.GetType().Name));
				}
				xML = role.ToXML();
				xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xML);
				XmlNode documentElement = xmlDocument.DocumentElement;
				string str = base.ToXML();
				item = (SPRole)this[role.RoleName];
				if (item != null)
				{
					base.RemoveFromCollection(item);
				}
				if (this.m_SharePointVersion.IsSharePoint2007OrLater)
				{
					sPRole2007 = new SPRole2007(documentElement);
				}
				else if (this.m_bIsPortal)
				{
					sPRole2007 = new SPS2003Role(documentElement);
				}
				else
				{
					sPRole2007 = new SPRole2003(documentElement);
				}
				SPRole sPRole = sPRole2007;
				base.AddToCollection(sPRole);
				string xML1 = base.ToXML();
				sPNode.SaveVirtualData(XmlUtility.StringToXmlNode(str), XmlUtility.StringToXmlNode(xML1), "Roles");
				role1 = sPRole;
			}
			return role1;
		}

		public override bool DeleteRole(string sRoleName)
		{
			SPRole item;
			bool flag;
			SPNode mParentList;
			if (this.m_parentList != null)
			{
				mParentList = this.m_parentList;
			}
			else
			{
				mParentList = this.m_parentWeb;
			}
			SPNode sPNode = mParentList;
			if (!sPNode.WriteVirtually)
			{
				if (this.m_parentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				if (this.CollectionType != typeof(SPRole2007))
				{
					throw new Exception("Write access is not allowed for this type of role collection");
				}
				item = (SPRole)this[sRoleName];
				if (item == null)
				{
					flag = false;
				}
				else
				{
					this.m_parentWeb.Adapter.Writer.DeleteRole(item.RoleName);
					flag = base.RemoveFromCollection(item);
				}
			}
			else
			{
				bool flag1 = false;
				item = (SPRole)this[sRoleName];
				if (item != null)
				{
					string xML = base.ToXML();
					flag1 = base.RemoveFromCollection(item);
					if (flag1)
					{
						string str = base.ToXML();
						sPNode.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(str), "Roles");
					}
				}
				flag = flag1;
			}
			return flag;
		}

		public void FetchData()
		{
			string constantID;
			string str = null;
			if (!this.IgnoreList)
			{
				if (this.m_parentList != null)
				{
					constantID = this.m_parentList.ConstantID;
				}
				else
				{
					constantID = null;
				}
				str = constantID;
			}
			string roles = this.ParentWeb.Adapter.Reader.GetRoles(str);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(roles);
			this.FromXML((this.m_parentList != null ? this.m_parentList.AttachVirtualData(xmlDocument.DocumentElement, "Roles") : this.m_parentWeb.AttachVirtualData(xmlDocument.DocumentElement, "Roles")));
		}

		public override void FromXML(XmlNode xmlNode)
		{
			Role sPRole2007;
			this.m_SharePointVersion = new SharePointVersion(xmlNode.Attributes["SharePointVersion"].Value);
			this.m_bIsPortal = (xmlNode.Attributes["IsPortal"] != null ? bool.Parse(xmlNode.Attributes["IsPortal"].Value) : false);
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//Role"))
			{
				if (this.m_SharePointVersion.IsSharePoint2007OrLater)
				{
					sPRole2007 = new SPRole2007(xmlNodes);
				}
				else if (this.m_bIsPortal)
				{
					sPRole2007 = new SPS2003Role(xmlNodes);
				}
				else
				{
					sPRole2007 = new SPRole2003(xmlNodes);
				}
				base.AddToCollection(sPRole2007);
			}
		}

		public override bool Remove(Role item)
		{
			return this.DeleteRole(item.RoleName);
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Roles");
			xmlWriter.WriteAttributeString("SharePointVersion", this.m_SharePointVersion.ToString());
			xmlWriter.WriteAttributeString("IsPortal", this.m_bIsPortal.ToString());
			foreach (SPRole sPRole in this)
			{
				xmlWriter.WriteRaw(sPRole.XML);
			}
			xmlWriter.WriteEndElement();
		}
	}
}