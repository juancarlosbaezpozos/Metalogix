using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestGroup
    {
        public string Description { get; set; }

        public int GroupId { get; set; }

        public List<ManifestGroupMember> GroupMembers { get; set; }

        public string Name { get; set; }

        public bool OnlyAllowMembersViewMembership { get; set; }

        public int Owner { get; set; }

        public bool OwnerIsUser { get; set; }

        public string RequestToJoinLeaveEmailSetting { get; set; }

        public ManifestGroup()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.RequestToJoinLeaveEmailSetting = string.Empty;
            this.GroupMembers = new List<ManifestGroupMember>();
        }
    }
}