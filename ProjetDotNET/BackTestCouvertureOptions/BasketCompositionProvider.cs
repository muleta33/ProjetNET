using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class BasketCompositionProvider : CompositionProvider
    {
        public BasketCompositionProvider(PricingLibrary.FinancialProducts.BasketOption basketOption)
        {
            Option = basketOption;
        }

        public override PricingLibrary.Computations.PricingResults getComposition(List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, DateTime date, int windowLength, int numberDaysPerYear)
        {
            double[] volatilities = BasketOptionUtilities.computeVolatilities(dataFeedList, date, windowLength, numberDaysPerYear);
            double[,] cholesky = BasketOptionUtilities.computeCholeskyCorrelation(dataFeedList, date, windowLength);

            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            double[] spotShareArray = Utilities.shareSpots(dataFeedList, date);
            // Eviter cast ???
            PricingLibrary.Computations.PricingResults pricingResults = pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)Option, date, numberDaysPerYear, spotShareArray, volatilities, cholesky);
            return pricingResults;
        }
    }
}
