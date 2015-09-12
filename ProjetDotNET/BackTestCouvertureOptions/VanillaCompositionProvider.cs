using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    public class VanillaCompositionProvider : CompositionProvider
    {
        public VanillaCompositionProvider(PricingLibrary.FinancialProducts.Option vanillaCall)
        {
            _option = (PricingLibrary.FinancialProducts.VanillaCall)vanillaCall;
        }

        public override PricingLibrary.Computations.PricingResults getComposition(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, 
                                                                                  System.DateTime date, int windowLength, int numberDaysPerYear)
        {
            //double volatility = ShareUtilities.computeVolatility(dataFeedList, date, _vanillaCall.UnderlyingShareIds[0], windowLength, numberDaysPerYear);
            double volatility = 0.4;
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            double[] spotShareArray = Utilities.shareSpots(dataFeedList, date);
            PricingLibrary.Computations.PricingResults pricingResults = pricer.PriceCall((PricingLibrary.FinancialProducts.VanillaCall)_option, date, numberDaysPerYear, spotShareArray[0], volatility);
            return pricingResults;
        }
    }
}
