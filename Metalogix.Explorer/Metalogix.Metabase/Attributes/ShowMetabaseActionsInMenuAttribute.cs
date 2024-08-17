using System;

namespace Metalogix.Metabase.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class ShowMetabaseActionsInMenuAttribute : Attribute
    {
        private readonly bool _isShown;

        public bool IsShown
        {
            get { return this._isShown; }
        }

        public ShowMetabaseActionsInMenuAttribute(bool isShown)
        {
            this._isShown = isShown;
        }
    }
}