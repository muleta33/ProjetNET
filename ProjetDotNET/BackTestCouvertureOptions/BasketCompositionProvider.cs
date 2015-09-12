using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    public class BasketCompositionProvider : CompositionProvider
    {
        public BasketCompositionProvider(PricingLibrary.FinancialProducts.Option basketOption)
        {
            _option = (PricingLibrary.FinancialProducts.BasketOption)basketOption;
        }

        public override PricingLibrary.Computations.PricingResults getComposition(List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime date, int windowLength, int numberDaysPerYear)
        {
            //double[] volatilities = BasketOptionUtilities.computeVolatilities(dataFeedList, date, windowLength, numberDaysPerYear);
            //double[,] cholesky = BasketOptionUtilities.computeCholeskyCorrelation(dataFeedList, date, windowLength);
            double[] volatilities = { 0.4, 0.4 };
            double[,] correlation = { { 1, 0.1 }, { 0.1, 1 } };
            double[,] cholesky = PricingLibrary.Utilities.LinearAlgebra.Cholesky(correlation);

            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            double[] spotShareArray = Utilities.shareSpots(dataFeedList, date);
            PricingLibrary.Computations.PricingResults pricingResults = pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)_option, date, numberDaysPerYear, spotShareArray, volatilities, cholesky);
            return pricingResults;
        }
    }
}
