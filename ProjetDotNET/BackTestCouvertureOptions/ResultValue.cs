using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    public class ResultValue
    {
        private double _payOff;
        private double _portfolioValue;
         public double PayOff
        {
            get { return _payOff; }
            set { _payOff = value; }
        }

         public double PortfolioValue
        {
            get { return _portfolioValue; }
            set { _portfolioValue = value; }
        }
        public ResultValue(double payOff , double portfolioValue )
        {
            PayOff = payOff;
            PortfolioValue = portfolioValue;
        }
    }
}
