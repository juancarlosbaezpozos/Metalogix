using System;

namespace Metalogix.Metabase.Data
{
    public class FillFactorAttribute : Attribute
    {
        private double m_dFillFactor;

        public double FillFactor
        {
            get { return this.m_dFillFactor; }
            set { this.m_dFillFactor = value; }
        }

        public FillFactorLevel Level
        {
            get
            {
                if (this.m_dFillFactor == 100)
                {
                    return FillFactorLevel.Full;
                }

                if (this.m_dFillFactor == 0)
                {
                    return FillFactorLevel.Empty;
                }

                return FillFactorLevel.PartiallyFull;
            }
        }

        public FillFactorAttribute(double dFillFactor)
        {
            this.m_dFillFactor = dFillFactor;
        }

        public override string ToString()
        {
            int num = Convert.ToInt32(this.FillFactor);
            return string.Concat(num.ToString(), "%");
        }
    }
}