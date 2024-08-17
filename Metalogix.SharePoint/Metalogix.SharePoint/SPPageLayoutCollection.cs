using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint
{
	public class SPPageLayoutCollection : List<SPPageLayout>
	{
		private const string PAGE_LAYOUT_NECESSARY_FIELDS = "<Fields><Field Name=\"ID\" ColName=\"tp_ID\" Type=\"Counter\" /><Field ColName=\"tp_Modified\" Name=\"Modified\" Type=\"DateTime\" /><Field Name=\"FileRef\" Type=\"File\" /><Field Name=\"FSObjType\" ColName=\"Type\" Type=\"Lookup\" FromBaseType=\"TRUE\" /><Field Name=\"ContentTypeId\" Type=\"ContentTypeId\" ColName=\"tp_ContentTypeId\"/><Field Name=\"FileLeafRef\" Type=\"File\" /><Field Name=\"FileDirRef\" Type=\"Lookup\" FromBaseType=\"TRUE\" /><Field Name=\"Title\" Type=\"Text\"/> <Field Name=\"PublishingAssociatedContentType\" /></Fields>";

		private SPMasterPageGallery _gallery;

		public SPPageLayoutCollection(SPMasterPageGallery gallery)
		{
			this._gallery = gallery;
			this.FetchData();
		}

		public void FetchData()
		{
			SPListItemCollection items = this._gallery.GetItems(true, ListItemQueryType.ListItem, "<Fields><Field Name=\"ID\" ColName=\"tp_ID\" Type=\"Counter\" /><Field ColName=\"tp_Modified\" Name=\"Modified\" Type=\"DateTime\" /><Field Name=\"FileRef\" Type=\"File\" /><Field Name=\"FSObjType\" ColName=\"Type\" Type=\"Lookup\" FromBaseType=\"TRUE\" /><Field Name=\"ContentTypeId\" Type=\"ContentTypeId\" ColName=\"tp_ContentTypeId\"/><Field Name=\"FileLeafRef\" Type=\"File\" /><Field Name=\"FileDirRef\" Type=\"Lookup\" FromBaseType=\"TRUE\" /><Field Name=\"Title\" Type=\"Text\"/> <Field Name=\"PublishingAssociatedContentType\" /></Fields>", false, false);
			foreach (SPPageLayout sPPageLayout in 
				from item in items
				where item is SPPageLayout
				select item)
			{
				base.Add(sPPageLayout);
			}
		}
	}
}