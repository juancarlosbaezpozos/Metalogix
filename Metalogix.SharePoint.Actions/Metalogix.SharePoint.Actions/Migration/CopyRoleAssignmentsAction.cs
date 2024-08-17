using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
    [Name("Paste Role Assignments")]
    [ShowInMenus(false)]
    public class CopyRoleAssignmentsAction : PasteAction<CopyRoleAssignmentsOptions>
    {
        private static SPRoleConverter03to07 s_RoleConverter;

        protected static TransformerDefinition<RoleAssignment, CopyRoleAssignmentsAction, RoleAssignmentCollection, RoleAssignmentCollection> s_definition;

        static CopyRoleAssignmentsAction()
        {
            CopyRoleAssignmentsAction.s_RoleConverter = new SPRoleConverter03to07();
            CopyRoleAssignmentsAction.s_definition = new TransformerDefinition<RoleAssignment, CopyRoleAssignmentsAction, RoleAssignmentCollection, RoleAssignmentCollection>("SharePoint Permissions", false);
        }

        public CopyRoleAssignmentsAction()
        {
        }

        private void AddRoleAssignmentsToAzureManifest(ISecurableObject source, ISecurableObject target, bool bDeleteSafely, IUploadManager uploadManager, BaseManifestItem baseManifestItem)
        {
            bool flag;
            SPListItem sPListItem;
            bool flag1 = (source is SPListItem ? true : false);
            Dictionary<string, Role> strs = null;
            if (!base.SharePointOptions.MapRolesByName && source.Roles.Count <= source.RoleAssignments.TotalAssignments)
            {
                strs = new Dictionary<string, Role>();
                foreach (Role role in source.Roles)
                {
                    strs.Add(role.RoleName, this.GetTargetRole(role, source, target, strs));
                }
            }
            this.EnsurePermissionedPrincipalExistence(source, target, uploadManager);
            if (!flag1)
            {
                CopyRoleAssignmentsAction.s_definition.BeginTransformation(this, source.RoleAssignments, target.RoleAssignments, this.Options.Transformers);
            }
            foreach (RoleAssignment roleAssignment in source.RoleAssignments)
            {
                if (!base.CheckForAbort())
                {
                    RoleAssignment roleAssignments = null;
                    if (flag1)
                    {
                        roleAssignments = roleAssignment;
                    }
                    else
                    {
                        roleAssignments = CopyRoleAssignmentsAction.s_definition.Transform(roleAssignment, this, source.RoleAssignments, target.RoleAssignments, this.Options.Transformers);
                        if (roleAssignments == null)
                        {
                            LogItem logItem = new LogItem("Skipping role assignment", roleAssignment.Principal.PrincipalName, null, target.DisplayUrl, ActionOperationStatus.Skipped);
                            base.FireOperationStarted(logItem);
                            logItem.Information = "Role assignment skipped due to the result of a transformation";
                            base.FireOperationFinished(logItem);
                            continue;
                        }
                    }
                    string str = base.MapPrincipal(roleAssignments.Principal.PrincipalName);
                    int userOrGroupIDByName = uploadManager.GetUserOrGroupIDByName(str);
                    if (userOrGroupIDByName != 0)
                    {
                        if (roleAssignments.Roles.Count != 1)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (!typeof(SPRole).IsAssignableFrom(roleAssignments.Roles[0].GetType()) ? false : ((SPRole)roleAssignments.Roles[0]).Hidden);
                        }
                        if (flag)
                        {
                            continue;
                        }
                        string empty = string.Empty;
                        if (source is SPListItem)
                        {
                            sPListItem = (SPListItem)source;
                        }
                        else
                        {
                            sPListItem = null;
                        }
                        SPListItem sPListItem1 = sPListItem;
                        if (sPListItem1 != null && !string.IsNullOrEmpty(sPListItem1.Name))
                        {
                            empty = string.Format("/{0}", sPListItem1.Name);
                        }
                        LogItem logItem1 = new LogItem("Processing role assignments", roleAssignments.Principal.PrincipalName, source.DisplayUrl, string.Concat(target.DisplayUrl, empty, " - ", str), ActionOperationStatus.Running);
                        base.FireOperationStarted(logItem1);
                        StringBuilder stringBuilder = new StringBuilder();
                        bool flag2 = false;
                        foreach (Role role1 in roleAssignments.Roles)
                        {
                            if (base.CheckForAbort())
                            {
                                break;
                            }
                            string roleName = role1.RoleName;
                            try
                            {
                                Role targetRole = this.GetTargetRole(role1, source, target, strs);
                                if (targetRole != null)
                                {
                                    roleName = targetRole.RoleName;
                                    stringBuilder.AppendLine(string.Format("Mapped role '{0}' to '{1}'", role1.RoleName, targetRole.RoleName));
                                    if (!typeof(SPRole).IsAssignableFrom(targetRole.GetType()) || !((SPRole)targetRole).Hidden)
                                    {
                                        int azureManifest = this.AddRoleToAzureManifest(uploadManager, targetRole);
                                        if (azureManifest != 0)
                                        {
                                            ManifestRoleAssignment manifestRoleAssignment = new ManifestRoleAssignment()
                                            {
                                                RoleId = azureManifest,
                                                PrincipalId = userOrGroupIDByName
                                            };
                                            baseManifestItem.RoleAssignments.Add(manifestRoleAssignment);
                                            logItem1.AddCompletionDetail(Resources.Migration_Detail_RoleAssignments, (long)1);
                                            LogItem licenseDataUsed = logItem1;
                                            licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + (long)5;
                                        }
                                    }
                                    else
                                    {
                                        stringBuilder.AppendLine(string.Format("Skipping hidden role {0}: Hidden roles such as Limited Access are automatically assigned by SharePoint when necessary", targetRole.RoleName));
                                    }
                                }
                                else
                                {
                                    stringBuilder.AppendLine(string.Format("Skipping role {0}: No matching role could be found", role1.RoleName));
                                }
                            }
                            catch (Exception exception1)
                            {
                                Exception exception = exception1;
                                flag2 = true;
                                stringBuilder.AppendLine(string.Format("Failed to process role '{0}': {1} StackTrace : {2}", roleName, exception.Message, exception.StackTrace));
                            }
                        }
                        logItem1.Details = stringBuilder.ToString();
                        logItem1.Status = (flag2 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
                        base.FireOperationFinished(logItem1);
                    }
                    else
                    {
                        LogItem logItem2 = new LogItem("Skipping role assignment", roleAssignment.Principal.PrincipalName, null, target.DisplayUrl, ActionOperationStatus.Skipped);
                        base.FireOperationStarted(logItem2);
                        logItem2.Information = string.Format("Role assignment skipped due to the Principal '{0}' does not exist at target", str);
                        base.FireOperationFinished(logItem2);
                    }
                }
                else
                {
                    return;
                }
            }
            if (!flag1)
            {
                CopyRoleAssignmentsAction.s_definition.EndTransformation(this, source.RoleAssignments, target.RoleAssignments, this.Options.Transformers);
            }
            source.ReleasePermissionsData();
            if (!flag1)
            {
                target.ReleasePermissionsData();
            }
        }

        private int AddRoleToAzureManifest(IUploadManager uploadManager, Role mappedRole)
        {
            SPRole sPRole = (SPRole)mappedRole;
            ManifestRole manifestRole = new ManifestRole()
            {
                RoleId = sPRole.RoleId,
                Title = sPRole.RoleName,
                Description = sPRole.Description,
                PermMask = sPRole.PermMask,
                IsHidden = sPRole.Hidden,
                RoleOrder = sPRole.RoleOrder,
                RoleType = sPRole.Type
            };
            return uploadManager.AddRole(manifestRole);
        }

        private void CopyRoleAssignments(ISecurableObject source, ISecurableObject target, bool bDeleteSafely)
        {
            bool flag;
            Dictionary<string, Role> strs = null;
            if (!base.SharePointOptions.MapRolesByName && source.Roles.Count <= source.RoleAssignments.TotalAssignments)
            {
                strs = new Dictionary<string, Role>();
                foreach (Role role in source.Roles)
                {
                    strs.Add(role.RoleName, this.GetTargetRole(role, source, target, strs));
                }
            }
            this.EnsurePermissionedPrincipalExistence(source, target, null);
            List<string> strs1 = new List<string>();
            Dictionary<string, List<string>> strs2 = new Dictionary<string, List<string>>();
            CopyRoleAssignmentsAction.s_definition.BeginTransformation(this, source.RoleAssignments, target.RoleAssignments, this.Options.Transformers);
            bool flag1 = false;
            foreach (RoleAssignment roleAssignment in source.RoleAssignments)
            {
                if (!base.CheckForAbort())
                {
                    RoleAssignment roleAssignments = CopyRoleAssignmentsAction.s_definition.Transform(roleAssignment, this, source.RoleAssignments, target.RoleAssignments, this.Options.Transformers);
                    if (roleAssignments != null)
                    {
                        SecurityPrincipal targetPrincipal = this.GetTargetPrincipal(roleAssignments.Principal, source, target);
                        if (targetPrincipal == null)
                        {
                            continue;
                        }
                        List<string> item = null;
                        if (!strs2.ContainsKey(targetPrincipal.PrincipalName))
                        {
                            item = new List<string>();
                            strs2.Add(targetPrincipal.PrincipalName, item);
                        }
                        else
                        {
                            item = strs2[targetPrincipal.PrincipalName];
                        }
                        RoleAssignment roleAssignmentsByPrincipalName = target.RoleAssignments.GetRoleAssignmentsByPrincipalName(targetPrincipal.PrincipalName);
                        if (roleAssignments.Roles.Count != 1)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (!typeof(SPRole).IsAssignableFrom(roleAssignments.Roles[0].GetType()) ? false : ((SPRole)roleAssignments.Roles[0]).Hidden);
                        }
                        flag1 = flag;
                        if (flag1)
                        {
                            continue;
                        }
                        LogItem logItem = new LogItem("Adding role assignments", roleAssignments.Principal.PrincipalName, source.DisplayUrl, string.Concat(target.DisplayUrl, " - ", targetPrincipal.PrincipalName), ActionOperationStatus.Running);
                        base.FireOperationStarted(logItem);
                        StringBuilder stringBuilder = new StringBuilder();
                        bool flag2 = false;
                        foreach (Role role1 in roleAssignments.Roles)
                        {
                            if (base.CheckForAbort())
                            {
                                break;
                            }
                            string roleName = role1.RoleName;
                            try
                            {
                                Role targetRole = this.GetTargetRole(role1, source, target, strs);
                                if (targetRole != null)
                                {
                                    roleName = targetRole.RoleName;
                                    stringBuilder.AppendLine(string.Format("Mapped role '{0}' to '{1}'", role1.RoleName, targetRole.RoleName));
                                    if (!typeof(SPRole).IsAssignableFrom(targetRole.GetType()) || !((SPRole)targetRole).Hidden)
                                    {
                                        if (roleAssignmentsByPrincipalName == null || !roleAssignmentsByPrincipalName.Roles.Contains(targetRole))
                                        {
                                            stringBuilder.AppendLine(string.Format("Adding role '{0}'", targetRole.RoleName));
                                            target.RoleAssignments.AddRoleAssignment(targetPrincipal, targetRole, base.SharePointOptions.AllowDBUserWriting);
                                            logItem.AddCompletionDetail(Resources.Migration_Detail_RoleAssignments, (long)1);
                                            LogItem licenseDataUsed = logItem;
                                            licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + (long)5;
                                        }
                                        else
                                        {
                                            stringBuilder.AppendLine(string.Format("Preserving role '{0}'", targetRole.RoleName));
                                        }
                                        item.Add(targetRole.RoleName);
                                    }
                                    else
                                    {
                                        stringBuilder.AppendLine(string.Format("Skipping hidden role {0}: Hidden roles such as Limited Access are automatically assigned by SharePoint when necessary", targetRole.RoleName));
                                        item.Add(targetRole.RoleName);
                                    }
                                }
                                else
                                {
                                    stringBuilder.AppendLine(string.Format("Skipping role {0}: No matching role could be found", role1.RoleName));
                                }
                            }
                            catch (Exception exception1)
                            {
                                Exception exception = exception1;
                                flag2 = true;
                                stringBuilder.AppendLine(string.Format("Failed to add role '{0}': {1}", roleName, exception.Message));
                            }
                        }
                        bool flag3 = false;
                        if (!base.CheckForAbort() && base.SharePointOptions.ClearRoleAssignments && roleAssignmentsByPrincipalName != null)
                        {
                            List<Role> roles = new List<Role>();
                            foreach (Role role2 in roleAssignmentsByPrincipalName.Roles)
                            {
                                if (item.Contains(role2.RoleName))
                                {
                                    continue;
                                }
                                roles.Add(role2);
                                if (typeof(SPRole).IsAssignableFrom(role2.GetType()) && ((SPRole)role2).Hidden)
                                {
                                    continue;
                                }
                                stringBuilder.AppendLine(string.Format("Removing role '{0}'", role2.RoleName));
                            }
                            stringBuilder.AppendLine(string.Format("Delete targetRoleAssignment '{0}' with {1} role(s)", roleAssignmentsByPrincipalName.Principal.PrincipalName, roles.Count));
                            this.DeleteRoles(roles, roleAssignmentsByPrincipalName, source, target, out flag3);
                        }
                        logItem.Details = stringBuilder.ToString();
                        logItem.Status = (flag2 || flag3 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
                        base.FireOperationFinished(logItem);
                        strs1.Add(targetPrincipal.PrincipalName);
                    }
                    else
                    {
                        LogItem logItem1 = new LogItem("Skipping role assignment", roleAssignment.Principal.PrincipalName, null, target.DisplayUrl, ActionOperationStatus.Skipped);
                        base.FireOperationStarted(logItem1);
                        logItem1.Information = "Role assignment skipped due to the result of a transformation";
                        base.FireOperationFinished(logItem1);
                    }
                }
                else
                {
                    return;
                }
            }
            if (base.SharePointOptions.ClearRoleAssignments)
            {
                List<RoleAssignment> roleAssignments1 = new List<RoleAssignment>();
                foreach (RoleAssignment roleAssignment1 in target.RoleAssignments)
                {
                    if (strs1.Contains(roleAssignment1.Principal.PrincipalName))
                    {
                        continue;
                    }
                    roleAssignments1.Add(roleAssignment1);
                }
                if (roleAssignments1.Count > 0)
                {
                    bool flag4 = false;
                    LogItem str = new LogItem("Deleting role assignments not from source", "", source.DisplayUrl, target.DisplayUrl, ActionOperationStatus.Running);
                    StringBuilder stringBuilder1 = new StringBuilder();
                    base.FireOperationStarted(str);
                    stringBuilder1.AppendLine(string.Format("Attempting to delete {0} role(s) that are not from the source:", roleAssignments1.Count));
                    stringBuilder1.AppendLine(string.Format("- deleteSafely={0}, sourceHasLimitedOnly={1}", bDeleteSafely, flag1));
                    stringBuilder1.AppendLine(string.Format("- source.Type={0}, target.Type={1}", source.GetType().Name, target.GetType().Name));
                    stringBuilder1.AppendLine(string.Format("- source:{0}, target:{1}", source.DisplayUrl, target.DisplayUrl));
                    foreach (RoleAssignment roleAssignment2 in roleAssignments1)
                    {
                        flag4 = false;
                        if (base.CheckForAbort())
                        {
                            break;
                        }
                        if (!bDeleteSafely)
                        {
                            stringBuilder1.AppendLine(string.Format("- delete principal '{0}' only. Type={1}, Role(s)={2}", roleAssignment2.Principal.PrincipalName, roleAssignment2.Principal.Type, roleAssignment2.Roles.Count));
                            this.DeleteRoles(null, roleAssignment2, source, target, out flag4);
                            if (!flag4)
                            {
                                continue;
                            }
                            stringBuilder1.AppendLine("- Error occured, list of role details:");
                            foreach (Role role3 in roleAssignment2.Roles)
                            {
                                SPRole sPRole = role3 as SPRole;
                                stringBuilder1.AppendLine(string.Format("- RoleName='{0}', Hidden={1}, Description={2}", (sPRole != null ? sPRole.RoleName : role3.RoleName), (sPRole != null ? sPRole.Hidden.ToString() : "?"), (sPRole != null ? sPRole.Description : "?")));
                            }
                        }
                        else
                        {
                            List<Role> roles1 = new List<Role>();
                            foreach (Role role4 in roleAssignment2.Roles)
                            {
                                roles1.Add(role4);
                            }
                            stringBuilder1.AppendLine(string.Format("- delete '{0}' with {1} role(s)", roleAssignment2.Principal.PrincipalName, roles1.Count));
                            this.DeleteRoles(roles1, roleAssignment2, source, target, out flag4);
                        }
                    }
                    str.Details = stringBuilder1.ToString();
                    str.Status = (flag4 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
                    base.FireOperationFinished(str);
                }
            }
            SPListItem sPListItem = source as SPListItem;
            SPListItem sPListItem1 = target as SPListItem;
            if (sPListItem != null && sPListItem1 != null && !sPListItem1.Adapter.IsDB && !sPListItem1.Adapter.IsNws && sPListItem.HasUniquePermissions != sPListItem1.HasUniquePermissions)
            {
                OperationReportingResult operationReportingResult = new OperationReportingResult(CopyRoleAssignmentsAction.UpdateListItemRoleInheritance(sPListItem, sPListItem1));
                if (operationReportingResult.ErrorOccured)
                {
                    LogItem logItem2 = new LogItem("Updating list item role inheritance", sPListItem1.Name, sPListItem.DisplayUrl, sPListItem1.DisplayUrl, ActionOperationStatus.Warning)
                    {
                        Details = operationReportingResult.AllReportElementsAsString
                    };
                    base.FireOperationStarted(logItem2);
                    base.FireOperationFinished(logItem2);
                }
            }
            CopyRoleAssignmentsAction.s_definition.EndTransformation(this, source.RoleAssignments, target.RoleAssignments, this.Options.Transformers);
            source.ReleasePermissionsData();
            target.ReleasePermissionsData();
        }

        private void DeleteRoles(List<Role> roles, RoleAssignment targetRoleAssignment, ISecurableObject source, ISecurableObject target, out bool errorsOccured)
        {
            errorsOccured = false;
            if (roles == null)
            {
                try
                {
                    target.RoleAssignments.RemoveRoleAssignment(targetRoleAssignment.Principal, null);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    errorsOccured = true;
                    Metalogix.Utilities.ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(exception);
                    LogItem logItem = new LogItem("Removing all role assignments failed", targetRoleAssignment.Principal.PrincipalName, source.DisplayUrl, target.DisplayUrl, ActionOperationStatus.Running);
                    base.FireOperationStarted(logItem);
                    logItem.Exception = exception;
                    logItem.Status = ActionOperationStatus.Failed;
                    logItem.Details = exceptionMessageAndDetail.Detail;
                    logItem.Information = string.Concat("Exception thrown: ", exception.Message);
                    base.FireOperationFinished(logItem);
                }
            }
            else
            {
                foreach (Role role in roles)
                {
                    try
                    {
                        target.RoleAssignments.RemoveRoleAssignment(targetRoleAssignment.Principal, role);
                    }
                    catch (Exception exception3)
                    {
                        Exception exception2 = exception3;
                        errorsOccured = true;
                        LogItem logItem1 = new LogItem("Removing old role assignment failed", string.Concat(targetRoleAssignment.Principal.PrincipalName, ":", role.RoleName), source.DisplayUrl, target.DisplayUrl, ActionOperationStatus.Running);
                        base.FireOperationStarted(logItem1);
                        logItem1.Exception = exception2;
                        if (typeof(SPRole).IsAssignableFrom(role.GetType()) && ((SPRole)role).Hidden)
                        {
                            logItem1.ItemName = "Unable to remove hidden role assignment";
                            logItem1.Status = ActionOperationStatus.Skipped;
                        }
                        base.FireOperationFinished(logItem1);
                    }
                }
            }
        }

        private void EnsurePermissionedPrincipalExistence(ISecurableObject source, ISecurableObject target, IUploadManager uploadManager = null)
        {
            SPWeb parentWeb = null;
            if (typeof(SPWeb).IsAssignableFrom(target.GetType()))
            {
                parentWeb = (SPWeb)target;
            }
            else if (typeof(SPFolder).IsAssignableFrom(target.GetType()))
            {
                parentWeb = ((SPFolder)target).ParentList.ParentWeb;
            }
            else if (typeof(SPListItem).IsAssignableFrom(target.GetType()))
            {
                parentWeb = ((SPListItem)target).ParentList.ParentWeb;
            }
            List<SPUser> sPUsers = new List<SPUser>();
            List<SPGroup> sPGroups = new List<SPGroup>();
            foreach (RoleAssignment roleAssignment in source.RoleAssignments)
            {
                if (base.PrincipalMappings.ContainsKey(roleAssignment.Principal.PrincipalName) && uploadManager == null)
                {
                    continue;
                }
                if (!(roleAssignment.Principal is SPUser))
                {
                    if (!(roleAssignment.Principal is SPGroup))
                    {
                        continue;
                    }
                    SPGroup principal = roleAssignment.Principal as SPGroup;
                    if (!sPGroups.Contains(principal))
                    {
                        sPGroups.Add(principal);
                    }
                    while (principal.Owner != null && (principal.OwnerIsUser && !sPUsers.Contains((SPUser)principal.Owner) || !principal.OwnerIsUser && !sPGroups.Contains((SPGroup)principal.Owner)))
                    {
                        if (!principal.OwnerIsUser)
                        {
                            principal = (SPGroup)principal.Owner;
                            sPGroups.Add(principal);
                        }
                        else
                        {
                            sPUsers.Add((SPUser)principal.Owner);
                        }
                    }
                }
                else
                {
                    sPUsers.Add(roleAssignment.Principal as SPUser);
                }
            }
            base.EnsurePrincipalExistence(sPUsers.ToArray(), sPGroups.ToArray(), parentWeb, uploadManager, null);
        }

        // Metalogix.SharePoint.Actions.Migration.CopyRoleAssignmentsAction
        private float GetListPermissionsSimilarity(SPRole2003 originalRole, SPRole2007 convertedRole, SPRole2007 targetRole)
        {
            bool flag = originalRole.ContainsRight(SPRights2003.ManageListPermissions.ToString());
            int num = targetRole.AvailableRights.Length - targetRole.ListRights.Length;
            int num2 = 0;
            int num3 = targetRole.ListRights.Length * (2 * (num - 1)) + targetRole.AvailableRights.Length + num;
            string[] listRights = targetRole.ListRights;
            for (int i = 0; i < listRights.Length; i++)
            {
                string sRightName = listRights[i];
                bool flag2 = convertedRole.ContainsRight(sRightName);
                bool flag3 = targetRole.ContainsRight(sRightName);
                if ((flag2 && flag3) || (!flag2 && !flag3))
                {
                    num2 += 2 * num;
                }
            }
            string[] webRights = targetRole.WebRights;
            for (int j = 0; j < webRights.Length; j++)
            {
                string sRightName2 = webRights[j];
                bool flag4 = convertedRole.ContainsRight(sRightName2);
                bool flag5 = targetRole.ContainsRight(sRightName2);
                if ((flag4 && flag5) || (!flag4 && !flag5))
                {
                    num2++;
                }
            }
            if (flag && targetRole.ContainsRight(SPRights2007.ManagePermissions.ToString()) && !convertedRole.ContainsRight(SPRights2007.ManagePermissions.ToString()))
            {
                num2 += num;
            }
            if (convertedRole.RoleName == targetRole.RoleName)
            {
                num2++;
            }
            return (float)num2 / (float)num3;
        }

        protected override List<ITransformerDefinition> GetSupportedDefinitions()
        {
            List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
            supportedDefinitions.Add(CopyRoleAssignmentsAction.s_definition);
            return supportedDefinitions;
        }

        private SecurityPrincipal GetTargetPrincipal(SecurityPrincipal sourcePrincipal, ISecurableObject source, ISecurableObject target)
        {
            SecurityPrincipal item = null;
            string str = null;
            if (base.PrincipalMappings.ContainsKey(sourcePrincipal.PrincipalName))
            {
                str = base.PrincipalMappings[sourcePrincipal.PrincipalName];
            }
            try
            {
                if (str != null)
                {
                    item = target.Principals[str];
                }
                else if (base.PrincipalMappings.Count == 0)
                {
                    item = target.Principals.MapSecurityPrincipal(sourcePrincipal);
                }
                if (item == null)
                {
                    LogItem logItem = new LogItem("Adding role assignments", sourcePrincipal.PrincipalName, source.DisplayUrl, target.DisplayUrl, ActionOperationStatus.Skipped)
                    {
                        SourceContent = sourcePrincipal.XML,
                        Information = "Could not find principal on target"
                    };
                    base.FireOperationStarted(logItem);
                    base.FireOperationFinished(logItem);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                LogItem logItem1 = new LogItem("Adding role assignments", sourcePrincipal.PrincipalName, source.DisplayUrl, target.DisplayUrl, ActionOperationStatus.Failed)
                {
                    Exception = exception,
                    SourceContent = sourcePrincipal.XML,
                    Information = string.Concat("Could not find principal on target: ", exception.Message),
                    Details = exception.StackTrace
                };
                base.FireOperationStarted(logItem1);
                base.FireOperationFinished(logItem1);
            }
            return item;
        }

        private Role GetTargetRole(Role sourceRole, ISecurableObject source, ISecurableObject target, Dictionary<string, Role> mappedRoles)
        {
            string roleName = null;
            if (base.SharePointOptions.OverrideRoleMappings)
            {
                roleName = base.SharePointOptions.RoleAssignmentMappings.Evaluate(sourceRole.RoleName, source, new CompareDatesInUtc());
            }
            if (base.SharePointOptions.MapRolesByName && roleName == null)
            {
                roleName = sourceRole.RoleName;
            }
            Role item = null;
            if (roleName != null)
            {
                item = target.Roles[roleName];
            }
            else if (mappedRoles != null && mappedRoles.ContainsKey(sourceRole.RoleName))
            {
                item = mappedRoles[sourceRole.RoleName];
            }
            else if (!(source is SPList) || !(target is SPList) || !(sourceRole is SPRole2003))
            {
                item = target.Roles.MapRole(sourceRole);
            }
            else
            {
                SPRole2007 sPRole2007 = CopyRoleAssignmentsAction.s_RoleConverter.Convert(sourceRole) as SPRole2007;
                float single = 0f;
                Role role = null;
                foreach (Role role1 in target.Roles)
                {
                    if (!(role1 is SPRole2007))
                    {
                        continue;
                    }
                    float listPermissionsSimilarity = this.GetListPermissionsSimilarity((SPRole2003)sourceRole, sPRole2007, (SPRole2007)role1);
                    if (listPermissionsSimilarity <= single)
                    {
                        continue;
                    }
                    single = listPermissionsSimilarity;
                    role = role1;
                }
                item = role;
            }
            return item;
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            ISecurableObject item = source[0] as ISecurableObject;
            this.CopyRoleAssignments(item, target[0] as ISecurableObject, true);
        }

        protected override void RunOperation(object[] oParams)
        {
            if (oParams == null || (int)oParams.Length < 2)
            {
                throw new Exception(string.Format("{0} is missing parameters", this.Name));
            }
            bool flag = false;
            if ((int)oParams.Length >= 3 && oParams[2] != null && oParams[2] is bool)
            {
                flag = (bool)oParams[2];
            }
            IUploadManager uploadManager = null;
            if ((int)oParams.Length >= 5)
            {
                uploadManager = oParams[3] as IUploadManager;
            }
            if (uploadManager == null)
            {
                this.CopyRoleAssignments(oParams[0] as ISecurableObject, oParams[1] as ISecurableObject, flag);
                return;
            }
            this.AddRoleAssignmentsToAzureManifest(oParams[0] as ISecurableObject, oParams[1] as ISecurableObject, flag, uploadManager, oParams[4] as BaseManifestItem);
        }

        public static string UpdateListItemRoleInheritance(SPListItem sourceListItem, SPListItem targetListItem)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
            {
                xmlWriter.WriteStartElement(XmlElementNames.UpdateListItemRoleInheritance.ToString());
                xmlWriter.WriteAttributeString(XmlAttributeNames.UniquePermissions.ToString(), sourceListItem.HasUniquePermissions.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }
            string str = targetListItem.Adapter.Writer.UpdateListItem(targetListItem.ParentList.ID, null, targetListItem.ID, stringBuilder.ToString(), null, null, new UpdateListItemOptions());
            return str;
        }
    }
}