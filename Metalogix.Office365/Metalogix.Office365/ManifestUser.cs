using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestUser
    {
        public string Email { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDomainGroup { get; set; }

        public bool IsSiteAdmin { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public string SystemId { get; set; }

        public int UserId { get; set; }

        public ManifestUser()
        {
            this.Name = string.Empty;
            this.Login = string.Empty;
            this.Email = string.Empty;
            this.SystemId = string.Empty;
        }
    }
}