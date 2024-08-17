using Metalogix.Actions;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ReferencedFolderDataUpdater : PreconfiguredTransformer<SPFolder, PasteFolderAction, SPFolderCollection, SPFolderCollection>
	{
		public override string Name
		{
			get
			{
				return "Folder Reference Updating";
			}
		}

		public ReferencedFolderDataUpdater()
		{
		}

		public override void BeginTransformation(PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
		}

		private SecurityPrincipalCollection CopyReferencedPrincipalsForFolder(SPFolder dataObject, XmlNode itemNode, SPFieldCollection userFields)
		{
			string value;
			SecurityPrincipalCollection principals;
			bool flag = false;
			if (dataObject.ParentList.Adapter.SharePointVersion.IsSharePoint2007OrLater)
			{
				foreach (SPField userField in userFields)
				{
					if (!userField.AllowsGroups)
					{
						continue;
					}
					flag = true;
					break;
				}
			}
			Hashtable hashtables = new Hashtable();
			foreach (SPField sPField in userFields)
			{
				if (itemNode.Attributes[sPField.Name] != null)
				{
					value = itemNode.Attributes[sPField.Name].Value;
				}
				else
				{
					value = null;
				}
				string str = value;
				if (str == null)
				{
					continue;
				}
				char[] chrArray = new char[] { ',' };
				string[] strArrays = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str1 = strArrays[i];
					if (!hashtables.ContainsKey(str1))
					{
						hashtables.Add(str1, str1);
					}
				}
			}
			List<SecurityPrincipal> securityPrincipals = new List<SecurityPrincipal>();
			if (flag)
			{
				principals = dataObject.ParentList.ParentWeb.Principals;
			}
			else
			{
				principals = dataObject.ParentList.ParentWeb.SiteUsers;
			}
			SecurityPrincipalCollection securityPrincipalCollection = principals;
			foreach (string key in hashtables.Keys)
			{
				SecurityPrincipal item = securityPrincipalCollection[key];
				if (item == null)
				{
					XmlDocument xmlDocument = new XmlDocument();
					XmlNode xmlNodes = xmlDocument.CreateElement("User");
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("LoginName");
					xmlNodes.Attributes.Append(xmlAttribute);
					xmlAttribute.Value = key;
					item = new SPUser(xmlNodes);
				}
				securityPrincipals.Add(item);
				if (!flag || !(item is SPGroup))
				{
					continue;
				}
				SPGroup owner = (SPGroup)item;
				while (owner.Owner != null && !securityPrincipals.Contains(owner.Owner))
				{
					if (!owner.OwnerIsUser)
					{
						owner = (SPGroup)owner.Owner;
						securityPrincipals.Add(owner);
					}
					else
					{
						securityPrincipals.Add((SPUser)owner.Owner);
					}
				}
			}
			return new SecurityPrincipalCollection(securityPrincipals.ToArray());
		}

		public override void EndTransformation(PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
		}

		public override SPFolder Transform(SPFolder dataObject, PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
			SPFolder parentFolder = targets.ParentFolder;
			if (parentFolder == null)
			{
				LogItem logItem = new LogItem("Transformer Casting Error", dataObject.Name, dataObject.DisplayUrl, targets.ParentList.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = new ArgumentException("The target item collection does not belong to a SharePoint folder")
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
			}
			SPFieldCollection fields = (SPFieldCollection)dataObject.ParentList.Fields;
			if (fields != null)
			{
				SPFieldCollection sPFieldCollections = (SPFieldCollection)dataObject.ParentList.Fields;
				string[] strArrays = new string[] { "User", "UserMulti" };
				SPFieldCollection fieldsOfTypes = sPFieldCollections.GetFieldsOfTypes(strArrays);
				action.EnsurePrincipalExistence(this.CopyReferencedPrincipalsForFolder(dataObject, xmlNode, fieldsOfTypes), parentFolder.ParentList.ParentWeb);
				BaseListItemDataUpdater<PasteListItemAction, PasteListItemOptions>.MapUserFields(action.PrincipalMappings, xmlNode, fieldsOfTypes);
				if (action.SharePointOptions.CorrectingLinks)
				{
					action.LinkCorrector.UpdateLinksInListItemXml(xmlNode, fields, action.SharePointOptions.LinkCorrectTextFields);
				}
				if (SPUtils.IsOneNoteFeatureEnabled(dataObject) && SPUtils.IsDefaultOneNoteFolder(dataObject) && xmlNode.Attributes["FileLeafRef"] != null)
				{
					xmlNode.Attributes["FileLeafRef"].Value = SPUtils.GetUpdatedOneNoteFolderName(parentFolder.ParentList.ParentWeb);
				}
				dataObject.UpdateSettings(xmlNode.OuterXml);
			}
			return dataObject;
		}
	}
}