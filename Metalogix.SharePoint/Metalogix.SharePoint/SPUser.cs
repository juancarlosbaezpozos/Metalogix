using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Permissions;
using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("User")]
	public class SPUser : SecurityPrincipal, Metalogix.DataStructures.IComparable
	{
		public string Email
		{
			get
			{
				string value;
				if (this.m_XML.Attributes["Email"] != null)
				{
					value = this.m_XML.Attributes["Email"].Value;
				}
				else
				{
					value = null;
				}
				return value;
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

		public string ImageName
		{
			get
			{
				return "User.ico";
			}
		}

		public bool IsSiteAdmin
		{
			get
			{
				bool flag = false;
				if (this.m_XML.Attributes["IsSiteAdmin"] != null)
				{
					string value = this.m_XML.Attributes["IsSiteAdmin"].Value;
					bool.TryParse(value, out flag);
				}
				return flag;
			}
		}

		public string LoginName
		{
			get
			{
				string value;
				if (this.m_XML.Attributes["LoginName"] != null)
				{
					value = this.m_XML.Attributes["LoginName"].Value;
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
				string str;
				str = (this.m_XML.Attributes["Name"] != null ? this.m_XML.Attributes["Name"].Value : this.LoginName);
				return str;
			}
		}

		public string Notes
		{
			get
			{
				string value;
				if (this.m_XML.Attributes["Notes"] != null)
				{
					value = this.m_XML.Attributes["Notes"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public override string PrincipalName
		{
			get
			{
				return this.LoginName;
			}
		}

		public string SID
		{
			get
			{
				string value;
				if (this.m_XML.Attributes["SID"] != null)
				{
					value = this.m_XML.Attributes["SID"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public override SecurityPrincipalType Type
		{
			get
			{
				return SecurityPrincipalType.User;
			}
		}

		public SPUser(XmlNode userXML) : base(userXML)
		{
		}

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			if (targetComparable == null)
			{
				throw new Exception("Cannot compare an SPUser to a null node'");
			}
			if (!(targetComparable is SPUser))
			{
				throw new Exception(string.Concat("Cannot compare an SPUser to a: '", targetComparable.GetType().ToString(), "'"));
			}
			SPUser sPUser = (SPUser)targetComparable;
			if (!(sPUser.LoginName.ToLower() != this.LoginName.ToLower()))
			{
				if (sPUser.Name != this.Name)
				{
					differencesOutput.Write("The 'Name' attribute is different. ", "Name");
				}
				if (sPUser.Email != this.Email)
				{
					differencesOutput.Write("The 'Email' attribute is different. ", "Email");
				}
				if (sPUser.Notes != this.Notes)
				{
					differencesOutput.Write("The 'Notes' attribute is different. ", "Notes");
				}
				flag = true;
			}
			else
			{
				differencesOutput.Write("The 'LoginName' attribute is different. ", "LoginName");
				flag = false;
			}
			return flag;
		}
	}
}