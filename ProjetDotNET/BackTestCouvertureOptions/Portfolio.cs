using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class Portfolio
    {
        private PricingLibrary.FinancialProducts.Share _underlyingShare;
        private double _underlyingShareQuantity;
        private double _riskFreeRateInvestment;
        private double _portfolioValue;

        public PricingLibrary.FinancialProducts.Share UnderlyingShare
        {
            get { return _underlyingShare; }
            set { _underlyingShare = value; }
        }

        public double UnderlyingShareQuantity
        {
            get { return _underlyingShareQuantity; }
            set { _underlyingShareQuantity = value; }
        }

        public double RiskFreeRateInvestment
        {
            get { return _riskFreeRateInvestment; }
            set { _riskFreeRateInvestment = value; }
        }

        public double PortfolioValue
        {
            get { return _portfolioValue; }
            set { _portfolioValue = value; }
        }

        public Portfolio(PricingLibrary.FinancialProducts.Share underlyingShare, PricingLibrary.FinancialProducts.VanillaCall call, DateTime initialDate, double underlyingSharePrice, double volatility)
        {
            UnderlyingShare = underlyingShare;
            // Initialisation du prix du portefeuille
            PricingLibrary.Computations.PricingResults res = new PricingLibrary.Computations.PricingResults(0, new double[0]);
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            res = pricer.PriceCall(call, initialDate, 365, underlyingSharePrice, volatility);
            PortfolioValue = res.Price;
            // Constitution du portefuille
            rebalancing(call, initialDate, underlyingSharePrice, volatility);
            
        }

        public void rebalancing(PricingLibrary.FinancialProducts.VanillaCall call, System.DateTime atTime, double underlyingSharePrice, double volatility)
        {
            // ATTENTION, Avant de rebalancer il faut calculer la valeur du portefeuille

            // Rebalancement
            PricingLibrary.Computations.PricingResults res = new PricingLibrary.Computations.PricingResults(0, new double [0]);
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            res = pricer.PriceCall(call, atTime, 365, underlyingSharePrice, volatility);
            UnderlyingShareQuantity = res.Deltas[0];
            RiskFreeRateInvestment = PortfolioValue - (UnderlyingShareQuantity * underlyingSharePrice);
        }

        public double computePortfolioValue(double underlyingSharePrice, double rate)
        {
            // Calcul de la valeur du portefeuille
            PortfolioValue = UnderlyingShareQuantity * underlyingSharePrice + RiskFreeRateInvestment * rate;
            return PortfolioValue;
        }
    }
}
