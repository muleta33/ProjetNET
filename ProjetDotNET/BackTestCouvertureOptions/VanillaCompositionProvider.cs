using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class VanillaCompositionProvider : CompositionProvider
    {
        public VanillaCompositionProvider(PricingLibrary.FinancialProducts.VanillaCall vanillaCall)
        {
            Option = vanillaCall;
        }

        public override PricingLibrary.Computations.PricingResults getComposition(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, 
                                                                                  System.DateTime date, int windowLength, int numberDaysPerYear)
        {
            double volatility = ShareUtilities.computeVolatility(dataFeedList, date, Option.UnderlyingShareIds[0], windowLength, numberDaysPerYear);

            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            double[] spotShareArray = Utilities.shareSpots(dataFeedList, date);
            // Eviter cast ???
            PricingLibrary.Computations.PricingResults pricingResults = pricer.PriceCall((PricingLibrary.FinancialProducts.VanillaCall)Option, date, numberDaysPerYear, spotShareArray[0], volatility);
            return pricingResults;
        }
    }
}
