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
using PricingLibrary.Utilities.MarketDataFeed;

namespace BackTestCouvertureOptions
{
    public class HistoricalDataFeedProvider : IDataFeedProvider
    {
        public String _name;
        public int _numberOfDaysPerYear;
        
        public String Name
        {
            get { return _name; }
            set { _name = value;}
        }

        public int NumberOfDaysPerYear 
        {
            get { return _numberOfDaysPerYear; }
            set { _numberOfDaysPerYear = value;}
        }

        public HistoricalDataFeedProvider(String dataName, int dataNumberOfDaysPerYear)
        {
            Name = dataName;
            NumberOfDaysPerYear = dataNumberOfDaysPerYear;
        }

        public System.Collections.Generic.List<DataFeed>  GetDataFeed(PricingLibrary.FinancialProducts.IOption option, System.DateTime fromDate){
            System.Collections.Generic.List<DataFeed> result = new System.Collections.Generic.List<DataFeed>() ;
            using (DataBaseDataContext mtdc = new DataBaseDataContext())
            {
                var result1 = (from s in mtdc.HistoricalShareValues where ((option.UnderlyingShareIds.Contains(s.id)) && (s.date >= fromDate)&&(s.date<=option.Maturity)) select s).OrderByDescending(d => d.date).ToList();
                System.DateTime curentdate = result1[result1.Count() - 1].date;
                System.Collections.Generic.Dictionary<String, decimal> priceList = new System.Collections.Generic.Dictionary<String, decimal>();
                for (int i = result1.Count() - 1; i >= 0 ; i--)
                {
                    if (result1[i].date==curentdate)
                    {
                        priceList.Add(result1[i].id.Trim(), result1[i].value);
                    }
                    else
                    {
                        DataFeed datafeed = new DataFeed(curentdate, priceList);
                        result.Add(datafeed);     
                        curentdate = result1[i].date;
                        priceList = new System.Collections.Generic.Dictionary<String, decimal>();
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
     
        public System.DateTime GetMinDate()
        {
            System.DateTime DateReturn = new System.DateTime();
            using (DataBaseDataContext mtdc = new DataBaseDataContext())
            {
                var result1 = (from s in mtdc.HistoricalShareValues select s).ToList();
                DateReturn = result1.First().date;
            }
            return DateReturn;
        }
    }
}
