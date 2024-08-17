using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms
{
    public static class SPUIUtils
    {
        public static bool DisplayPublishingInfrastructureWarning(IXMLAbleList sourceCollection, IXMLAbleList targetCollection)
        {
            IConfigurationVariable showPubInfrastructureWarning = SharePointConfigurationVariables.ShowPubInfrastructureWarning;
            if (!showPubInfrastructureWarning.GetValue<bool>())
            {
                return true;
            }
            Type type = typeof(SPWeb);
            SPWeb sPWeb = null;
            foreach (object obj in sourceCollection)
            {
                if (!type.IsAssignableFrom(obj.GetType()))
                {
                    continue;
                }
                sPWeb = obj as SPWeb;
            }
            SPWeb sPWeb1 = null;
            foreach (object obj1 in targetCollection)
            {
                if (!type.IsAssignableFrom(obj1.GetType()))
                {
                    continue;
                }
                sPWeb1 = obj1 as SPWeb;
            }
            if (sPWeb == null || sPWeb1 == null || sPWeb.PublishingInfrastructureActive == sPWeb1.PublishingInfrastructureActive)
            {
                return true;
            }
            if (!sPWeb.PublishingInfrastructureActive)
            {
                return true;
            }
            return VerifyUserActionDialog.GetUserVerification(showPubInfrastructureWarning, Metalogix.SharePoint.UI.WinForms.Properties.Resources.PublishingWarningInformation, "Information", null, "Click here for more information", MessageBoxButtons.OKCancel);
        }

        public static bool ExecutePreSiteCopyConfigurationChecks(IXMLAbleList sourceCollection, IXMLAbleList targetCollection, PasteSiteOptions options)
        {
            SPWeb item = sourceCollection[0] as SPWeb;
            SPWeb sPWeb = targetCollection[0] as SPWeb;
            if (options.WebTemplateName == null)
            {
                PasteSiteAction.InitializeMappings(options, item, sPWeb.Templates);
            }
            if (!SPUIUtils.DisplayPublishingInfrastructureWarning(sourceCollection, targetCollection))
            {
                return false;
            }
            return true;
        }

        private static List<Attribute> GetAttributes(Type type, Type typeAttribute)
        {
            List<Attribute> attributes = new List<Attribute>(4);
            SPUIUtils.GetAttributes(type, typeAttribute, ref attributes);
            return attributes;
        }

        private static void GetAttributes(Type typeWithAttributes, Type typeAttribute, ref List<Attribute> listAttributeInstances)
        {
            if (typeWithAttributes == null || typeWithAttributes == typeof(object))
            {
                return;
            }
            if (!typeWithAttributes.IsSubclassOf(typeof(SharePointAction)))
            {
                return;
            }
            Attribute[] customAttributes = Attribute.GetCustomAttributes(typeWithAttributes, typeAttribute, false);
            if (customAttributes != null && (int)customAttributes.Length > 0)
            {
                Attribute[] attributeArray = customAttributes;
                for (int i = 0; i < (int)attributeArray.Length; i++)
                {
                    Attribute attribute = attributeArray[i];
                    if (!listAttributeInstances.Contains(attribute))
                    {
                        listAttributeInstances.Add(attribute);
                    }
                }
            }
            SPUIUtils.GetAttributes(typeWithAttributes.BaseType, typeAttribute, ref listAttributeInstances);
        }

        public static void GetDisabledActions(Type typeAction, IXMLAbleList target, ref Dictionary<string, string> dictionaryDisabledActions, ref Dictionary<string, bool> dictionaryCheckedActions)
        {
            if (dictionaryCheckedActions != null && dictionaryCheckedActions.ContainsKey(typeAction.FullName))
            {
                return;
            }
            dictionaryCheckedActions.Add(typeAction.FullName, true);
            List<Attribute> attributes = SPUIUtils.GetAttributes(typeAction, typeof(SubActionTypesAttribute));
            if (attributes == null || attributes.Count == 0)
            {
                return;
            }
            foreach (SubActionTypesAttribute attribute in attributes)
            {
                foreach (Type subActionType in attribute.SubActionTypes)
                {
                    if (!subActionType.IsSubclassOf(typeof(SharePointAction)) || dictionaryDisabledActions.ContainsKey(subActionType.FullName))
                    {
                        continue;
                    }
                    if (target != null)
                    {
                        foreach (SPNode sPNode in target)
                        {
                            if (SharePointAction<ActionOptions>.EnabledOnTarget(subActionType, sPNode))
                            {
                                continue;
                            }
                            if (dictionaryDisabledActions.ContainsKey(subActionType.FullName))
                            {
                                break;
                            }
                            string fullName = subActionType.FullName;
                            object[] customAttributes = subActionType.GetCustomAttributes(typeof(NameAttribute), false);
                            if (customAttributes != null && (int)customAttributes.Length > 0)
                            {
                                Attribute attribute1 = customAttributes[0] as Attribute;
                                if (attribute1 != null && attribute1 is NameAttribute)
                                {
                                    fullName = ((NameAttribute)attribute1).Name;
                                }
                            }
                            dictionaryDisabledActions.Add(subActionType.FullName, fullName);
                            break;
                        }
                    }
                    SPUIUtils.GetDisabledActions(subActionType, target, ref dictionaryDisabledActions, ref dictionaryCheckedActions);
                }
            }
        }

        public static bool IsAzureConnectionStringEmpty(SPNode sourceNode, SPNode targetNode, bool encryptAzureMigrationJobs, ref bool isAzureUserMappingWarningMessageRepeating)
        {
            string empty = string.Empty;
            if (SPUtils.IsGlobalUserMappingNeeded(sourceNode, targetNode) && !isAzureUserMappingWarningMessageRepeating)
            {
                empty = Metalogix.SharePoint.UI.WinForms.Properties.Resources.AzureUserMappingWarning;
                isAzureUserMappingWarningMessageRepeating = true;
            }
            if (!encryptAzureMigrationJobs && string.IsNullOrEmpty(SharePointConfigurationVariables.UploadManagerAzureStorageConnectionString))
            {
                if (!string.IsNullOrEmpty(empty))
                {
                    empty = string.Format("{0}{1}{2}", empty, Environment.NewLine, Environment.NewLine);
                }
                empty = string.Format("{0}{1}", empty, Metalogix.Actions.Properties.Resources.AzureConnectionStringWarning);
            }
            if (string.IsNullOrEmpty(empty))
            {
                return false;
            }
            FlatXtraMessageBox.Show(empty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return true;
        }

        public static bool IsMigrationPipelineAllowed(SharePointObjectScope scope, NodeCollection targetNodes)
        {
            bool isSharePointOnline = false;
            SharePointObjectScope[] sharePointObjectScopeArray = new SharePointObjectScope[] { SharePointObjectScope.List, SharePointObjectScope.Folder, SharePointObjectScope.Site, SharePointObjectScope.SiteCollection };
            bool flag = scope.In<SharePointObjectScope>(sharePointObjectScopeArray);
            if (flag && targetNodes != null && targetNodes.Count > 0)
            {
                isSharePointOnline = ((SPNode)targetNodes[0]).Adapter.SharePointVersion.IsSharePointOnline;
            }
            if (isSharePointOnline)
            {
                return flag;
            }
            return false;
        }

        public static bool IsOptionsCopyingOnlyTopLinkAndQuickLaunchToBeDisabled(NodeCollection source, NodeCollection target)
        {
            if (source == null || source.Count == 0 || target == null || target.Count == 0)
            {
                return false;
            }
            bool flag = (((SPNode)source[0]).Adapter.IsClientOM ? true : ((SPNode)source[0]).Adapter.IsNws);
            bool isClientOM = ((SPNode)target[0]).Adapter.IsClientOM;
            if (!flag)
            {
                return isClientOM;
            }
            return false;
        }

        public static void LogTelemetry(string actionName, bool isAdvancedMode)
        {
            string str = string.Format("{0}{1}", actionName.Replace(" ", string.Empty), (isAdvancedMode ? "_AdvancedMode" : UIUtils.SimplifiedMode));
            LongAccumulator.Message.Send(str, (long)1, null);
        }

        public static bool NotifyDisabledSubactions(Metalogix.Actions.Action action, IXMLAbleList target)
        {
            bool flag = true;
            Dictionary<string, string> strs = new Dictionary<string, string>(8);
            Dictionary<string, bool> strs1 = new Dictionary<string, bool>(8);
            SPUIUtils.GetDisabledActions(action.GetType(), target, ref strs, ref strs1);
            if (strs.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder("This action uses the following sub-actions that are not enabled on one or more of the selected target servers. Any configuration options selected relating to these actions will be ignored: \n\n");
                foreach (string key in strs.Keys)
                {
                    stringBuilder.AppendFormat("{0}\n", strs[key]);
                }
                DialogResult dialogResult = FlatXtraMessageBox.Show(stringBuilder.ToString(), "Disabled sub-actions", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                flag = dialogResult == DialogResult.OK;
            }
            return flag;
        }

        public static bool ShouldSetExplicitOptions(Metalogix.Actions.Action action)
        {
            SharePointActionOptions options = action.Options as SharePointActionOptions;
            if (options == null)
            {
                return false;
            }
            bool flag = (!options.IsFromAdvancedMode.HasValue ? false : options.IsFromAdvancedMode.Value);
            if (!string.IsNullOrEmpty(action.JobID))
            {
                return false;
            }
            return !flag;
        }

        public static bool ShouldShowAdvancedMode(ActionOptions actionOptions)
        {
            bool flag = false;
            if (actionOptions != null)
            {
                SharePointActionOptions sharePointActionOption = actionOptions as SharePointActionOptions;
                flag = (sharePointActionOption == null || !sharePointActionOption.IsFromAdvancedMode.HasValue ? UIConfigurationVariables.ShowAdvanced : !sharePointActionOption.IsFromAdvancedMode.Value);
            }
            return flag;
        }
    }
}