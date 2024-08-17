using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.Utilities;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public abstract class SPRole : Role, Metalogix.DataStructures.IComparable
	{
		private string[] m_rights = null;

		public string Description
		{
			get
			{
				return this.m_XML.Attributes["Description"].Value;
			}
		}

		public bool Hidden
		{
			get
			{
				bool flag = bool.Parse(this.m_XML.Attributes["Hidden"].Value);
				return flag;
			}
		}

		public long PermMask
		{
			get
			{
				return this.m_XML.GetAttributeValueAsLong("PermMask");
			}
		}

		public override string[] Rights
		{
			get
			{
				if (this.m_rights == null)
				{
					this.m_rights = this.GetRightsDescription();
				}
				return this.m_rights;
			}
		}

		public int RoleId
		{
			get
			{
				return this.m_XML.GetAttributeValueAsInt("RoleId");
			}
		}

		public override string RoleName
		{
			get
			{
				if (this.m_XML.Attributes["Name"].Value.Contains("\\"))
				{
					this.m_XML.Attributes["Name"].Value = this.m_XML.Attributes["Name"].Value.Replace("\\", "%5C");
				}
				return this.m_XML.Attributes["Name"].Value;
			}
		}

		public string RoleOrder
		{
			get
			{
				return this.m_XML.GetAttributeValueAsString("RoleOrder");
			}
		}

		public string Type
		{
			get
			{
				return this.m_XML.GetAttributeValueAsString("Type");
			}
		}

		public SPRole(XmlNode xml) : base(xml)
		{
		}

		protected abstract string[] GetRightsDescription();

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			if (targetComparable == null)
			{
				throw new Exception("Cannot compare an SPRole to a null node");
			}
			if (!typeof(Role).IsAssignableFrom(targetComparable.GetType()))
			{
				throw new Exception(string.Concat("Cannot compare an SPRole to a node of type ", targetComparable.GetType().Name));
			}
			Role role = (Role)targetComparable;
			SPRole sPRole = this;
			if (base.GetType() == targetComparable.GetType())
			{
				flag = this.IsEqual((SPRole)role, differencesOutput, options);
			}
			else
			{
				differencesOutput.Write("The roles to be compared are of different types. ", "Role");
				Role role1 = RoleConverter.ConvertRole(role, base.GetType()) as SPRole;
				Role role2 = RoleConverter.ConvertRole(sPRole, role.GetType());
				if ((role1 != null ? true : role2 != null))
				{
					differencesOutput.Write("Comparison is based on an automatic conversion to the same role type", "Role");
					flag = ((role1 != null ? true : role2.GetType().GetInterface(typeof(Metalogix.DataStructures.IComparable).Name) == null) ? this.IsEqual((SPRole)role1, differencesOutput, options) : ((Metalogix.DataStructures.IComparable)role2).IsEqual((Metalogix.DataStructures.IComparable)role, differencesOutput, options));
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		private bool IsEqual(SPRole targetRole, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			string[] rights = this.Rights;
			int num = 0;
			while (true)
			{
				if (num < (int)rights.Length)
				{
					string str = rights[num];
					bool flag1 = false;
					string[] strArrays = targetRole.Rights;
					int num1 = 0;
					while (num1 < (int)strArrays.Length)
					{
						string str1 = strArrays[num1];
						if (!(str.ToLower() == str1.ToLower()))
						{
							num1++;
						}
						else
						{
							flag1 = true;
							break;
						}
					}
					if (flag1)
					{
						num++;
					}
					else
					{
						differencesOutput.Write(string.Concat("The right ", str, " is missing."), str, DifferenceStatus.Missing);
						flag = false;
						break;
					}
				}
				else if ((int)this.Rights.Length < (int)targetRole.Rights.Length)
				{
					differencesOutput.Write("Additional rights are included", "Additional rights");
					flag = false;
					break;
				}
				else if (this.RoleName != targetRole.RoleName)
				{
					differencesOutput.Write("The role names are different", "Role names");
					flag = false;
					break;
				}
				else if (this.Description != targetRole.Description)
				{
					differencesOutput.Write("The role descriptions are different", "Role desctiprions");
					flag = false;
					break;
				}
				else if (this.Hidden == targetRole.Hidden)
				{
					flag = true;
					break;
				}
				else
				{
					differencesOutput.Write("The 'Hidden' property is different", "Hidden");
					flag = false;
					break;
				}
			}
			return flag;
		}
	}
}