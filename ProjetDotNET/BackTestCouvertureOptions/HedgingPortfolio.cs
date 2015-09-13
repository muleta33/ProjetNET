/***
 * Authors: Lachkar Fadoua
 *          Margot John-Elie
 *          Moussi Nermine
 *          Mulet Antoine
 */

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
        }

        public void update(System.Collections.Generic.Dictionary<String, decimal> sharesPrices, double[] deltas, double rate)
        {
            computeValue(sharesPrices, rate);
            rebalancingPortfolio(deltas, sharesPrices);
        }
    }
}
