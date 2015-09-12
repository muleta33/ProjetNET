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
        private PlotModel _plotModel;
        private string _selectedOption;
        private string _optionDescription;
        private bool _simulated;

        private bool TickerStarted
        {
            get { return tickerStarted; }
            set
            {
                SetProperty(ref tickerStarted, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { SetProperty(ref _plotModel, value); }
        }

        public string SelectedOption
        {
            get { return _selectedOption; }
            set { SetProperty(ref _selectedOption, value); }
        }

        public List<String> OptionTitles
        {
            get { return new List<string>(){"Vanilla Call", "Basket Option"}; }
        }

        public string OptionDescription
        {
            get { return _optionDescription; }
            set { SetProperty(ref _optionDescription, value); }
        }

        public bool Simulated
        {
            get { return _simulated; }
            set { SetProperty(ref _simulated, value); }
        }

        public DelegateCommand StartCommand { get; private set; }

        public MainWindowViewModel()
        {
            StartCommand = new DelegateCommand(StartTicker, CanStartTicker);           
        }

        // Intialisation du graphique
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

        // Charge les résultats dans le graphique
        private void LoadData(Dictionary<DateTime,ResultValue> plotResults, double payOff)
        {
            var lineSerie1 = new OxyPlot.Series.LineSeries
            {
                Title = "Portfolio Value",
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            var lineSerie2 = new OxyPlot.Series.LineSeries
            {
                Title = "Option Value",
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            var lineSerie3 = new OxyPlot.Series.LineSeries
            {
                Title = "PayOff",
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };

            foreach (var data in plotResults)
            {
                lineSerie1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Key), data.Value.PortfolioValue));
                lineSerie2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Key), data.Value.OptionValue));
                lineSerie3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Key), payOff));
            }
            PlotModel.Series.Add(lineSerie1);
            PlotModel.Series.Add(lineSerie2);
            PlotModel.Series.Add(lineSerie3);
        }

        // Trace le graphique
        private void DrawPlot(Dictionary<DateTime, ResultValue> plotResults, double payOff)
        {
            PlotModel.InvalidatePlot(true);
            LoadData(plotResults, payOff);
            PlotModel.InvalidatePlot(true);
        }

        private bool CanStartTicker()
        {
            return !TickerStarted;
        }

        // Action déclenché à l'appui sur le bouton start
        private void StartTicker()
        {

            // Déclaration des paramètres
            DateTime maturity = new DateTime(2014, 12, 20);
            DateTime initialDate = new DateTime(2013, 1, 20);
            DateTime estimationDate = new DateTime();
            int windowLength = 10;
            int numberOfDaysPerYear = 0;
            double strike = 0;
            List<Share> shareList = new List<Share>();
            List<DataFeed> dataFeedList = new List<DataFeed>();

            if (SelectedOption == "Vanilla Call")
            {
                if (Simulated)
                {
                    // ----------- Vanilla Call donnees simulees --------

                    // Parametrage
                    strike = 8;
                    estimationDate = getEstimationDateForSimulatedData(initialDate, windowLength);
                    shareList.Add(new Share("BNP PARIBAS", "BNP FP"));

                    // Creation de l'option
                    Option option = new VanillaCall("Vanilla Call", shareList.ToArray(), maturity, strike);

                    // Récupération des donnes simulées
                    IDataFeedProvider data = new SimulatedDataFeedProvider();
                    dataFeedList = data.GetDataFeed(option, estimationDate);
                    numberOfDaysPerYear = data.NumberOfDaysPerYear;

                    // Fournisseur de composition du portefeuille de réplication
                    CompositionProvider compositionProvider = new VanillaCompositionProvider(option);

                    // Main partagé
                    sharedMain(option, compositionProvider, dataFeedList, shareList, initialDate, strike, maturity, windowLength, numberOfDaysPerYear, Simulated);
                }
                else
                {
                    // ----------- Vanilla Call donnees historiques -------- 
                    strike = 35;
                    estimationDate = getEstimationDateForHistoricalData(initialDate, windowLength);
                    shareList.Add(new Share("BNP PARIBAS", "BNP FP"));

                    Option option = new VanillaCall("Vanilla Call", shareList.ToArray(), maturity, strike);

                    IDataFeedProvider data = new HistoricalDataFeedProvider("historicalData", 365);
                    dataFeedList = data.GetDataFeed(option, estimationDate);
                    numberOfDaysPerYear = data.NumberOfDaysPerYear;

                    CompositionProvider compositionProvider = new VanillaCompositionProvider(option);

                    sharedMain(option, compositionProvider, dataFeedList, shareList, initialDate, strike, maturity, windowLength, numberOfDaysPerYear, Simulated);
                }
            }
            else if (SelectedOption == "Basket Option")
            {
                if (Simulated)
                {
                    // ----------- Basket option donnees simulees --------
                    strike = 8;
                    estimationDate = getEstimationDateForSimulatedData(initialDate, windowLength);
                    shareList.Add(new Share("BNP PARIBAS", "BNP FP"));
                    shareList.Add(new Share("ACCOR SA", "ALO FP"));
                    double[] weights = { 0.3, 0.7 };

                    Option option = new BasketOption("Basket option", shareList.ToArray(), weights, maturity, strike);

                    IDataFeedProvider data = new SimulatedDataFeedProvider();
                    dataFeedList = data.GetDataFeed(option, estimationDate);
                    numberOfDaysPerYear = data.NumberOfDaysPerYear;

                    CompositionProvider compositionProvider = new BasketCompositionProvider(option);

                    sharedMain(option, compositionProvider, dataFeedList, shareList, initialDate, strike, maturity, windowLength, numberOfDaysPerYear, Simulated);
                }
                else
                {
                    // ----------- Basket option donnees historiques -------- 
                    strike = 35;
                    estimationDate = getEstimationDateForHistoricalData(initialDate, windowLength);
                    shareList.Add(new Share("BNP PARIBAS", "BNP FP"));
                    shareList.Add(new Share("ACCOR SA", "ALO FP"));
                    double[] weights = { 0.3, 0.7 };

                    Option option = new BasketOption("Basket option", shareList.ToArray(), weights, maturity, strike);

                    IDataFeedProvider data = new HistoricalDataFeedProvider("historicalData", 365);
                    dataFeedList = data.GetDataFeed(option, estimationDate);
                    numberOfDaysPerYear = data.NumberOfDaysPerYear;

                    CompositionProvider compositionProvider = new BasketCompositionProvider(option);

                    sharedMain(option, compositionProvider, dataFeedList, shareList, initialDate, strike, maturity, windowLength, numberOfDaysPerYear, Simulated);
                }
            }    
        }

        public DateTime getEstimationDateForSimulatedData(DateTime initialDate, int windowLength)
        {
            return initialDate.AddDays(-windowLength);
        }

        public DateTime getEstimationDateForHistoricalData(DateTime initialDate, int windowLength)
        {
            // Recuperation de la date de debut d'estimation (pour volatilite)
            //System.Collections.Generic.List<DateTime> datesBeforeInitialDate = new System.Collections.Generic.List<DateTime>();
            //using (DataBaseDataContext mtdc = new DataBaseDataContext())
            //{
            //    datesBeforeInitialDate = (from historical in mtdc.HistoricalShareValues
            //                              where (historical.date <= initialDate)
            //                              select historical.date).Distinct().OrderByDescending(date => date).ToList();
            //}
            //return datesBeforeInitialDate[windowLength];
            return new DateTime();
        }

        public void sharedMain(Option option, CompositionProvider compositionProvider, List<DataFeed> dataFeedList, List<Share> shareList, DateTime initialDate, double strike, DateTime maturity, int windowLength, int numberOfDaysPerYear, bool simulated)
        {
            PricingResults pricingResults = compositionProvider.getComposition(dataFeedList, initialDate, windowLength, numberOfDaysPerYear);
            HedgingPortfolio portfolio = createPortfolio(option, pricingResults, dataFeedList, initialDate);
            Dictionary<System.DateTime, ResultValue> plotResults = computeResults(option, portfolio, compositionProvider, dataFeedList, windowLength, numberOfDaysPerYear, !simulated);

            // Calcul du PayOff
            double payoff = option.GetPayoff(dataFeedList.Last().PriceList);

            Console.WriteLine(portfolio.Value);
            Console.WriteLine(payoff);

            // Création du graphique de résultats
            PlotModel = new PlotModel();
            SetUpModel();
            OptionDescription = "Underlying share: " + shareListString(shareList.ToArray()) + "\n Strike: " + strike + "\n Start Date: " + initialDate + "\n Maturity: " + maturity;

            // Tracé du plot
            DrawPlot(plotResults, payoff);
        }

        private Dictionary<System.DateTime, ResultValue> computeResults(Option option, HedgingPortfolio portfolio, CompositionProvider compositionProvider, List<DataFeed> dataFeedList, int windowLength, int nbDaysYear, bool isSimulated)
        {
            Dictionary<System.DateTime, ResultValue> plotResults = new Dictionary<System.DateTime, ResultValue>();

            // Rebalancement du portfeuille au cours du temps
            double riskFreeRate = 0;
            for (int i = windowLength; i < dataFeedList.Count() - 2; i++)
            {
                // Calcul du taux sans risque proratisé
                riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[i].Date, dataFeedList[i + 1].Date, nbDaysYear, isSimulated);

                // Rebalancement et actualisation de la valeur du portefeuille
                PricingResults pricingResults = compositionProvider.getComposition(dataFeedList, dataFeedList[i].Date, windowLength, nbDaysYear);
                portfolio.update(dataFeedList[i].PriceList, pricingResults.Deltas, riskFreeRate);
                ResultValue curentValue = new ResultValue(pricingResults.Price, portfolio.Value);
                plotResults.Add(dataFeedList[i].Date, curentValue);
            }
            // Calcul du taux sans risque proratisé
            riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[dataFeedList.Count() - 2].Date, dataFeedList[dataFeedList.Count() - 1].Date, nbDaysYear, isSimulated);
            // Valeur finale du portefeuille
            portfolio.computeValue(dataFeedList[dataFeedList.Count() - 1].PriceList, riskFreeRate);
            ResultValue finalValue = new ResultValue(option.GetPayoff(dataFeedList.Last().PriceList), portfolio.Value);
            plotResults.Add(dataFeedList.Last().Date, finalValue);

            return plotResults;
        }

        private string shareListString(Share[] shareList)
        {
            string shareListString = "";
            for (int i = 0; i < shareList.Length - 1; i++) { shareListString += shareList[i].Name + ", "; }
            shareListString += shareList[shareList.Length - 1].Name;
            return shareListString;
        }

        // Création et initialisation du portefeuille de couverture de l'option
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

        // Vérifie la validité des dates
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
