using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Reflection;
using System.Web.UI.WebControls.WebParts;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class ListViewWebPart2007 : Metalogix.SharePoint.Adapters.OM.ListViewWebPart
    {
        private Microsoft.SharePoint.WebPartPages.ListViewWebPart wrappedListViewWebPart;

        public override int BaseViewID
        {
            get { throw new Exception("This property is only available in SharePoint 2010."); }
            set { throw new Exception("This property is only available in SharePoint 2010."); }
        }

        public override string ListName
        {
            get { return this.wrappedListViewWebPart.ListName; }
            set { this.wrappedListViewWebPart.ListName = (value); }
        }

        public override SPView View
        {
            get
            {
                PropertyInfo property = this.wrappedListViewWebPart.GetType().GetProperty("View",
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
                if (property != null)
                {
                    return property.GetValue(this.wrappedListViewWebPart, null) as SPView;
                }

                FieldInfo field = this.wrappedListViewWebPart.GetType().GetField("View",
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    return null;
                }

                return field.GetValue(this.wrappedListViewWebPart) as SPView;
            }
            set
            {
                PropertyInfo property = this.wrappedListViewWebPart.GetType().GetProperty("View",
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
                if (property != null)
                {
                    property.SetValue(this.wrappedListViewWebPart, value, null);
                    return;
                }

                FieldInfo field = this.wrappedListViewWebPart.GetType().GetField("View",
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(this.wrappedListViewWebPart, value);
                }
            }
        }

        public override string ViewGuid
        {
            get { return this.wrappedListViewWebPart.ViewGuid; }
            set { this.wrappedListViewWebPart.ViewGuid = (value); }
        }

        public override Guid WebID
        {
            get { return this.wrappedListViewWebPart.WebId; }
            set { this.wrappedListViewWebPart.WebId = (value); }
        }

        public ListViewWebPart2007(System.Web.UI.WebControls.WebParts.WebPart webPart) : base(webPart)
        {
            this.wrappedListViewWebPart = (Microsoft.SharePoint.WebPartPages.ListViewWebPart)webPart;
        }
    }
}