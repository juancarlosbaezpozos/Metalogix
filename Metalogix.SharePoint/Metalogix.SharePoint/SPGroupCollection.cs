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
	public class SPGroupCollection : SecurityPrincipalCollection
	{
		private SPWeb m_parentWeb = null;

		private object m_oLockWritingGroups = new object();

		public override Type CollectionType
		{
			get
			{
				return typeof(SPGroup);
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
					foreach (SPGroup sPGroup in (IEnumerable<SecurityPrincipal>)this)
					{
						if (num != index)
						{
							num++;
						}
						else
						{
							obj = sPGroup;
							return obj;
						}
					}
				}
				obj = null;
				return obj;
			}
		}

		public override SecurityPrincipal this[string sPrincipalName]
		{
			get
			{
				SecurityPrincipal item;
				if (!string.IsNullOrEmpty(sPrincipalName))
				{
					item = base[sPrincipalName.ToLowerInvariant()];
				}
				else
				{
					item = null;
				}
				return item;
			}
		}

		public SPGroupCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
		}

		public SPGroupCollection(SPGroup[] groups)
		{
			lock (this.m_oLockWritingGroups)
			{
				this.ClearCollection();
				base.AddRangeToCollection(groups);
			}
		}

		public override void Add(SecurityPrincipal item)
		{
			if (!(item is SPGroup))
			{
				throw new Exception("The node being added is not a SPGroup");
			}
			this.AddOrUpdateGroup((SPGroup)item);
		}

		public SPGroup AddOrUpdateGroup(SPGroup group)
		{
			XmlNode xmlNodes = null;
			return this.AddOrUpdateGroup(group, out xmlNodes);
		}

		public SPGroup AddOrUpdateGroup(SPGroup group, out XmlNode resultNode)
		{
			SPGroup item;
			bool writeVirtually = this.m_parentWeb.WriteVirtually;
			if ((writeVirtually ? false : this.m_parentWeb.Adapter.Writer == null))
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			lock (this.m_oLockWritingGroups)
			{
				string xML = group.XML;
				string str = null;
				if (!writeVirtually)
				{
					string str1 = this.m_parentWeb.Adapter.Writer.AddOrUpdateGroup(xML);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(str1);
					resultNode = xmlDocument.SelectSingleNode(".//Results");
					xML = xmlDocument.SelectSingleNode(".//Group").OuterXml;
				}
				else
				{
					str = base.ToXML();
					resultNode = XmlUtility.StringToXmlNode("<Results Failures='0' />");
				}
				SPGroup sPGroup = (SPGroup)this[group.Name];
				if (sPGroup != null)
				{
					base.RemoveFromCollection(sPGroup);
				}
				base.AddToCollection(new SPGroup(XmlUtility.StringToXmlNode(xML), this.m_parentWeb));
				if (writeVirtually)
				{
					string xML1 = base.ToXML();
					this.m_parentWeb.SaveVirtualData(XmlUtility.StringToXmlNode(str), XmlUtility.StringToXmlNode(xML1), "Groups");
				}
				item = (SPGroup)this[group.Name];
			}
			return item;
		}

		public override SecurityPrincipal AddPrincipal(SecurityPrincipal principal)
		{
			SecurityPrincipal securityPrincipal;
			SPGroup sPGroup = (SPGroup)PrincipalConverter.ConvertPrincipal(principal, typeof(SPGroup));
			if (sPGroup == null)
			{
				securityPrincipal = null;
			}
			else
			{
				securityPrincipal = this.AddOrUpdateGroup(sPGroup);
			}
			return securityPrincipal;
		}

		public bool DeleteGroup(SPGroup group)
		{
			bool flag;
			flag = (!this.Contains(group) ? false : this.DeleteGroup(group.Name));
			return flag;
		}

		public bool DeleteGroup(string sGroupName)
		{
			SPGroup item;
			bool flag;
			if (!this.m_parentWeb.WriteVirtually)
			{
				if (this.m_parentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				lock (this.m_oLockWritingGroups)
				{
					item = (SPGroup)this[sGroupName];
					if (item == null)
					{
						flag = false;
					}
					else
					{
						this.m_parentWeb.Adapter.Writer.DeleteGroup(sGroupName);
						flag = base.RemoveFromCollection(item);
					}
				}
			}
			else
			{
				bool flag1 = false;
				item = (SPGroup)this[sGroupName];
				if (item != null)
				{
					string xML = base.ToXML();
					flag1 = base.RemoveFromCollection(item);
					if (flag1)
					{
						string str = base.ToXML();
						this.m_parentWeb.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(str), "Groups");
					}
				}
				flag = flag1;
			}
			return flag;
		}

		public override void DeletePrincipal(SecurityPrincipal principal)
		{
			if (principal is SPGroup)
			{
				this.DeleteGroup((SPGroup)principal);
			}
		}

		public void FetchData()
		{
			lock (this.m_oLockWritingGroups)
			{
				string groups = this.m_parentWeb.Adapter.Reader.GetGroups();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(groups);
				XmlNode xmlNodes = this.m_parentWeb.AttachVirtualData(xmlDocument.DocumentElement, "Groups");
				this.FromXML(xmlNodes);
			}
		}

		public override void FromXML(XmlNode xmlNode)
		{
			lock (this.m_oLockWritingGroups)
			{
				this.ClearCollection();
				foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//Group"))
				{
					base.AddToCollection(new SPGroup(xmlNodes, this.m_parentWeb));
				}
			}
		}

		public SPGroup GetGroupByID(string id)
		{
			SPGroup sPGroup;
			foreach (SPGroup sPGroup1 in (IEnumerable<SecurityPrincipal>)this)
			{
				if ((sPGroup1.ID == null ? false : sPGroup1.ID == id))
				{
					sPGroup = sPGroup1;
					return sPGroup;
				}
			}
			sPGroup = null;
			return sPGroup;
		}

		public override bool Remove(SecurityPrincipal item)
		{
			if (!(item is SPGroup))
			{
				throw new Exception("The node being removed is not a SPGroup");
			}
			return this.DeleteGroup((SPGroup)item);
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Groups");
			foreach (SPGroup sPGroup in (IEnumerable<SecurityPrincipal>)this)
			{
				xmlWriter.WriteRaw(sPGroup.XML);
			}
			xmlWriter.WriteEndElement();
		}
	}
}