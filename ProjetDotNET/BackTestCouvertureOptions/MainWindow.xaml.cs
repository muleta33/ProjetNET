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
            double riskFreeRate = 0;
            decimal sharePrice = 0;
            
            //SimulatedDataFeedProvider simulatedData = new SimulatedDataFeedProvider();
            HistoricalDataFeedProvider HistoricalData = new HistoricalDataFeedProvider("HistoricalData", 365);
            
            DateTime maturityDate = new DateTime(2014, 7, 23);
            DateTime initialDate = new DateTime(2013, 9, 18);
            //DateTime initialDate = simulatedData.GetMinDate();
            
            
            Share [] shareList= {new Share("ACCOR SA", "BNP FP")};
            VanillaCall vanillaCall = new VanillaCall("V1", shareList, maturityDate, 100);



           
            //List<DataFeed> dataFeedList = simulatedData.GetDataFeed(vanillaCall, initialDate);
            List<DataFeed> dataFeedList = HistoricalData.GetHistoricalDataFeed(vanillaCall, initialDate);

            ShareVolatility shareVolatility = new ShareVolatility(shareList[0].Id, 15, new DateTime(2013, 11, 29));
            double volatility = shareVolatility.computeVolatility(dataFeedList);

            PricingLibrary.Computations.PricingResults res = new PricingLibrary.Computations.PricingResults(0, new double[0]);
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            res = pricer.PriceCall(vanillaCall, initialDate, 365, 10, volatility);
            double delta = res.Deltas[0];
            System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double> sharesQuantities = new System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double>();
            sharesQuantities.Add(shareList[0], delta);
            double riskFreeRateInvestment = res.Price - delta * (double)dataFeedList[0].PriceList[shareList[0].Id];
            HedgingPortfolio portefolio = new HedgingPortfolio(sharesQuantities, riskFreeRateInvestment);

            
            for (int i=0; i < dataFeedList.Count() - 2; i++)
            {
                DateTime currentDate = dataFeedList[i].Date;
                DateTime followingDate = dataFeedList[i + 1].Date;
                int nbDays = PricingLibrary.Utilities.DayCount.CountBusinessDays(currentDate, followingDate);
                double timespan = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, 365);
                riskFreeRate = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(timespan);
                bool existValue = dataFeedList[i].PriceList.TryGetValue(shareList[0].Id, out sharePrice);
                System.Collections.Generic.Dictionary<String, double> sharesPrices = new System.Collections.Generic.Dictionary<String, double>();
                sharesPrices.Add(shareList[0].Id, (double)sharePrice);
                portefolio.update(vanillaCall, currentDate, sharesPrices, volatility, riskFreeRate);
                var Val = portefolio.Value;
            }
            if (dataFeedList[dataFeedList.Count() - 1].PriceList.TryGetValue(shareList[0].Id, out sharePrice))
            {
                DateTime currentDate = dataFeedList[dataFeedList.Count() - 2].Date;
                DateTime followingDate = dataFeedList[dataFeedList.Count() - 1].Date;
                int nbDays = PricingLibrary.Utilities.DayCount.CountBusinessDays(currentDate, followingDate);
                double timespan = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, 365);
                riskFreeRate = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(timespan);
                System.Collections.Generic.Dictionary<String, double> sharesPricesDictionary = new System.Collections.Generic.Dictionary<String, double>();
                sharesPricesDictionary.Add(shareList[0].Id, (double)sharePrice);
                portefolio.computeValue(sharesPricesDictionary, riskFreeRate);
            }

            double payoff = vanillaCall.GetPayoff(dataFeedList.Last().PriceList);
            Console.WriteLine(portefolio.Value);
            Console.WriteLine(payoff);
            Console.WriteLine(Math.Abs((portefolio.Value - payoff) / 10));


        }
    }
}
