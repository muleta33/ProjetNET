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
            try
            {
                // InitializeComponent();
                double riskFreeRate = 0;
                decimal sharePrice = 0;

                // Paramètres de l'étude
                DateTime maturityDate = new DateTime(2014, 7, 23);
                DateTime initialDate = new DateTime(2013, 9, 18);
                int window = 10;
                double strike = 25;
                Share[] shareList = { new Share("BNP", "BNP FP") };
                VanillaCall vanillaCall = new VanillaCall("V1", shareList, maturityDate, strike);


                // Récupération des données via le data provider
                HistoricalDataFeedProvider HistoricalData = new HistoricalDataFeedProvider("HistoricalData", 365);
                List<DataFeed> dataFeedList = HistoricalData.GetHistoricalDataFeed(vanillaCall, initialDate);
                //SimulatedDataFeedProvider simulatedData = new SimulatedDataFeedProvider();
                //List<DataFeed> dataFeedList = simulatedData.GetDataFeed(vanillaCall, initialDate);
                checkValidDate(HistoricalData.GetHistoricalMinDate(), initialDate, window);


                ShareVolatility shareVolatility = new ShareVolatility(shareList[0].Id, 15, new DateTime(2013, 11, 29));
                double volatility = shareVolatility.computeVolatility(dataFeedList);

                PricingLibrary.Computations.PricingResults res = new PricingLibrary.Computations.PricingResults(0, new double[0]);
                PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
                res = pricer.PriceCall(vanillaCall, initialDate, 365, 50, volatility);
                double delta = res.Deltas[0];
                System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double> sharesQuantities = new System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double>();
                sharesQuantities.Add(shareList[0], delta);
                double riskFreeRateInvestment = res.Price - delta * (double)dataFeedList[0].PriceList[shareList[0].Id];
                HedgingPortfolio portefolio = new HedgingPortfolio(sharesQuantities, riskFreeRateInvestment);


                for (int i = 0; i < dataFeedList.Count() - 2; i++)
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
            catch (ParameterException e)
            {
                Console.WriteLine(e.Message);
            }


        }

        private void checkValidDate(DateTime minDate, DateTime initialDate, int window)
        {
            TimeSpan ts = initialDate - minDate;
            int dif = ts.Days;
            if (dif <= 0)
            {
                throw new ParameterException("The initial date is lower than the first available data date");
            }
            dif = PricingLibrary.Utilities.DayCount.CountBusinessDays(minDate, initialDate);
            if (dif < window)
            {
                throw new ParameterException("The estimation window is too large");
            }
        }
    }
}
