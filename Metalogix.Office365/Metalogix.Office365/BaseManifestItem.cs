using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class BaseManifestItem
    {
        public int Author { get; set; }

        public string CheckinComment { get; set; }

        public string ContentTypeId { get; set; }

        public List<Field> FieldValues { get; protected set; }

        public bool HasUniquePermissions { get; set; }

        public bool HasVersioning { get; protected set; }

        public Guid ItemGuid { get; set; }

        public int ListItemIntegerId { get; set; }

        public string ModerationStatus { get; set; }

        public int ModifiedBy { get; set; }

        public ManifestObjectType ObjectType { get; protected set; }

        public string ParentFolderId { get; set; }

        public List<ManifestRoleAssignment> RoleAssignments { get; set; }

        public string TargetParentPath { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime TimeLastModified { get; set; }

        public string Version { get; set; }

        protected BaseManifestItem()
        {
            this.FieldValues = new List<Field>();
            this.RoleAssignments = new List<ManifestRoleAssignment>();
            this.HasVersioning = false;
            this.ObjectType = ManifestObjectType.Undefined;
        }
    }
}