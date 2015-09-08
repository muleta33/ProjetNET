using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class Portfolio
    {
        private String _underlyingShareId;
        private double _underlyingShareQuantity;
        private double _riskFreeRateQuantity;

        public String UnderlyingShareId
        {
            get { return _underlyingShareId; }
            set { _underlyingShareId = value; }
        }

        public double UnderlyingShareQuantity
        {
            get { return _underlyingShareQuantity; }
            set { _underlyingShareQuantity = value; }
        }

        public double RiskFreeRateQuantity
        {
            get { return _riskFreeRateQuantity; }
            set { _riskFreeRateQuantity = value; }
        }

        public Portfolio()
        {
        }

        public double computeValue(double riskFreeRateAccruedValue)
        {
            return 0;
        }

        public void rebalancing()
        {

        }
    }
}
