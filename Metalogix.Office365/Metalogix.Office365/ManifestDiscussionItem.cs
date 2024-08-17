using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestDiscussionItem : ManifestListItem
    {
        public string ThreadIndex { get; set; }

        public ManifestDiscussionItem(bool hasVersioning) : base(hasVersioning)
        {
            base.ObjectType = ManifestObjectType.DiscussionItem;
        }
    }
}