using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.SharePoint.Adapters.DB
{
    public class ResourceLocalizerArgsBuilder
    {
        public uint LCID { get; set; }

        public string Resource { get; set; }

        public ResourceLocalizerArgsBuilder()
        {
            this.LCID = 1033;
            this.Resource = string.Empty;
        }

        public string Build()
        {
            return (new StringBuilder()).Append(string.Concat(this.LCID, " ")).AppendFormat("\"{0}\" ", this.Resource)
                .ToString();
        }
    }
}