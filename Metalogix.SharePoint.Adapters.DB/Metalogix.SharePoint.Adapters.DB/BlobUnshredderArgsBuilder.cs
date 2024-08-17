using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.SharePoint.Adapters.DB
{
    public sealed class BlobUnshredderArgsBuilder
    {
        public int ChunkSize { get; set; }

        public string ConnectionString { get; set; }

        public Guid DocumentId { get; set; }

        public FileLevel Level { get; set; }

        public string SaveFileName { get; set; }

        public int UIVersion { get; set; }

        public BlobUnshredderArgsBuilder()
        {
            this.UIVersion = 0;
            this.Level = (FileLevel)0;
        }

        public string Build()
        {
            return (new StringBuilder()).AppendFormat("-c \"{0}\" ", this.ConnectionString)
                .AppendFormat("-d {0} ", this.DocumentId).AppendFormat("-v {0} ", this.UIVersion)
                .AppendFormat("-l {0} ", (int)this.Level).AppendFormat("-s {0} ", this.ChunkSize)
                .AppendFormat("-o \"{0}\"", this.SaveFileName).ToString();
        }
    }
}