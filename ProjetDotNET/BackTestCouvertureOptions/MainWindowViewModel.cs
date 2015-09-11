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
using System.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;

namespace BackTestCouvertureOptions
{
    public class MainWindowViewModel : BindableBase
    {
        private bool tickerStarted;
        private List<String> _property;
        private double _strike = 10;
        private string _maturity = "01/01/2013";
        private PlotModel _plotModel;
        private System.Collections.Generic.Dictionary<System.DateTime, ResultValue> _results = new System.Collections.Generic.Dictionary<System.DateTime, ResultValue>();

        public PlotModel PlotModel
        {
            get { return _plotModel; }

            set { SetProperty(ref _plotModel, value); }
        }

        private bool TickerStarted
        {
            get { return tickerStarted; }
            set
            {
                SetProperty(ref tickerStarted, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public System.Collections.Generic.Dictionary<System.DateTime, ResultValue> Results
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
            PlotModel = new PlotModel();
            SetUpModel();
            StartCommand = new DelegateCommand(StartTicker, CanStartTicker);           
        }

        private void SetUpModel()
        {
            PlotModel.LegendTitle = "Legend";
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.TopRight;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;

            DateTimeAxis dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy HH:mm") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            PlotModel.Axes.Add(dateAxis);
            LinearAxis valueAxis = new LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel.Axes.Add(valueAxis);
        }

        private void LoadData()
        {
            var lineSerie1 = new OxyPlot.Series.LineSeries
            {
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            var lineSerie2 = new OxyPlot.Series.LineSeries
            {
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            foreach (var data in Results)
            {
                lineSerie1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Key), data.Value.PortfolioValue));
                lineSerie2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Key), data.Value.PayOff));
            }
            PlotModel.Series.Add(lineSerie1);
            PlotModel.Series.Add(lineSerie2);
        }

        private bool CanStartTicker()
        {
            return !TickerStarted;
        }

        public HedgingPortfolio createPortfolio(Option option, PricingResults pricingResults, List<DataFeed> dataFeedList, DateTime date)
        {
            System.Collections.Generic.Dictionary<string, double> sharesQuantities = new System.Collections.Generic.Dictionary<string, double>();

            double[] shareSpots = Utilities.shareSpots(dataFeedList, date);
            double portfolioSharesValue = 0;
            for (int i = 0; i < pricingResults.Deltas.Length; i++)
            {
                sharesQuantities.Add(option.UnderlyingShareIds[i], pricingResults.Deltas[i]);
                portfolioSharesValue += pricingResults.Deltas[i] * shareSpots[i];
            }
            double riskFreeRateInvestment = pricingResults.Price - portfolioSharesValue;
            HedgingPortfolio portfolio = new HedgingPortfolio(sharesQuantities, riskFreeRateInvestment);
            return portfolio;
        }

        private void StartTicker()
        {
            //// ----------- Test Basket Option donnees simulees -------- 
            //// Recuperation des donnees
            //DateTime maturity = new DateTime(2014, 12, 20);
            //DateTime initialDate = new DateTime(2013, 1, 20);
            //DateTime estimationDate = new DateTime(2013, 1, 5);
            //int windowLength = 15;
            //double strike = 8;
            //Share[] shareList = { new Share("BNP Paribas", "BNP FP"),
            //                        new Share("ACCOR SA", "ALO FP") };
            //double[] weights = { 0.4, 0.6 };
            //BasketOption basketOption = new BasketOption("Basket Option", shareList, weights, maturity, strike);

            //IDataFeedProvider data = new SimulatedDataFeedProvider();
            //List<DataFeed> dataFeedList = data.GetDataFeed(basketOption, estimationDate);

            //// Creation du portefeuille
            //CompositionProvider compositionProvider = new BasketCompositionProvider(basketOption);
            //PricingResults pricingResults = compositionProvider.getComposition(dataFeedList, initialDate, windowLength, data.NumberOfDaysPerYear);
            //HedgingPortfolio portfolio = createPortfolio(basketOption, pricingResults, dataFeedList, initialDate);

            //// Rebalancement du portfeuille au cours du temps
            //double riskFreeRate = 0;
            //for (int i = windowLength; i < dataFeedList.Count() - 2; i++)
            //{
            //    // Calcul du taux sans risque proratisé
            //    riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[i].Date, dataFeedList[i + 1].Date, data.NumberOfDaysPerYear, false);

            //    // Rebalancement et actualisation de la valeur du portefeuille
            //    pricingResults = compositionProvider.getComposition(dataFeedList, dataFeedList[i].Date, windowLength, data.NumberOfDaysPerYear);
            //    portfolio.update(dataFeedList[i].PriceList, pricingResults.Deltas, riskFreeRate);
            //    ResultValue curentValue = new ResultValue(pricingResults.Price, portfolio.Value);
            //    Results.Add(dataFeedList[i].Date, curentValue);
            //}
            //// Calcul du taux sans risque proratisé
            //riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[dataFeedList.Count() - 2].Date, dataFeedList[dataFeedList.Count() - 1].Date, data.NumberOfDaysPerYear, false);
            //// Valeur finale du portefeuille
            //portfolio.computeValue(dataFeedList[dataFeedList.Count() - 1].PriceList, riskFreeRate);
            //ResultValue finalValue = new ResultValue(basketOption.GetPayoff(dataFeedList.Last().PriceList), portfolio.Value);
            //Results.Add(dataFeedList.Last().Date, finalValue);

            //// Calcul du PayOff
            //double payoff = basketOption.GetPayoff(dataFeedList.Last().PriceList);

            //Console.WriteLine(portfolio.Value);
            //Console.WriteLine(payoff);
            //Console.WriteLine(Math.Abs((portfolio.Value - payoff) / 10));

            //// Tracé du plot
            //PlotModel.InvalidatePlot(true);
            //LoadData();
            //PlotModel.InvalidatePlot(true);

            // ----------- Test Basket Option donnees historiques -------- 
            // Recuperation des donnees
            DateTime maturity = new DateTime(2014, 12, 20);
            DateTime initialDate = new DateTime(2013, 1, 20);
            DateTime estimationDate = new DateTime(2013, 1, 5);
            int windowLength = 15;
            double strike = 8;
            Share[] shareList = { new Share("BNP Paribas", "BNP FP"),
                                    new Share("ACCOR SA", "ALO FP") };
            double[] weights = { 0.4, 0.6 };
            BasketOption basketOption = new BasketOption("Basket Option", shareList, weights, maturity, strike);

            // Recuperation de la date de debut d'estimation (pour volatilite)
            // Check validDate doit être appelé avant
            System.Collections.Generic.List<DateTime> datesBeforeInitialDate = new System.Collections.Generic.List<DateTime>();
            using (DataBaseDataContext mtdc = new DataBaseDataContext())
            {
                datesBeforeInitialDate = (from historical in mtdc.HistoricalShareValues
                                          where (historical.date <= initialDate)
                                          select historical.date).Distinct().OrderByDescending(date => date).ToList();
            }
            DateTime estimationBeginDate = datesBeforeInitialDate[windowLength];

            IDataFeedProvider data = new SimulatedDataFeedProvider();
            List<DataFeed> dataFeedList = data.GetDataFeed(basketOption, estimationDate);

            // Creation du portefeuille
            CompositionProvider compositionProvider = new BasketCompositionProvider(basketOption);
            PricingResults pricingResults = compositionProvider.getComposition(dataFeedList, initialDate, windowLength, data.NumberOfDaysPerYear);
            HedgingPortfolio portfolio = createPortfolio(basketOption, pricingResults, dataFeedList, initialDate);

            // Rebalancement du portfeuille au cours du temps
            double riskFreeRate = 0;
            for (int i = windowLength; i < dataFeedList.Count() - 2; i++)
            {
                // Calcul du taux sans risque proratisé
                riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[i].Date, dataFeedList[i + 1].Date, data.NumberOfDaysPerYear, false);

                // Rebalancement et actualisation de la valeur du portefeuille
                pricingResults = compositionProvider.getComposition(dataFeedList, dataFeedList[i].Date, windowLength, data.NumberOfDaysPerYear);
                portfolio.update(dataFeedList[i].PriceList, pricingResults.Deltas, riskFreeRate);
                ResultValue curentValue = new ResultValue(pricingResults.Price, portfolio.Value);
                Results.Add(dataFeedList[i].Date, curentValue);
            }
            // Calcul du taux sans risque proratisé
            riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[dataFeedList.Count() - 2].Date, dataFeedList[dataFeedList.Count() - 1].Date, data.NumberOfDaysPerYear, false);
            // Valeur finale du portefeuille
            portfolio.computeValue(dataFeedList[dataFeedList.Count() - 1].PriceList, riskFreeRate);
            ResultValue finalValue = new ResultValue(basketOption.GetPayoff(dataFeedList.Last().PriceList), portfolio.Value);
            Results.Add(dataFeedList.Last().Date, finalValue);

            // Calcul du PayOff
            double payoff = basketOption.GetPayoff(dataFeedList.Last().PriceList);

            Console.WriteLine(portfolio.Value);
            Console.WriteLine(payoff);
            Console.WriteLine(Math.Abs((portfolio.Value - payoff) / 10));

            // Tracé du plot
            PlotModel.InvalidatePlot(true);
            LoadData();
            PlotModel.InvalidatePlot(true);
        }

    }
}
