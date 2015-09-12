using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    public class ResultValue
    {
        private double _optionValue;
        private double _portfolioValue;
         public double OptionValue
        {
            get { return _optionValue; }
            set { _optionValue = value; }
        }

         public double PortfolioValue
        {
            get { return _portfolioValue; }
            set { _portfolioValue = value; }
        }
        public ResultValue(double payOff , double portfolioValue )
        {
            OptionValue = payOff;
            PortfolioValue = portfolioValue;
        }
    }
}
