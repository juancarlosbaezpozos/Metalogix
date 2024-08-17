using System;

namespace Metalogix.Permissions
{
    public interface ISecurableObject
    {
        string DisplayUrl { get; }

        SecurityPrincipalCollection Principals { get; }

        RoleAssignmentCollection RoleAssignments { get; }

        RoleCollection Roles { get; }

        void ReleasePermissionsData();
    }
}