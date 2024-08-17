using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.People;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class PeopleService : BaseServiceWrapper
    {
        public PeopleService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.People.People();
            base.InitializeWrappedWebService("People");
        }

        public PrincipalInfo[] ResolvePrincipals(string[] principalKeys, SPPrincipalType principalTypes,
            bool addToUserInfoList)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { principalKeys, principalTypes, addToUserInfoList };
            return (PrincipalInfo[])WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}