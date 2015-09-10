using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Utilities.MarketDataFeed;

namespace BackTestCouvertureOptions
{
    class HistoricalDataFeedProvider
    {
        public String _Name;
        public int _NumberOfDaysPerYear;
        public String Name
            {
            get { return _Name; }
            set { _Name = value;}
            }
        public int NumberOfDaysPerYear 
            {
            get { return _NumberOfDaysPerYear; }
            set { _NumberOfDaysPerYear = value;}
            }

        public HistoricalDataFeedProvider(String DName, int DNumberOfDaysPerYear)
        {
            Name = DName;
            NumberOfDaysPerYear = DNumberOfDaysPerYear;
        }
       public System.Collections.Generic.List<DataFeed>  GetHistoricalDataFeed(PricingLibrary.FinancialProducts.IOption option, System.DateTime fromDate){
           System.Collections.Generic.List<DataFeed> result = new System.Collections.Generic.List<DataFeed>() ;
           using (DataClasses1DataContext mtdc = new DataClasses1DataContext())
           {
               var result1 = (from s in mtdc.HistoricalShareValues where ((option.UnderlyingShareIds.Contains(s.id)) && (s.date >= fromDate)&&(s.date<=option.Maturity)) select s).OrderByDescending(d => d.date).ToList();
               System.DateTime curentdate = result1[result1.Count() - 1].date;
               Dictionary<String, decimal> priceList = new Dictionary<String, decimal>();
               for (int i = result1.Count() - 1; i >= 0 ; i--)
               {
                   if (result1[i].date==curentdate){
                       priceList.Add(result1[i].id.Trim(), result1[i].value);
                   }
                   else
                   {
                       DataFeed datafeed = new DataFeed(curentdate, priceList);
                       result.Add(datafeed);
                       priceList.Clear();
                       curentdate = result1[i].date;
                       priceList.Add(result1[i].id.Trim(), result1[i].value);
                   }
                   if (i == 0)
                   {
                       DataFeed datafeedOut = new DataFeed(curentdate, priceList);
                       result.Add(datafeedOut);
                      
                   }
               }
               return result;
                  
               }
      }
     
       public System.DateTime GetHistoricalMinDate()
       {
           System.DateTime DateReturn = new System.DateTime();
           using (DataClasses1DataContext mtdc = new DataClasses1DataContext())
           {
               var result1 = (from s in mtdc.HistoricalShareValues select s).ToList();
               DateReturn = result1.First().date;
           }
           return DateReturn;
       }
    }
}
