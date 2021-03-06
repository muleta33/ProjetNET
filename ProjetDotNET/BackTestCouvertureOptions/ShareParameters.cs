﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BackTestCouvertureOptions
{
    class ShareParameters
    {
        private String _id;
        private int _windowLength;
        private double _volatility;

        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int WindowLength
        {
            get { return _windowLength; }
            set { _windowLength = value; }
        }

        public double Volatility
        {
            get { return _volatility; }
            set { _volatility = value; }
        }

        public ShareParameters(String id, int windowLength)
        {
            Id = id;
            WindowLength = windowLength;
            Volatility = 0;
        }

        // import WRE dll
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility")]

        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] shareReturns,
            ref double expostVolatility,
            ref int info
        );

        public double computeVolatility(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime date)
        {
            double[] shareReturns = computeShareReturns(dataFeedList, date);
            int nbValues = shareReturns.GetLength(0);
            double expostVolatility = 0;
            int info = 0;
            int res = WREanalysisExpostVolatility(ref nbValues, shareReturns, ref expostVolatility, ref info);
            expostVolatility = expostVolatility * Math.Sqrt(365);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREanalysisExpostVolatility encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREanalysisExpostVolatility encountred a problem. See info parameter for more details");
            }
            Volatility = expostVolatility;
            return expostVolatility;
        }

        public double[] computeShareReturns(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime date)
        {
            double[] shareReturns = new double[WindowLength - 1];

            int beginDateIndex = dataFeedList.FindIndex(dataFeed => dataFeed.Date == date) - WindowLength;

            decimal previousPrice = dataFeedList[beginDateIndex].PriceList[Id];
            for (int index = 0; index < WindowLength - 1; ++index)
            {
                decimal currentPrice = dataFeedList[beginDateIndex + 1 + index].PriceList[Id];
                shareReturns[index] = Math.Log((double)(currentPrice / previousPrice));
                previousPrice = currentPrice;
            }

            return shareReturns;
        }
    }
}
