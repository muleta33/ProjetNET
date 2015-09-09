using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class HedgingPortfolio
    {
        private System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double> _sharesQuantities;
        private double _riskFreeRateInvestment;
        private double _value;

        public System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double> SharesQuantities
        {
            get { return _sharesQuantities; }
            set { _sharesQuantities = value; }
        }

        public double RiskFreeRateInvestment
        {
            get { return _riskFreeRateInvestment; }
            set { _riskFreeRateInvestment = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public HedgingPortfolio(System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double> sharesQuantities, double riskFreeRateInvestment)
        {
            SharesQuantities = sharesQuantities;
            RiskFreeRateInvestment = riskFreeRateInvestment;
            //UnderlyingShare = underlyingShare;
            //// Initialisation du prix du portefeuille
            //PricingLibrary.Computations.PricingResults res = new PricingLibrary.Computations.PricingResults(0, new double[0]);
            //PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            //res = pricer.PriceCall(call, initialDate, 365, underlyingSharePrice, volatility);
            //PortfolioValue = res.Price;
            //// Constitution du portefuille
            //rebalancing(call, initialDate, underlyingSharePrice, volatility);
            
        }

        public void computeValue(System.Collections.Generic.Dictionary<String, double> sharesPrices, double rate)
        {
            double value = 0;
            foreach (KeyValuePair<PricingLibrary.FinancialProducts.Share, double> shareQuantity in SharesQuantities)
            {
                value = value + sharesPrices[shareQuantity.Key.Id] * shareQuantity.Value;
            }
            Value = value + RiskFreeRateInvestment * rate;
        }

        // Fonctionne pour call avec un seul sous-jacent
        private void rebalancing(PricingLibrary.FinancialProducts.VanillaCall call, System.DateTime atTime, System.Collections.Generic.Dictionary<String, double> sharesPrices, double volatility)
        {
            // ATTENTION, Avant de rebalancer il faut calculer la valeur du portefeuille

            // Rebalancement
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            PricingLibrary.Computations.PricingResults res = pricer.PriceCall(call, atTime, 365, sharesPrices[call.UnderlyingShare.Id], volatility);
            SharesQuantities[call.UnderlyingShare] = res.Deltas[0];
            RiskFreeRateInvestment = Value - (SharesQuantities[call.UnderlyingShare] * sharesPrices[call.UnderlyingShare.Id]);
        }

        public void update(PricingLibrary.FinancialProducts.VanillaCall call, System.DateTime atTime, System.Collections.Generic.Dictionary<String, double> sharesPrices, double volatility, double rate)
        {
            computeValue(sharesPrices, rate);
            rebalancing(call, atTime, sharesPrices, volatility);
        }
    }
}
