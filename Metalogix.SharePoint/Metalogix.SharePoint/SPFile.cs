using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("Other Root File")]
	[PluralName("Other Root Files")]
	public class SPFile
	{
		private string m_displayUrl = null;

		private byte[] _content;

		public string Author
		{
			get;
			set;
		}

		[IsSystem(true)]
		public SPCustomizedPageStatus CustomizedPageStatus
		{
			get;
			set;
		}

		public string DisplayUrl
		{
			get
			{
				if (this.m_displayUrl == null)
				{
					string[] displayUrl = new string[] { this.Web.DisplayUrl, this.WebRelativeUrl };
					this.m_displayUrl = UrlUtils.ConcatUrls(displayUrl);
				}
				return this.m_displayUrl;
			}
		}

		public string ModifiedBy
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string ServerRelativeUrl
		{
			get
			{
				string[] serverRelativeUrl = new string[] { this.Web.ServerRelativeUrl, this.WebRelativeUrl };
				return UrlUtils.ConcatUrls(serverRelativeUrl);
			}
		}

		public DateTime TimeCreated
		{
			get;
			private set;
		}

		public DateTime TimeLastModified
		{
			get;
			private set;
		}

		public Guid UniqueId
		{
			get;
			private set;
		}

		public SPWeb Web
		{
			get;
			private set;
		}

		public string WebRelativeUrl
		{
			get;
			private set;
		}

		[IsSystem(true)]
		public string XML
		{
			get;
			private set;
		}

		public SPFile(SPWeb web, XmlNode node)
		{
			this.Web = web;
			this.FromXml(node);
		}

		private void FromXml(XmlNode node)
		{
			string value;
			string str;
			string value1;
			this.Author = node.Attributes["Author"].Value;
			this.CustomizedPageStatus = (SPCustomizedPageStatus)Enum.Parse(typeof(SPCustomizedPageStatus), node.Attributes["CustomizedPageStatus"].Value, true);
			if (node.Attributes["ModifiedBy"] != null)
			{
				value = node.Attributes["ModifiedBy"].Value;
			}
			else
			{
				value = null;
			}
			this.ModifiedBy = value;
			if (node.Attributes["Name"] != null)
			{
				str = node.Attributes["Name"].Value;
			}
			else
			{
				str = null;
			}
			this.Name = str;
			this.TimeCreated = Utils.ParseDateAsUtc(node.Attributes["TimeCreated"].Value);
			this.TimeLastModified = Utils.ParseDateAsUtc(node.Attributes["TimeLastModified"].Value);
			this.UniqueId = new Guid(node.Attributes["UniqueId"].Value);
			if (node.Attributes["Url"] != null)
			{
				value1 = node.Attributes["Url"].Value;
			}
			else
			{
				value1 = null;
			}
			this.WebRelativeUrl = value1;
			this.XML = node.OuterXml;
		}

		public byte[] GetContent()
		{
			if (this._content == null)
			{
				ISharePointReader reader = this.Web.Adapter.Reader;
				Guid uniqueId = this.UniqueId;
				this._content = reader.GetDocument(uniqueId.ToString(), UrlUtils.RemoveAfterLastSlash(this.ServerRelativeUrl), this.Name, 1);
			}
			return this._content;
		}

		public SecurityPrincipalCollection GetReferencedPrincipals()
		{
			Hashtable hashtables = new Hashtable();
			if (!string.IsNullOrEmpty(this.ModifiedBy))
			{
				hashtables.Add(this.ModifiedBy, this.ModifiedBy);
			}
			if ((string.IsNullOrEmpty(this.Author) ? false : !hashtables.ContainsKey(this.Author)))
			{
				hashtables.Add(this.Author, this.Author);
			}
			return SPUtils.GetReferencedPrincipals(hashtables, this.Web.SiteUsers, false);
		}

		public void ReleaseContent()
		{
			this._content = null;
		}

		public void SetContent(byte[] content)
		{
			this._content = content;
		}
	}
}