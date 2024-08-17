using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.NWS;
using Metalogix.Utilities;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Metalogix.SharePoint.Adapters.ClientOM
{
    public class ClientOMTools
    {
        private const int SLIDE_LIBRARY_TEMPLATE_VALUE = 2100;

        private const int MODERATION_STATUS_APPROVED = 0;

        private const string PEOPLE_AND_GROUPS = "PeopleAndGroups";

        private const string TEMPVERSIONSTRING = "Temporary Version - (To be deleted)";

        private const string HIDDEN_WEBPART_ZONE = "wpz";

        protected static uint? s_iGetListQueryRowLimit = null;

        private string[] fieldsThatSupportEmbedding = new string[]
        {
            "WikiField",
            "PublishingPageContent"
        };

        private static Int32Converter s_IntConverter = new Int32Converter();

        private static DoubleConverter s_DoubleConverter = new DoubleConverter();

        private static CultureInfo s_CultureInfo = new CultureInfo("en-US");

        private static int PAGES_LIBRARY_TEMPLATE_ID = 850;

        private ClientContext GetClientContext(NWSAdapter callingAdapter)
        {
            return this.GetClientContext(callingAdapter.Url, callingAdapter);
        }

        private ClientContext GetClientContext(string sUrl, NWSAdapter callingAdapter)
        {
            return this.GetClientContext(sUrl, callingAdapter.Credentials, callingAdapter.CookieManager,
                callingAdapter.AdapterProxy, callingAdapter.IncludedCertificates);
        }

        private ClientContext GetClientContext(string sUrl, Credentials credentials, CookieManager cookieManager,
            WebProxy proxy, X509CertificateWrapperCollection includedCertificates)
        {
            bool bCookieManagerIsActive = cookieManager != null && cookieManager.IsActive;
            ClientContext clientContext = new ClientContext(sUrl);
            clientContext.RequestTimeout = AdapterConfigurationVariables.WebServiceTimeoutTime;
            bool bUsingFormsAuthentication = false;
            if (credentials != null)
            {
                if (bCookieManagerIsActive &&
                    typeof(FormsAuthenticationManager).IsAssignableFrom(cookieManager.GetType()))
                {
                    bUsingFormsAuthentication = true;
                    clientContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    FormsAuthenticationLoginInfo formsAuthenticationLoginInfo =
                        new FormsAuthenticationLoginInfo(credentials.UserName, credentials.Password.ToInsecureString());
                    clientContext.FormsAuthenticationLoginInfo = formsAuthenticationLoginInfo;
                }
                else
                {
                    clientContext.Credentials = credentials.NetworkCredentials;
                }
            }

            clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs e)
            {
                if (bCookieManagerIsActive && !bUsingFormsAuthentication)
                {
                    e.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Cookie, cookieManager.GetCookieString());
                }

                if (proxy != null && proxy.Address != null && !string.IsNullOrEmpty(proxy.Address.AbsoluteUri))
                {
                    e.WebRequestExecutor.WebRequest.Proxy = proxy;
                }

                if (includedCertificates != null)
                {
                    includedCertificates.CopyCertificatesToCollection(
                        e.WebRequestExecutor.WebRequest.ClientCertificates);
                }
            };
            return clientContext;
        }

        private static void CallExecuteQuery(ClientRuntimeContext ctx, SharePointAdapter callingAdapter)
        {
            bool flag = callingAdapter.CookieManager != null && callingAdapter.CookieManager.IsActive &&
                        !typeof(FormsAuthenticationManager).IsAssignableFrom(callingAdapter.CookieManager.GetType());
            Dictionary<string, object> privateFieldData = ClientOMTools.GetPrivateFieldData(ctx.PendingRequest, false);
            try
            {
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (!flag || ex is ServerException)
                {
                    throw;
                }

                callingAdapter.CookieManager.UpdateCookie();
                ClientOMTools.SetPrivateFieldData(ctx.PendingRequest, privateFieldData);
                ctx.ExecuteQuery();
            }
        }

        private static Dictionary<string, object> GetPrivateFieldData(object o, bool bGetDeepData)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            if (bGetDeepData)
            {
                bindingFlags |= BindingFlags.FlattenHierarchy;
            }

            FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            Dictionary<string, object> dictionary = new Dictionary<string, object>(fields.Length);
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                dictionary.Add(fieldInfo.Name, fieldInfo.GetValue(o));
            }

            return dictionary;
        }

        private static void SetPrivateFieldData(object o, Dictionary<string, object> data)
        {
            Type type = o.GetType();
            foreach (KeyValuePair<string, object> current in data)
            {
                FieldInfo field = type.GetField(current.Key,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (field != null)
                {
                    field.SetValue(o, current.Value);
                }
            }
        }

        public void TestClientOMTools(string Url)
        {
            using (new ClientContext(Url))
            {
            }

            if (new XDocument() == null)
            {
                throw new Exception(".NET 3.5 is not installed.");
            }
        }

        public string AddExternalList(string title, string description, string entity, string entityNamespace,
            string lobInstance, string specificFinder, NWSAdapter callingAdapter)
        {
            string str;
            if (callingAdapter == null)
            {
                throw new ArgumentException("Calling adapter must be specified.", "callingAdapter");
            }

            if (string.IsNullOrEmpty(entity))
            {
                throw new ArgumentException("Entity name must be specified.", "entity");
            }

            if (string.IsNullOrEmpty(entityNamespace))
            {
                throw new ArgumentException("Entity namespace must be specified.", "entityNamespace");
            }

            if (string.IsNullOrEmpty(lobInstance))
            {
                throw new ArgumentException("Lob instance name must be specified.", "lobInstance");
            }

            try
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.AquireCookieLock();
                }

                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    ListCollection lists = clientContext.Web.Lists;
                    ListDataSource listDataSource = new ListDataSource();
                    IDictionary<string, string> strs = new Dictionary<string, string>()
                    {
                        { "Entity", entity },
                        { "EntityNamespace", entityNamespace },
                        { "LobSystemInstance", lobInstance },
                        { "SpecificFinder", specificFinder }
                    };
                    ListCreationInformation listCreationInformation = new ListCreationInformation()
                    {
                        Title = title,
                        Description = description,
                        DataSourceProperties = strs,
                        TemplateType = 600
                    };
                    List list = lists.Add(listCreationInformation);
                    Expression<Func<List, object>>[] expressionArray = new Expression<Func<List, object>>[1];
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(List), "list");
                    UnaryExpression unaryExpression = Expression.Convert(
                        Expression.Property(parameterExpression,
                            (MethodInfo)MethodBase.GetMethodFromHandle(typeof(List).GetMethod("get_Id").MethodHandle)),
                        typeof(object));
                    ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
                    expressionArray[0] =
                        Expression.Lambda<Func<List, object>>(unaryExpression, parameterExpressionArray);
                    clientContext.Load<List>(list, expressionArray);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    str = list.Id.ToString();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return str;
        }

        public string GetExternalItems(string entity, string entityNamespace, string operation, string listID,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter == null)
            {
                throw new ArgumentException("Calling adapter must be specified.", "callingAdapter");
            }

            if (string.IsNullOrEmpty(listID))
            {
                throw new ArgumentException("ListID must be specified.", "listID");
            }

            string result;
            try
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.AquireCookieLock();
                }

                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web = clientContext.Web;
                    List byId = web.Lists.GetById(new Guid(listID));
                    if (byId == null)
                    {
                        throw new Exception(string.Format(
                            "Unable to find the list with ID '{0}' on the following URL '{1}'", listID,
                            callingAdapter.Url));
                    }

                    CamlQuery query_ = new CamlQuery();
                    IQueryable<ListItem> items = byId.GetItems(query_);
                    IEnumerable<ListItem> enumerable = clientContext.LoadQuery<ListItem>(items);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    StringWriter stringWriter = new StringWriter();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                    xmlTextWriter.WriteStartElement("SPExternalItemCollection");
                    foreach (ListItem current in enumerable)
                    {
                        xmlTextWriter.WriteStartElement("SPExternalItem");
                        xmlTextWriter.WriteAttributeString("BdcIdentity",
                            (current.FieldValues["BdcIdentity"] != null)
                                ? current.FieldValues["BdcIdentity"].ToString()
                                : string.Empty);
                        xmlTextWriter.WriteAttributeString("Identity", current.Id.ToString());
                        foreach (KeyValuePair<string, object> current2 in current.FieldValues)
                        {
                            if (string.CompareOrdinal(current2.Key, "BdcIdentity") != 0)
                            {
                                xmlTextWriter.WriteStartElement("SPExternalItemProperty");
                                xmlTextWriter.WriteAttributeString("Name", current2.Key);
                                xmlTextWriter.WriteAttributeString("Value",
                                    (current2.Value == null) ? string.Empty : current2.Value.ToString());
                                xmlTextWriter.WriteAttributeString("Identifier", false.ToString());
                                xmlTextWriter.WriteEndElement();
                            }
                        }

                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                    result = stringWriter.ToString();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        public string GetListProperties(string sListTitle, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("List");
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    List byTitle = clientContext.Web.Lists.GetByTitle(sListTitle);
                    clientContext.Load<List>(byTitle, new Expression<Func<List, object>>[]
                    {
                        (List list) => (object)list.OnQuickLaunch
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    xmlTextWriter.WriteAttributeString("OnQuickLaunch", byTitle.OnQuickLaunch.ToString());
                }

                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                result = stringWriter.ToString();
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private string GetFullPath(List list, string sFolder)
        {
            return list.RootFolder.ServerRelativeUrl +
                   ((string.IsNullOrEmpty(sFolder) || sFolder.StartsWith("/")) ? "" : "/") + sFolder;
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string listItemsInternal;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web = clientContext.Web;
                    listItemsInternal = this.GetListItemsInternal(sListID, sIDs, sFields, sParentFolder, bRecursive,
                        itemTypes, web, sListSettings, getOptions, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return listItemsInternal;
        }

        private string GetListItemsInternal(string sListID, string sIDs, string sFields, string sParentFolder,
            bool bRecursive, ListItemQueryType itemTypes, Web currentWeb, string sListSettings,
            GetListItemOptions getOptions, NWSAdapter callingAdapter)
        {
            DateTime arg_05_0 = DateTime.Now;
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            List<string> list = null;
            if (!string.IsNullOrEmpty(sFields))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sFields);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("./Fields/Field");
                list = new List<string>(xmlNodeList.Count);
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    list.Add(xmlNode.Attributes["Name"].Value);
                }
            }

            ListItemCollection listItemCollection = this.GetListItemCollection(sListID, sIDs, sParentFolder, bRecursive,
                itemTypes, currentWeb, sFields, callingAdapter);
            Hashtable htExternalization = null;
            if (list == null)
            {
                list = new List<string>();
                foreach (ListItem current in ((IEnumerable<ListItem>)listItemCollection))
                {
                    foreach (KeyValuePair<string, object> current2 in current.FieldValues)
                    {
                        string item = current2.Key.StartsWith("ows_") ? current2.Key.Substring(4) : current2.Key;
                        list.Add(item);
                    }
                }
            }

            xmlTextWriter.WriteStartElement("ListItems");
            foreach (ListItem current3 in ((IEnumerable<ListItem>)listItemCollection))
            {
                this.GetItemXML(currentWeb, xmlTextWriter, list, current3, true, htExternalization, getOptions,
                    callingAdapter);
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        private ListItemCollection GetListItemCollection(string sListID, string sIDs, string sParentFolder,
            bool bRecursive, ListItemQueryType itemTypes, Web currentWeb, string sFields,
            SharePointAdapter callingAdapter)
        {
            ListCollection lists1 = currentWeb.Lists;
            List list2;
            currentWeb.Context.Load<ListCollection>(lists1, new Expression<Func<ListCollection, object>>[]
            {
                (ListCollection lists) => from list in lists.Include(new Expression<Func<List, object>>[]
                    {
                        (List list) => (object)list.BaseTemplate,
                        (List list) => (object)list.BaseType,
                        (List list) => list.RootFolder.ServerRelativeUrl,
                        (List list) => list.Title,
                        (List list) => (object)list.EnableModeration
                    })
                    where list.Id == new Guid(sListID)
                    select list
            });
            ClientOMTools.CallExecuteQuery(currentWeb.Context, callingAdapter);
            if (lists1.Count == 1)
            {
                list2 = lists1[0];
                return this.GetListItemCollection(list2, sIDs, sParentFolder, bRecursive, itemTypes, sFields,
                    callingAdapter);
            }

            throw new Exception("List could not be retrieved");
        }

        private ListItemCollection GetListItemCollection(List list, string sIDs, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sFields, SharePointAdapter callingAdapter)
        {
            itemTypes = ((list.BaseTemplate == 108) ? (itemTypes | ListItemQueryType.Folder) : itemTypes);
            CamlQuery listQuery = ClientOMTools.GetListQuery();
            string text = listQuery.ViewXml;
            string text2 = NWSAdapter.BuildQuery(sIDs, sParentFolder, bRecursive, false, itemTypes, new string[]
            {
                "FileDirRef",
                "ID"
            });
            if (!string.IsNullOrEmpty(text2))
            {
                text = text.Insert(listQuery.ViewXml.IndexOf("<ExpandUserField>"), "<Query>" + text2 + "</Query>");
            }

            if (!string.IsNullOrEmpty(sFields))
            {
                text = text.Insert(listQuery.ViewXml.IndexOf("<ExpandUserField>"),
                    "<ViewFields>" + this.MapFieldsStringToSPQueryViewFields(sFields) + "</ViewFields>");
            }

            if (!listQuery.ViewXml.Contains("<RowLimit>"))
            {
                text = text.Insert(listQuery.ViewXml.IndexOf("</View>") + 1,
                    "<RowLimit>" + ClientOMTools.s_iGetListQueryRowLimit.Value.ToString() + "</RowLimit>");
            }

            listQuery.DatesInUtc = true;
            listQuery.ViewXml = text;
            ListItemCollection items = list.GetItems(listQuery);
            list.Context.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[0]);
            ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            return items;
        }

        private static CamlQuery GetListQuery()
        {
            if (!ClientOMTools.s_iGetListQueryRowLimit.HasValue)
            {
                ClientOMTools.s_iGetListQueryRowLimit = new uint?(4294967295u);
            }

            return new CamlQuery
            {
                ViewXml = new StringBuilder().Append("<View Scope='RecursiveAll'>")
                    .Append("<ExpandUserField>False</ExpandUserField>")
                    .Append("<IncludeMandatoryColumns>False</IncludeMandatoryColumns>")
                    .Append("<MeetingInstanceId>-2</MeetingInstanceId>")
                    .AppendFormat("<RowLimit>{0}</RowLimit>", ClientOMTools.s_iGetListQueryRowLimit.Value)
                    .Append("</View>").ToString(),
                DatesInUtc = true
            };
        }

        public string GetFolders(string sListID, string sFileLeafRefs, string sParentFolder, List list,
            NWSAdapter callingAdapter)
        {
            Web parentWeb = list.ParentWeb;
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            string[] collection = string.IsNullOrEmpty(sFileLeafRefs)
                ? new string[0]
                : sFileLeafRefs.Split(new char[]
                {
                    ','
                });
            List<string> list2 = new List<string>(collection);
            string text = parentWeb.ServerRelativeUrl.Trim(new char[]
            {
                '/'
            });
            string text2 = (text.Length > 0 && sParentFolder.ToLower().StartsWith(text.ToLower()))
                ? sParentFolder.Trim(new char[]
                {
                    '/'
                }).Substring(text.Length + 1)
                : sParentFolder;
            xmlTextWriter.WriteStartElement("Folders");
            foreach (string current in list2)
            {
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = new StringBuilder().Append("<View>").Append("<Query>").Append("<Where><And>")
                    .Append("<Eq><FieldRef Name=\"FSObjType\"/><Value Type=\"Integer\">1</Value></Eq>")
                    .AppendFormat("<Eq><FieldRef Name=\"FileLeafRef\"/><Value Type=\"Text\">{0}</Value></Eq>", current)
                    .Append("</And></Where>").Append("</Query>").Append("</View>").ToString();
                if (!string.IsNullOrEmpty(text2))
                {
                    camlQuery.FolderServerRelativeUrl = text2;
                }

                ListItem listItem = null;
                ListItemCollection items = list.GetItems(camlQuery);
                bool flag = true;
                list.Context.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[]
                {
                    (ListItemCollection f) => f.Include(new Expression<Func<ListItem, object>>[]
                    {
                        (ListItem folder) => (object)folder.Id,
                        (ListItem folder) => folder["FileLeafRef"],
                        (ListItem folder) => folder["FileDirRef"],
                        (ListItem folder) => folder["ContentTypeId"],
                        (ListItem folder) => folder["Editor"],
                        (ListItem folder) => folder["Author"],
                        (ListItem folder) => folder["Created"],
                        (ListItem folder) => folder["Modified"],
                        (ListItem folder) => (object)folder.HasUniqueRoleAssignments
                    })
                });
                try
                {
                    ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
                }
                catch (Exception ex)
                {
                    if (!(ex is ServerException) || (ex as ServerException).ServerErrorCode != -2147024809)
                    {
                        throw;
                    }

                    list.Context.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[]
                    {
                        (ListItemCollection f) => f.Include(new Expression<Func<ListItem, object>>[]
                        {
                            (ListItem folder) => (object)folder.Id,
                            (ListItem folder) => folder["FileLeafRef"],
                            (ListItem folder) => folder["FileDirRef"],
                            (ListItem folder) => folder["ContentTypeId"],
                            (ListItem folder) => folder["Editor"],
                            (ListItem folder) => folder["Created"],
                            (ListItem folder) => folder["Modified"],
                            (ListItem folder) => (object)folder.HasUniqueRoleAssignments
                        })
                    });
                    ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
                    flag = false;
                }

                if (items.Count == 1)
                {
                    listItem = items[0];
                }

                if (listItem != null)
                {
                    xmlTextWriter.WriteStartElement("Folder");
                    xmlTextWriter.WriteAttributeString("ID", listItem.Id.ToString());
                    xmlTextWriter.WriteAttributeString("FileLeafRef", listItem["FileLeafRef"].ToString());
                    xmlTextWriter.WriteAttributeString("FileDirRef", listItem["FileDirRef"].ToString());
                    xmlTextWriter.WriteAttributeString("ContentTypeId", listItem["ContentTypeId"].ToString());
                    string text3 = listItem["Editor"].ToString();
                    if (text3 != null)
                    {
                        xmlTextWriter.WriteAttributeString("Editor", text3);
                    }

                    if (flag)
                    {
                        string text4 = listItem["Author"].ToString();
                        if (text4 != null)
                        {
                            xmlTextWriter.WriteAttributeString("Author", text4);
                        }
                    }

                    xmlTextWriter.WriteAttributeString("Created",
                        Utils.FormatDate(callingAdapter.TimeZone.LocalTimeToUtc((DateTime)listItem["Created"])));
                    xmlTextWriter.WriteAttributeString("Modified",
                        Utils.FormatDate(callingAdapter.TimeZone.LocalTimeToUtc((DateTime)listItem["Modified"])));
                    xmlTextWriter.WriteAttributeString("HasUniquePermissions",
                        listItem.HasUniqueRoleAssignments.ToString());
                    xmlTextWriter.WriteEndElement();
                }
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        private void GetAllDayEventFields(List list, XmlNode itemXml, out string sStartDate, out string sEndDate)
        {
            sStartDate = null;
            sEndDate = null;
            Field field = null;
            FieldType fieldType = FieldType.AllDayEvent;
            foreach (Field current in ((IEnumerable<Field>)list.Fields))
            {
                if (current.FieldTypeKind == fieldType)
                {
                    field = current;
                    break;
                }
            }

            if (field == null)
            {
                return;
            }

            XmlAttribute xmlAttribute = itemXml.Attributes[field.InternalName];
            if (xmlAttribute == null)
            {
                return;
            }

            bool flag;
            if (bool.TryParse(xmlAttribute.Value, out flag))
            {
                if (!flag)
                {
                    return;
                }
            }
            else if (xmlAttribute.Value != "1")
            {
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(field.SchemaXml);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//FieldRefs/FieldRef[@RefType=\"StartDate\"]");
            if (xmlNode != null && xmlNode.Attributes["Name"] != null)
            {
                sStartDate = xmlNode.Attributes["Name"].Value;
            }

            XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//FieldRefs/FieldRef[@RefType=\"EndDate\"]");
            if (xmlNode2 != null && xmlNode2.Attributes["Name"] != null)
            {
                sEndDate = xmlNode2.Attributes["Name"].Value;
            }
        }

        private string MapFieldsStringToSPQueryViewFields(string sQuery)
        {
            if (sQuery != null)
            {
                sQuery = sQuery.Replace("<Fields>", "");
                sQuery = sQuery.Replace("</Fields>", "");
                sQuery = sQuery.Replace("<Field ", "<FieldRef ");
                return sQuery;
            }

            return "";
        }

        public bool SupportsEmbedding(string sPageUrl, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            bool result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    if (!sPageUrl.StartsWith("/"))
                    {
                        sPageUrl = "/" + sPageUrl;
                    }

                    Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                        clientContext.Web.GetFileByServerRelativeUrl(sPageUrl);
                    result = this.SupportsEmbedding(fileByServerRelativeUrl, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private bool SupportsEmbedding(Microsoft.SharePoint.Client.File wppFile, SharePointAdapter callingAdapter)
        {
            bool result;
            try
            {
                result = (this.GetRichTextEmbeddingField(wppFile, callingAdapter) != null);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private string GetRichTextEmbeddingField(Microsoft.SharePoint.Client.File wppFile,
            SharePointAdapter callingAdapter)
        {
            string result = null;
            wppFile.Context.Load<ListItem>(wppFile.ListItemAllFields, new Expression<Func<ListItem, object>>[0]);
            ClientOMTools.CallExecuteQuery(wppFile.Context, callingAdapter);
            string[] array = this.fieldsThatSupportEmbedding;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if (wppFile.ListItemAllFields.FieldValues.ContainsKey(text))
                {
                    result = text;
                    break;
                }
            }

            return result;
        }

        public string AddFolder(string folderXmlString, string sParentFolderPath, string sListId,
            AddFolderOptions folderOptions, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string folders;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    bool overwrite = folderOptions.Overwrite;
                    ListItem listItem = null;
                    List byId = clientContext.Web.Lists.GetById(new Guid(sListId));
                    clientContext.Load<Folder>(byId.RootFolder, new Expression<Func<Folder, object>>[0]);
                    clientContext.Load<List>(byId, new Expression<Func<List, object>>[]
                    {
                        (List list) => list.Title,
                        (List list) => (object)list.BaseTemplate,
                        (List list) => list.ContentTypes,
                        (List list) => list.ParentWeb.ServerRelativeUrl
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(folderXmlString);
                    string text = (xmlNode.Attributes["FileLeafRef"] != null)
                        ? xmlNode.Attributes["FileLeafRef"].Value
                        : xmlNode.Attributes["Title"].Value;
                    ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                    listItemCreationInformation.UnderlyingObjectType = FileSystemObjectType.Folder;
                    listItemCreationInformation.LeafName = text;
                    string fullPath = this.GetFullPath(byId, sParentFolderPath);
                    if (!string.IsNullOrEmpty(sParentFolderPath))
                    {
                        listItemCreationInformation.FolderUrl =
                            Utils.JoinUrl(byId.RootFolder.ServerRelativeUrl, sParentFolderPath);
                    }

                    byId.Context.Load<FieldCollection>(byId.Fields, new Expression<Func<FieldCollection, object>>[]
                    {
                        (FieldCollection fields) => fields.Include(new Expression<Func<Field, object>>[]
                        {
                            (Field field) => field.TypeAsString,
                            (Field field) => field.InternalName,
                            (Field field) => field.SchemaXml,
                            (Field field) => (object)field.ReadOnlyField,
                            (Field field) => (object)field.Required,
                            (Field field) => (object)field.FieldTypeKind,
                            (Field field) => field.DefaultValue
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = new StringBuilder().Append("<View>").Append("<Query>").Append("<Where><And>")
                        .Append("<Eq><FieldRef Name=\"FSObjType\"/><Value Type=\"Integer\">1</Value></Eq>")
                        .AppendFormat("<Eq><FieldRef Name=\"FileLeafRef\"/><Value Type=\"Text\">{0}</Value></Eq>", text)
                        .Append("</And></Where>").Append("</Query>").Append("</View>").ToString();
                    if (!string.IsNullOrEmpty(listItemCreationInformation.FolderUrl))
                    {
                        camlQuery.FolderServerRelativeUrl = listItemCreationInformation.FolderUrl;
                    }

                    ListItemCollection items = byId.GetItems(camlQuery);
                    clientContext.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[]
                    {
                        (ListItemCollection folders0) => folders0.Include(new Expression<Func<ListItem, object>>[]
                        {
                            (ListItem folder) => (object)folder.Id
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (items.Count == 1)
                    {
                        listItem = items[0];
                    }

                    if (overwrite && listItem != null)
                    {
                        listItem.DeleteObject();
                        listItem = null;
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    }

                    if (!overwrite && listItem != null)
                    {
                        folders = this.GetFolders(sListId, text, fullPath, byId, callingAdapter);
                    }
                    else
                    {
                        if (listItem == null)
                        {
                            listItem = byId.AddItem(listItemCreationInformation);
                            string a = (xmlNode.Attributes["HTML_x0020_File_x0020_Type"] != null)
                                ? xmlNode.Attributes["HTML_x0020_File_x0020_Type"].Value
                                : string.Empty;
                            if (a == "SharePoint.DocumentSet")
                            {
                                ContentType documentSetContentType =
                                    this.GetDocumentSetContentType(clientContext, byId, xmlNode, callingAdapter);
                                listItem["ContentTypeId"] = documentSetContentType.Id.ToString();
                                listItem.Update();
                                ClientOMTools.CallExecuteQuery(listItem.Context, callingAdapter);
                            }
                            else if (xmlNode.Attributes["ContentTypeId"] != null)
                            {
                                listItem["ContentTypeId"] = xmlNode.Attributes["ContentTypeId"].Value;
                                listItem.Update();
                                ClientOMTools.CallExecuteQuery(listItem.Context, callingAdapter);
                            }

                            this.UpdateItemMetadata(byId, listItem, xmlNode, false, callingAdapter);
                            listItem.Update();
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        }

                        folders = this.GetFolders(sListId, text, fullPath, byId, callingAdapter);
                    }
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return folders;
        }

        private ContentType GetDocumentSetContentType(ClientContext clientContext, List targetList, XmlNode folderXML,
            SharePointAdapter callingAdapter)
        {
            string sDocSetContentTypeName = folderXML.Attributes["ContentType"].Value;
            ContentTypeCollection contentTypes = targetList.ContentTypes;
            clientContext.Load<ContentTypeCollection>(contentTypes,
                new Expression<Func<ContentTypeCollection, object>>[]
                {
                    (ContentTypeCollection types) => types.Include(new Expression<Func<ContentType, object>>[]
                    {
                        (ContentType type) => type.Id,
                        (ContentType type) => type.Name,
                        (ContentType type) => type.Parent
                    })
                });
            IEnumerable<ContentType> source = clientContext.LoadQuery<ContentType>(from c in contentTypes
                where c.Name == sDocSetContentTypeName
                select c);
            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            return source.FirstOrDefault<ContentType>();
        }

        public string AddDocument(string sListID, string sFolder, string sListItemXML, byte[] fileContents,
            AddDocumentOptions options, NWSAdapter callingAdapter)
        {
            bool overwrite = options.Overwrite;
            bool arg_1E_0 = options.CorrectInvalidNames;
            List list0 = null;
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web0 = clientContext.Web;
                    clientContext.Load<Web>(web0, new Expression<Func<Web, object>>[]
                    {
                        (Web web) => web.Title
                    });
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
                    XmlNode xmlNode2 = XmlUtility.StringToXmlNode(sListItemXML);
                    Convert.ToInt32(xmlNode2.Attributes["ID"].Value);
                    string value = xmlNode2.Attributes["FileLeafRef"].Value;
                    DateTime dateTime = (xmlNode2.Attributes["Modified"] == null)
                        ? DateTime.UtcNow
                        : Utils.ParseDateAsUtc(xmlNode2.Attributes["Modified"].Value);
                    dateTime = callingAdapter.TimeZone.UtcToLocalTime(dateTime);
                    string text;
                    string text2;
                    CheckinType checkinType;
                    string text3;
                    this.GetVersionInfo(xmlNode, out text, out text2, out checkinType, out text3);
                    ListCollection lists0 = clientContext.Web.Lists;
                    clientContext.Load<ListCollection>(lists0, new Expression<Func<ListCollection, object>>[]
                    {
                        (ListCollection lists) => from list in lists.Include(new Expression<Func<List, object>>[]
                            {
                                (List list) => (object)list.BaseTemplate,
                                (List list) => (object)list.BaseType,
                                (List list) => list.RootFolder.ServerRelativeUrl,
                                (List list) => list.Title,
                                (List list) => (object)list.EnableModeration,
                                (List list) => list.ContentTypes,
                                (List list) => (object)list.EnableVersioning,
                                (List list) => (object)list.EnableMinorVersions,
                                (List list) => (object)list.ForceCheckout,
                                (List list) => (object)list.Id
                            })
                            where list.Id == new Guid(sListID)
                            select list
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (lists0.Count != 1)
                    {
                        throw new Exception("List could not be retrieved");
                    }

                    list0 = lists0[0];
                    list0.Context.Load<FieldCollection>(list0.Fields, new Expression<Func<FieldCollection, object>>[]
                    {
                        (FieldCollection fields) => fields.Include(new Expression<Func<Field, object>>[]
                        {
                            (Field field) => field.TypeAsString,
                            (Field field) => field.InternalName,
                            (Field field) => field.SchemaXml,
                            (Field field) => (object)field.ReadOnlyField,
                            (Field field) => (object)field.Required,
                            (Field field) => (object)field.FieldTypeKind,
                            (Field field) => field.DefaultValue
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    string fullPath = this.GetFullPath(list0, sFolder);
                    Folder folderByServerRelativeUrl = web0.GetFolderByServerRelativeUrl(fullPath);
                    clientContext.Load<Folder>(folderByServerRelativeUrl, new Expression<Func<Folder, object>>[]
                    {
                        (Folder folder) => folder.ServerRelativeUrl
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    string fileUrl = fullPath + "/" + value;
                    string fileUrlToLower = fullPath + "/" + value.ToLower();
                    Microsoft.SharePoint.Client.File file0 = null;
                    FileCollection files1 = folderByServerRelativeUrl.Files;
                    clientContext.Load<FileCollection>(files1, new Expression<Func<FileCollection, object>>[]
                    {
                        (FileCollection files) => from file in files.Include(
                                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                                {
                                    (Microsoft.SharePoint.Client.File file) => (object)file.Exists,
                                    (Microsoft.SharePoint.Client.File file) => (object)file.CheckOutType,
                                    (Microsoft.SharePoint.Client.File file) => (object)file.ListItemAllFields.Id,
                                    (Microsoft.SharePoint.Client.File file) => file.Name
                                })
                            where file.ServerRelativeUrl == fileUrl || file.ServerRelativeUrl == fileUrlToLower
                            select file
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (files1.Count == 1)
                    {
                        file0 = files1[0];
                    }

                    if (file0 != null && overwrite)
                    {
                        file0.DeleteObject();
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        file0 = null;
                    }

                    bool flag = file0 == null;
                    bool flag2 = true;
                    if (list0.EnableVersioning && !list0.ForceCheckout)
                    {
                        list0.ForceCheckout = true;
                        list0.Update();
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        flag2 = false;
                    }

                    ArrayList arrayList = new ArrayList();
                    if (flag)
                    {
                        bool flag3 = Utils.IsDocumentWikiPage(xmlNode, "");
                        bool flag4 = list0.BaseTemplate == 212;
                        if (flag3)
                        {
                            file0 = folderByServerRelativeUrl.Files.AddTemplateFile(fullPath + "/" + value,
                                TemplateFileType.WikiPage);
                        }
                        else
                        {
                            if (flag4)
                            {
                                clientContext.Load<Web>(web0, new Expression<Func<Web, object>>[]
                                {
                                    (Web web) => web.ServerRelativeUrl
                                });
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                string str = folderByServerRelativeUrl.ServerRelativeUrl
                                    .Remove(0, web0.ServerRelativeUrl.Length).TrimStart(new char[]
                                    {
                                        '/'
                                    });
                                callingAdapter.AddMeetingWorkspacePage(str + "/" + value, overwrite, null);
                                Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                                    web0.GetFileByServerRelativeUrl(fullPath + "/" + value);
                                clientContext.Load<Microsoft.SharePoint.Client.File>(fileByServerRelativeUrl,
                                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[0]);
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                StringBuilder stringBuilder = new StringBuilder();
                                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                                xmlTextWriter.WriteStartElement("ListItems");
                                clientContext.Load<ListItem>(fileByServerRelativeUrl.ListItemAllFields,
                                    new Expression<Func<ListItem, object>>[]
                                    {
                                        (ListItem item) => (object)item.HasUniqueRoleAssignments
                                    });
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                this.GetItemXML(web0, xmlTextWriter, new List<string>(new string[]
                                    {
                                        "ID",
                                        "Modified",
                                        "FileRef",
                                        "FileLeafRef",
                                        "GUID"
                                    }), fileByServerRelativeUrl.ListItemAllFields, true, null, new GetListItemOptions(),
                                    callingAdapter);
                                xmlTextWriter.WriteEndElement();
                                result = stringBuilder.ToString();
                                return result;
                            }

                            FileCreationInformation fileCreationInformation = new FileCreationInformation();
                            fileCreationInformation.Content = new byte[0];
                            fileCreationInformation.Url = fullPath + "/" + value;
                            file0 = folderByServerRelativeUrl.Files.Add(fileCreationInformation);
                        }

                        clientContext.Load<Microsoft.SharePoint.Client.File>(file0,
                            new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                            {
                                (Microsoft.SharePoint.Client.File file) => (object)file.ListItemAllFields.Id,
                                (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_UIVersionString"],
                                (Microsoft.SharePoint.Client.File file) => (object)file.UIVersion
                            });
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        double num;
                        double.TryParse(text2, out num);
                        if (list0.EnableVersioning && !file0.ListItemAllFields["_UIVersionString"].ToString()
                                .Equals(num.ToString("0.0")))
                        {
                            flag = false;
                            if (list0.EnableMinorVersions)
                            {
                                file0.CheckIn("To Be Deleted", CheckinType.MinorCheckIn);
                            }
                            else
                            {
                                file0.CheckIn("To Be Deleted", CheckinType.MajorCheckIn);
                            }

                            ClientOMTools.CallExecuteQuery(file0.Context, callingAdapter);
                            arrayList.Add(file0.UIVersion);
                        }
                    }
                    else if (options.OverrideCheckouts && file0.CheckOutType == CheckOutType.None)
                    {
                        try
                        {
                            file0.UndoCheckOut();
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    this.IncrementDocumentVersions(text2, folderByServerRelativeUrl, callingAdapter, ref file0, list0,
                        ref arrayList);
                    this.UpdateDocumentData(web0, list0, folderByServerRelativeUrl, file0, xmlNode, fileContents,
                        options, flag, fullPath, clientContext, callingAdapter);
                    this.DeleteTempDocumentVersions(file0, arrayList, callingAdapter);
                    if (!flag2)
                    {
                        list0.ForceCheckout = false;
                        list0.Update();
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    }

                    clientContext.Load<ListItem>(file0.ListItemAllFields, new Expression<Func<ListItem, object>>[]
                    {
                        (ListItem item) => (object)item.Id,
                        (ListItem item) => item["GUID"]
                    });
                    clientContext.Load<Microsoft.SharePoint.Client.File>(file0,
                        new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                        {
                            (Microsoft.SharePoint.Client.File file) => file.Name
                        });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    StringBuilder stringBuilder2 = new StringBuilder(300);
                    using (StringWriter stringWriter = new StringWriter(stringBuilder2))
                    {
                        using (XmlTextWriter xmlTextWriter2 = new XmlTextWriter(stringWriter))
                        {
                            xmlTextWriter2.WriteStartElement("ListItems");
                            xmlTextWriter2.WriteStartElement("ListItem");
                            xmlTextWriter2.WriteAttributeString("ID", file0.ListItemAllFields.Id.ToString());
                            xmlTextWriter2.WriteAttributeString("FileDirRef",
                                folderByServerRelativeUrl.ServerRelativeUrl.Trim(new char[]
                                {
                                    '/'
                                }));
                            xmlTextWriter2.WriteAttributeString("GUID", file0.ListItemAllFields["GUID"].ToString());
                            xmlTextWriter2.WriteAttributeString("FileLeafRef", file0.Name);
                            xmlTextWriter2.WriteAttributeString("Modified", Utils.FormatDate(dateTime));
                            xmlTextWriter2.WriteEndElement();
                            xmlTextWriter2.WriteEndElement();
                        }
                    }

                    result = stringBuilder2.ToString();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        public string AddListItem(string slistItemXML, string sParentFolderPath, string sListId,
            string[] attachementNames, byte[][] attachmentContents, AddListItemOptions itemOptions, string sWebUrl,
            NWSAdapter callingAdapter)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            ListItem listItem = null;
            List list = null;
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(sWebUrl, callingAdapter))
                {
                    ListItem listItem2 = null;
                    bool initialVersion = itemOptions.InitialVersion;
                    ListCollection lists0 = clientContext.Web.Lists;
                    clientContext.Load<ListCollection>(lists0, new Expression<Func<ListCollection, object>>[]
                    {
                        (ListCollection lists) => from list1 in lists.Include(new Expression<Func<List, object>>[]
                            {
                                (List list2) => (object)list2.BaseTemplate,
                                (List list3) => (object)list3.BaseType,
                                (List list4) => list4.RootFolder.ServerRelativeUrl,
                                (List list5) => list5.Title,
                                (List list6) => (object)list6.EnableModeration,
                                (List list7) => list7.ContentTypes,
                                (List list8) => (object)list8.EnableVersioning,
                                (List list9) => (object)list9.Id
                            })
                            where list1.Id == new Guid(sListId)
                            select list1
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (lists0.Count != 1)
                    {
                        throw new Exception("List could not be retrieved");
                    }

                    list = lists0[0];
                    if (list.BaseTemplate == 202)
                    {
                        string result = null;
                        return result;
                    }

                    list.Context.Load<FieldCollection>(list.Fields, new Expression<Func<FieldCollection, object>>[]
                    {
                        (FieldCollection fields) => fields.Include(new Expression<Func<Field, object>>[]
                        {
                            (Field field) => field.TypeAsString,
                            (Field field) => field.InternalName,
                            (Field field) => field.SchemaXml,
                            (Field field) => (object)field.ReadOnlyField,
                            (Field field) => (object)field.Required,
                            (Field field) => (object)field.FieldTypeKind,
                            (Field field) => field.DefaultValue
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(slistItemXML);
                    int num = 0;
                    if (itemOptions.ParentID.HasValue)
                    {
                        num = itemOptions.ParentID.Value;
                    }

                    bool flag = xmlNode.Attributes["ContentType"] != null &&
                                xmlNode.Attributes["ContentType"].Value == "Message";
                    bool flag2 = xmlNode.Attributes["FSObjType"] != null &&
                                 xmlNode.Attributes["FSObjType"].Value == "1";
                    string text = (xmlNode.Attributes["Title"] != null) ? xmlNode.Attributes["Title"].Value : "";
                    if (flag2 && xmlNode.Attributes["FileLeafRef"] != null)
                    {
                        text = xmlNode.Attributes["FileLeafRef"].Value;
                    }

                    int num2;
                    if (list.BaseType == BaseType.Issue && xmlNode.Attributes["IssueID"] != null &&
                        !string.IsNullOrEmpty(xmlNode.Attributes["IssueID"].Value))
                    {
                        num2 = Convert.ToInt32(xmlNode.Attributes["IssueID"].Value);
                    }
                    else if (xmlNode.Attributes["ID"] != null)
                    {
                        num2 = Convert.ToInt32(xmlNode.Attributes["ID"].Value);
                    }
                    else
                    {
                        num2 = -1;
                    }

                    ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                    if (flag2)
                    {
                        listItemCreationInformation.UnderlyingObjectType = FileSystemObjectType.Folder;
                        listItemCreationInformation.LeafName = text;
                    }

                    if (!string.IsNullOrEmpty(sParentFolderPath))
                    {
                        listItemCreationInformation.FolderUrl =
                            Utils.JoinUrl(list.RootFolder.ServerRelativeUrl, sParentFolderPath);
                    }

                    int num3 = flag2 ? 1 : 0;
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = new StringBuilder().Append("<View>").Append("<Query>").Append("<Where><And>")
                        .Append("<Eq><FieldRef Name=\"FSObjType\"/><Value Type=\"Integer\">" + num3 + "</Value></Eq>")
                        .AppendFormat("<Eq><FieldRef Name=\"ID\"/><Value Type=\"Text\">{0}</Value></Eq>", num2)
                        .Append("</And></Where>").Append("</Query>").Append("</View>").ToString();
                    if (listItemCreationInformation != null &&
                        !string.IsNullOrEmpty(listItemCreationInformation.FolderUrl))
                    {
                        camlQuery.FolderServerRelativeUrl = listItemCreationInformation.FolderUrl;
                    }

                    ListItemCollection items3 = list.GetItems(camlQuery);
                    clientContext.Load<ListItemCollection>(items3, new Expression<Func<ListItemCollection, object>>[]
                    {
                        (ListItemCollection items) => items.Include(new Expression<Func<ListItem, object>>[]
                        {
                            (ListItem item) => (object)item.Id
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (items3.Count == 1)
                    {
                        listItem2 = items3[0];
                    }

                    if (num2 >= 0 && (flag2 || !initialVersion))
                    {
                        if (flag2 && text != null)
                        {
                            try
                            {
                                if (!itemOptions.Overwrite)
                                {
                                    StringBuilder stringBuilder = new StringBuilder();
                                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                                    xmlTextWriter.WriteStartElement("ListItems");
                                    this.GetItemXML(list.ParentWeb, xmlTextWriter, null, listItem, false, null,
                                        new GetListItemOptions(), callingAdapter);
                                    xmlTextWriter.WriteEndElement();
                                    string result = stringBuilder.ToString();
                                    return result;
                                }

                                listItem = listItem2;
                                goto IL_B33;
                            }
                            catch
                            {
                                goto IL_B33;
                            }
                        }

                        listItem = listItem2;
                    }

                    IL_B33:
                    bool flag3 = listItem == null || (listItem != null && itemOptions.Overwrite);
                    if (flag3 && listItem != null)
                    {
                        listItem.DeleteObject();
                        listItem = null;
                    }

                    if (!flag2 || listItem == null)
                    {
                        if (list.BaseTemplate == 102)
                        {
                            callingAdapter.UpdateListAllowMultiResponses(sListId, callingAdapter.Url, true);
                        }

                        if (flag3)
                        {
                            if (list.BaseTemplate == 108)
                            {
                                if (num == 0)
                                {
                                    listItem = Utility.CreateNewDiscussion(clientContext, list, text);
                                }
                                else
                                {
                                    ListItem itemById = list.GetItemById(num);
                                    clientContext.Load<ListItem>(itemById, new Expression<Func<ListItem, object>>[0]);
                                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                    listItem = Utility.CreateNewDiscussionReply(clientContext, itemById);
                                }

                                listItem.Update();
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                            }

                            if (listItem == null)
                            {
                                listItem = list.AddItem(listItemCreationInformation);
                                string a = (xmlNode.Attributes["HTML_x0020_File_x0020_Type"] != null)
                                    ? xmlNode.Attributes["HTML_x0020_File_x0020_Type"].Value
                                    : string.Empty;
                                if (flag2 && a == "SharePoint.DocumentSet")
                                {
                                    ContentType documentSetContentType =
                                        this.GetDocumentSetContentType(clientContext, list, xmlNode, callingAdapter);
                                    listItem["ContentTypeId"] = documentSetContentType.Id.ToString();
                                }

                                listItem.Update();
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                            }
                        }

                        ArrayList arrayList = new ArrayList();
                        Microsoft.SharePoint.Client.File file3 = null;
                        if (!flag2 && list.EnableVersioning && !flag)
                        {
                            clientContext.Load<ListItem>(listItem, new Expression<Func<ListItem, object>>[]
                            {
                                (ListItem item) => item["FileRef"]
                            });
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                            file3 = list.ParentWeb.GetFileByServerRelativeUrl(listItem["FileRef"].ToString());
                            clientContext.Load<Microsoft.SharePoint.Client.File>(file3,
                                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[0]);
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                            if (flag3)
                            {
                                string obj = (xmlNode.Attributes["_VersionString"] == null)
                                    ? "1.0"
                                    : xmlNode.Attributes["_VersionString"].Value;
                                clientContext.Load<Microsoft.SharePoint.Client.File>(file3,
                                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                                    {
                                        (Microsoft.SharePoint.Client.File file) =>
                                            file.ListItemAllFields["_UIVersionString"]
                                    });
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                if (!file3.ListItemAllFields["_UIVersionString"].Equals(obj))
                                {
                                    flag3 = false;
                                    arrayList.Add(file3.ListItemAllFields["_UIVersionString"]);
                                }
                            }

                            this.IncrementListItemVersions(list, xmlNode, file3, flag3, callingAdapter, ref arrayList);
                        }

                        this.UpdateListItemData(list.ParentWeb, list, listItem, xmlNode, attachementNames,
                            attachmentContents, itemOptions, false, callingAdapter);
                        if (!flag2 && list.EnableVersioning && !flag)
                        {
                            this.DeleteTempListItemVersions(file3, arrayList, callingAdapter);
                        }

                        if (list.BaseTemplate == 102)
                        {
                            callingAdapter.UpdateListAllowMultiResponses(sListId, callingAdapter.Url, false);
                        }
                    }
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            StringBuilder stringBuilder2 = new StringBuilder();
            XmlTextWriter xmlTextWriter2 = new XmlTextWriter(new StringWriter(stringBuilder2));
            xmlTextWriter2.WriteStartElement("ListItems");
            this.GetItemXML(list.ParentWeb, xmlTextWriter2, new List<string>(new string[]
            {
                "ID",
                "Modified",
                "FileRef",
                "FSObjType"
            }), listItem, false, null, new GetListItemOptions(), callingAdapter);
            xmlTextWriter2.WriteEndElement();
            return stringBuilder2.ToString();
        }

        private void IncrementListItemVersions(List list, XmlNode itemXML, Microsoft.SharePoint.Client.File targetItem,
            bool bAdding, SharePointAdapter callingAdapter, ref ArrayList tempVersions)
        {
            if (!list.EnableVersioning || itemXML.Attributes["_UIVersionString"] == null)
            {
                return;
            }

            targetItem.Context.Load<Microsoft.SharePoint.Client.File>(targetItem,
                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                {
                    (Microsoft.SharePoint.Client.File item) => item.ListItemAllFields["Modified"],
                    (Microsoft.SharePoint.Client.File item) => item.ListItemAllFields["_UIVersionString"]
                });
            ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
            try
            {
                int num = (int)Convert.ToDouble(itemXML.Attributes["_UIVersionString"].Value,
                    ClientOMTools.s_CultureInfo);
                int num2 = (targetItem.ListItemAllFields["_UIVersionString"] == null)
                    ? 0
                    : ((int)Convert.ToDouble(targetItem.ListItemAllFields["_UIVersionString"],
                        ClientOMTools.s_CultureInfo));
                int num3 = num - num2 - 1;
                if (num3 > 0)
                {
                    for (int i = 0; i < num3; i++)
                    {
                        targetItem.ListItemAllFields["Modified"] = targetItem.ListItemAllFields["Modified"];
                        targetItem.ListItemAllFields.Update();
                        ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
                        tempVersions.Add(targetItem.ListItemAllFields["_UIVersionString"]);
                        if (list.EnableModeration)
                        {
                            targetItem.ListItemAllFields["_ModerationStatus"] = 0;
                            targetItem.ListItemAllFields.Update();
                            ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void IncrementDocumentVersions(string sVersionString, Folder folder, SharePointAdapter callingAdapter,
            ref Microsoft.SharePoint.Client.File targetFile, List parentList, ref ArrayList tempVersions)
        {
            if (!parentList.EnableVersioning)
            {
                return;
            }

            folder.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                {
                    (Microsoft.SharePoint.Client.File file) => (object)file.CheckedOutByUser.Id,
                    (Microsoft.SharePoint.Client.File file) => file.Name,
                    (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_UIVersionString"]
                });
            ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
            int num = (int)Convert.ToDouble(sVersionString, ClientOMTools.s_CultureInfo);
            int num2;
            int num3;
            try
            {
                num2 = ((targetFile.ListItemAllFields.Id == -1)
                    ? 0
                    : ((targetFile.ListItemAllFields["_UIVersionString"] == null)
                        ? 0
                        : ((int)Convert.ToDouble(targetFile.ListItemAllFields["_UIVersionString"],
                            ClientOMTools.s_CultureInfo))));
                num3 = ((targetFile.ListItemAllFields.Id == -1)
                    ? 0
                    : ((targetFile.ListItemAllFields["_UIVersionString"] == null)
                        ? 0
                        : Convert.ToInt32(targetFile.ListItemAllFields["_UIVersionString"].ToString()
                            .Substring(targetFile.ListItemAllFields["_UIVersionString"].ToString().IndexOf('.') + 1,
                                1))));
            }
            catch (InvalidOperationException)
            {
                num2 = 0;
                num3 = 0;
            }

            int num4 = 0;
            if (sVersionString.Contains('.'))
            {
                num4 = Convert.ToInt32(sVersionString.Substring(sVersionString.IndexOf('.') + 1, 1));
            }

            int num5 = num - num2 - 1 + ((num4 > 0) ? 1 : 0);
            for (int i = 0; i < num5; i++)
            {
                targetFile.CheckOut();
                ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
                FileCreationInformation fileCreationInformation = new FileCreationInformation();
                fileCreationInformation.Url = targetFile.Name;
                fileCreationInformation.Overwrite = true;
                fileCreationInformation.Content = new byte[0];
                targetFile = folder.Files.Add(fileCreationInformation);
                ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
                targetFile.CheckIn("Temporary Version - (To be deleted)", CheckinType.MajorCheckIn);
                if (parentList.EnableModeration)
                {
                    targetFile.ListItemAllFields["_ModerationStatus"] = 0;
                    targetFile.ListItemAllFields.Update();
                }

                folder.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => (object)file.UIVersion
                    });
                ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
                tempVersions.Add(targetFile.UIVersion);
            }

            if (parentList.EnableMinorVersions)
            {
                int num6 = num4 - num3 - 1;
                for (int j = 0; j < num6; j++)
                {
                    try
                    {
                        int arg_3B8_0 = targetFile.CheckedOutByUser.Id;
                    }
                    catch (InvalidOperationException)
                    {
                        targetFile.CheckOut();
                        ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
                    }

                    FileCreationInformation fileCreationInformation2 = new FileCreationInformation();
                    fileCreationInformation2.Url = targetFile.Name;
                    fileCreationInformation2.Overwrite = true;
                    fileCreationInformation2.Content = new byte[0];
                    targetFile = folder.Files.Add(fileCreationInformation2);
                    ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
                    targetFile.CheckIn("Temporary Version - To be deleted", CheckinType.MinorCheckIn);
                    folder.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                        new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                        {
                            (Microsoft.SharePoint.Client.File file) => (object)file.UIVersion
                        });
                    ClientOMTools.CallExecuteQuery(folder.Context, callingAdapter);
                    tempVersions.Add(targetFile.UIVersion);
                }
            }
        }

        private void CheckInFile(Microsoft.SharePoint.Client.File targetFile, List parentList, string sCheckinComments,
            CheckinType checkinType, bool bAdding, string sVersionString, SharePointAdapter callingAdapter)
        {
            if (!parentList.EnableVersioning)
            {
                targetFile.CheckIn(sCheckinComments, checkinType);
                ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                return;
            }

            if (!bAdding)
            {
                targetFile.CheckIn(sCheckinComments, checkinType);
            }
            else
            {
                targetFile.Context.Load<FileVersionCollection>(targetFile.Versions,
                    new Expression<Func<FileVersionCollection, object>>[0]);
                ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                FileVersion fileVersion = targetFile.Versions[0];
                string versionLabel = fileVersion.VersionLabel;
                if (sVersionString == versionLabel || (!parentList.EnableMinorVersions &&
                                                       sVersionString.StartsWith("0.") && versionLabel == "1.0"))
                {
                    targetFile.CheckIn(sCheckinComments, CheckinType.OverwriteCheckIn);
                }
                else
                {
                    targetFile.CheckIn(sCheckinComments, checkinType);
                    targetFile.Versions.DeleteByID(fileVersion.ID);
                }
            }

            ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
        }

        private void DeleteTempListItemVersions(Microsoft.SharePoint.Client.File targetFile, ArrayList tempVersions,
            SharePointAdapter callingAdapter)
        {
            string[] array = new string[tempVersions.Count];
            tempVersions.CopyTo(array);
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string versionlabel = array2[i];
                try
                {
                    targetFile.Versions.DeleteByLabel(versionlabel);
                    ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                }
                catch
                {
                }
            }
        }

        private void DeleteTempDocumentVersions(Microsoft.SharePoint.Client.File targetFile, ArrayList tempVersions,
            SharePointAdapter callingAdapter)
        {
            int[] array = new int[tempVersions.Count];
            tempVersions.CopyTo(array);
            int[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                int vid = array2[i];
                try
                {
                    targetFile.Versions.DeleteByID(vid);
                    ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                }
                catch
                {
                }
            }

            targetFile.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                {
                    (Microsoft.SharePoint.Client.File file) => (object)file.Level,
                    (Microsoft.SharePoint.Client.File file) => file.Versions
                });
            ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
            for (int j = targetFile.Versions.Count - 1; j >= 0; j--)
            {
                FileVersion fileVersion = targetFile.Versions[j];
                if (!fileVersion.IsCurrentVersion &&
                    fileVersion.CheckInComment == "Temporary Version - (To be deleted)")
                {
                    try
                    {
                        targetFile.Versions.DeleteByID(fileVersion.ID);
                        break;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        private void UpdateDocumentData(Web currentWeb, List targetList, Folder folder,
            Microsoft.SharePoint.Client.File targetFile, XmlNode listItemXML, byte[] fileContents,
            IUpdateDocumentOptions options, bool bAdding, string sFullFolderPath, ClientContext clientContext,
            NWSAdapter callingAdapter)
        {
            bool flag = Utils.IsDocumentWikiPage(listItemXML, "");
            bool flag2 = targetList.BaseTemplate == 212;
            bool bPreserveSharePointDocumentID = options is AddDocumentOptions &&
                                                 (options as AddDocumentOptions).PreserveSharePointDocumentIDs;
            int num;
            string value;
            string value2;
            DateTime dateTime;
            DateTime dateTime2;
            this.GetListItemFileInfo(listItemXML, out num, out value, out value2, out dateTime, out dateTime2, false,
                currentWeb, callingAdapter);
            string text;
            string sVersionString;
            CheckinType checkinType;
            string sCheckinComments;
            this.GetVersionInfo(listItemXML, out text, out sVersionString, out checkinType, out sCheckinComments);
            bool flag3 = false;
            try
            {
                targetFile.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => (object)file.CheckedOutByUser.Id
                    });
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                int arg_F2_0 = targetFile.CheckedOutByUser.Id;
            }
            catch (InvalidOperationException)
            {
                targetFile.CheckOut();
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                flag3 = true;
            }

            targetFile.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                {
                    (Microsoft.SharePoint.Client.File file) => file.Name
                });
            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            if (!flag && !flag2)
            {
                if (!ExternalizationUtils.IsExternalizedContent(listItemXML.Attributes["IsExternalized"]) ||
                    !options.ShallowCopyExternalizedData)
                {
                    targetFile = ClientOMTools.AddFileToFolder(clientContext, folder,
                        sFullFolderPath + "/" + targetFile.Name, fileContents, callingAdapter);
                }
                else
                {
                    targetFile = ClientOMTools.AddFileToFolder(clientContext, folder,
                        sFullFolderPath + "/" + targetFile.Name, new byte[0], callingAdapter);
                }
            }

            this.UpdateItemMetadata(targetList, targetFile.ListItemAllFields, listItemXML, false, callingAdapter,
                clientContext, bPreserveSharePointDocumentID);
            targetFile.ListItemAllFields.Update();
            targetFile.Context.Load<Microsoft.SharePoint.Client.File>(targetFile,
                new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                {
                    (Microsoft.SharePoint.Client.File file) => (object)file.ListItemAllFields.Id,
                    (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["Editor"],
                    (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["FileLeafRef"]
                });
            try
            {
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            }
            catch (Exception ex)
            {
                if (!(ex.Message == "Value does not fall within the expected range."))
                {
                    throw ex;
                }

                clientContext.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["FileLeafRef"]
                    });
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                string serverRelativeUrl = targetList.RootFolder.ServerRelativeUrl + "/" +
                                           targetFile.ListItemAllFields["FileLeafRef"].ToString();
                targetFile = targetList.ParentWeb.GetFileByServerRelativeUrl(serverRelativeUrl);
                clientContext.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => (object)file.ListItemAllFields.Id,
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["Editor"]
                    });
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            }

            FieldUserValue fieldUserValue = null;
            try
            {
                fieldUserValue = (FieldUserValue)targetFile.ListItemAllFields["Editor"];
            }
            catch (InvalidOperationException)
            {
            }

            this.CheckInFile(targetFile, targetList, sCheckinComments, checkinType, bAdding && flag3, sVersionString,
                callingAdapter);
            bool enableMinorVersions = targetList.EnableMinorVersions;
            bool enableVersioning = targetList.EnableVersioning;
            string text2 = (listItemXML.Attributes["_ModerationStatus"] == null)
                ? "2"
                : listItemXML.Attributes["_ModerationStatus"].Value;
            if (targetList.EnableModeration)
            {
                clientContext.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_UIVersionString"],
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_UIVersion"]
                    });
                ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                targetFile.ListItemAllFields["_ModerationStatus"] = text2;
                targetFile.ListItemAllFields.Update();
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            }

            if (!enableMinorVersions || checkinType != CheckinType.MinorCheckIn)
            {
                bool enableModeration = targetList.EnableModeration;
                if (enableVersioning)
                {
                    if (enableModeration && text2.Equals("0"))
                    {
                        targetList.EnableModeration = false;
                    }

                    targetList.EnableVersioning = false;
                    targetList.ForceCheckout = false;
                    targetList.Update();
                }
                else if (enableModeration && text2.Equals("0"))
                {
                    targetList.EnableModeration = false;
                    targetList.ForceCheckout = false;
                    targetList.Update();
                }

                clientContext.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_UIVersionString"],
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_UIVersion"]
                    });
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                if (!string.IsNullOrEmpty(value2))
                {
                    targetFile.ListItemAllFields["Modified_x0020_By"] = value2;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    targetFile.ListItemAllFields["Created_x0020_By"] = value;
                }

                if (fieldUserValue != null)
                {
                    targetFile.ListItemAllFields["Editor"] = fieldUserValue;
                }

                targetFile.ListItemAllFields["Modified"] = dateTime2;
                targetFile.ListItemAllFields["Created"] = dateTime;
                targetFile.ListItemAllFields.Update();
                ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                if (enableVersioning)
                {
                    targetList.EnableVersioning = true;
                    if (enableMinorVersions)
                    {
                        targetList.EnableMinorVersions = true;
                    }

                    if (enableModeration && !targetList.EnableModeration)
                    {
                        targetList.EnableModeration = true;
                    }

                    targetList.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
                else if (enableModeration && !targetList.EnableModeration)
                {
                    targetList.EnableModeration = true;
                    targetList.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }

            if ((!enableVersioning && !targetList.EnableModeration) || (!enableMinorVersions && text2.Equals("1")))
            {
                if (enableVersioning)
                {
                    targetList.EnableVersioning = false;
                    targetList.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }

                clientContext.Load<Microsoft.SharePoint.Client.File>(targetFile,
                    new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                    {
                        (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["_ModerationStatus"]
                    });
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                if (targetFile.ListItemAllFields["_ModerationStatus"].ToString() != text2)
                {
                    targetFile.ListItemAllFields["_ModerationStatus"] = text2;
                    targetFile.ListItemAllFields.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (text2 == "2")
                    {
                        if (!string.IsNullOrEmpty(value2))
                        {
                            targetFile.ListItemAllFields["Modified_x0020_By"] = value2;
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            targetFile.ListItemAllFields["Created_x0020_By"] = value;
                        }

                        if (fieldUserValue != null)
                        {
                            targetFile.ListItemAllFields["Editor"] = fieldUserValue;
                        }

                        targetFile.ListItemAllFields["Modified"] = dateTime2;
                        targetFile.ListItemAllFields["Created"] = dateTime;
                        targetFile.ListItemAllFields.Update();
                        ClientOMTools.CallExecuteQuery(targetFile.Context, callingAdapter);
                    }
                }

                if (enableVersioning)
                {
                    targetList.EnableVersioning = true;
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }
        }

        private static Microsoft.SharePoint.Client.File AddFileToFolder(ClientContext clientContext, Folder folder,
            string sServerRelativeUrl, byte[] fileContents, SharePointAdapter callingAdapter)
        {
            Site site = clientContext.Site;
            clientContext.Load<Site>(site, new Expression<Func<Site, object>>[]
            {
                (Site s) => (object)s.MaxItemsPerThrottledOperation
            });
            clientContext.Load<Folder>(folder, new Expression<Func<Folder, object>>[]
            {
                (Folder f) => (object)f.ItemCount
            });
            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            if ((long)folder.ItemCount >= (long)((ulong)site.MaxItemsPerThrottledOperation))
            {
                FileCreationInformation fileCreationInformation = new FileCreationInformation();
                fileCreationInformation.Content = fileContents;
                fileCreationInformation.Url = sServerRelativeUrl;
                fileCreationInformation.Overwrite = true;
                Microsoft.SharePoint.Client.File result = folder.Files.Add(fileCreationInformation);
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                return result;
            }

            try
            {
                if (callingAdapter.AdapterProxy == null || callingAdapter.AdapterProxy.Address == null ||
                    string.IsNullOrEmpty(callingAdapter.AdapterProxy.Address.AbsoluteUri))
                {
                    using (MemoryStream memoryStream = new MemoryStream(fileContents))
                    {
                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, sServerRelativeUrl,
                            memoryStream, true);
                        goto IL_188;
                    }
                }

                FileCreationInformation fileCreationInformation2 = new FileCreationInformation();
                fileCreationInformation2.Content = fileContents;
                fileCreationInformation2.Url = sServerRelativeUrl;
                fileCreationInformation2.Overwrite = true;
                folder.Files.Add(fileCreationInformation2);
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                IL_188: ;
            }
            catch (Exception)
            {
                FileCreationInformation fileCreationInformation3 = new FileCreationInformation();
                fileCreationInformation3.Content = fileContents;
                fileCreationInformation3.Url = sServerRelativeUrl;
                fileCreationInformation3.Overwrite = true;
                folder.Files.Add(fileCreationInformation3);
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            }

            FileCollection files2 = folder.Files;
            clientContext.Load<FileCollection>(files2, new Expression<Func<FileCollection, object>>[]
            {
                (FileCollection files) => from file in files
                    where file.ServerRelativeUrl == sServerRelativeUrl
                    select file
            });
            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
            if (files2.Count == 1)
            {
                return files2[0];
            }

            return null;
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    ListCollection lists6 = clientContext.Web.Lists;
                    List list1;
                    clientContext.Load<ListCollection>(lists6, new Expression<Func<ListCollection, object>>[]
                    {
                        (ListCollection lists) => from list in lists.Include(new Expression<Func<List, object>>[]
                            {
                                (List list) => (object)list.BaseTemplate,
                                (List list) => (object)list.BaseType,
                                (List list) => list.RootFolder.ServerRelativeUrl,
                                (List list) => list.Title,
                                (List list) => (object)list.EnableModeration,
                                (List list) => list.ContentTypes,
                                (List list) => (object)list.EnableVersioning,
                                (List list) => (object)list.Id
                            })
                            where list.Id == new Guid(sListID)
                            select list
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (lists6.Count != 1)
                    {
                        throw new Exception("List could not be retrieved");
                    }

                    list1 = lists6[0];
                    clientContext.Load<FieldCollection>(list1.Fields, new Expression<Func<FieldCollection, object>>[]
                    {
                        (FieldCollection fields) => fields.Include(new Expression<Func<Field, object>>[]
                        {
                            (Field field) => field.TypeAsString,
                            (Field field) => field.InternalName,
                            (Field field) => field.SchemaXml,
                            (Field field) => (object)field.ReadOnlyField,
                            (Field field) => (object)field.Required,
                            (Field field) => (object)field.FieldTypeKind,
                            (Field field) => field.DefaultValue
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    XmlNode itemXML = XmlUtility.StringToXmlNode(sListItemXML);
                    ListItem itemById = list1.GetItemById(iItemID);
                    clientContext.Load<ListItem>(itemById, new Expression<Func<ListItem, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    this.UpdateListItemData(list1.ParentWeb, list1, itemById, itemXML, attachmentNames,
                        attachmentContents, updateOptions, false, callingAdapter);
                    bool flag = attachmentNames != null && attachmentNames.Length > 0;
                    if (flag)
                    {
                        callingAdapter.AddListItemAttachments(attachmentNames, attachmentContents, list1.Id.ToString(),
                            itemById.Id.ToString());
                    }
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return this.GetListItems(sListID, iItemID.ToString(), null, null, true,
                ListItemQueryType.ListItem | ListItemQueryType.Folder, null, new GetListItemOptions(), callingAdapter);
        }

        private void UpdateListItemData(Web currentWeb, List list, ListItem targetItem, XmlNode itemXML,
            string[] attachmentNames, byte[][] attachmentContents, IUpdateListItemOptions options,
            bool bConvertToFolder, NWSAdapter callingAdapter)
        {
            int num;
            string text;
            string text2;
            DateTime dateTime;
            DateTime dateTime2;
            this.GetListItemFileInfo(itemXML, out num, out text, out text2, out dateTime, out dateTime2, false,
                currentWeb, callingAdapter);
            targetItem.Context.Load<ListItem>(targetItem, new Expression<Func<ListItem, object>>[]
            {
                (ListItem item) => item["_UIVersionString"],
                (ListItem item) => item["_UIVersion"],
                (ListItem item) => (object)item.Id
            });
            ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
            bool enableVersioning = list.EnableVersioning;
            bool enableModeration = list.EnableModeration;
            string text3 = (itemXML.Attributes["_UIVersionString"] == null)
                ? "1.0"
                : itemXML.Attributes["_UIVersionString"].Value;
            if (text3.Equals("1.0") && enableVersioning)
            {
                list.EnableVersioning = false;
                list.Update();
                ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            }

            this.UpdateItemMetadata(list, targetItem, itemXML, bConvertToFolder, callingAdapter);
            targetItem.Update();
            ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            if (enableVersioning && !list.EnableVersioning)
            {
                list.EnableVersioning = true;
                list.Update();
                ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            }

            FieldUserValue fieldUserValue = null;
            try
            {
                fieldUserValue = (FieldUserValue)targetItem["Editor"];
            }
            catch (InvalidOperationException)
            {
            }

            bool flag = attachmentNames != null && attachmentNames.Length > 0;
            if (flag)
            {
                if (enableVersioning)
                {
                    list.EnableVersioning = false;
                    list.Update();
                    ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
                }

                callingAdapter.AddListItemAttachments(attachmentNames, attachmentContents, list.Id.ToString(),
                    targetItem.Id.ToString());
                if (enableVersioning && !list.EnableVersioning)
                {
                    list.EnableVersioning = true;
                    list.Update();
                    ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
                }
            }

            targetItem.Context.Load<ListItem>(targetItem, new Expression<Func<ListItem, object>>[]
            {
                (ListItem item) => item["_UIVersionString"],
                (ListItem item) => item["_UIVersion"]
            });
            ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
            if (!enableModeration)
            {
                if (enableVersioning)
                {
                    list.EnableVersioning = false;
                    list.Update();
                    ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
                }
            }
            else if (itemXML.Attributes["FSObjType"] != null && itemXML.Attributes["FSObjType"].Value.Equals("1") &&
                     enableModeration && list.BaseType == BaseType.DocumentLibrary &&
                     itemXML.Attributes["_ModerationStatus"].Value.Equals("0"))
            {
                list.EnableModeration = false;
                list.Update();
                ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            }
            else if (enableModeration && list.BaseType != BaseType.DocumentLibrary)
            {
                targetItem["_ModerationStatus"] = itemXML.Attributes["_ModerationStatus"].Value;
            }

            if (fieldUserValue != null && fieldUserValue.LookupValue != null && fieldUserValue.LookupValue != "***")
            {
                targetItem["Editor"] = fieldUserValue;
            }

            targetItem["Created"] = dateTime;
            targetItem["Modified"] = dateTime2;
            targetItem.Update();
            ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
            if (itemXML.Attributes["FSObjType"] != null && itemXML.Attributes["FSObjType"].Value.Equals("1") &&
                list.EnableModeration && list.BaseType == BaseType.DocumentLibrary &&
                itemXML.Attributes["_ModerationStatus"].Value.Equals("1"))
            {
                targetItem["_ModerationStatus"] = itemXML.Attributes["_ModerationStatus"].Value;
                targetItem.Update();
                ClientOMTools.CallExecuteQuery(targetItem.Context, callingAdapter);
            }

            if (enableModeration && !list.EnableModeration)
            {
                list.EnableModeration = true;
                list.Update();
                ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            }

            if (enableVersioning && !list.EnableVersioning)
            {
                list.EnableVersioning = true;
                list.Update();
                ClientOMTools.CallExecuteQuery(list.Context, callingAdapter);
            }
        }

        private void UpdateItemMetadata(List spList, ListItem spListItem, XmlNode itemXML, bool bConvertToFolder,
            NWSAdapter callingAdapter)
        {
            this.UpdateItemMetadata(spList, spListItem, itemXML, bConvertToFolder, callingAdapter, null);
        }

        private void UpdateItemMetadata(List spList, ListItem spListItem, XmlNode itemXML, bool bConvertToFolder,
            NWSAdapter callingAdapter, ClientContext context)
        {
            this.UpdateItemMetadata(spList, spListItem, itemXML, bConvertToFolder, callingAdapter, context, false);
        }

        private void UpdateItemMetadata(List spList, ListItem spListItem, XmlNode itemXML, bool bConvertToFolder,
            NWSAdapter callingAdapter, ClientContext clientContext, bool bPreserveSharePointDocumentID)
        {
            TimeZoneInformation arg_07_0 = callingAdapter.TimeZone;
            FileSystemObjectType fileSystemObjectType = FileSystemObjectType.File;
            try
            {
                fileSystemObjectType = spListItem.FileSystemObjectType;
            }
            catch
            {
                if (spList.BaseTemplate == 108)
                {
                    fileSystemObjectType = FileSystemObjectType.Folder;
                }
            }

            string b;
            string b2;
            this.GetAllDayEventFields(spList, itemXML, out b, out b2);
            bPreserveSharePointDocumentID = (bPreserveSharePointDocumentID &&
                                             this.DocumentIDFeatureEnabled(spList, spListItem, callingAdapter,
                                                 clientContext));
            foreach (Field current in ((IEnumerable<Field>)spList.Fields))
            {
                if (!(current.InternalName == "ContentType"))
                {
                    try
                    {
                        bool flag = bConvertToFolder &&
                                    current.InternalName.Equals("FSObjType", StringComparison.OrdinalIgnoreCase) &&
                                    spList.BaseTemplate == 2100;
                        if (flag)
                        {
                            try
                            {
                                spListItem[current.InternalName] = FileSystemObjectType.Folder;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(
                                    ("The folder '" + itemXML.Attributes["Title"] != null)
                                        ? itemXML.Attributes["Title"].Value
                                        : ("' could not be created properly in slide library '" + spList.Title +
                                           "'. Message: " + ex.Message), ex);
                            }
                        }
                        else if (fileSystemObjectType != FileSystemObjectType.Folder &&
                                 current.InternalName == "ContentTypeId" && spList.BaseTemplate != 108)
                        {
                            this.SetContentTypeID(spList, spListItem, current, itemXML, callingAdapter);
                        }
                        else if (Utils.IsWritableColumn(current.InternalName, current.ReadOnlyField,
                                     current.TypeAsString, spList.BaseTemplate,
                                     fileSystemObjectType == FileSystemObjectType.Folder,
                                     current.SchemaXml.Contains(" BdcField=\""), false) ||
                                 (bPreserveSharePointDocumentID && (current.InternalName.Equals("_dlc_DocId") ||
                                                                    current.InternalName.Equals("_dlc_DocIdUrl"))))
                        {
                            XmlAttribute xmlAttribute = itemXML.Attributes[current.InternalName];
                            if (xmlAttribute != null)
                            {
                                try
                                {
                                    if (current.InternalName == b ||
                                        (current.InternalName == b2 && current.TypeAsString == "DateTime"))
                                    {
                                        if (!string.IsNullOrEmpty(xmlAttribute.Value))
                                        {
                                            DateTime dateTime = Utils.ParseDateAsUtc(xmlAttribute.Value);
                                            spListItem[current.InternalName] = dateTime;
                                        }
                                    }
                                    else
                                    {
                                        object obj = this.CastStringToFieldType(xmlAttribute.Value, current,
                                            callingAdapter);
                                        if (obj != null && !obj.ToString().Equals(""))
                                        {
                                            spListItem[current.InternalName] = obj;
                                        }
                                    }

                                    continue;
                                }
                                catch (Exception ex2)
                                {
                                    throw new Exception("Source Value: " + xmlAttribute.Value + ", Exception: " +
                                                        ex2.Message);
                                }
                            }

                            if (!current.Required)
                            {
                                if (string.IsNullOrEmpty(current.DefaultValue))
                                {
                                    continue;
                                }
                            }

                            try
                            {
                                if (fileSystemObjectType != FileSystemObjectType.Folder &&
                                    spListItem[current.InternalName] == null)
                                {
                                    object obj2 = this.CastStringToFieldType(
                                        (current.DefaultValue == null) ? "" : current.DefaultValue, current,
                                        callingAdapter);
                                    if (obj2 != null)
                                    {
                                        spListItem[current.InternalName] = obj2;
                                    }
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                object obj3;
                                if (current.GetType().IsAssignableFrom(typeof(FieldDateTime)))
                                {
                                    obj3 = null;
                                }
                                else
                                {
                                    obj3 = this.CastStringToFieldType(
                                        (current.DefaultValue == null) ? "" : current.DefaultValue, current,
                                        callingAdapter);
                                }

                                if (obj3 != null)
                                {
                                    spListItem[current.InternalName] = obj3;
                                }
                            }
                        }
                    }
                    catch (Exception ex3)
                    {
                        throw new Exception(string.Concat(new string[]
                        {
                            "Error setting field data: Target Field Name: ",
                            current.InternalName,
                            ", Target Type: ",
                            current.TypeAsString,
                            ", ",
                            ex3.Message
                        }));
                    }
                }
            }
        }

        private bool DocumentIDFeatureEnabled(List spList, ListItem spListItem, NWSAdapter callingAdapter,
            ClientContext clientContext)
        {
            bool result;
            try
            {
                if (clientContext == null)
                {
                    result = false;
                }
                else if (spList.BaseType != BaseType.DocumentLibrary)
                {
                    result = false;
                }
                else
                {
                    Site site = clientContext.Site;
                    FeatureCollection features0 = site.Features;
                    clientContext.Load<FeatureCollection>(features0, new Expression<Func<FeatureCollection, object>>[]
                    {
                        (FeatureCollection features) => from feature in features
                            where feature.DefinitionId == FeatureGuids.SharePointDocumentId
                            select feature
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    result = (features0.Count > 0);
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private void SetContentTypeID(List spList, ListItem spListItem, Field field, XmlNode itemXML,
            NWSAdapter callingAdapter)
        {
            XmlNode attribute = XmlUtility.GetAttribute(itemXML, null, "ContentType", false);
            object obj = null;
            if (attribute != null)
            {
                string value = attribute.Value;
                if (spList.BaseTemplate == 107 &&
                    (value == "Workflow Task" || value == "Office SharePoint Server Workflow Task"))
                {
                    spListItem[field.InternalName] = "";
                    return;
                }

                foreach (ContentType current in ((IEnumerable<ContentType>)spList.ContentTypes))
                {
                    if (current.Name == value)
                    {
                        obj = current.Id;
                    }
                }
            }

            if (obj == null)
            {
                XmlNode attribute2 = XmlUtility.GetAttribute(itemXML, null, field.InternalName, false);
                if (attribute2 != null)
                {
                    obj = this.CastStringToFieldType(attribute2.Value, field, callingAdapter);
                }
            }

            try
            {
                spListItem[field.InternalName] = obj;
            }
            catch (Exception ex)
            {
                throw new Exception("Source Value: " + obj.ToString() + ", Exception: " + ex.Message);
            }
        }

        private object CastStringToFieldType(string sValue, Field field, NWSAdapter callingAdapter)
        {
            return this.CastStringToFieldType(sValue, field.TypeAsString, field.SchemaXml, callingAdapter);
        }

        private object CastStringToFieldType(string sValue, string fieldType, string fieldSchemaXml,
            NWSAdapter callingAdapter)
        {
            switch (fieldType)
            {
                case "Text":
                    return sValue;
                case "DateTime":
                case "PublishingScheduleStartDateFieldType":
                case "PublishingScheduleEndDateFieldType":
                    if (string.IsNullOrEmpty(sValue))
                    {
                        return null;
                    }

                    return Utils.ParseDateAsUtc(sValue);
                case "User":
                {
                    FieldUserValue fieldUserValue = new FieldUserValue();
                    string text = callingAdapter.GetIDFromUser(sValue);
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(fieldSchemaXml);
                    if (text == null && xmlNode.Attributes["UserSelectionMode"] != null &&
                        xmlNode.Attributes["UserSelectionMode"].Value.Equals("PeopleAndGroups"))
                    {
                        text = callingAdapter.GetIDFromGroup(sValue);
                    }

                    int value = -1;
                    if (int.TryParse(text, out value))
                    {
                        fieldUserValue.LookupId = Convert.ToInt32(value);
                        return fieldUserValue;
                    }

                    return text;
                }
                case "UserMulti":
                {
                    string[] array = sValue.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries);
                    int num2 = 0;
                    List<FieldUserValue> list = new List<FieldUserValue>();
                    string[] array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        string text2 = array2[i];
                        string value2 = null;
                        string iDFromUser = callingAdapter.GetIDFromUser(text2);
                        if (iDFromUser != null)
                        {
                            value2 = iDFromUser;
                        }
                        else
                        {
                            XmlNode xmlNode2 = XmlUtility.StringToXmlNode(fieldSchemaXml);
                            if (xmlNode2.Attributes["UserSelectionMode"] != null &&
                                xmlNode2.Attributes["UserSelectionMode"].Value.Equals("PeopleAndGroups"))
                            {
                                string iDFromGroup = callingAdapter.GetIDFromGroup(text2);
                                if (iDFromGroup != null)
                                {
                                    value2 = iDFromGroup;
                                }
                            }
                        }

                        list.Add(new FieldUserValue
                        {
                            LookupId = Convert.ToInt32(value2)
                        });
                        num2++;
                    }

                    return list.ToArray();
                }
                case "Lookup":
                {
                    int num3 = -1;
                    if (!int.TryParse(sValue, out num3))
                    {
                        return sValue;
                    }

                    if (num3 == 0)
                    {
                        return null;
                    }

                    return new FieldLookupValue
                    {
                        LookupId = num3
                    };
                }
                case "LookupMulti":
                {
                    string[] array3 = sValue.Split(new string[]
                    {
                        ";#"
                    }, StringSplitOptions.RemoveEmptyEntries);
                    List<FieldLookupValue> list2 = new List<FieldLookupValue>();
                    int num4 = -1;
                    for (int j = 0; j < array3.Length; j++)
                    {
                        if (int.TryParse(array3[j], out num4) && num4 != 0)
                        {
                            list2.Add(new FieldLookupValue
                            {
                                LookupId = num4
                            });
                        }
                    }

                    if (list2.Count > 0)
                    {
                        return list2.ToArray();
                    }

                    return sValue;
                }
                case "URL":
                {
                    string[] array4 = sValue.Split(new char[]
                    {
                        ','
                    });
                    string text3 = array4[0];
                    if (text3.Length >= 256)
                    {
                        return text3.Substring(0, 255) + "," + ((array4.Length > 1) ? array4[1] : "");
                    }

                    return sValue;
                }
            }

            return sValue;
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref Dictionary<string, string> fieldsLookupCache,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Microsoft.SharePoint.Client.File file = null;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(fileXml);
                    XmlAttribute xmlAttribute = xmlDocument.DocumentElement.Attributes["FileLeafRef"];
                    if (xmlAttribute == null)
                    {
                        throw new ArgumentException("FileLeafRef cannot be missing");
                    }

                    string value = xmlAttribute.Value;
                    Uri uri = new Uri(string.Concat(new string[]
                    {
                        callingAdapter.Url.Trim(new char[]
                        {
                            '/',
                            '\\',
                            ' '
                        }),
                        "/",
                        listName,
                        "/",
                        folderPath.Trim(new char[]
                        {
                            '/',
                            '\\',
                            ' '
                        })
                    }));
                    string text = uri.LocalPath.TrimEnd(new char[]
                    {
                        '/'
                    }) + "/" + value;
                    if (fileContents.LongLength > 1048576L)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(fileContents))
                        {
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, text, memoryStream,
                                options.Overwrite);
                            file = clientContext.Web.GetFileByServerRelativeUrl(text);
                            goto IL_1AA;
                        }
                    }

                    FileCreationInformation parameters = new FileCreationInformation
                    {
                        Overwrite = options.Overwrite,
                        Url = text,
                        Content = fileContents
                    };
                    Folder folderByServerRelativeUrl = clientContext.Web.GetFolderByServerRelativeUrl(uri.AbsolutePath);
                    file = folderByServerRelativeUrl.Files.Add(parameters);
                    IL_1AA:
                    if (fieldsLookupCache == null)
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        List byId = clientContext.Web.Lists.GetById(listId);
                        clientContext.Load<FieldCollection>(byId.Fields, new Expression<Func<FieldCollection, object>>[]
                        {
                            (FieldCollection fields) => fields.Include(new Expression<Func<Field, object>>[]
                            {
                                (Field field) => field.InternalName,
                                (Field field) => field.SchemaXml
                            })
                        });
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        foreach (Field current in ((IEnumerable<Field>)byId.Fields))
                        {
                            if (!dictionary.ContainsKey(current.InternalName))
                            {
                                dictionary.Add(current.InternalName, current.SchemaXml);
                            }
                        }

                        fieldsLookupCache = dictionary;
                    }

                    bool flag = fieldsLookupCache.ContainsKey("_ModerationStatus") &&
                                ClientOMTools.IsTargetModerationStatusRequired(fieldsLookupCache["_ModerationStatus"]);
                    foreach (XmlAttribute xmlAttribute2 in xmlDocument.DocumentElement.Attributes)
                    {
                        if (!string.Equals("Id", xmlAttribute2.Name, StringComparison.InvariantCultureIgnoreCase) &&
                            !string.Equals("ContentType", xmlAttribute2.Name,
                                StringComparison.InvariantCultureIgnoreCase) &&
                            !string.Equals("_ModerationStatus", xmlAttribute2.Name,
                                StringComparison.InvariantCultureIgnoreCase) &&
                            fieldsLookupCache.ContainsKey(xmlAttribute2.Name))
                        {
                            XmlDocument xmlDocument2 = new XmlDocument();
                            xmlDocument2.LoadXml(fieldsLookupCache[xmlAttribute2.Name]);
                            XmlNode documentElement = xmlDocument2.DocumentElement;
                            XmlAttribute xmlAttribute3 = documentElement.Attributes["Type"];
                            file.ListItemAllFields[xmlAttribute2.Name] = this.CastStringToFieldType(xmlAttribute2.Value,
                                xmlAttribute3.Value, xmlDocument2.OuterXml, callingAdapter);
                        }
                    }

                    file.ListItemAllFields.Update();
                    clientContext.Load<Microsoft.SharePoint.Client.File>(file,
                        new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                        {
                            (Microsoft.SharePoint.Client.File temp) => (object)temp.ListItemAllFields.Id,
                            (Microsoft.SharePoint.Client.File temp) => temp.Name,
                            (Microsoft.SharePoint.Client.File temp) => (object)temp.TimeLastModified
                        });
                    if (flag)
                    {
                        clientContext.Load<Microsoft.SharePoint.Client.File>(file,
                            new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                            {
                                (Microsoft.SharePoint.Client.File f) => f.ListItemAllFields["_ModerationStatus"]
                            });
                        file.ListItemAllFields["_ModerationStatus"] = 0;
                        file.ListItemAllFields.Update();
                    }

                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    StringWriter stringWriter = new StringWriter();
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlTextWriter.WriteStartElement("ListItems");
                        xmlTextWriter.WriteStartElement("ListItem");
                        xmlTextWriter.WriteAttributeString("ID",
                            file.ListItemAllFields.Id.ToString(CultureInfo.InvariantCulture));
                        xmlTextWriter.WriteAttributeString("FileDirRef", text.Substring(0, text.LastIndexOf('/')));
                        xmlTextWriter.WriteAttributeString("FileLeafRef", file.Name);
                        xmlTextWriter.WriteAttributeString("Modified", Utils.FormatDate(file.TimeLastModified));
                        xmlTextWriter.WriteEndElement();
                        xmlTextWriter.WriteEndElement();
                    }

                    result = stringWriter.ToString();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private static bool IsTargetModerationStatusRequired(string fieldLookupValue)
        {
            bool result = false;
            if (fieldLookupValue != null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(fieldLookupValue);
                if (xmlDocument.DocumentElement != null)
                {
                    string text = (xmlDocument.DocumentElement.Attributes["Required"] != null)
                        ? xmlDocument.DocumentElement.Attributes["Required"].Value
                        : "";
                    bool.TryParse(text.ToLower(), out result);
                }
            }

            return result;
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref Dictionary<string, string> fieldsLookupCache, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    ListItem listItem = null;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(folderXml);
                    XmlAttribute xmlAttribute = xmlDocument.DocumentElement.Attributes["FileLeafRef"];
                    if (xmlAttribute == null)
                    {
                        throw new ArgumentException("FileLeafRef cannot be missing");
                    }

                    string value = xmlAttribute.Value;
                    List byId = clientContext.Web.Lists.GetById(listId);
                    Uri uri = new Uri(string.Concat(new string[]
                    {
                        callingAdapter.Url.Trim(new char[]
                        {
                            '/',
                            '\\',
                            ' '
                        }),
                        "/",
                        listName,
                        "/",
                        folderPath.Trim(new char[]
                        {
                            '/',
                            '\\',
                            ' '
                        })
                    }));
                    string text = uri.LocalPath.TrimEnd(new char[]
                    {
                        '/'
                    });
                    CamlQuery query_ = new CamlQuery
                    {
                        ViewXml = new StringBuilder().Append("<View>").Append("<Query>").Append("<Where><And>")
                            .Append("<Eq><FieldRef Name=\"FSObjType\"/><Value Type=\"Integer\">1</Value></Eq>")
                            .AppendFormat("<Eq><FieldRef Name=\"FileLeafRef\"/><Value Type=\"Text\">{0}</Value></Eq>",
                                value).Append("</And></Where>").Append("</Query>").Append("</View>").ToString(),
                        FolderServerRelativeUrl = text
                    };
                    ListItemCollection items = byId.GetItems(query_);
                    clientContext.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[]
                    {
                        (ListItemCollection foldersDel) => foldersDel.Include(new Expression<Func<ListItem, object>>[]
                        {
                            (ListItem folderDel) => (object)folderDel.Id
                        })
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    listItem = ((items.Count > 0) ? items[0] : null);
                    if (options.Overwrite && listItem != null)
                    {
                        listItem.DeleteObject();
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        listItem = null;
                    }

                    if (listItem == null)
                    {
                        ListItemCreationInformation parameters = new ListItemCreationInformation
                        {
                            UnderlyingObjectType = FileSystemObjectType.Folder,
                            LeafName = value,
                            FolderUrl = text
                        };
                        listItem = byId.AddItem(parameters);
                    }

                    if (fieldsLookupCache == null)
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        clientContext.Load<FieldCollection>(byId.Fields, new Expression<Func<FieldCollection, object>>[]
                        {
                            (FieldCollection fields) => fields.Include(new Expression<Func<Field, object>>[]
                            {
                                (Field field) => field.InternalName,
                                (Field field) => field.SchemaXml
                            })
                        });
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        foreach (Field current in ((IEnumerable<Field>)byId.Fields))
                        {
                            if (!dictionary.ContainsKey(current.InternalName))
                            {
                                dictionary.Add(current.InternalName, current.SchemaXml);
                            }
                        }

                        fieldsLookupCache = dictionary;
                    }

                    foreach (XmlAttribute xmlAttribute2 in xmlDocument.DocumentElement.Attributes)
                    {
                        if (!string.Equals("Id", xmlAttribute2.Name, StringComparison.InvariantCultureIgnoreCase) &&
                            !string.Equals("ContentType", xmlAttribute2.Name,
                                StringComparison.InvariantCultureIgnoreCase) &&
                            fieldsLookupCache.ContainsKey(xmlAttribute2.Name))
                        {
                            XmlDocument xmlDocument2 = new XmlDocument();
                            xmlDocument2.LoadXml(fieldsLookupCache[xmlAttribute2.Name]);
                            XmlNode documentElement = xmlDocument2.DocumentElement;
                            XmlAttribute xmlAttribute3 = documentElement.Attributes["Type"];
                            listItem[xmlAttribute2.Name] = this.CastStringToFieldType(xmlAttribute2.Value,
                                xmlAttribute3.Value, xmlDocument2.OuterXml, callingAdapter);
                        }
                    }

                    listItem.Update();
                    clientContext.Load<ListItem>(listItem, new Expression<Func<ListItem, object>>[]
                    {
                        (ListItem temp) => (object)temp.Id,
                        (ListItem temp) => temp["FileLeafRef"],
                        (ListItem temp) => temp["FileDirRef"],
                        (ListItem temp) => temp["ContentTypeId"],
                        (ListItem temp) => temp["Author"],
                        (ListItem temp) => temp["Created"],
                        (ListItem temp) => temp["Editor"],
                        (ListItem temp) => temp["Modified"],
                        (ListItem temp) => (object)temp.HasUniqueRoleAssignments
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    StringWriter stringWriter = new StringWriter();
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlTextWriter.WriteStartElement("Folder");
                        xmlTextWriter.WriteAttributeString("ID", listItem.Id.ToString(CultureInfo.InvariantCulture));
                        xmlTextWriter.WriteAttributeString("FileLeafRef", listItem["FileLeafRef"].ToString());
                        xmlTextWriter.WriteAttributeString("FileDirRef", listItem["FileDirRef"].ToString());
                        xmlTextWriter.WriteAttributeString("ContentTypeId", listItem["ContentTypeId"].ToString());
                        xmlTextWriter.WriteAttributeString("Editor", listItem["Editor"].ToString());
                        xmlTextWriter.WriteAttributeString("Author", listItem["Author"].ToString());
                        xmlTextWriter.WriteAttributeString("Created",
                            Utils.FormatDate(callingAdapter.TimeZone.LocalTimeToUtc((DateTime)listItem["Created"])));
                        xmlTextWriter.WriteAttributeString("Modified",
                            Utils.FormatDate(callingAdapter.TimeZone.LocalTimeToUtc((DateTime)listItem["Modified"])));
                        xmlTextWriter.WriteAttributeString("HasUniquePermissions",
                            listItem.HasUniqueRoleAssignments.ToString());
                        xmlTextWriter.WriteEndElement();
                    }

                    result = stringWriter.ToString();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        public string GetFileProperties(string sServerRelativeUrl, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("File");
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web = clientContext.Web;
                    clientContext.Load<Web>(web, new Expression<Func<Web, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                        web.GetFileByServerRelativeUrl(sServerRelativeUrl);
                    clientContext.Load<Microsoft.SharePoint.Client.File>(fileByServerRelativeUrl,
                        new Expression<Func<Microsoft.SharePoint.Client.File, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    FileVersionCollection versions = fileByServerRelativeUrl.Versions;
                    clientContext.Load<FileVersionCollection>(versions,
                        new Expression<Func<FileVersionCollection, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    clientContext.Load<Microsoft.SharePoint.Client.File>(fileByServerRelativeUrl,
                        new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                        {
                            (Microsoft.SharePoint.Client.File file) => file.ListItemAllFields["Author"]
                        });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    FieldUserValue fieldUserValue = (FieldUserValue)fileByServerRelativeUrl.ListItemAllFields["Author"];
                    string nameFromID = callingAdapter.GetNameFromID(fieldUserValue.LookupId.ToString());
                    xmlTextWriter.WriteStartElement("Version");
                    xmlTextWriter.WriteAttributeString("VersionNumber", "@" + fileByServerRelativeUrl.UIVersion);
                    xmlTextWriter.WriteAttributeString("VersionString", fileByServerRelativeUrl.UIVersionLabel);
                    xmlTextWriter.WriteAttributeString("Url", fileByServerRelativeUrl.ServerRelativeUrl);
                    xmlTextWriter.WriteAttributeString("Created", fileByServerRelativeUrl.TimeCreated.ToString());
                    xmlTextWriter.WriteAttributeString("CreatedBy", nameFromID);
                    xmlTextWriter.WriteAttributeString("Comments", fileByServerRelativeUrl.CheckInComment);
                    xmlTextWriter.WriteEndElement();
                    if (versions.Count > 0)
                    {
                        foreach (FileVersion current in ((IEnumerable<FileVersion>)versions))
                        {
                            clientContext.Load<FileVersion>(current, new Expression<Func<FileVersion, object>>[0]);
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                            string value = "";
                            if (current.IsPropertyAvailable("CreatedBy"))
                            {
                                clientContext.Load<User>(current.CreatedBy, new Expression<Func<User, object>>[0]);
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                User createdBy = current.CreatedBy;
                                value = createdBy.LoginName;
                            }

                            xmlTextWriter.WriteStartElement("Version");
                            xmlTextWriter.WriteAttributeString("VersionNumber", current.ID.ToString());
                            xmlTextWriter.WriteAttributeString("VersionString", current.VersionLabel);
                            xmlTextWriter.WriteAttributeString("Url", current.Url);
                            xmlTextWriter.WriteAttributeString("Created", current.Created.ToString());
                            xmlTextWriter.WriteAttributeString("CreatedBy", value);
                            xmlTextWriter.WriteAttributeString("Comments", current.CheckInComment);
                            xmlTextWriter.WriteEndElement();
                        }
                    }
                }

                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                result = stringWriter.ToString();
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private void GetVersionInfo(XmlNode listItemXml, out string sLevel, out string sVersionString,
            out CheckinType checkInType, out string sCheckInComments)
        {
            sLevel = ((listItemXml.Attributes["_VersionLevel"] == null)
                ? "1"
                : listItemXml.Attributes["_VersionLevel"].Value);
            checkInType = ((sLevel == "2") ? CheckinType.MinorCheckIn : CheckinType.MajorCheckIn);
            sCheckInComments = ((listItemXml.Attributes["_CheckinComment"] != null)
                ? listItemXml.Attributes["_CheckinComment"].Value
                : "");
            sVersionString = ((listItemXml.Attributes["_VersionString"] == null)
                ? "1.0"
                : listItemXml.Attributes["_VersionString"].Value);
        }

        private void GetListItemFileInfo(XmlNode listItemXml, out int iItemId, out string creationUser,
            out string modificationUser, out DateTime createdOn, out DateTime modifiedOn, bool bCorrectInvalidNames,
            Web currentWeb, NWSAdapter callingAdapter)
        {
            iItemId = -1;
            if (listItemXml.Attributes["ID"] != null)
            {
                Convert.ToInt32(listItemXml.Attributes["ID"].Value);
            }

            creationUser = ((listItemXml.Attributes["Author"] == null) ? null : listItemXml.Attributes["Author"].Value);
            modificationUser = ((listItemXml.Attributes["Editor"] == null)
                ? null
                : listItemXml.Attributes["Editor"].Value);
            createdOn = ((listItemXml.Attributes["Created"] == null)
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(listItemXml.Attributes["Created"].Value));
            modifiedOn = ((listItemXml.Attributes["Modified"] == null)
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(listItemXml.Attributes["Modified"].Value));
        }

        private void GetItemXML(Web currentWeb, XmlWriter xmlWriter, List<string> fieldNamesRequested, ListItem item,
            bool bDatesInUtc, Hashtable htExternalization, GetListItemOptions getOptions, NWSAdapter callingAdapter)
        {
            xmlWriter.WriteStartElement("ListItem");
            if (getOptions != null && getOptions.IncludePermissionsInheritance)
            {
                xmlWriter.WriteAttributeString("HasUniquePermissions", item.HasUniqueRoleAssignments.ToString());
            }

            FieldCollection fields = item.ParentList.Fields;
            currentWeb.Context.Load<FieldCollection>(fields, new Expression<Func<FieldCollection, object>>[0]);
            currentWeb.Context.Load<ListItem>(item, new Expression<Func<ListItem, object>>[0]);
            ClientOMTools.CallExecuteQuery(currentWeb.Context, callingAdapter);
            foreach (string current in fieldNamesRequested)
            {
                try
                {
                    object obj = item[current];
                    string localName = XmlUtility.EncodeNameStartChars(current);
                    if (obj != null)
                    {
                        xmlWriter.WriteAttributeString(localName,
                            this.GetFieldValue(obj, fields.GetByInternalNameOrTitle(current), currentWeb, bDatesInUtc,
                                callingAdapter));
                    }
                    else
                    {
                        xmlWriter.WriteAttributeString(localName, "");
                    }
                }
                catch (Exception)
                {
                }
            }

            xmlWriter.WriteEndElement();
        }

        private string GetFieldValue(object objectValue, Field field, Web currentWeb, bool bDatesInUtc,
            NWSAdapter callingAdapter)
        {
            currentWeb.Context.Load<Field>(field, new Expression<Func<Field, object>>[0]);
            ClientOMTools.CallExecuteQuery(currentWeb.Context, callingAdapter);
            string text = null;
            switch (field.FieldTypeKind)
            {
                case FieldType.Text:
                case FieldType.Note:
                case FieldType.URL:
                case FieldType.Computed:
                case FieldType.GridChoice:
                    text = objectValue.ToString();
                    return text;
                case FieldType.DateTime:
                {
                    DateTime dateTime = (DateTime)objectValue;
                    if (bDatesInUtc)
                    {
                        dateTime = Utils.MakeTrueUTCDateTime(dateTime);
                    }
                    else
                    {
                        dateTime = callingAdapter.TimeZone.LocalTimeToUtc(dateTime);
                    }

                    text = Utils.FormatDate(dateTime);
                    return text;
                }
                case FieldType.Lookup:
                    objectValue.GetType();
                    if (objectValue.GetType().IsAssignableFrom(typeof(List<FieldLookupValue>)))
                    {
                        List<FieldLookupValue> list = (List<FieldLookupValue>)objectValue;
                        string text2 = null;
                        for (int i = 0; i < list.Count; i++)
                        {
                            FieldLookupValue fieldLookupValue = list[i];
                            string text3 = fieldLookupValue.LookupId.ToString();
                            if (string.IsNullOrEmpty(text2))
                            {
                                text2 = text3;
                            }
                            else
                            {
                                text2 = text2 + ";#" + text3;
                            }
                        }

                        text = text2;
                        return text;
                    }

                    if (objectValue.GetType().IsAssignableFrom(typeof(FieldLookupValue)))
                    {
                        FieldLookupValue fieldLookupValue2 = (FieldLookupValue)objectValue;
                        XmlNode xmlNode = XmlUtility.StringToXmlNode(field.SchemaXml);
                        string text4 = (xmlNode.Attributes["List"] != null) ? xmlNode.Attributes["List"].Value : "";
                        if (!field.CanBeDeleted && (string.IsNullOrEmpty(text4) || !Utils.IsGUID(text4)))
                        {
                            text = fieldLookupValue2.LookupValue;
                            return text;
                        }

                        bool flag = true;
                        try
                        {
                            new Guid(text4);
                        }
                        catch
                        {
                            flag = false;
                        }

                        if (flag)
                        {
                            text = fieldLookupValue2.LookupId.ToString();
                            return text;
                        }

                        text = fieldLookupValue2.LookupValue;
                        return text;
                    }
                    else
                    {
                        text = objectValue.ToString();
                        string[] array = text.Split(new string[]
                        {
                            ";#"
                        }, StringSplitOptions.None);
                        if (array.Length > 1)
                        {
                            XmlNode xmlNode2 = XmlUtility.StringToXmlNode(field.SchemaXml);
                            string text5 = (xmlNode2.Attributes["List"] != null)
                                ? xmlNode2.Attributes["List"].Value
                                : "";
                            if (!field.CanBeDeleted && (string.IsNullOrEmpty(text5) || !Utils.IsGUID(text5)))
                            {
                                text = array[1];
                            }
                            else
                            {
                                bool flag2 = true;
                                try
                                {
                                    new Guid(text5);
                                }
                                catch
                                {
                                    flag2 = false;
                                }

                                if (flag2)
                                {
                                    text = array[0];
                                }
                                else
                                {
                                    text = array[1];
                                }
                            }
                        }

                        if ((field.InternalName == "FileRef" || field.InternalName == "FileDirRef") && text != null &&
                            text.StartsWith("/"))
                        {
                            text = text.TrimStart(new char[]
                            {
                                '/'
                            });
                            return text;
                        }

                        return text;
                    }

                    break;
                case FieldType.Number:
                    if (objectValue.GetType() == typeof(int))
                    {
                        text = ClientOMTools.s_IntConverter.ConvertToString(null, ClientOMTools.s_CultureInfo,
                            objectValue);
                        return text;
                    }

                    if (objectValue.GetType() == typeof(double))
                    {
                        text = ClientOMTools.s_DoubleConverter.ConvertToString(null, ClientOMTools.s_CultureInfo,
                            objectValue);
                        return text;
                    }

                    return text;
                case FieldType.Calculated:
                {
                    FieldCalculated fieldCalculated = (FieldCalculated)field;
                    text = objectValue.ToString();
                    int num = text.IndexOf(";#");
                    if (num >= 0)
                    {
                        text = text.Substring(num + 2);
                    }

                    if (fieldCalculated.OutputType == FieldType.DateTime)
                    {
                        DateTime dateTime2;
                        if (DateTime.TryParse(text, out dateTime2))
                        {
                            if (bDatesInUtc)
                            {
                                dateTime2 = Utils.MakeTrueUTCDateTime(dateTime2);
                            }
                            else
                            {
                                dateTime2 = callingAdapter.TimeZone.LocalTimeToUtc(dateTime2);
                            }

                            text = Utils.FormatDate(dateTime2);
                            return text;
                        }

                        text = "";
                        return text;
                    }
                    else
                    {
                        if (fieldCalculated.OutputType != FieldType.Number)
                        {
                            return text;
                        }

                        int num2;
                        if (int.TryParse(text, out num2))
                        {
                            text = ClientOMTools.s_IntConverter.ConvertToString(null, ClientOMTools.s_CultureInfo,
                                num2);
                            return text;
                        }

                        double num3;
                        if (double.TryParse(text, out num3))
                        {
                            text = ClientOMTools.s_DoubleConverter.ConvertToString(null, ClientOMTools.s_CultureInfo,
                                num3);
                            return text;
                        }

                        return text;
                    }

                    break;
                }
                case FieldType.User:
                {
                    objectValue.GetType();
                    if (objectValue.GetType().IsAssignableFrom(typeof(List<FieldUserValue>)))
                    {
                        text = "";
                        using (List<FieldUserValue>.Enumerator enumerator =
                               ((List<FieldUserValue>)objectValue).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                FieldUserValue current = enumerator.Current;
                                ListItem listItem = null;
                                try
                                {
                                    listItem = currentWeb.SiteUserInfoList.GetItemById(current.LookupId);
                                    currentWeb.Context.Load<ListItem>(listItem,
                                        new Expression<Func<ListItem, object>>[0]);
                                    ClientOMTools.CallExecuteQuery(currentWeb.Context, callingAdapter);
                                }
                                catch
                                {
                                }

                                string text6;
                                if (listItem != null)
                                {
                                    text6 = listItem.FieldValues["Name"].ToString();
                                }
                                else
                                {
                                    text6 = current.LookupId.ToString();
                                }

                                if (string.IsNullOrEmpty(text))
                                {
                                    text = text6;
                                }
                                else
                                {
                                    text = text + "," + text6;
                                }
                            }

                            return text;
                        }
                    }

                    FieldUserValue fieldUserValue = (FieldUserValue)objectValue;
                    if (fieldUserValue.LookupValue == "***")
                    {
                        text = "";
                        return text;
                    }

                    ListItem listItem2 = null;
                    try
                    {
                        listItem2 = currentWeb.SiteUserInfoList.GetItemById(fieldUserValue.LookupId);
                        currentWeb.Context.Load<ListItem>(listItem2, new Expression<Func<ListItem, object>>[0]);
                        ClientOMTools.CallExecuteQuery(currentWeb.Context, callingAdapter);
                    }
                    catch
                    {
                    }

                    if (listItem2 != null)
                    {
                        text = listItem2.FieldValues["Name"].ToString();
                        return text;
                    }

                    text = fieldUserValue.LookupId.ToString();
                    return text;
                }
            }

            if (field.FieldTypeKind == FieldType.Invalid && objectValue != null &&
                typeof(DateTime).IsAssignableFrom(objectValue.GetType()))
            {
                DateTime dateTime3 = (DateTime)objectValue;
                if (bDatesInUtc)
                {
                    dateTime3 = Utils.MakeTrueUTCDateTime(dateTime3);
                }
                else
                {
                    dateTime3 = callingAdapter.TimeZone.LocalTimeToUtc(dateTime3);
                }

                text = Utils.FormatDate(dateTime3);
            }
            else
            {
                text = objectValue.ToString();
                if (string.Equals("HTML", field.TypeAsString, StringComparison.InvariantCultureIgnoreCase))
                {
                    return text;
                }

                int num4 = text.IndexOf(";#");
                if (num4 > 0)
                {
                    text = text.Substring(num4 + 2);
                }
            }

            return text;
        }

        public void SetContentTypesEnabled(bool bEnable, string sListId, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    List byId = clientContext.Web.Lists.GetById(new Guid(sListId));
                    byId.ContentTypesEnabled = bEnable;
                    byId.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }

        public XmlNode AddView(string sListID, string sWantedViewUrlName, XmlNode nodeViewFields, XmlNode nodeQuery,
            XmlNode nodeRowLimit, XmlNode viewData, string sType, bool bDefaultView, string sContentTypeId,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            XmlNode result;
            try
            {
                XmlNodeList xmlNodeList = nodeViewFields.SelectNodes("./FieldRef/@Name");
                List<string> list = new List<string>();
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    list.Add(xmlNode.Value);
                }

                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    List byId = clientContext.Web.Lists.GetById(new Guid(sListID));
                    ViewCreationInformation viewCreationInformation = new ViewCreationInformation();
                    viewCreationInformation.Title = sWantedViewUrlName;
                    viewCreationInformation.ViewTypeKind = (ViewType)Enum.Parse(typeof(ViewType), sType, true);
                    viewCreationInformation.RowLimit = uint.Parse(nodeRowLimit.InnerText);
                    viewCreationInformation.SetAsDefaultView = bDefaultView;
                    viewCreationInformation.Query = nodeQuery.InnerXml;
                    viewCreationInformation.ViewFields = list.ToArray();
                    byId.Views.Add(viewCreationInformation);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    View byTitle = byId.Views.GetByTitle(sWantedViewUrlName);
                    if (viewData != null)
                    {
                        byTitle.ViewData = viewData.InnerXml;
                    }

                    ContentTypeId contentTypeId =
                        this.GetContentTypeId(sContentTypeId, byId, clientContext, callingAdapter);
                    if (contentTypeId != null)
                    {
                        byTitle.ContentTypeId = contentTypeId;
                    }

                    byTitle.Update();
                    clientContext.Load<View>(byTitle, new Expression<Func<View, object>>[]
                    {
                        (View o) => o.HtmlSchemaXml
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    XmlTextReader reader = new XmlTextReader(new StringReader(byTitle.HtmlSchemaXml));
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlNode xmlNode2 = xmlDocument.ReadNode(reader);
                    result = xmlNode2;
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        public XmlNode UpdateViewSettings(string sListID, string sViewName, string sContentTypeId,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter.CookieManager != null && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            XmlNode result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    List byId = clientContext.Web.Lists.GetById(new Guid(sListID));
                    View byId2 = byId.Views.GetById(new Guid(sViewName));
                    ContentTypeId contentTypeId =
                        this.GetContentTypeId(sContentTypeId, byId, clientContext, callingAdapter);
                    if (contentTypeId != null)
                    {
                        byId2.ContentTypeId = contentTypeId;
                    }

                    byId2.Update();
                    clientContext.Load<View>(byId2, new Expression<Func<View, object>>[]
                    {
                        (View o) => o.HtmlSchemaXml
                    });
                    clientContext.ExecuteQuery();
                    XmlTextReader reader = new XmlTextReader(new StringReader(byId2.HtmlSchemaXml));
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlNode xmlNode = xmlDocument.ReadNode(reader);
                    result = xmlNode;
                }
            }
            finally
            {
                if (callingAdapter.CookieManager != null && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private ContentTypeId GetContentTypeId(string sContentTypeId, List list, ClientContext clientContext,
            NWSAdapter callingAdapter)
        {
            if (string.IsNullOrEmpty(sContentTypeId))
            {
                return null;
            }

            if (sContentTypeId.Equals("0x012001"))
            {
                string rootCollection = this.GetRootCollection(callingAdapter);
                using (ClientContext clientContext2 = this.GetClientContext(rootCollection, callingAdapter))
                {
                    ContentType webContentType = this.GetWebContentType(clientContext2.Web, sContentTypeId);
                    if (webContentType != null)
                    {
                        ContentTypeId id = webContentType.Id;
                        return id;
                    }
                }
            }

            clientContext.Load<ContentTypeCollection>(list.ContentTypes,
                new Expression<Func<ContentTypeCollection, object>>[0]);
            clientContext.ExecuteQuery();
            foreach (ContentType current in ((IEnumerable<ContentType>)list.ContentTypes))
            {
                if (current.Id.ToString().Equals(sContentTypeId, StringComparison.OrdinalIgnoreCase))
                {
                    ContentTypeId id = current.Id;
                    return id;
                }
            }

            return null;
        }

        public string GetWebNavigationStructure(NWSAdapter callingAdapter, string[] hiddenGlobalUrls,
            string[] hiddenCurrentUrls)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web = clientContext.Web;
                    Navigation navigation = web.Navigation;
                    clientContext.Load<Navigation>(navigation, new Expression<Func<Navigation, object>>[0]);
                    clientContext.Load<NavigationNodeCollection>(navigation.QuickLaunch,
                        new Expression<Func<NavigationNodeCollection, object>>[0]);
                    clientContext.Load<NavigationNodeCollection>(navigation.TopNavigationBar,
                        new Expression<Func<NavigationNodeCollection, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    xmlWriter.WriteStartElement("NavNode");
                    xmlWriter.WriteAttributeString("ID", "0");
                    xmlWriter.WriteAttributeString("Title", "");
                    xmlWriter.WriteAttributeString("Url", "");
                    xmlWriter.WriteAttributeString("IsVisible", "True");
                    xmlWriter.WriteAttributeString("IsExternal", "True");
                    xmlWriter.WriteAttributeString("LastModified", Utils.FormatDate(DateTime.UtcNow));
                    xmlWriter.WriteStartElement("NavNode");
                    xmlWriter.WriteAttributeString("ID", "1025");
                    xmlWriter.WriteAttributeString("Title", "Quick Launch");
                    xmlWriter.WriteAttributeString("Url", "");
                    xmlWriter.WriteAttributeString("IsVisible", "True");
                    xmlWriter.WriteAttributeString("IsExternal", "True");
                    xmlWriter.WriteAttributeString("LastModified", Utils.FormatDate(DateTime.UtcNow));
                    foreach (NavigationNode current in ((IEnumerable<NavigationNode>)navigation.QuickLaunch))
                    {
                        this.WriteNavNodeXml(xmlWriter, current, "1025", callingAdapter, hiddenCurrentUrls,
                            callingAdapter);
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("NavNode");
                    xmlWriter.WriteAttributeString("ID", "1002");
                    xmlWriter.WriteAttributeString("Title", "SharePoint Top Navbar");
                    xmlWriter.WriteAttributeString("Url", "");
                    xmlWriter.WriteAttributeString("IsVisible", "True");
                    xmlWriter.WriteAttributeString("IsExternal", "True");
                    xmlWriter.WriteAttributeString("LastModified", Utils.FormatDate(DateTime.UtcNow));
                    foreach (NavigationNode current2 in ((IEnumerable<NavigationNode>)navigation.TopNavigationBar))
                    {
                        this.WriteNavNodeXml(xmlWriter, current2, "1002", callingAdapter, hiddenGlobalUrls,
                            callingAdapter);
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                    result = stringBuilder.ToString();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private void WriteNavNodeXml(XmlWriter writer, NavigationNode node, string sParentID, NWSAdapter adapter,
            string[] hiddenUrls, SharePointAdapter callingAdapter)
        {
            string text = node.Id.ToString();
            string title = node.Title;
            string url = node.Url;
            string value = Utils.FormatDate(DateTime.UtcNow);
            bool flag = url.Contains("://") ||
                        (url.StartsWith("/") && !adapter.GetServerRelativeUrlWithinSiteCollection(url));
            bool navNodeIsHidden = this.GetNavNodeIsHidden(node, hiddenUrls, adapter);
            writer.WriteStartElement("NavNode");
            writer.WriteAttributeString("ID", text);
            writer.WriteAttributeString("Title", title);
            writer.WriteAttributeString("Url", url);
            writer.WriteAttributeString("IsVisible", navNodeIsHidden.ToString());
            writer.WriteAttributeString("IsExternal", flag.ToString());
            writer.WriteAttributeString("LastModified", value);
            node.Context.Load<NavigationNodeCollection>(node.Children,
                new Expression<Func<NavigationNodeCollection, object>>[0]);
            ClientOMTools.CallExecuteQuery(node.Context, callingAdapter);
            foreach (NavigationNode current in ((IEnumerable<NavigationNode>)node.Children))
            {
                this.WriteNavNodeXml(writer, current, text, adapter, hiddenUrls, callingAdapter);
            }

            writer.WriteEndElement();
        }

        private bool GetNavNodeIsHidden(NavigationNode node, string[] hiddenUrls, SharePointAdapter adapter)
        {
            if (hiddenUrls == null || node.Url.Contains("://"))
            {
                return true;
            }

            StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(adapter, node.Url);
            string webRelative = standardizedUrl.WebRelative;
            for (int i = 0; i < hiddenUrls.Length; i++)
            {
                string a = hiddenUrls[i];
                if (a == webRelative)
                {
                    return false;
                }
            }

            return true;
        }

        private Guid GetNavNodeHiddenUrlID(string sUrl, Dictionary<Guid, string> pagesLibIDMap,
            Dictionary<Guid, string> subWebIDMap, SharePointAdapter adapter)
        {
            if (string.IsNullOrEmpty(sUrl))
            {
                return Guid.Empty;
            }

            StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(adapter, sUrl);
            string webRelative = standardizedUrl.WebRelative;
            foreach (KeyValuePair<Guid, string> current in subWebIDMap)
            {
                if (current.Value == webRelative)
                {
                    Guid key = current.Key;
                    return key;
                }
            }

            foreach (KeyValuePair<Guid, string> current2 in pagesLibIDMap)
            {
                if (current2.Value == webRelative)
                {
                    Guid key = current2.Key;
                    return key;
                }
            }

            return Guid.Empty;
        }

        private NavigationNode GetNavNodeByID(int iID, Navigation navigation, SharePointAdapter callingAdapter,
            out bool bOnQuickLaunch, out bool bOnTopNav)
        {
            bOnQuickLaunch = false;
            bOnTopNav = false;
            if (iID < 1000 || iID == 1002 || iID == 1025)
            {
                return null;
            }

            foreach (NavigationNode current in ((IEnumerable<NavigationNode>)navigation.TopNavigationBar))
            {
                NavigationNode navNodeByID = this.GetNavNodeByID(iID, current, callingAdapter);
                if (navNodeByID != null)
                {
                    bOnTopNav = true;
                    NavigationNode result = navNodeByID;
                    return result;
                }
            }

            foreach (NavigationNode current2 in ((IEnumerable<NavigationNode>)navigation.QuickLaunch))
            {
                NavigationNode navNodeByID = this.GetNavNodeByID(iID, current2, callingAdapter);
                if (navNodeByID != null)
                {
                    bOnQuickLaunch = true;
                    NavigationNode result = navNodeByID;
                    return result;
                }
            }

            return null;
        }

        private NavigationNode GetNavNodeByID(int iID, NavigationNode nodeToSearchUnder,
            SharePointAdapter callingAdapter)
        {
            if (!nodeToSearchUnder.IsPropertyAvailable("Id"))
            {
                return null;
            }

            if (nodeToSearchUnder.Id == iID)
            {
                return nodeToSearchUnder;
            }

            if (!nodeToSearchUnder.IsPropertyAvailable("Children"))
            {
                nodeToSearchUnder.Context.Load<NavigationNodeCollection>(nodeToSearchUnder.Children,
                    new Expression<Func<NavigationNodeCollection, object>>[0]);
                ClientOMTools.CallExecuteQuery(nodeToSearchUnder.Context, callingAdapter);
            }

            NavigationNode navigationNode = null;
            foreach (NavigationNode current in ((IEnumerable<NavigationNode>)nodeToSearchUnder.Children))
            {
                navigationNode = this.GetNavNodeByID(iID, current, callingAdapter);
                if (navigationNode != null)
                {
                    break;
                }
            }

            return navigationNode;
        }

        private NavigationNodeCollection GetNavNodeCollectionUnder(int iID, Navigation navigation,
            SharePointAdapter callingAdapter, out bool bOnQuickLaunch, out bool bOnTopNav)
        {
            bOnQuickLaunch = false;
            bOnTopNav = false;
            if (iID < 1000)
            {
                return null;
            }

            if (iID == 1002)
            {
                bOnTopNav = true;
                return navigation.TopNavigationBar;
            }

            if (iID == 1025)
            {
                bOnQuickLaunch = true;
                return navigation.QuickLaunch;
            }

            NavigationNode navNodeByID =
                this.GetNavNodeByID(iID, navigation, callingAdapter, out bOnQuickLaunch, out bOnTopNav);
            if (navNodeByID != null)
            {
                return navNodeByID.Children;
            }

            return null;
        }

        public void UpdateWebNavigationStructure(XmlNode additionsAndUpdates, XmlNode deletions,
            NWSAdapter callingAdapter, bool bWatchForIsVisibleChanges, string[] hiddenGlobalUrls,
            string[] hiddenCurrentUrls, Dictionary<Guid, string> pagesLibIDMap, Dictionary<Guid, string> subWebIDMap,
            ref List<Guid> hiddenGlobalNodes, ref List<Guid> hiddenCurrentNodes, out bool bHiddenChangesMade)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                bHiddenChangesMade = false;
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web = clientContext.Web;
                    Navigation navigation = web.Navigation;
                    clientContext.Load<NavigationNodeCollection>(navigation.QuickLaunch,
                        new Expression<Func<NavigationNodeCollection, object>>[0]);
                    clientContext.Load<NavigationNodeCollection>(navigation.TopNavigationBar,
                        new Expression<Func<NavigationNodeCollection, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    foreach (XmlNode xmlNode in deletions.ChildNodes)
                    {
                        int iID = int.Parse(xmlNode.Attributes["ID"].Value);
                        bool flag = false;
                        bool flag2 = false;
                        NavigationNode navNodeByID =
                            this.GetNavNodeByID(iID, navigation, callingAdapter, out flag, out flag2);
                        if (navNodeByID != null)
                        {
                            navNodeByID.DeleteObject();
                        }
                    }

                    foreach (XmlNode xmlNode2 in additionsAndUpdates.ChildNodes)
                    {
                        bool bOnQuickLaunch = false;
                        bool bOnTopNav = false;
                        int iID2 = int.Parse(xmlNode2.Attributes["ID"].Value);
                        NavigationNode navNodeByID2 = this.GetNavNodeByID(iID2, navigation, callingAdapter,
                            out bOnQuickLaunch, out bOnTopNav);
                        if (navNodeByID2 != null)
                        {
                            this.UpdateNavigationNode(xmlNode2, navNodeByID2, callingAdapter, bWatchForIsVisibleChanges,
                                hiddenGlobalUrls, hiddenCurrentUrls, pagesLibIDMap, subWebIDMap, bOnQuickLaunch,
                                bOnTopNav, ref hiddenGlobalNodes, ref hiddenCurrentNodes, ref bHiddenChangesMade);
                        }
                        else
                        {
                            int iID3 = int.Parse(xmlNode2.Attributes["ParentID"].Value);
                            NavigationNodeCollection navNodeCollectionUnder = this.GetNavNodeCollectionUnder(iID3,
                                navigation, callingAdapter, out bOnQuickLaunch, out bOnTopNav);
                            if (navNodeCollectionUnder != null)
                            {
                                this.AddNavigationNode(xmlNode2, navNodeCollectionUnder, callingAdapter,
                                    bWatchForIsVisibleChanges, pagesLibIDMap, subWebIDMap, bOnQuickLaunch, bOnTopNav,
                                    ref hiddenGlobalNodes, ref hiddenCurrentNodes, ref bHiddenChangesMade);
                            }
                        }
                    }

                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }

        private void AddNavigationNode(XmlNode node, NavigationNodeCollection parentCollection,
            SharePointAdapter adapter, bool bWatchForIsVisibleChanges, Dictionary<Guid, string> pagesLibIDMap,
            Dictionary<Guid, string> subWebIDMap, bool bOnQuickLaunch, bool bOnTopNav, ref List<Guid> hiddenGlobalNodes,
            ref List<Guid> hiddenCurrentNodes, ref bool bHiddenChangesMade)
        {
            if (bWatchForIsVisibleChanges && node.Attributes["IsVisible"].Value.ToLower() == "false")
            {
                Guid navNodeHiddenUrlID =
                    this.GetNavNodeHiddenUrlID(node.Attributes["Url"].Value, pagesLibIDMap, subWebIDMap, adapter);
                if (navNodeHiddenUrlID != Guid.Empty)
                {
                    if (bOnQuickLaunch)
                    {
                        if (!hiddenCurrentNodes.Contains(navNodeHiddenUrlID))
                        {
                            bHiddenChangesMade = true;
                            hiddenCurrentNodes.Add(navNodeHiddenUrlID);
                        }
                    }
                    else if (bOnTopNav && !hiddenGlobalNodes.Contains(navNodeHiddenUrlID))
                    {
                        bHiddenChangesMade = true;
                        hiddenGlobalNodes.Add(navNodeHiddenUrlID);
                    }
                }
            }

            NavigationNodeCreationInformation navigationNodeCreationInformation =
                new NavigationNodeCreationInformation();
            navigationNodeCreationInformation.Title = node.Attributes["Title"].Value;
            navigationNodeCreationInformation.Url = node.Attributes["Url"].Value;
            navigationNodeCreationInformation.IsExternal = ((node.Attributes["IsExternal"] == null)
                ? navigationNodeCreationInformation.Url.Contains("://")
                : bool.Parse(node.Attributes["IsExternal"].Value));
            navigationNodeCreationInformation.AsLastNode = true;
            NavigationNode navigationNode = parentCollection.Add(navigationNodeCreationInformation);
            foreach (XmlNode node2 in node.ChildNodes)
            {
                this.AddNavigationNode(node2, navigationNode.Children, adapter, bWatchForIsVisibleChanges,
                    pagesLibIDMap, subWebIDMap, bOnQuickLaunch, bOnTopNav, ref hiddenGlobalNodes,
                    ref hiddenCurrentNodes, ref bHiddenChangesMade);
            }
        }

        private void UpdateNavigationNode(XmlNode node, NavigationNode navNode, SharePointAdapter adapter,
            bool bWatchForIsVisibleChanges, string[] hiddenGlobalUrls, string[] hiddenCurrentUrls,
            Dictionary<Guid, string> pagesLibIDMap, Dictionary<Guid, string> subWebIDMap, bool bOnQuickLaunch,
            bool bOnTopNav, ref List<Guid> hiddenGlobalNodes, ref List<Guid> hiddenCurrentNodes,
            ref bool bHiddenChangesMade)
        {
            if (bWatchForIsVisibleChanges && node.Attributes["IsVisible"] != null)
            {
                bool flag = bool.Parse(node.Attributes["IsVisible"].Value);
                bool flag2 = true;
                if (bOnTopNav)
                {
                    flag2 = this.GetNavNodeIsHidden(navNode, hiddenGlobalUrls, adapter);
                }
                else if (bOnQuickLaunch)
                {
                    flag2 = this.GetNavNodeIsHidden(navNode, hiddenCurrentUrls, adapter);
                }

                if (flag2 != flag)
                {
                    string sUrl = (node.Attributes["Url"] == null) ? navNode.Url : node.Attributes["Url"].Value;
                    Guid navNodeHiddenUrlID = this.GetNavNodeHiddenUrlID(sUrl, pagesLibIDMap, subWebIDMap, adapter);
                    if (navNodeHiddenUrlID != Guid.Empty)
                    {
                        if (flag)
                        {
                            if (bOnQuickLaunch)
                            {
                                if (hiddenCurrentNodes.Remove(navNodeHiddenUrlID))
                                {
                                    bHiddenChangesMade = true;
                                }
                            }
                            else if (bOnTopNav && hiddenGlobalNodes.Remove(navNodeHiddenUrlID))
                            {
                                bHiddenChangesMade = true;
                            }
                        }
                        else if (bOnQuickLaunch)
                        {
                            if (!hiddenCurrentNodes.Contains(navNodeHiddenUrlID))
                            {
                                bHiddenChangesMade = true;
                                hiddenCurrentNodes.Add(navNodeHiddenUrlID);
                            }
                        }
                        else if (bOnTopNav && !hiddenGlobalNodes.Contains(navNodeHiddenUrlID))
                        {
                            bHiddenChangesMade = true;
                            hiddenGlobalNodes.Add(navNodeHiddenUrlID);
                        }
                    }
                }
            }

            if (node.Attributes["Title"] != null)
            {
                navNode.Title = node.Attributes["Title"].Value;
            }

            if (node.Attributes["Url"] != null)
            {
                navNode.Url = node.Attributes["Url"].Value;
            }

            if (node.ChildNodes.Count > 0 && !navNode.IsPropertyAvailable("Children"))
            {
                navNode.Context.Load<NavigationNodeCollection>(navNode.Children,
                    new Expression<Func<NavigationNodeCollection, object>>[0]);
                ClientOMTools.CallExecuteQuery(navNode.Context, adapter);
            }

            foreach (XmlNode xmlNode in node.ChildNodes)
            {
                int num = int.Parse(xmlNode.Attributes["ID"].Value);
                NavigationNode navigationNode = null;
                if (num >= 1000)
                {
                    foreach (NavigationNode current in ((IEnumerable<NavigationNode>)navNode.Children))
                    {
                        if (current.Id == num)
                        {
                            navigationNode = current;
                            break;
                        }
                    }
                }

                if (navigationNode != null)
                {
                    this.UpdateNavigationNode(xmlNode, navigationNode, adapter, bWatchForIsVisibleChanges,
                        hiddenGlobalUrls, hiddenCurrentUrls, pagesLibIDMap, subWebIDMap, bOnQuickLaunch, bOnTopNav,
                        ref hiddenGlobalNodes, ref hiddenCurrentNodes, ref bHiddenChangesMade);
                }
                else
                {
                    this.AddNavigationNode(xmlNode, navNode.Children, adapter, bWatchForIsVisibleChanges, pagesLibIDMap,
                        subWebIDMap, bOnQuickLaunch, bOnTopNav, ref hiddenGlobalNodes, ref hiddenCurrentNodes,
                        ref bHiddenChangesMade);
                }
            }
        }

        public void DeleteWebParts(string sPageUrl, List<Guid> webPartIDs, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    if (!sPageUrl.StartsWith("/"))
                    {
                        sPageUrl = "/" + sPageUrl;
                    }

                    Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                        clientContext.Web.GetFileByServerRelativeUrl(sPageUrl);
                    LimitedWebPartManager limitedWebPartManager =
                        fileByServerRelativeUrl.GetLimitedWebPartManager(PersonalizationScope.Shared);
                    foreach (Guid current in webPartIDs)
                    {
                        WebPartDefinition byId = limitedWebPartManager.WebParts.GetById(current);
                        byId.DeleteWebPart();
                    }

                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }

        public Guid AddWebPartToHiddenZone(string sPageUrl, string sWebPartXml, int iZoneIndex,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            Guid id;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    if (!sPageUrl.StartsWith("/"))
                    {
                        sPageUrl = "/" + sPageUrl;
                    }

                    Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                        clientContext.Web.GetFileByServerRelativeUrl(sPageUrl);
                    LimitedWebPartManager limitedWebPartManager =
                        fileByServerRelativeUrl.GetLimitedWebPartManager(PersonalizationScope.Shared);
                    clientContext.Load<LimitedWebPartManager>(limitedWebPartManager,
                        new Expression<Func<LimitedWebPartManager, object>>[0]);
                    WebPartDefinition webPartDefinition = limitedWebPartManager.ImportWebPart(sWebPartXml);
                    WebPartDefinition webPartDefinition2 =
                        limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, "wpz", iZoneIndex);
                    clientContext.Load<WebPartDefinition>(webPartDefinition2,
                        new Expression<Func<WebPartDefinition, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    id = webPartDefinition2.Id;
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return id;
        }

        public void EmbedWebParts(string sWebPageUrl, List<NWSAdapter.EmbeddedWebPart> webPartsToEmbed,
            string sEmbeddedContent, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    if (!sWebPageUrl.StartsWith("/"))
                    {
                        sWebPageUrl = "/" + sWebPageUrl;
                    }

                    Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                        clientContext.Web.GetFileByServerRelativeUrl(sWebPageUrl);
                    clientContext.Load<ListItem>(fileByServerRelativeUrl.ListItemAllFields,
                        new Expression<Func<ListItem, object>>[0]);
                    clientContext.Load<List>(fileByServerRelativeUrl.ListItemAllFields.ParentList,
                        new Expression<Func<List, object>>[0]);
                    if (!string.IsNullOrEmpty(sEmbeddedContent))
                    {
                        clientContext.Load<FieldCollection>(fileByServerRelativeUrl.ListItemAllFields.ParentList.Fields,
                            new Expression<Func<FieldCollection, object>>[0]);
                    }

                    fileByServerRelativeUrl.CheckOut();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (webPartsToEmbed.Count > 0)
                    {
                        string richTextEmbeddingField =
                            this.GetRichTextEmbeddingField(fileByServerRelativeUrl, callingAdapter);
                        if (!string.IsNullOrEmpty(sEmbeddedContent))
                        {
                            Field field = null;
                            foreach (Field current in ((IEnumerable<Field>)fileByServerRelativeUrl.ListItemAllFields
                                         .ParentList.Fields))
                            {
                                if (current.InternalName == richTextEmbeddingField)
                                {
                                    field = current;
                                    break;
                                }
                            }

                            if (field != null)
                            {
                                sEmbeddedContent =
                                    (this.CastStringToFieldType(sEmbeddedContent, field, callingAdapter) as string);
                            }
                        }

                        if (fileByServerRelativeUrl.ListItemAllFields.FieldValues.ContainsKey(richTextEmbeddingField))
                        {
                            if (string.IsNullOrEmpty(sEmbeddedContent))
                            {
                                sEmbeddedContent =
                                    (fileByServerRelativeUrl.ListItemAllFields[richTextEmbeddingField] as string);
                            }

                            if (!string.IsNullOrEmpty(sEmbeddedContent))
                            {
                                List<NWSAdapter.EmbeddedWebPart> list = new List<NWSAdapter.EmbeddedWebPart>();
                                foreach (NWSAdapter.EmbeddedWebPart current2 in webPartsToEmbed)
                                {
                                    if (this.UpdateEmbeddedWebPartReference(ref sEmbeddedContent, current2.SourceID,
                                            current2.TargetID))
                                    {
                                        list.Add(current2);
                                    }
                                }

                                foreach (NWSAdapter.EmbeddedWebPart current3 in list)
                                {
                                    webPartsToEmbed.Remove(current3);
                                }

                                if (0 <= sEmbeddedContent.IndexOf("ms-rte-layoutszone-inner",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    this.EmbedWebPartsInTeamSite(ref sEmbeddedContent, webPartsToEmbed);
                                    if (!string.IsNullOrEmpty(sEmbeddedContent))
                                    {
                                        fileByServerRelativeUrl.CheckIn(
                                            "Checkout required in order to add web parts to page during migration",
                                            ClientOMTools.GetCheckinType(fileByServerRelativeUrl));
                                        clientContext.Load<ListItem>(fileByServerRelativeUrl.ListItemAllFields,
                                            new Expression<Func<ListItem, object>>[]
                                            {
                                                (ListItem item) => item["_UIVersion"],
                                                (ListItem item) => item["_UIVersionString"]
                                            });
                                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                        fileByServerRelativeUrl.ListItemAllFields[richTextEmbeddingField] =
                                            sEmbeddedContent;
                                        fileByServerRelativeUrl.ListItemAllFields.Update();
                                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                    }
                                }
                                else
                                {
                                    this.EmbedWebPartsInGeneralSite(ref sEmbeddedContent, webPartsToEmbed);
                                    if (!string.IsNullOrEmpty(sEmbeddedContent))
                                    {
                                        fileByServerRelativeUrl.CheckIn(
                                            "Checkout required in order to add web parts to page during migration",
                                            ClientOMTools.GetCheckinType(fileByServerRelativeUrl));
                                        clientContext.Load<ListItem>(fileByServerRelativeUrl.ListItemAllFields,
                                            new Expression<Func<ListItem, object>>[]
                                            {
                                                (ListItem item) => item["_UIVersion"],
                                                (ListItem item) => item["_UIVersionString"]
                                            });
                                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                        fileByServerRelativeUrl.CheckOut();
                                        fileByServerRelativeUrl.ListItemAllFields[richTextEmbeddingField] =
                                            sEmbeddedContent;
                                        fileByServerRelativeUrl.ListItemAllFields.Update();
                                        fileByServerRelativeUrl.CheckIn(
                                            "Checkout required in order to add web parts to page during migration",
                                            ClientOMTools.GetCheckinType(fileByServerRelativeUrl));
                                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                        if (fileByServerRelativeUrl.ListItemAllFields.ParentList.EnableModeration)
                                        {
                                            clientContext.Load<ListItem>(fileByServerRelativeUrl.ListItemAllFields,
                                                new Expression<Func<ListItem, object>>[]
                                                {
                                                    (ListItem item) => item["_UIVersion"],
                                                    (ListItem item) => item["_UIVersionString"]
                                                });
                                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                            fileByServerRelativeUrl.ListItemAllFields["_ModerationStatus"] = "0";
                                            fileByServerRelativeUrl.ListItemAllFields.Update();
                                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }

        private static CheckinType GetCheckinType(Microsoft.SharePoint.Client.File pageFile)
        {
            if (pageFile.ListItemAllFields != null && pageFile.ListItemAllFields.ParentList.BaseTemplate ==
                ClientOMTools.PAGES_LIBRARY_TEMPLATE_ID)
            {
                return CheckinType.MinorCheckIn;
            }

            return CheckinType.OverwriteCheckIn;
        }

        private bool UpdateEmbeddedWebPartReference(ref string sEmbeddedContent, string sOldWebPartGuid,
            string sNewWebPartGuid)
        {
            if (string.IsNullOrEmpty(sOldWebPartGuid))
            {
                return false;
            }

            Regex regex = new Regex(sOldWebPartGuid, RegexOptions.IgnoreCase);
            MatchCollection matchCollection = regex.Matches(sEmbeddedContent);
            if (matchCollection.Count > 0)
            {
                foreach (Match match in matchCollection)
                {
                    sEmbeddedContent = sEmbeddedContent.Remove(match.Index, match.Length);
                    sEmbeddedContent = sEmbeddedContent.Insert(match.Index, sNewWebPartGuid.ToLower());
                }

                return true;
            }

            return false;
        }

        private void EmbedWebPartsInTeamSite(ref string sEmbeddedContent,
            List<NWSAdapter.EmbeddedWebPart> webPartsToEmbed)
        {
            if (webPartsToEmbed.Count <= 0)
            {
                return;
            }

            string text = "";
            string text2 = "";
            foreach (NWSAdapter.EmbeddedWebPart current in webPartsToEmbed)
            {
                if (0 <= current.ZoneID.IndexOf("right", StringComparison.OrdinalIgnoreCase))
                {
                    text2 = text2 + this.GetEmbeddedWebPartString(current.TargetID) + "\n";
                }
                else
                {
                    text = text + this.GetEmbeddedWebPartString(current.TargetID) + "\n";
                }
            }

            try
            {
                XmlNode embeddedNode = XmlUtility.StringToXmlNode(sEmbeddedContent);
                sEmbeddedContent = this.AddEmbeddedWebPartReferencesUsingXml(embeddedNode, text, text2);
            }
            catch
            {
                sEmbeddedContent = this.AddEmbeddedWebPartReferencesUsingRegex(sEmbeddedContent, text, text2);
            }
        }

        private void EmbedWebPartsInGeneralSite(ref string sEmbeddedContent,
            List<NWSAdapter.EmbeddedWebPart> webPartsToEmbed)
        {
            if (webPartsToEmbed.Count <= 0)
            {
                return;
            }

            string text = "<div>" + this.GenerateEmbeddedWebPartsHtml(webPartsToEmbed) + "</div>";
            int num = sEmbeddedContent.LastIndexOf("</div>", StringComparison.OrdinalIgnoreCase);
            if (num >= 0)
            {
                sEmbeddedContent = sEmbeddedContent.Insert(num, text);
                return;
            }

            sEmbeddedContent += text;
        }

        private string GenerateEmbeddedWebPartsHtml(List<NWSAdapter.EmbeddedWebPart> webPartsToEmbed)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            foreach (NWSAdapter.EmbeddedWebPart current in webPartsToEmbed)
            {
                stringBuilder.Append(this.GetEmbeddedWebPartString(current.TargetID) + "\n");
            }

            return stringBuilder.ToString();
        }

        private string GetEmbeddedWebPartString(string sWebPartGuid)
        {
            return string.Format(
                "<div class=\"ms-rtestate-read ms-rte-wpbox\"><div class=\"ms-rtestate-notify ms-rtestate-read {0}\" id=\"div_{0}\"></div><div id=\"vid_{0}\" style=\"display:none\"></div></div>",
                sWebPartGuid.ToLower());
        }

        private string AddEmbeddedWebPartReferencesUsingXml(XmlNode embeddedNode, string sLeftZoneEmbeddingHtml,
            string sRightZoneEmbeddingHtml)
        {
            string result = "";
            if (embeddedNode != null)
            {
                XmlNodeList xmlNodeList = embeddedNode.SelectNodes(
                    ".//tbody//div[@class='ms-rte-layoutszone-outer']/div[@class='ms-rte-layoutszone-inner']");
                if (xmlNodeList != null && xmlNodeList.Count > 0)
                {
                    XmlNode expr_28 = xmlNodeList[0];
                    expr_28.InnerXml += sLeftZoneEmbeddingHtml;
                    if (xmlNodeList.Count > 1)
                    {
                        XmlNode expr_49 = xmlNodeList[1];
                        expr_49.InnerXml += sRightZoneEmbeddingHtml;
                    }
                    else
                    {
                        XmlNode expr_63 = xmlNodeList[0];
                        expr_63.InnerXml += sRightZoneEmbeddingHtml;
                    }
                }

                result = embeddedNode.OuterXml;
            }

            return result;
        }

        private string AddEmbeddedWebPartReferencesUsingRegex(string sEmbeddedContent, string sLeftZoneEmbeddingHtml,
            string sRightZoneEmbeddingHtml)
        {
            string text = sEmbeddedContent;
            string text2 = "Inner";
            string pattern = "<\\s*tbody\\s*>(.*?<\\s*div[^>]+class\\s*=\\s*\"ms-rte-layoutszone-outer\".*?>.*?(?<" +
                             text2 + "><\\s*div[^>]+class\\s*=\\s*\"ms-rte-layoutszone-inner\".*?>))+";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match match = regex.Match(sEmbeddedContent);
            if (match.Success)
            {
                CaptureCollection captures = match.Groups[text2].Captures;
                if (captures != null && captures.Count > 0)
                {
                    int num = captures[0].Index + captures[0].Length;
                    text = sEmbeddedContent.Substring(0, num);
                    text += sLeftZoneEmbeddingHtml;
                    if (captures.Count > 1)
                    {
                        int num2 = captures[1].Index + captures[1].Length;
                        text += sEmbeddedContent.Substring(num, num2 - num);
                        num = num2;
                    }

                    text += sRightZoneEmbeddingHtml;
                    text += sEmbeddedContent.Substring(num);
                }
            }

            return text;
        }

        public string GetWebContentTypes(NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                bool flag = false;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("ContentTypes");
                xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/sharepoint/soap/");
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    flag = this.WriteWebContentTypes(clientContext.Web, xmlTextWriter, callingAdapter);
                }

                string text = null;
                while (!flag)
                {
                    if (text == null)
                    {
                        text = ((callingAdapter.Url.LastIndexOf('/') == 5 || callingAdapter.Url.LastIndexOf('/') == 6)
                            ? null
                            : callingAdapter.Url.Substring(0, callingAdapter.Url.LastIndexOf('/')));
                    }
                    else
                    {
                        text = ((text.LastIndexOf('/') == 5 || text.LastIndexOf('/') == 6)
                            ? null
                            : text.Substring(0, text.LastIndexOf('/')));
                    }

                    if (text == null)
                    {
                        break;
                    }

                    using (ClientContext clientContext2 = this.GetClientContext(text, callingAdapter))
                    {
                        flag = this.WriteWebContentTypes(clientContext2.Web, xmlTextWriter, callingAdapter);
                    }
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                result = stringWriter.ToString();
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private bool WriteWebContentTypes(Web currentWeb, XmlWriter xmlWriter, SharePointAdapter callingAdapter)
        {
            bool result = false;
            ContentTypeCollection contentTypes1 = currentWeb.ContentTypes;
            currentWeb.Context.Load<ContentTypeCollection>(contentTypes1,
                new Expression<Func<ContentTypeCollection, object>>[]
                {
                    (ContentTypeCollection contentTypes) => contentTypes.Include(
                        new Expression<Func<ContentType, object>>[]
                        {
                            (ContentType contentType) => contentType.Name,
                            (ContentType contentType) => contentType.Id,
                            (ContentType contentType) => contentType.Description,
                            (ContentType contentType) => contentType.Group
                        })
                });
            ClientOMTools.CallExecuteQuery(currentWeb.Context, callingAdapter);
            foreach (ContentType current in ((IEnumerable<ContentType>)contentTypes1))
            {
                xmlWriter.WriteStartElement("ContentType");
                xmlWriter.WriteAttributeString("Name", current.Name);
                xmlWriter.WriteAttributeString("ID", current.Id.ToString());
                xmlWriter.WriteAttributeString("Description", current.Description);
                xmlWriter.WriteAttributeString("Group", current.Group);
                xmlWriter.WriteEndElement();
                if (current.Id.ToString().Equals("0x"))
                {
                    result = true;
                }
            }

            return result;
        }

        public string GetRootCollection(NWSAdapter callingAdapter)
        {
            if (callingAdapter.CookieManager != null && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    if (this.ContainsSystemContentType(clientContext.Web))
                    {
                        result = callingAdapter.Url;
                        return result;
                    }
                }

                string text = null;
                bool flag = false;
                while (!flag)
                {
                    if (text == null)
                    {
                        text = ((callingAdapter.Url.LastIndexOf('/') == 5 || callingAdapter.Url.LastIndexOf('/') == 6)
                            ? null
                            : callingAdapter.Url.Substring(0, callingAdapter.Url.LastIndexOf('/')));
                    }
                    else
                    {
                        text = ((text.LastIndexOf('/') == 5 || text.LastIndexOf('/') == 6)
                            ? null
                            : text.Substring(0, text.LastIndexOf('/')));
                    }

                    if (text == null)
                    {
                        break;
                    }

                    using (ClientContext clientContext2 = this.GetClientContext(text, callingAdapter))
                    {
                        if (this.ContainsSystemContentType(clientContext2.Web))
                        {
                            result = text;
                            return result;
                        }
                    }
                }

                result = null;
            }
            finally
            {
                if (callingAdapter.CookieManager != null && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        private bool ContainsSystemContentType(Web currentWeb)
        {
            ContentTypeCollection contentTypes4 = currentWeb.ContentTypes;
            currentWeb.Context.Load<ContentTypeCollection>(contentTypes4,
                new Expression<Func<ContentTypeCollection, object>>[]
                {
                    (ContentTypeCollection contentTypes) => contentTypes.Include(
                        new Expression<Func<ContentType, object>>[]
                        {
                            (ContentType contentType) => contentType.Id
                        })
                });
            currentWeb.Context.ExecuteQuery();
            foreach (ContentType current in ((IEnumerable<ContentType>)contentTypes4))
            {
                if (current.Id.ToString().Equals("0x"))
                {
                    return true;
                }
            }

            return false;
        }

        private ContentType GetWebContentType(Web currentWeb, string contentTypeId)
        {
            ContentTypeCollection contentTypes0 = currentWeb.ContentTypes;
            currentWeb.Context.Load<ContentTypeCollection>(contentTypes0,
                new Expression<Func<ContentTypeCollection, object>>[]
                {
                    (ContentTypeCollection contentTypes) => contentTypes.Include(
                        new Expression<Func<ContentType, object>>[]
                        {
                            (ContentType contentType) => contentType.Name,
                            (ContentType contentType) => contentType.Id,
                            (ContentType contentType) => contentType.Description,
                            (ContentType contentType) => contentType.Group
                        })
                });
            currentWeb.Context.ExecuteQuery();
            foreach (ContentType current in ((IEnumerable<ContentType>)contentTypes0))
            {
                if (current.Id.ToString().Equals(contentTypeId, StringComparison.OrdinalIgnoreCase))
                {
                    return current;
                }
            }

            return null;
        }

        public string GetGroups(NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Groups");
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Microsoft.SharePoint.Client.GroupCollection siteGroups = clientContext.Web.SiteGroups;
                    clientContext.Load<Microsoft.SharePoint.Client.GroupCollection>(siteGroups,
                        new Expression<Func<Microsoft.SharePoint.Client.GroupCollection, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    foreach (Microsoft.SharePoint.Client.Group current in
                             ((IEnumerable<Microsoft.SharePoint.Client.Group>)siteGroups))
                    {
                        this.WriteGroupXml(xmlTextWriter, clientContext, current, callingAdapter);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                xmlTextWriter.WriteEndElement();
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return stringWriter.ToString();
        }

        private void WriteGroupXml(XmlTextWriter xmlWriter, ClientContext clientContext,
            Microsoft.SharePoint.Client.Group group, SharePointAdapter callingAdapter)
        {
            try
            {
                xmlWriter.WriteStartElement("Group");
                clientContext.Load<Microsoft.SharePoint.Client.Group>(group,
                    new Expression<Func<Microsoft.SharePoint.Client.Group, object>>[0]);
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                xmlWriter.WriteAttributeString("ID", group.Id.ToString());
                xmlWriter.WriteAttributeString("Name", group.Title);
                xmlWriter.WriteAttributeString("Description", group.Description);
                Principal owner = group.Owner;
                clientContext.Load<Principal>(owner, new Expression<Func<Principal, object>>[]
                {
                    (Principal o) => (object)o.PrincipalType,
                    (Principal o) => o.LoginName
                });
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                xmlWriter.WriteAttributeString("Owner", owner.LoginName);
                xmlWriter.WriteAttributeString("OwnerIsUser",
                    owner.PrincipalType.Equals(PrincipalType.User).ToString());
                xmlWriter.WriteAttributeString("OnlyAllowMembersViewMembership",
                    group.OnlyAllowMembersViewMembership.ToString());
                xmlWriter.WriteAttributeString("AllowMembersEditMembership",
                    group.AllowMembersEditMembership.ToString());
                xmlWriter.WriteAttributeString("AllowRequestToJoinLeave", group.AllowRequestToJoinLeave.ToString());
                xmlWriter.WriteAttributeString("AutoAcceptRequestToJoinLeave",
                    group.AutoAcceptRequestToJoinLeave.ToString());
                xmlWriter.WriteAttributeString("RequestToJoinLeaveEmailSetting", group.RequestToJoinLeaveEmailSetting);
                UserCollection users = group.Users;
                clientContext.Load<UserCollection>(users, new Expression<Func<UserCollection, object>>[0]);
                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                foreach (Principal current in ((IEnumerable<User>)users))
                {
                    string text = (current.LoginName != null) ? current.LoginName : null;
                    if (text != null)
                    {
                        xmlWriter.WriteStartElement("Member");
                        xmlWriter.WriteAttributeString("Login", text);
                        xmlWriter.WriteEndElement();
                    }
                }
            }
            finally
            {
                xmlWriter.WriteEndElement();
            }
        }

        public string GetUIVersion(NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            string result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    clientContext.Load<Web>(clientContext.Web, new Expression<Func<Web, object>>[]
                    {
                        (Web w) => (object)w.UIVersion
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    string text = clientContext.Web.UIVersion.ToString();
                    result = text;
                }
            }
            catch
            {
                result = string.Empty;
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        public bool CreateSite(string sDescription, uint lcid, string sTitle, string sName, string sTemplate,
            NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            bool result;
            try
            {
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Web web = clientContext.Web;
                    WebCreationInformation webCreationInformation = new WebCreationInformation();
                    webCreationInformation.Description = sDescription;
                    webCreationInformation.Language = Convert.ToInt32(lcid);
                    webCreationInformation.Title = sTitle;
                    webCreationInformation.Url = sName;
                    webCreationInformation.UseSamePermissionsAsParentSite = false;
                    webCreationInformation.WebTemplate = sTemplate;
                    Web clientObject = web.Webs.Add(webCreationInformation);
                    clientContext.Load<Web>(clientObject, new Expression<Func<Web, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }

                result = true;
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return result;
        }

        public void ClearTeamSiteWikiField(string sUrl, string sDefaultPage, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(sUrl, callingAdapter))
                {
                    Web web4 = clientContext.Web;
                    Guid gWikiPageHomePageFeature = new Guid("00bfea71-d8fe-4fec-8dad-01c19a6e4053");
                    FeatureCollection features4 = web4.Features;
                    clientContext.Load<FeatureCollection>(features4, new Expression<Func<FeatureCollection, object>>[]
                    {
                        (FeatureCollection features) => from feature in features
                            where feature.DefinitionId == gWikiPageHomePageFeature
                            select feature
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    if (features4.Count > 0)
                    {
                        clientContext.Load<Web>(web4, new Expression<Func<Web, object>>[]
                        {
                            (Web web) => web.ServerRelativeUrl
                        });
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        string serverRelativeUrl = web4.ServerRelativeUrl + "/" + sDefaultPage;
                        Microsoft.SharePoint.Client.File fileByServerRelativeUrl =
                            web4.GetFileByServerRelativeUrl(serverRelativeUrl);
                        clientContext.Load<Microsoft.SharePoint.Client.File>(fileByServerRelativeUrl,
                            new Expression<Func<Microsoft.SharePoint.Client.File, object>>[]
                            {
                                (Microsoft.SharePoint.Client.File file) => file.Title
                            });
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        if (fileByServerRelativeUrl == null)
                        {
                            throw new Exception("Could not retrieve default page of site.");
                        }

                        string value =
                            "<table id=\"layoutsTable\" style=\"width: 100%\"> \r\n                                                                                    <tbody> \r\n                                                                                        <tr style=\"vertical-align: top\"> \r\n                                                                                            <td style=\"width: 66.6%\"> \r\n                                                                                                <div class=\"ms-rte-layoutszone-outer\" style=\"width: 100%\"> \r\n                                                                                                    <div class=\"ms-rte-layoutszone-inner\" style=\"min-height: 60px; word-wrap: break-word\">                              \r\n                                                                                                    </div> \r\n                                                                                                </div> \r\n                                                                                            </td> \r\n                                                                                            <td style=\"width: 33.3%\"> \r\n                                                                                                <div class=\"ms-rte-layoutszone-outer\" style=\"width: 100%\"> \r\n                                                                                                    <div class=\"ms-rte-layoutszone-inner\" style=\"min-height: 60px; word-wrap: break-word\">                             \r\n                                                                                                    </div> \r\n                                                                                                </div> \r\n                                                                                            </td> \r\n                                                                                        </tr> \r\n                                                                                    </tbody> \r\n                                                                                </table><span id=\"layoutsData\" style=\"display:none\">false,false,2</span>";
                        fileByServerRelativeUrl.ListItemAllFields["WikiField"] = value;
                        fileByServerRelativeUrl.ListItemAllFields.Update();
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    }
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }

        public string AddorUpdateGroup(NWSAdapter callingAdapter, string groupXml)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            try
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(groupXml);
                string att = XmlAdapterUtility.getAtt(xmlNode, "Name");
                using (ClientContext clientContext = this.GetClientContext(callingAdapter))
                {
                    Microsoft.SharePoint.Client.GroupCollection siteGroups = clientContext.Web.SiteGroups;
                    clientContext.Load<Microsoft.SharePoint.Client.GroupCollection>(siteGroups,
                        new Expression<Func<Microsoft.SharePoint.Client.GroupCollection, object>>[0]);
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    try
                    {
                        foreach (Microsoft.SharePoint.Client.Group current in
                                 ((IEnumerable<Microsoft.SharePoint.Client.Group>)siteGroups))
                        {
                            if (current.Title == att)
                            {
                                siteGroups.Remove(current);
                                ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }

                    Microsoft.SharePoint.Client.Group group = null;
                    group = siteGroups.Add(new GroupCreationInformation
                    {
                        Title = XmlAdapterUtility.getAtt(xmlNode, "Name"),
                        Description = XmlAdapterUtility.getAtt(xmlNode, "Description")
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    string att2 = XmlAdapterUtility.getAtt(xmlNode, "Owner");
                    Principal principal = null;
                    if (att == att2)
                    {
                        principal = clientContext.Web.CurrentUser;
                    }
                    else
                    {
                        bool boolAtt = XmlAdapterUtility.getBoolAtt(xmlNode, "OwnerIsUser");
                        if (boolAtt)
                        {
                            principal = clientContext.Web.EnsureUser(att2);
                        }
                        else
                        {
                            foreach (Microsoft.SharePoint.Client.Group current2 in
                                     ((IEnumerable<Microsoft.SharePoint.Client.Group>)siteGroups))
                            {
                                if (current2.Title == att2)
                                {
                                    principal = current2;
                                    break;
                                }
                            }
                        }

                        if (principal == null)
                        {
                            principal = clientContext.Web.CurrentUser;
                        }
                    }

                    group.Owner = principal;
                    if (xmlNode.Attributes != null)
                    {
                        if (xmlNode.Attributes["OnlyAllowMembersViewMembership"] != null)
                        {
                            group.OnlyAllowMembersViewMembership =
                                XmlAdapterUtility.getBoolAtt(xmlNode, "OnlyAllowMembersViewMembership");
                        }

                        if (xmlNode.Attributes["AllowMembersEditMembership"] != null)
                        {
                            group.AllowMembersEditMembership =
                                XmlAdapterUtility.getBoolAtt(xmlNode, "AllowMembersEditMembership");
                        }

                        if (xmlNode.Attributes["AllowRequestToJoinLeave"] != null)
                        {
                            group.AllowRequestToJoinLeave =
                                XmlAdapterUtility.getBoolAtt(xmlNode, "AllowRequestToJoinLeave");
                        }

                        if (xmlNode.Attributes["AutoAcceptRequestToJoinLeave"] != null)
                        {
                            group.AutoAcceptRequestToJoinLeave =
                                XmlAdapterUtility.getBoolAtt(xmlNode, "AutoAcceptRequestToJoinLeave");
                        }

                        if (xmlNode.Attributes["RequestToJoinLeaveEmailSetting"] != null)
                        {
                            group.RequestToJoinLeaveEmailSetting =
                                XmlAdapterUtility.getAtt(xmlNode, "RequestToJoinLeaveEmailSetting");
                        }
                    }

                    group.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    bool flag = false;
                    Principal currentUser = clientContext.Web.CurrentUser;
                    clientContext.Load<Principal>(currentUser, new Expression<Func<Principal, object>>[]
                    {
                        (Principal o) => o.LoginName
                    });
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    bool flag2 = currentUser.LoginName.Contains("|");
                    List<string> list = new List<string>();
                    foreach (XmlNode xNode in xmlNode.SelectNodes("./Member"))
                    {
                        string att3 = XmlAdapterUtility.getAtt(xNode, "Login");
                        if (flag2)
                        {
                            if (Utils.ConvertWinOrFormsUserToClaimString(att3.ToUpper()) ==
                                currentUser.LoginName.ToUpper())
                            {
                                flag = true;
                            }
                        }
                        else if (att3.ToUpper() == currentUser.LoginName.ToUpper())
                        {
                            flag = true;
                        }

                        User user = null;
                        try
                        {
                            user = clientContext.Web.EnsureUser(att3);
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                            group.Users.AddUser(user);
                            ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                        }
                        catch
                        {
                            list.Add(user.LoginName);
                        }
                    }

                    if (att == att2)
                    {
                        group.Owner = group;
                        group.Update();
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    }

                    if (!flag)
                    {
                        group.Users.Remove(clientContext.Web.CurrentUser);
                        ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                    }

                    xmlTextWriter.WriteStartElement("AddOrUpdateGroup");
                    xmlTextWriter.WriteStartElement("Results");
                    xmlTextWriter.WriteAttributeString("Failures", list.Count.ToString());
                    foreach (string current3 in list)
                    {
                        xmlTextWriter.WriteStartElement("Failure");
                        xmlTextWriter.WriteAttributeString("LoginName", current3);
                        xmlTextWriter.WriteEndElement();
                    }

                    this.WriteGroupXml(xmlTextWriter, clientContext, group, callingAdapter);
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return stringWriter.ToString();
        }

        public void UpdateUIVersion(string sUrl, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(sUrl, callingAdapter))
                {
                    Web web = clientContext.Web;
                    web.UIVersion = 3;
                    web.UIVersionConfigurationEnabled = true;
                    web.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }

        public void ApplyWebTemplate(string sUrl, string sTemplateName, NWSAdapter callingAdapter)
        {
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                using (ClientContext clientContext = this.GetClientContext(sUrl, callingAdapter))
                {
                    Web web = clientContext.Web;
                    web.ApplyWebTemplate(sTemplateName);
                    web.Update();
                    ClientOMTools.CallExecuteQuery(clientContext, callingAdapter);
                }
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }
            }
        }
    }
}