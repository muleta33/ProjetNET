using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    class VanillaCompositionProvider : CompositionProvider
    {
        private PricingLibrary.FinancialProducts.VanillaCall _vanillaCall;

        public VanillaCompositionProvider(PricingLibrary.FinancialProducts.VanillaCall vanillaCall)
        {
            _vanillaCall = vanillaCall;
        }

        public override PricingLibrary.Computations.PricingResults getComposition(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, 
                                                                                  System.DateTime date, int windowLength, int numberDaysPerYear)
        {
            double volatility = ShareUtilities.computeVolatility(dataFeedList, date, _vanillaCall.UnderlyingShareIds[0], windowLength, numberDaysPerYear);

            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            double[] spotShareArray = Utilities.shareSpots(dataFeedList, date);
            PricingLibrary.Computations.PricingResults pricingResults = pricer.PriceCall(_vanillaCall, date, numberDaysPerYear, spotShareArray[0], volatility);
            return pricingResults;
        }
    }
}
