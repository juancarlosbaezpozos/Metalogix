using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class PageLayoutGuidMapper : PreconfiguredTransformer<byte[], CopyMasterPageGalleryAction, SPListItemCollection, SPListItemCollection>
	{
		public override string Name
		{
			get
			{
				return "Page Layout GUID Mapping";
			}
		}

		public PageLayoutGuidMapper()
		{
		}

		public override void BeginTransformation(CopyMasterPageGalleryAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override void EndTransformation(CopyMasterPageGalleryAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override byte[] Transform(byte[] dataObject, CopyMasterPageGalleryAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			byte[] numArray;
			try
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder(Encoding.UTF8.GetString(dataObject));
				MatchCollection matchCollections = Regex.Matches(stringBuilder.ToString(), "FieldName=\"[a-z0-9]{8}(-[a-z0-9]{4}){3}-[a-z0-9]{12}\"");
				Dictionary<Guid, Guid> guids = new Dictionary<Guid, Guid>();
				foreach (Match match in matchCollections)
				{
					string str = "[a-z0-9]{8}(-[a-z0-9]{4}){3}-[a-z0-9]{12}";
					Match match1 = Regex.Match(match.Groups[0].Value, str);
					Guid guid = new Guid(match1.Value);
					if (!action.GuidMappings.ContainsKey(guid))
					{
						continue;
					}
					guids.Add(guid, action.GuidMappings[guid]);
				}
				foreach (KeyValuePair<Guid, Guid> keyValuePair in guids)
				{
					string str1 = keyValuePair.Key.ToString();
					Guid value = keyValuePair.Value;
					stringBuilder.Replace(str1, value.ToString());
					if (flag)
					{
						continue;
					}
					flag = true;
				}
				numArray = (!flag ? dataObject : Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			}
			catch
			{
				numArray = null;
			}
			return numArray;
		}
	}
}