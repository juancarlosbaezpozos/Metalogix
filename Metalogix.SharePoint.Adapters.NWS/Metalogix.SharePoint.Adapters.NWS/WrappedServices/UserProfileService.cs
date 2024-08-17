using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.UserProfile;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class UserProfileService : BaseServiceWrapper
    {
        public UserProfileService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.UserProfile.UserProfileService();
            base.InitializeWrappedWebService("UserProfileService");
        }

        public int GetUserProfileByIndex(int index, out string personalSiteUrl)
        {
            personalSiteUrl = string.Empty;
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { index };
            GetUserProfileByIndexResult getUserProfileByIndexResult =
                (GetUserProfileByIndexResult)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
            PropertyData propertyDatum =
                ((IEnumerable<PropertyData>)getUserProfileByIndexResult.UserProfile).FirstOrDefault<PropertyData>(
                    (PropertyData a) => a.Name.Equals("PersonalSpace"));
            PropertyData propertyDatum1 =
                ((IEnumerable<PropertyData>)getUserProfileByIndexResult.UserProfile).FirstOrDefault<PropertyData>(
                    (PropertyData a) => a.Name.Equals("SPS-PersonalSiteInstantiationState"));
            if (propertyDatum1.Values.Count<ValueData>() > 0 &&
                string.Equals(Convert.ToString(propertyDatum1.Values[0].Value), "2") &&
                propertyDatum.Values.Count<ValueData>() > 0)
            {
                string str = Convert.ToString(propertyDatum.Values[0].Value).TrimEnd(new char[] { '/' });
                if (!string.IsNullOrEmpty(str))
                {
                    personalSiteUrl = str;
                }
            }

            return int.Parse(getUserProfileByIndexResult.NextValue);
        }

        public string GetUserProfileByName(string siteURL, string loginName, out string errors)
        {
            string str;
            string str1;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                string name = MethodBase.GetCurrentMethod().Name;
                object[] objArray = new object[] { loginName };
                PropertyData[] propertyDataArray =
                    (PropertyData[])WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
                Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo[] userProfileSchema =
                    this.GetUserProfileSchema();
                stringBuilder.AppendLine(string.Format("Obtain user profile for site '{0}'", siteURL));
                stringBuilder.AppendLine(string.Format("Obtain user profile for user '{0}'", loginName));
                StringBuilder stringBuilder1 = new StringBuilder();
                XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true
                };
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder1, xmlWriterSetting))
                {
                    xmlWriter.WriteStartElement("UserProfile");
                    Type type = typeof(DateTime);
                    xmlWriter.WriteStartElement("Sections");
                    xmlWriter.WriteEndElement();
                    stringBuilder.AppendLine("Obtain Properties");
                    PropertyData[] propertyDataArray1 = propertyDataArray;
                    for (int i = 0; i < (int)propertyDataArray1.Length; i++)
                    {
                        PropertyData propertyDatum = propertyDataArray1[i];
                        if (!string.IsNullOrEmpty(propertyDatum.Name) && propertyDatum.Values.Count<ValueData>() > 0)
                        {
                            Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo propertyInfo =
                                ((IEnumerable<Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo>)
                                    userProfileSchema)
                                .FirstOrDefault<Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo>(
                                    (Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo pi) =>
                                        pi.Name.Equals(propertyDatum.Name));
                            if (propertyInfo != null)
                            {
                                xmlWriter.WriteStartElement(propertyDatum.Name);
                                xmlWriter.WriteAttributeString("Type",
                                    (propertyInfo.Type == null ? string.Empty : propertyInfo.Type));
                                xmlWriter.WriteAttributeString("DisplayName",
                                    (propertyInfo.DisplayName == null ? string.Empty : propertyInfo.DisplayName));
                                xmlWriter.WriteAttributeString("Description",
                                    (propertyInfo.Description == null ? string.Empty : propertyInfo.Description));
                                xmlWriter.WriteAttributeString("Length", propertyInfo.Length.ToString());
                                xmlWriter.WriteAttributeString("DefaultPrivacy",
                                    propertyInfo.DefaultPrivacy.ToString());
                                xmlWriter.WriteAttributeString("IsSystem", propertyInfo.IsSystem.ToString());
                                xmlWriter.WriteAttributeString("IsAlias", propertyInfo.IsAlias.ToString());
                                xmlWriter.WriteAttributeString("IsColleagueEventLog",
                                    propertyInfo.IsColleagueEventLog.ToString());
                                xmlWriter.WriteAttributeString("IsMultivalued", propertyInfo.IsMultiValue.ToString());
                                xmlWriter.WriteAttributeString("IsReplicable", propertyInfo.IsReplicable.ToString());
                                xmlWriter.WriteAttributeString("IsSearchable", propertyInfo.IsSearchable.ToString());
                                xmlWriter.WriteAttributeString("IsAdminEditable",
                                    propertyInfo.IsAdminEditable.ToString());
                                xmlWriter.WriteAttributeString("IsUserEditable",
                                    propertyInfo.IsUserEditable.ToString());
                                xmlWriter.WriteAttributeString("IsVisibleOnEditor",
                                    propertyInfo.IsVisibleOnEditor.ToString());
                                xmlWriter.WriteAttributeString("IsVisibleOnViewer",
                                    propertyInfo.IsVisibleOnViewer.ToString());
                                xmlWriter.WriteAttributeString("MaximumShown", propertyInfo.MaximumShown.ToString());
                                xmlWriter.WriteAttributeString("UserOverridePrivacy",
                                    propertyInfo.UserOverridePrivacy.ToString());
                                ValueData[] values = propertyDatum.Values;
                                for (int j = 0; j < (int)values.Length; j++)
                                {
                                    ValueData valueDatum = values[j];
                                    if (type.IsAssignableFrom(valueDatum.GetType()))
                                    {
                                        str = Utils.FormatDate((DateTime)valueDatum.Value);
                                    }
                                    else if (!propertyInfo.Type.Equals("timezone",
                                                 StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        str = (valueDatum == null ? "" : valueDatum.Value.ToString());
                                    }
                                    else
                                    {
                                        str = ((SPTimeZone)valueDatum.Value).ID.ToString();
                                    }

                                    xmlWriter.WriteElementString("Value", str);
                                }

                                xmlWriter.WriteEndElement();
                            }
                        }
                    }

                    xmlWriter.WriteStartElement("QuickLinks");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                }

                errors = string.Empty;
                str1 = stringBuilder1.ToString();
            }
            catch (Exception exception)
            {
                Metalogix.Utilities.ExceptionDetail exceptionMessageAndDetail =
                    ExceptionUtils.GetExceptionMessageAndDetail(exception);
                errors = string.Format("Failed to get user profile: {0}{1}{2}", exceptionMessageAndDetail.Message,
                    Environment.NewLine, exceptionMessageAndDetail.Detail.ToString());
                str1 = null;
            }

            return str1;
        }

        public long GetUserProfileCount()
        {
            return (long)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                new object[0]);
        }

        private Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo[] GetUserProfileSchema()
        {
            Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo[] propertyInfoArray =
                (Metalogix.SharePoint.Adapters.NWS.UserProfile.PropertyInfo[])WebServiceWrapperUtilities.ExecuteMethod(
                    this, MethodBase.GetCurrentMethod().Name, new object[0]);
            return propertyInfoArray;
        }
    }
}