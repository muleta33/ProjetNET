using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class Utilities
    {
        public static double computeAccruedRiskFreeRate(DateTime currentDate, DateTime followingDate, int nbDaysPerYear, bool areHistoricalData)
        {
            int nbDays = 0;
            if (areHistoricalData)
                nbDays = PricingLibrary.Utilities.DayCount.CountBusinessDays(currentDate, followingDate);
            else
                nbDays = (followingDate - currentDate).Days;
            double dayDouble = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, nbDaysPerYear);
            return PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dayDouble);
        }

        public static double[] shareSpots(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, System.DateTime date)
        {
            if (!dataFeedList.Any(dataFeed => dataFeed.Date == date))
            {
                throw new ParameterException("Not a business date.");
            }
            int index = dataFeedList.FindIndex(dataFeed => dataFeed.Date == date);
            double[] spots = new double[dataFeedList[index].PriceList.Count];
            int i = 0;
            foreach (KeyValuePair<string, decimal> spot in dataFeedList[index].PriceList)
            {
                spots[i] = (double)spot.Value;
                ++i;
            }
            return spots;
        }
    }
}
