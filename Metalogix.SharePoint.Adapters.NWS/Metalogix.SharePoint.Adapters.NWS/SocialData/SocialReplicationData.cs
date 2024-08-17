using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService")]
    public class SocialReplicationData
    {
        private SocialTagDetail[] tagsField;

        private SocialCommentDetail[] commentsField;

        private SocialRatingDetail[] ratingsField;

        private DeletedSocialTagDetail[] deletedTagsField;

        private DeletedSocialCommentDetail[] deletedCommentsField;

        private DeletedSocialRatingDetail[] deletedRatingsField;

        public SocialCommentDetail[] Comments
        {
            get { return this.commentsField; }
            set { this.commentsField = value; }
        }

        public DeletedSocialCommentDetail[] DeletedComments
        {
            get { return this.deletedCommentsField; }
            set { this.deletedCommentsField = value; }
        }

        public DeletedSocialRatingDetail[] DeletedRatings
        {
            get { return this.deletedRatingsField; }
            set { this.deletedRatingsField = value; }
        }

        public DeletedSocialTagDetail[] DeletedTags
        {
            get { return this.deletedTagsField; }
            set { this.deletedTagsField = value; }
        }

        public SocialRatingDetail[] Ratings
        {
            get { return this.ratingsField; }
            set { this.ratingsField = value; }
        }

        public SocialTagDetail[] Tags
        {
            get { return this.tagsField; }
            set { this.tagsField = value; }
        }

        public SocialReplicationData()
        {
        }
    }
}