using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPRoleConverter03to07 : RoleConverter
	{
		public override Type SourceRoleType
		{
			get
			{
				return typeof(SPRole2003);
			}
		}

		public override Type TargetRoleType
		{
			get
			{
				return typeof(SPRole2007);
			}
		}

		public SPRoleConverter03to07()
		{
		}

		public override Role Convert(Role sourceRole)
		{
			bool flag;
			if (!(sourceRole is SPRole2003))
			{
				object[] fullName = new object[] { typeof(SPRole2003).FullName, sourceRole.GetType().FullName };
				throw new ArgumentException(string.Format("Source role is of the wrong type.\nExpected Type: {0}\nArgument Type: {1}", fullName));
			}
			SPRole2003 sPRole2003 = (SPRole2003)sourceRole;
			Set<SPRights2007> set = new Set<SPRights2007>();
			string[] names = Enum.GetNames(typeof(SPRights2007));
			for (int i = 0; i < (int)names.Length; i++)
			{
				string str = names[i];
				if (sPRole2003.ContainsRight(str) || str == "ManagePermissions" && sPRole2003.ContainsRight("ManageRoles") || str == "CreateGroups" && sPRole2003.ContainsRight("CreatePersonalGroups") || str == "DeleteVersions" && sPRole2003.ContainsRight("DeleteListItems"))
				{
					flag = false;
				}
				else
				{
					flag = (str != "ViewVersions" ? true : !sPRole2003.ContainsRight("ViewListItems"));
				}
				if (!flag)
				{
					SPRights2007 sPRights2007 = (SPRights2007)Enum.Parse(typeof(SPRights2007), str);
					set.Add(sPRights2007);
					SPRole2007.AddRightsDependencies(sPRights2007, set);
				}
			}
			long num = (long)0;
			if ((set.Contains(SPRights2007.EnumeratePermissions) ? false : sPRole2003.ContainsRight("ManageListPermissions")))
			{
				set.Add(SPRights2007.EnumeratePermissions);
				SPRole2007.AddRightsDependencies(SPRights2007.EnumeratePermissions, set);
			}
			foreach (SPRights2007 sPRights20071 in set)
			{
				num += (long)sPRights20071;
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("Role");
			xmlTextWriter.WriteAttributeString("Name", sPRole2003.RoleName);
			xmlTextWriter.WriteAttributeString("Description", sPRole2003.Description);
			xmlTextWriter.WriteAttributeString("Hidden", sPRole2003.Hidden.ToString());
			xmlTextWriter.WriteAttributeString("PermMask", num.ToString());
			xmlTextWriter.WriteEndElement();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(stringBuilder.ToString());
			return new SPRole2007(xmlDocument.DocumentElement);
		}
	}
}