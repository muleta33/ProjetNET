using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BackTestCouvertureOptions
{
    class BasketOptionCholeskyCorrelation
    {
        private System.Collections.Generic.List<String> _ids;
        private int _windowLength;
        private double[,] _choleskyCorrelation;

        public System.Collections.Generic.List<String> Ids
        {
            get { return _ids; }
            set { _ids = value; }
        }

        public int WindowLength
        {
            get { return _windowLength; }
            set { _windowLength = value; }
        }

        public BasketOptionCholeskyCorrelation(System.Collections.Generic.List<String> ids, int windowLength)
        {
            Ids = ids;
            WindowLength = windowLength;
            CholeskyCorrelation = new double[Ids.Count, Ids.Count];
        }

        public double[,] CholeskyCorrelation
        {
            get { return _choleskyCorrelation; }
            set { _choleskyCorrelation = value; }
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

        public double[,] computeReturns(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime estimationDate)
        {
            double[,] returns = new double[WindowLength - 1, Ids.Count];
            for (int shareIndex = 0; shareIndex < Ids.Count; ++shareIndex)
            {
                ShareParameters shareVolatility = new ShareParameters(Ids[shareIndex], WindowLength);
                double[] shareReturns = shareVolatility.computeShareReturns(dataFeedList, estimationDate);
                for (int i = 0; i < shareReturns.Length; ++i)
                {
                    returns[i, shareIndex] = shareReturns[i];
                }
            }
            return returns;
        }

        public double[,] computeCorrelationMatrix(double[,] covarianceMatrix)
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

        public double[,] computeCholeskyCorrelation(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime estimationDate)
        {
            double[,] returns = computeReturns(dataFeedList, estimationDate);
            double[,] covarianceMatrix = computeCovarianceMatrix(returns);
            double[,] correlationMatrix = computeCorrelationMatrix(covarianceMatrix);
            CholeskyCorrelation = PricingLibrary.Utilities.LinearAlgebra.Cholesky(covarianceMatrix);
            return CholeskyCorrelation;
        }

    }
}
