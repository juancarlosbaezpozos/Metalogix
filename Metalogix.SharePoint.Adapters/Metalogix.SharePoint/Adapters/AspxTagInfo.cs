using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public class AspxTagInfo
    {
        public int EndIndex { get; set; }

        public string Name { get; set; }

        public int StartIndex { get; set; }

        public AspxTagType Type { get; set; }

        public AspxTagInfo()
        {
        }
    }
}