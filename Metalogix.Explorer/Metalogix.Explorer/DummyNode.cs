using Metalogix.Actions;
using Metalogix.DataStructures;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Metalogix.Explorer
{
    public class DummyNode : ExplorerNode
    {
        private List<Node> m_childNodes = new List<Node>();

        private string m_sServerRelativeUrl;

        private string m_sName;

        private string m_sUrl;

        private string m_sDisplayName;

        protected Color m_fontColor = Color.Empty;

        public override string DisplayName
        {
            get { return this.m_sDisplayName; }
        }

        public virtual Color FontColor
        {
            get { return this.m_fontColor; }
        }

        public override string Name
        {
            get { return this.m_sName; }
        }

        public override string ServerRelativeUrl
        {
            get { return this.m_sServerRelativeUrl; }
        }

        public override string Url
        {
            get { return this.m_sUrl; }
        }

        public override string XML
        {
            get { return "<SPNode />"; }
        }

        public DummyNode(Node parent) : base(parent)
        {
        }

        public void AddDummyChild(DummyNode node)
        {
            this.m_childNodes.Add(node);
            base.FireChildrenChanged();
        }

        protected override void ClearChildNodes()
        {
        }

        protected override Node[] FetchChildNodes()
        {
            return this.m_childNodes.ToArray();
        }

        public override bool IsEqual(Metalogix.DataStructures.IComparable comparableNode,
            DifferenceLog differencesOutput, ComparisonOptions options)
        {
            differencesOutput.Write("Dummy Nodes are not equal to any other node");
            return false;
        }

        public void SetColor(Color color)
        {
            this.m_fontColor = color;
        }

        public void SetDisplayName(string sName)
        {
            this.m_sDisplayName = sName;
        }

        public void SetImageName(string sImageName)
        {
            this.m_sImageName = sImageName;
        }

        public void SetName(string sName)
        {
            this.m_sName = sName;
        }

        public void SetServerRelativeUrl(string sUrl)
        {
            this.m_sServerRelativeUrl = sUrl;
        }

        public void SetUrl(string sUrl)
        {
            this.m_sUrl = sUrl;
        }
    }
}