using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.FinancialProducts;
using System.Collections;
using PricingLibrary.Computations;


namespace BackTestCouvertureOptions
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {
           // InitializeComponent();
            //Dictionary<string, Decimal> e =  new Dictionary<string,decimal>();

            
            //using (DataClasses1DataContext mtdc = new DataClasses1DataContext())
            //{
                //var req = mtdc.ShareNames.Select(el => el.name).Distinct().ToList(); 
              //  DateTime Date = new DateTime(2010, 1, 1);
              // e =  (from s in mtdc.HistoricalShareValues  where (s.date==Date) select s).ToDictionary(s=>s.id, s=>s.value);
              // DataFeed d = new DataFeed(Date, e);

                //var l= req.;
           // SimulatedDataFeedProvider Test = new SimulatedDataFeedProvider();
            //DateTime Date = new DateTime(2015,3,2);
            //Test = GetDataFeed(req, Date);
            // var req = mtdc.ShareNames.Select(el => el.id).Distinct().ToList(); 
               // req2.UnderlyingShareIds = req;
            //SimulatedDataFeedProvider Test = new SimulatedDataFeedProvider();

            double PorteFolioValue, payoff;
            double RiskFreeRate = 0;
            decimal d = 0;
            SimulatedDataFeedProvider Data = new SimulatedDataFeedProvider();
            DateTime DateMaturity = new DateTime(2013, 1, 1);
            DateTime DateFrom = Data.GetMinDate();
            var listeDataFeed = new List<DataFeed>();
            Share [] Shareliste= {new Share("ALO FP", "ACCOR SA")};
            VanillaCall vanillaCall = new VanillaCall("V1", Shareliste, DateMaturity, 8);
            listeDataFeed = Data.GetDataFeed(vanillaCall, DateFrom);
            payoff = vanillaCall.GetPayoff(listeDataFeed.Last().PriceList);
            Pricer P = new Pricer();
            Portfolio portefolio = new Portfolio(Shareliste[0], vanillaCall, DateFrom, 10, 0.4);
            for (int i=0; i < listeDataFeed.Count()-1; i++)
            {
            double vol = 0.4;
            DateTime DayStart = listeDataFeed[i].Date;
            DateTime DayEnd = listeDataFeed[i+1].Date;
            int nbDays = PricingLibrary.Utilities.DayCount.CountBusinessDays(DayStart, DayEnd);
            double pro = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays,365);
            RiskFreeRate = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(pro);
            bool ExistValue = listeDataFeed[i].PriceList.TryGetValue(Shareliste[0].Id, out d);
            PorteFolioValue = portefolio.computePortfolioValue((double)d, RiskFreeRate);
            portefolio.rebalancing(vanillaCall, DayStart, (double)d, vol);
            }
           PorteFolioValue = portefolio.computePortfolioValue((double)d, RiskFreeRate);
           //Console.WriteLine(valeur);
           //Console.WriteLine(payoff);
           //Console.WriteLine((valeur - payoff) / 10);
        }
    }
}
