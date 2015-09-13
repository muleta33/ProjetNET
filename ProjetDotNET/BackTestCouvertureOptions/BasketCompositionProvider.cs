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
            double[] volatilities = BasketOptionUtilities.computeVolatilities(dataFeedList, date, windowLength, numberDaysPerYear);
            double[,] cholesky = BasketOptionUtilities.computeCholeskyCorrelation(dataFeedList, date, windowLength);

            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            double[] spotShareArray = Utilities.shareSpots(dataFeedList, date);
            PricingLibrary.Computations.PricingResults pricingResults = pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)_option, date, numberDaysPerYear, spotShareArray, volatilities, cholesky);
            return pricingResults;
        }
    }
}
