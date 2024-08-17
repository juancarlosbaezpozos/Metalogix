using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Web.UI.WebControls.WebParts;

namespace Metalogix.SharePoint.Adapters.OM
{
    public abstract class ListViewWebPart
    {
        protected System.Web.UI.WebControls.WebParts.WebPart _webPart;

        public abstract int BaseViewID { get; set; }

        public abstract string ListName { get; set; }

        public abstract SPView View { get; set; }

        public abstract string ViewGuid { get; set; }

        public abstract Guid WebID { get; set; }

        public ListViewWebPart(System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            this._webPart = webPart;
        }

        public static Metalogix.SharePoint.Adapters.OM.ListViewWebPart CreateInstance(
            System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            if (!typeof(Microsoft.SharePoint.WebPartPages.ListViewWebPart).IsAssignableFrom(webPart.GetType()))
            {
                throw new ArgumentException(string.Concat("Unknown ListViewWebPart type: ", webPart.GetType()));
            }

            return new ListViewWebPart2007(webPart);
        }

        public static bool IsListViewWebPart(System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            if (typeof(Microsoft.SharePoint.WebPartPages.ListViewWebPart).IsAssignableFrom(webPart.GetType()))
            {
                return true;
            }

            return false;
        }
    }
}