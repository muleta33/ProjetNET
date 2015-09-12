using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    // Objet permettant de connaitre la composition du portefeuille de couverture d'une option 
    public abstract class CompositionProvider
    {
        protected PricingLibrary.FinancialProducts.Option _option;

        public PricingLibrary.FinancialProducts.Option getOption()
        {
            return _option;
        }

        public abstract PricingLibrary.Computations.PricingResults getComposition(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, 
                                                                                  System.DateTime date, int windowLength, int numberDaysPerYear);
    }
}
