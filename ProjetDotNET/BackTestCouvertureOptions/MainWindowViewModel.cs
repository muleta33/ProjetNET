using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using System.Windows.Documents;

namespace BackTestCouvertureOptions
{
    public class MainWindowViewModel : BindableBase
    {
        private bool tickerStarted;
        private List<String> _property;
        private string _results;
        private double _strike = 10;
        private string _maturity = "01/01/2013";

        private bool TickerStarted
        {
            get { return tickerStarted; }
            set
            {
                SetProperty(ref tickerStarted, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public string Results
        {
            get { return _results; }
            set {SetProperty(ref _results, value);} 
        }

        public double Strike
        {
            get { return _strike; }
            set { _strike = value; } 
        }

        public string Maturity
        {
            get { return _maturity; }
            set { _maturity = value; } 
        }

        public List<String> Property
        {
            get { return new List<string>(){"ACCOR SA", "AIR LIQUIDE SA",
                                            "AIRBUS GROUP SE", "ALSTOM",
                                            "AXA SA", "BNP PARIBAS",
                                            "BOUYGUES SA", "CAP GEMINI",
                                            "CARREFOUR SA", "CREDIT AGRICOLE SA",
                                            "DANONE", "EDF",
                                            "ESSILOR INTERNATIONAL",
                                            "SOCIETE GENERALE SA"}; }
            set { _property = value; }
        }


        public DelegateCommand StartCommand { get; private set; }

        public MainWindowViewModel()
        {
            StartCommand = new DelegateCommand(StartTicker, CanStartTicker);           
        }

        private bool CanStartTicker()
        {
            return !TickerStarted;
        }

        private void StartTicker()
        {
            double riskFreeRate = 0;
            decimal sharePrice = 0;

            SimulatedDataFeedProvider simulatedData = new SimulatedDataFeedProvider();
            DateTime maturityDate = Convert.ToDateTime(Maturity);
            DateTime initialDate = simulatedData.GetMinDate();
            Share [] shareList= {new Share("ALO FP", "ACCOR SA")};
            VanillaCall vanillaCall = new VanillaCall("V1", shareList, maturityDate, Strike);
            List<DataFeed> dataFeedList = simulatedData.GetDataFeed(vanillaCall, initialDate);

            PricingLibrary.Computations.PricingResults res = new PricingLibrary.Computations.PricingResults(0, new double[0]);
            PricingLibrary.Computations.Pricer pricer = new PricingLibrary.Computations.Pricer();
            res = pricer.PriceCall(vanillaCall, initialDate, 365, 10, 0.4);
            double delta = res.Deltas[0];
            System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double> sharesQuantities = new System.Collections.Generic.Dictionary<PricingLibrary.FinancialProducts.Share, double>();
            sharesQuantities.Add(shareList[0], delta);
            double riskFreeRateInvestment = res.Price - delta * (double)dataFeedList[0].PriceList[shareList[0].Id];
            HedgingPortfolio portefolio = new HedgingPortfolio(sharesQuantities, riskFreeRateInvestment);

            double volatility = 0.4;
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
            double gap = (portefolio.Value - payoff) / Strike;
            Results = Convert.ToString(portefolio.Value) + "\n" + Convert.ToString(payoff) + "\n" + Convert.ToString(gap);
        }

    }
}
