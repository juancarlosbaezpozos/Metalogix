using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Remoting
{
    public class AgentDetails : Agent
    {
        public string AgentDBConnectionString { get; set; }

        public string CertificateLocation { get; set; }

        public string CertificatePassword { get; set; }

        public bool IsCertificateDeployed { get; set; }

        public string MetabaseDBConnectionString { get; set; }

        public AgentDetails()
        {
        }
    }
}