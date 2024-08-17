using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestRole
    {
        public string Description { get; set; }

        public bool IsHidden { get; set; }

        public long PermMask { get; set; }

        public int RoleId { get; set; }

        public string RoleOrder { get; set; }

        public string RoleType { get; set; }

        public string Title { get; set; }

        public ManifestRole()
        {
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.RoleOrder = string.Empty;
            this.RoleType = string.Empty;
        }
    }
}