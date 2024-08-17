using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Areas;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class AreaService : BaseServiceWrapper
    {
        public AreaService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Areas.AreaService();
            base.InitializeWrappedWebService("AreaService");
        }

        public Guid CreateArea(Guid parentID, string name, string template)
        {
            string str = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { parentID, name, template };
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, str, objArray);
        }

        public Guid CreateAreaListing(Guid parentID, string title, string description, ListingType Type, string url)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { parentID, title, description, Type, url };
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteArea(Guid areaID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { areaID };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteAreaListing(Guid areaID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { areaID };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public AreaData GetAreaData(Guid areaID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { areaID };
            return (AreaData)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public AreaListingData GetAreaListingData(Guid listingAreaID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listingAreaID };
            return (AreaListingData)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public Guid[] GetAreaListings(Guid parentID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { parentID };
            return (Guid[])WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public Guid GetDocumentsAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid GetHomeAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid GetKeywordsAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid GetMySiteAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid GetNewsAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid GetSearchAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid GetSitesDirectoryAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public Guid[] GetSubAreas(Guid parentID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { parentID };
            return (Guid[])WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public Guid GetTopicsAreaID()
        {
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public void SetAreaData(Guid areaID, AreaData areaData)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { areaID, areaData };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void SetAreaListingData(Guid areaListingID, AreaListingData listingData)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { areaListingID, listingData };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}