using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    abstract class CompositionProvider
    {
        private PricingLibrary.FinancialProducts.Option _option;

        public PricingLibrary.FinancialProducts.Option Option
        {
            get { return _option; }
            set { _option = value; }
        }

        public abstract PricingLibrary.Computations.PricingResults getComposition(System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> dataFeedList, 
                                                                                  System.DateTime date, int windowLength, int numberDaysPerYear);

        public System.Collections.Generic.List<PricingLibrary.FinancialProducts.Share> getUnderlyingShares()
        {
            System.Collections.Generic.List<PricingLibrary.FinancialProducts.Share> underlyingShares = new System.Collections.Generic.List<PricingLibrary.FinancialProducts.Share>();
            System.Collections.Generic.Dictionary<String, String> UnderlyingSharesNames = new System.Collections.Generic.Dictionary<String, String>();
            using (DataBaseDataContext mtdc = new DataBaseDataContext())
            {
                UnderlyingSharesNames = (from s in mtdc.ShareNames where (Option.UnderlyingShareIds.Contains(s.id)) select s).ToDictionary(s => s.name, s => s.id);
            }
            for (int index = 0; index < UnderlyingSharesNames.Count; index++)
            {
                var item = UnderlyingSharesNames.ElementAt(index);
                String itemKey = item.Key;
                String itemValue = item.Value.TrimEnd();
                PricingLibrary.FinancialProducts.Share share = new PricingLibrary.FinancialProducts.Share(itemKey, itemValue);
                underlyingShares[index] = share;
            }
            return underlyingShares;
        }

    }
}
