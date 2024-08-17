using System;

namespace Metalogix.SharePoint.Adapters
{
    public struct SYSTEM_INFO
    {
        private InternalSYSTEM_INFO m_ProcessorInformation;

        private int m_dwPageSize;

        private int m_lpMinimumApplicationAddress;

        private int m_lpMaximumApplicationAddress;

        private int m_dwActiveProcessorMask;

        private int m_dwNumberOfProcessors;

        private int m_dwProcessorType;

        private int m_dwAllocationGranularity;

        private int m_dwProcessorLevel;

        private int m_dwProcessorRevision;

        public int dwActiveProcessorMask
        {
            get { return this.m_dwActiveProcessorMask; }
            set { this.m_dwActiveProcessorMask = value; }
        }

        public int dwAllocationGranularity
        {
            get { return this.m_dwAllocationGranularity; }
            set { this.m_dwAllocationGranularity = value; }
        }

        public int dwNumberOfProcessors
        {
            get { return this.m_dwNumberOfProcessors; }
            set { this.m_dwNumberOfProcessors = value; }
        }

        public int dwPageSize
        {
            get { return this.m_dwPageSize; }
            set { this.m_dwPageSize = value; }
        }

        public int dwProcessorLevel
        {
            get { return this.m_dwProcessorLevel; }
            set { this.m_dwProcessorLevel = value; }
        }

        public int dwProcessorRevision
        {
            get { return this.m_dwProcessorRevision; }
            set { this.m_dwProcessorRevision = value; }
        }

        public int dwProcessorType
        {
            get { return this.m_dwProcessorType; }
            set { this.m_dwProcessorType = value; }
        }

        public int lpMaximumApplicationAddress
        {
            get { return this.m_lpMaximumApplicationAddress; }
            set { this.m_lpMaximumApplicationAddress = value; }
        }

        public int lpMinimumApplicationAddress
        {
            get { return this.m_lpMinimumApplicationAddress; }
            set { this.m_lpMinimumApplicationAddress = value; }
        }

        public InternalSYSTEM_INFO ProcessorInformation
        {
            get { return this.m_ProcessorInformation; }
            set { this.m_ProcessorInformation = value; }
        }
    }
}