using System;

namespace Metalogix.Office365
{
    public interface ICommonGlobalManifestOperations
    {
        ManifestList ListManifest { get; set; }

        void AddDependencyFolder(string folderItemId, string parentFolderId, string folderXml);

        int AddGroup(ManifestGroup group);

        int AddRole(ManifestRole role);

        int AddUser(ManifestUser user);

        string GetAllGroupsXml();

        string GetAllRolesXml(string webTemplate);

        string GetAllUsersXml();

        DependencyFolder GetDependencyFolder(string folderItemId);

        int GetUserOrGroupIDByName(string principalName);

        int SetGroupOwner(int groupId, int owner);
    }
}