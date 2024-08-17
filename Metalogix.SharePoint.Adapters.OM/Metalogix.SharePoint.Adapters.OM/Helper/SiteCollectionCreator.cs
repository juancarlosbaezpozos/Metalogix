using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.OM;
using Metalogix.SharePoint.Adapters.OM.Exceptions;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.OM.Helper
{
    internal sealed class SiteCollectionCreator
    {
        private OMAdapter _adapter;

        private string _webApp;

        private string _siteCollectionXML;

        private AddSiteCollectionOptions _options;

        private string _resultSiteXml;

        public SiteCollectionCreator()
        {
        }

        private void Create()
        {
            SPWebService value;
            SPWebApplication webApplication;
            XmlDocument xmlDocument;
            uint num;
            string str;
            int num1;
            string[] strArrays;
            using (Context context = this._adapter.GetContext())
            {
                bool flag = false;
                string str1 = null;
                string str2 = null;
                string str3 = null;
                string str4 = null;
                string str5 = null;
                string str6 = null;
                string str7 = null;
                string str8 = null;
                string str9 = null;
                string str10 = null;
                SPSite item = null;
                try
                {
                    xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(this._siteCollectionXML);
                    XmlNode xmlNodes = xmlDocument.SelectSingleNode("//Site");
                    string value1 = xmlNodes.Attributes["ServerRelativeUrl"].Value;
                    string value2 = xmlNodes.Attributes["Title"].Value;
                    if (xmlNodes.Attributes["Description"] != null)
                    {
                        str = xmlNodes.Attributes["Description"].Value;
                    }
                    else
                    {
                        str = null;
                    }

                    string str11 = str;
                    string value3 = xmlNodes.Attributes["Language"].Value;
                    string attributeValueAsString = xmlNodes.GetAttributeValueAsString("HostHeaderURL");
                    int num2 = Convert.ToInt32(xmlNodes.Attributes["WebTemplateID"].Value);
                    int num3 = Convert.ToInt32(xmlNodes.Attributes["WebTemplateConfig"].Value);
                    string value4 = null;
                    if (xmlNodes.Attributes["WebTemplateName"] != null)
                    {
                        value4 = xmlNodes.Attributes["WebTemplateName"].Value;
                    }

                    bool attributeValueAsBoolean = xmlNodes.GetAttributeValueAsBoolean("IsHostHeader");
                    int? nullable = null;
                    if (xmlNodes.Attributes["ExperienceVersion"] != null)
                    {
                        nullable = new int?(Convert.ToInt32(xmlNodes.Attributes["ExperienceVersion"].Value));
                    }

                    if (this._webApp == null || this._options.SelfServiceCreateMode)
                    {
                        webApplication = context.Site.WebApplication;
                    }
                    else
                    {
                        value = SPFarm.Local.Services.GetValue<SPWebService>("");
                        webApplication = value.WebApplications[this._webApp];
                    }

                    if (!uint.TryParse(value3, out num))
                    {
                        throw new ArgumentException("Failed: Could not parse language code");
                    }

                    char[] chrArray = new char[] { '/' };
                    SPWebTemplate webTemplateByIDOrName = null;
                    bool flag1 = true;
                    try
                    {
                        webTemplateByIDOrName =
                            this._adapter.GetWebTemplateByIDOrName(num, num2, num3, value4, nullable, context.Site);
                    }
                    catch (LanguageTemplatesMissingException languageTemplatesMissingException1)
                    {
                        LanguageTemplatesMissingException languageTemplatesMissingException =
                            languageTemplatesMissingException1;
                        Utils.LogExceptionDetails(languageTemplatesMissingException, MethodBase.GetCurrentMethod().Name,
                            MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                        flag1 = false;
                    }

                    if (!attributeValueAsBoolean)
                    {
                        string[] strArrays1 = value1.Split(chrArray);
                        value1 = "";
                        string[] strArrays2 = strArrays1;
                        for (int i = 0; i < (int)strArrays2.Length; i++)
                        {
                            string str12 = strArrays2[i];
                            value1 = string.Concat(value1, Utils.CleanSharePointURL(str12), "/");
                        }

                        value1 = value1.TrimEnd(chrArray);
                    }
                    else
                    {
                        value1 = attributeValueAsString;
                    }

                    item = webApplication.Sites[value1];
                    if (attributeValueAsBoolean && item != null)
                    {
                        string str13 = item.Url.Trim(new char[] { '/' });
                        char[] chrArray1 = new char[] { '/' };
                        if (!str13.Equals(value1.Trim(chrArray1), StringComparison.InvariantCulture))
                        {
                            item = null;
                        }
                    }

                    if (item != null && this._options.Overwrite)
                    {
                        item.Delete();
                        item = null;
                    }

                    if (item == null)
                    {
                        if (!flag1)
                        {
                            throw new Exception(string.Format(Resources.MsgCannotFindLanguageTemplates,
                                num.ToString()));
                        }

                        if (webTemplateByIDOrName == null)
                        {
                            throw new Exception(Resources.MsgWebTemplateMissing);
                        }

                        chrArray = new char[] { '\\' };
                        string[] strArrays3 = xmlNodes.Attributes["Owner"].Value.Split(chrArray);
                        str1 = ((int)strArrays3.Length > 1 ? strArrays3[0] : "");
                        str2 = ((int)strArrays3.Length > 1 ? strArrays3[1] : strArrays3[0]);
                        if (this._options.ValidateOwners)
                        {
                            bool userInformation =
                                this._adapter.GetUserInformation(str1, str2, out str2, out str4, out str3, false);
                            if (this._options.ValidateOwners && !userInformation)
                            {
                                throw new ArgumentException(string.Concat(
                                    "Could not create site collection: Cannot find ",
                                    xmlNodes.Attributes["Owner"].Value));
                            }
                        }

                        str5 = (str1.Length > 0 ? string.Concat(str1, "\\", str2) : str2);
                        if (xmlNodes.Attributes["SecondaryOwner"] != null)
                        {
                            strArrays = xmlNodes.Attributes["SecondaryOwner"].Value.Split(chrArray);
                        }
                        else
                        {
                            strArrays = null;
                        }

                        string[] strArrays4 = strArrays;
                        if (strArrays4 != null && ((int)strArrays4.Length != 1 || strArrays4[0].Length != 0))
                        {
                            str6 = ((int)strArrays4.Length > 1 ? strArrays4[0] : "");
                            str7 = ((int)strArrays4.Length > 1 ? strArrays4[1] : strArrays4[0]);
                            flag = (!this._options.ValidateOwners
                                ? true
                                : this._adapter.GetUserInformation(str6, str7, out str7, out str9, out str8, false));
                            str10 = (str6.Length > 0 ? string.Concat(str6, "\\", str7) : str7);
                        }

                        if (this._options.SelfServiceCreateMode)
                        {
                            if (!this._adapter.SharePointVersion.IsSharePoint2013OrLater || !nullable.HasValue)
                            {
                                item = context.Site.SelfServiceCreateSite(value1, value2, str11, num,
                                    webTemplateByIDOrName.Name, str5, str3, str4, str10, str8, str9);
                            }
                        }
                        else if (!string.IsNullOrEmpty(this._options.ContentDatabase))
                        {
                            bool flag2 = false;
                            foreach (SPContentDatabase contentDatabasis in webApplication.ContentDatabases)
                            {
                                if (contentDatabasis.Name != this._options.ContentDatabase)
                                {
                                    continue;
                                }

                                if (!this._adapter.SharePointVersion.IsSharePoint2013OrLater || !nullable.HasValue)
                                {
                                    item = (!flag
                                        ? contentDatabasis.Sites.Add(value1, value2, str11, num,
                                            webTemplateByIDOrName.Name, str5, str3, str4, null, null, null,
                                            attributeValueAsBoolean)
                                        : contentDatabasis.Sites.Add(value1, value2, str11, num,
                                            webTemplateByIDOrName.Name, str5, str3, str4, str10, str8, str9,
                                            attributeValueAsBoolean));
                                }

                                flag2 = true;
                                break;
                            }

                            if (!flag2)
                            {
                                throw new Exception(Resources.MsgCannotFindContentDatabase);
                            }
                        }
                        else if (!this._adapter.SharePointVersion.IsSharePoint2013OrLater || !nullable.HasValue)
                        {
                            item = (!flag
                                ? webApplication.Sites.Add(value1, value2, str11, num, webTemplateByIDOrName.Name, str5,
                                    str3, str4, null, null, null, attributeValueAsBoolean)
                                : webApplication.Sites.Add(value1, value2, str11, num, webTemplateByIDOrName.Name, str5,
                                    str3, str4, str10, str8, str9, attributeValueAsBoolean));
                        }
                    }

                    this.CreateDefaultGroups(item.RootWeb, item);
                    if (this._options.Overwrite)
                    {
                        this._adapter.ClearDefaultSiteData(item.RootWeb);
                    }

                    if (this._options.CopySiteAdmins)
                    {
                        this._adapter.MigrateSiteCollectionAdmins(item, xmlNodes);
                    }

                    if (this._options.CopySiteQuota)
                    {
                        this._adapter.SetSiteQuota(item, xmlNodes);
                    }

                    using (SPWeb sPWeb = item.OpenWeb())
                    {
                        if (this._options.PreserveUIVersion)
                        {
                            this._adapter.ChangeUIVersion(sPWeb);
                        }

                        bool allowUnsafeUpdates = sPWeb.AllowUnsafeUpdates;
                        try
                        {
                            if (this._options.CopyFeatures)
                            {
                                string str14 = (xmlNodes.Attributes["SiteCollFeatures"] != null
                                    ? xmlNodes.Attributes["SiteCollFeatures"].Value
                                    : "");
                                this._adapter.AddFeatures(item.Features, item, str14, this._options.MergeFeatures,
                                    false);
                            }

                            if (this._options.CopyFeatures)
                            {
                                string str15 = (xmlNodes.Attributes["SiteFeatures"] != null
                                    ? xmlNodes.Attributes["SiteFeatures"].Value
                                    : "");
                                this._adapter.AddFeatures(sPWeb.Features, item, str15, this._options.MergeFeatures,
                                    string.Equals(sPWeb.WebTemplate, "ENTERWIKI", StringComparison.OrdinalIgnoreCase));
                            }
                        }
                        finally
                        {
                            sPWeb.AllowUnsafeUpdates = (allowUnsafeUpdates);
                        }
                    }

                    using (SPWeb sPWeb1 = item.OpenWeb())
                    {
                        XmlNode xmlNodes1 = xmlDocument.SelectSingleNode("//Fields");
                        if (xmlNodes1 != null)
                        {
                            this._adapter.AddFieldsXML(sPWeb1.Fields, xmlNodes1);
                        }

                        string str16 = sPWeb1.CurrentUser.IsSiteAdmin.ToString();
                        if (this._options.ApplyMasterPage)
                        {
                            if (xmlNodes.Attributes["CustomMasterPage"] != null)
                            {
                                if (sPWeb1.ServerRelativeUrl != "/")
                                {
                                    sPWeb1.CustomMasterUrl = (string.Concat(sPWeb1.ServerRelativeUrl,
                                        xmlNodes.Attributes["CustomMasterPage"].Value));
                                }
                                else
                                {
                                    sPWeb1.CustomMasterUrl = (xmlNodes.Attributes["CustomMasterPage"].Value);
                                }
                            }

                            if (xmlNodes.Attributes["MasterPage"] != null)
                            {
                                if (sPWeb1.ServerRelativeUrl != "/")
                                {
                                    sPWeb1.MasterUrl = (string.Concat(sPWeb1.ServerRelativeUrl,
                                        xmlNodes.Attributes["MasterPage"].Value));
                                }
                                else
                                {
                                    sPWeb1.MasterUrl = (xmlNodes.Attributes["MasterPage"].Value);
                                }
                            }
                        }

                        num1 = (xmlNodes.Attributes["Locale"] == null
                            ? Convert.ToInt32(value3)
                            : Convert.ToInt32(xmlNodes.Attributes["Locale"].Value));
                        this._adapter.UpdateWebProperties(sPWeb1, xmlNodes, this._options, num1, null);
                        sPWeb1.Update();
                        if (sPWeb1.DoesUserHavePermissions(SPBasePermissions.EmptyMask | SPBasePermissions.Open |
                                                           SPBasePermissions.ViewPages))
                        {
                            this._resultSiteXml = this._adapter.GetSiteXml(item, sPWeb1, true);
                        }

                        this._resultSiteXml.Replace("IsSiteAdmin=\"True", string.Concat("IsSiteAdmin=\"", str16));
                    }
                }
                finally
                {
                    if (item != null)
                    {
                        item.Dispose();
                        item = null;
                    }

                    value = null;
                    webApplication = null;
                    xmlDocument = null;
                    str1 = null;
                    str2 = null;
                    str3 = null;
                    str4 = null;
                    str5 = null;
                    str6 = null;
                    str7 = null;
                    str8 = null;
                    str9 = null;
                    str10 = null;
                    GC.Collect();
                }
            }
        }

        private void CreateAsElevated()
        {
            SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(() => this.Create()));
        }

        private void CreateDefaultGroups(SPWeb rootSpWeb, SPSite site)
        {
            rootSpWeb.CreateDefaultAssociatedGroups(site.Owner.LoginName,
                (site.SecondaryContact != null ? site.SecondaryContact.LoginName : string.Empty), string.Empty);
        }

        public string CreateSiteCollection(OMAdapter adapter, string webApp, string siteCollectionXml,
            AddSiteCollectionOptions options)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (webApp == null)
            {
                throw new ArgumentNullException("webApp");
            }

            if (siteCollectionXml == null)
            {
                throw new ArgumentNullException("siteCollectionXml");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this._adapter = adapter;
            this._webApp = webApp;
            this._siteCollectionXML = siteCollectionXml;
            this._options = options;
            this._resultSiteXml = string.Empty;
            try
            {
                if (SPContext.Current == null || !this._adapter.SharePointVersion.IsSharePoint2010OrLater)
                {
                    this.Create();
                }
                else
                {
                    IsAllowedResult isAllowedResult = this.IsAllowed(this._options.SelfServiceCreateMode);
                    if (!isAllowedResult.Allowed)
                    {
                        throw new Exception(isAllowedResult.ErrorMessage);
                    }

                    this.CreateAsElevated();
                }
            }
            finally
            {
                this._adapter = null;
                this._webApp = null;
                this._siteCollectionXML = null;
                this._options = null;
            }

            return this._resultSiteXml;
        }

        private IsAllowedResult IsAllowed(bool checkSelfServiceMode)
        {
            IsAllowedResult isAllowedResult;
            IsAllowedResult unableToCreateSCUnderCentralAdmin = new IsAllowedResult();
            using (Context context = this._adapter.GetContext())
            {
                bool flag = SPFarm.Local.CurrentUserIsAdministrator();
                string loginName = context.Web.CurrentUser.LoginName;
                SPWebApplication webApplication = null;
                if (string.IsNullOrEmpty(this._webApp) || checkSelfServiceMode)
                {
                    webApplication = context.Site.WebApplication;
                }
                else
                {
                    SPWebService value = SPFarm.Local.Services.GetValue<SPWebService>();
                    webApplication = value.WebApplications[this._webApp];
                }

                if (webApplication == null)
                {
                    unableToCreateSCUnderCentralAdmin.ErrorMessage =
                        string.Format(Resources.FS_UnableToObtainWebApplication, this._webApp);
                    isAllowedResult = unableToCreateSCUnderCentralAdmin;
                }
                else if (webApplication.IsAdministrationWebApplication && checkSelfServiceMode)
                {
                    unableToCreateSCUnderCentralAdmin.ErrorMessage = Resources.UnableToCreateSCUnderCentralAdmin;
                    isAllowedResult = unableToCreateSCUnderCentralAdmin;
                }
                else if (!flag)
                {
                    if (!unableToCreateSCUnderCentralAdmin.Allowed && checkSelfServiceMode)
                    {
                        unableToCreateSCUnderCentralAdmin.Allowed =
                            context.Web.DoesUserHavePermissions(loginName, SPBasePermissions.CreateSSCSite);
                        if (unableToCreateSCUnderCentralAdmin.Allowed)
                        {
                            isAllowedResult = unableToCreateSCUnderCentralAdmin;
                            return isAllowedResult;
                        }
                    }

                    if (!unableToCreateSCUnderCentralAdmin.Allowed)
                    {
                        SPPolicy item = webApplication.Policies[loginName];
                        if (item != null)
                        {
                            foreach (SPPolicyRole policyRoleBinding in item.PolicyRoleBindings)
                            {
                                if (policyRoleBinding.Type != (SPPolicyRoleType)4)
                                {
                                    continue;
                                }

                                unableToCreateSCUnderCentralAdmin.Allowed = true;
                                isAllowedResult = unableToCreateSCUnderCentralAdmin;
                                return isAllowedResult;
                            }
                        }
                    }

                    if (!unableToCreateSCUnderCentralAdmin.Allowed)
                    {
                        unableToCreateSCUnderCentralAdmin.ErrorMessage =
                            string.Format(Resources.FS_UnableToCreateSC, loginName);
                    }

                    return unableToCreateSCUnderCentralAdmin;
                }
                else
                {
                    unableToCreateSCUnderCentralAdmin.Allowed = true;
                    isAllowedResult = unableToCreateSCUnderCentralAdmin;
                }
            }

            return isAllowedResult;
        }
    }
}