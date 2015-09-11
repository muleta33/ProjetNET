using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BackTestCouvertureOptions
{
    class BasketOptionUtilities
    {
        public static double[] computeVolatilities(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, 
                                                   System.DateTime date, int windowLength, int numberDaysPerYear)
        {
            double[] volatilities = new double[dataFeedList[0].PriceList.Count];
            int volatilitiesIndex = 0;
            foreach (KeyValuePair<string, decimal> shareSpot in dataFeedList[0].PriceList)
            {
                volatilities[volatilitiesIndex] = ShareUtilities.computeVolatility(dataFeedList, date, shareSpot.Key, windowLength, numberDaysPerYear);
                ++volatilitiesIndex;
            }
            return volatilities;
        }

        // import WRE dll
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov")]

        public static extern int WREmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );

        public static double[,] computeCovarianceMatrix(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int res;
            res = WREmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return covMatrix;
        }

        private static double[,] computeReturns(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime estimationDate, int windowLength)
        {
            double[,] returns = new double[windowLength - 1, dataFeedList[0].PriceList.Count];

            int shareIndex = 0;
            foreach (KeyValuePair<string, decimal> shareSpot in dataFeedList[0].PriceList)
            {
                double[] shareReturns = ShareUtilities.computeShareReturns(dataFeedList, estimationDate, shareSpot.Key, windowLength);
                for (int i = 0; i < shareReturns.Length; ++i)
                {
                    returns[i, shareIndex] = shareReturns[i];
                }
            }
            return returns;
        }

        private static double[,] computeCorrelationMatrix(double[,] covarianceMatrix)
        {
            double[,] correlationMatrix = new double[covarianceMatrix.GetLength(0), covarianceMatrix.GetLength(1)];
            for (int row = 0; row < covarianceMatrix.GetLength(0); ++row)
            {
                for (int column = 0; column < covarianceMatrix.GetLength(1); ++column)
                {
                    if (row == column)
                        correlationMatrix[row, column] = 1;
                    else
                        correlationMatrix[row, column] = covarianceMatrix[row, column] / Math.Sqrt(covarianceMatrix[row, row] * covarianceMatrix[column, column]);
                }
            }
            return correlationMatrix;
        }

        public static double[,] computeCholeskyCorrelation(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime estimationDate, int windowLength)
        {
            double[,] returns = computeReturns(dataFeedList, estimationDate, windowLength);
            double[,] covarianceMatrix = computeCovarianceMatrix(returns);
            double[,] correlationMatrix = computeCorrelationMatrix(covarianceMatrix);
            return PricingLibrary.Utilities.LinearAlgebra.Cholesky(covarianceMatrix);
        }
    }
}
