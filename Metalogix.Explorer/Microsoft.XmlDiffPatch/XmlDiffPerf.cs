using System;
using System.Runtime.InteropServices;

namespace Microsoft.XmlDiffPatch
{
    [ComVisible(true)]
    public class XmlDiffPerf
    {
        public int _loadTime = 0;

        public int _hashValueComputeTime = 0;

        public int _identicalOrNoDiffWriterTime = 0;

        public int _matchTime = 0;

        public int _preprocessTime = 0;

        public int _treeDistanceTime = 0;

        public int _diffgramGenerationTime = 0;

        public int _diffgramSaveTime = 0;

        public int TotalTime
        {
            get
            {
                return this._loadTime + this._hashValueComputeTime + this._identicalOrNoDiffWriterTime +
                       this._matchTime + this._preprocessTime + this._treeDistanceTime + this._diffgramGenerationTime +
                       this._diffgramSaveTime;
            }
        }

        public XmlDiffPerf()
        {
        }

        public void Clean()
        {
            this._loadTime = 0;
            this._hashValueComputeTime = 0;
            this._identicalOrNoDiffWriterTime = 0;
            this._matchTime = 0;
            this._preprocessTime = 0;
            this._treeDistanceTime = 0;
            this._diffgramGenerationTime = 0;
            this._diffgramSaveTime = 0;
        }
    }
}