using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPComposedLooksGallery : SPList
	{
		private SPListItem _currentItem = null;

		public SPListItemCollection ComposedLooks
		{
			get
			{
				return base.Items;
			}
		}

		public SPListItem CurrentItem
		{
			get
			{
				this._currentItem = this.GetCurrentItem();
				return this._currentItem;
			}
		}

		public SPComposedLooksGallery(SPWeb parentWeb, XmlNode listXML) : base(parentWeb, listXML)
		{
		}

		public SPListItem GetCurrentItem()
		{
			//SPListItem item = null;
			SPListItem sPListItem = null;
			List<SPListItem> sPListItems = new List<SPListItem>();
			foreach (SPListItem item in base.Items)
			{
				if ((item["Name"].Equals("Current", StringComparison.OrdinalIgnoreCase) && item["DisplayOrder"] == "0"))
				{
					sPListItems.Add(item);
				}
			}
			int num = 0;
			foreach (SPListItem sPListItem1 in sPListItems)
			{
				int num1 = int.Parse(sPListItem1["ID"]);
				if (num1 > num)
				{
					sPListItem = sPListItem1;
					num = num1;
				}
			}
			return sPListItem;
		}
	}
}