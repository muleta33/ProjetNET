using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BackTestCouvertureOptions
{
    class PortfolioEstimator
    {
        private HedgingPortfolio _hedgingPortfolio;

        public HedgingPortfolio HedgingPortfolio
        {
            get { return _hedgingPortfolio; }
            set { _hedgingPortfolio = value; }
        }

        public PortfolioEstimator(HedgingPortfolio hedgingPortfolio)
        {
            HedgingPortfolio = hedgingPortfolio;
        }

        
    }
}
