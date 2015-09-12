﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    public class Utilities
    {
        // Calcule le taux sans risque proratisé
        public static double computeAccruedRiskFreeRate(DateTime currentDate, DateTime followingDate, int nbDaysPerYear, bool isSimulatedData)
        {
            int nbDays = 0;
            if (isSimulatedData)
                nbDays = (followingDate - currentDate).Days;
            else
                nbDays = PricingLibrary.Utilities.DayCount.CountBusinessDays(currentDate, followingDate);
            double dayDouble = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, nbDaysPerYear);
            return PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dayDouble);
        }

        // Renvoie les prix spots des actifs à une date donnée
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

        //// Renvoie la liste des sous-jacents d'une option s'ils existent
        //public System.Collections.Generic.List<PricingLibrary.FinancialProducts.Share> getUnderlyingShares(PricingLibrary.FinancialProducts.Option option)
        //{
        //    System.Collections.Generic.List<PricingLibrary.FinancialProducts.Share> underlyingShares = new System.Collections.Generic.List<PricingLibrary.FinancialProducts.Share>();
        //    System.Collections.Generic.Dictionary<String, String> UnderlyingSharesNames = new System.Collections.Generic.Dictionary<String, String>();
        //    using (DataBaseDataContext mtdc = new DataBaseDataContext())
        //    {
        //        UnderlyingSharesNames = (from s in mtdc.ShareNames where (option.UnderlyingShareIds.Contains(s.id)) select s).ToDictionary(s => s.name, s => s.id);
        //    }
        //    for (int index = 0; index < UnderlyingSharesNames.Count; index++)
        //    {
        //        var item = UnderlyingSharesNames.ElementAt(index);
        //        String itemKey = item.Key;
        //        String itemValue = item.Value.TrimEnd();
        //        PricingLibrary.FinancialProducts.Share share = new PricingLibrary.FinancialProducts.Share(itemKey, itemValue);
        //        underlyingShares[index] = share;
        //    }
        //    return underlyingShares;
        //}
    }
}
