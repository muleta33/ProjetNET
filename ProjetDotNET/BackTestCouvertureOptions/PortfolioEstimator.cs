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

        // import WRE dlls
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility")]
        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] portfolioReturns,
            ref double expostVolatility,
            ref int info
        );

        public double computeVolatility(double[] sharesReturns)
        {
            int nbValues = sharesReturns.GetLength(0);
            double expostVolatility = 0;
            int info = 0;
            int res = WREanalysisExpostVolatility(ref nbValues, sharesReturns, ref expostVolatility, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREanalysisExpostVolatility encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREanalysisExpostVolatility encountred a problem. See info parameter for more details");
            }
            return expostVolatility;
        }

        // Valable que pour une action
        public double[] computeSharesReturns(int nbDates, System.DateTime beginDate, System.DateTime endDate, System.Collections.Generic.Dictionary<System.DateTime, double> spotsAccordingToDates)
        {
            double[] sharesReturns = new double[nbDates];
            double previousValue = 0;
            int index = 0;
            foreach (KeyValuePair<System.DateTime, double> dateAndSpot in spotsAccordingToDates)
            {
                if (dateAndSpot.Key == beginDate)
                    previousValue = previousValue + dateAndSpot.Value;
                else if (dateAndSpot.Key > beginDate && dateAndSpot.Key <= endDate)
                {
                    sharesReturns[index] = Math.Log(dateAndSpot.Value / previousValue);
                    previousValue = dateAndSpot.Value;
                    ++index;
                }
            }
            return sharesReturns;
        }

        public double[] computeSharesReturns(int nbDates, System.DateTime beginDate, System.DateTime endDate, System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList)
        {
            double[] sharesReturns = new double[nbDates - 1];
            int index = 0;
            decimal previousValue = 0;

            // On suppose la liste ordonnée selon les dates
            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed dataFeed in dataFeedList)
            {
                if (dataFeed.Date == beginDate)
                {
                    foreach (KeyValuePair<String, decimal> shareSpot in dataFeed.PriceList)
                    {
                        previousValue = previousValue + shareSpot.Value;
                    }
                }
                else if (dataFeed.Date > beginDate && dataFeed.Date <= endDate)
                {
                    decimal currentValue = 0;
                    foreach (KeyValuePair<String, decimal> shareSpot in dataFeed.PriceList)
                    {
                        currentValue = currentValue + shareSpot.Value;
                    }
                    sharesReturns[index] = Math.Log((double)currentValue / (double)previousValue);
                    previousValue = currentValue;
                    ++index;
                }
            }
            return sharesReturns;
        }
    }
}
