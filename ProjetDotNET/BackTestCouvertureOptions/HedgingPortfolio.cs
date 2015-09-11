using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.FinancialProducts;

namespace BackTestCouvertureOptions
{
    public class HedgingPortfolio
    {
        private System.Collections.Generic.Dictionary<string, double> _sharesQuantities;
        private double _riskFreeRateInvestment;
        private double _value;

        public System.Collections.Generic.Dictionary<string, double> SharesQuantities
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

        public HedgingPortfolio(System.Collections.Generic.Dictionary<string, double> sharesQuantities, double riskFreeRateInvestment)
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

        public void computeValue(System.Collections.Generic.Dictionary<String, decimal> sharesPrices, double rate)
        {
            double value = 0;
            foreach (KeyValuePair<string, double> shareQuantity in SharesQuantities)
            {
                value = value + (double)sharesPrices[shareQuantity.Key] * shareQuantity.Value;
            }
            Value = value + RiskFreeRateInvestment * rate;
        }

        // Fonctionne pour call avec un seul sous-jacent
        private void rebalancingPortfolio(double[] deltas, System.Collections.Generic.Dictionary<String, decimal> sharesPrices)
        {
            // ATTENTION, Avant de rebalancer il faut calculer la valeur du portefeuille

            int i = 0;
            double sumValue = 0;
            foreach (KeyValuePair<String, decimal> sharePrice in sharesPrices)
            {
                SharesQuantities[sharePrice.Key] = deltas[i];
                sumValue += SharesQuantities[sharePrice.Key] * (double)sharePrice.Value;
            }
            RiskFreeRateInvestment = Value - sumValue;


            //// VanillaCall
            //if (option is VanillaCall)
            //{
            //    PricingLibrary.Computations.PricingResults res = pricer.PriceCall((VanillaCall)option, atTime, 365, (double)sharesPrices[option.UnderlyingShareIds[0]], volatilities[0]);
            //    SharesQuantities[option.UnderlyingShareIds[0]] = res.Deltas[0];
            //    RiskFreeRateInvestment = Value - (SharesQuantities[option.UnderlyingShareIds[0]] * (double)sharesPrices[option.UnderlyingShareIds[0]]);
            //}
            //// BasketOPtion
            //else if (option is BasketOption)
            //{
            //    double[] spotPrices = new double[sharesPrices.Count];
            //    int i = 0;
            //    foreach (KeyValuePair<string, decimal> sharePrice in sharesPrices)
            //    {
            //        spotPrices[i] = (double)sharePrice.Value;
            //        ++i;
            //    }
            //    PricingLibrary.Computations.PricingResults res = pricer.PriceBasket((BasketOption)option, atTime, 365, spotPrices, volatilities, cholesky);
            //    double sumValue = 0;
            //    for (int j = 0; j < res.Deltas.Length; j++)
            //    {
            //        SharesQuantities[option.UnderlyingShareIds[j]] = res.Deltas[j];
            //        sumValue += SharesQuantities[option.UnderlyingShareIds[j]] * (double)sharesPrices[option.UnderlyingShareIds[j]];
            //    }
            //    RiskFreeRateInvestment = Value - sumValue;
            //}
        }

        public void update(System.Collections.Generic.Dictionary<String, decimal> sharesPrices, double[] deltas, double rate)
        {
            computeValue(sharesPrices, rate);
            rebalancingPortfolio(deltas, sharesPrices);
        }
    }
}
