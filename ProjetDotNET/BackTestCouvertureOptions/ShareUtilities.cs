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
using System.Runtime.InteropServices;

namespace BackTestCouvertureOptions
{
    public class ShareUtilities
    {
        // import WRE dll
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility")]

        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] shareReturns,
            ref double expostVolatility,
            ref int info
        );

        public static double computeVolatility(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime date, String id, int windowLength, int numberDaysPerYear)
        {
            double[] shareReturns = computeShareReturns(dataFeedList, date, id, windowLength);
            int nbValues = shareReturns.GetLength(0);
            double expostVolatility = 0;
            int info = 0;
            int res = WREanalysisExpostVolatility(ref nbValues, shareReturns, ref expostVolatility, ref info);
            expostVolatility = expostVolatility * Math.Sqrt(numberDaysPerYear);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREanalysisExpostVolatility encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREanalysisExpostVolatility encountred a problem. See info parameter for more details");
            }
            return expostVolatility;
        }

        public static double[] computeShareReturns(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime date, String id, int windowLength)
        {
            double[] shareReturns = new double[windowLength - 1];

            int beginDateIndex = dataFeedList.FindIndex(dataFeed => dataFeed.Date == date) - windowLength;

            decimal previousPrice = dataFeedList[beginDateIndex].PriceList[id];
            for (int index = 0; index < windowLength - 1; ++index)
            {
                decimal currentPrice = dataFeedList[beginDateIndex + 1 + index].PriceList[id];
                shareReturns[index] = Math.Log((double)(currentPrice / previousPrice));
                previousPrice = currentPrice;
            }
            return shareReturns;
        }
    }
}
