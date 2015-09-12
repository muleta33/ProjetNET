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
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            //// ----------- Test Vanilla Call donnees simulees -------- 
            //// Recuperation des donnees
            //DateTime maturity = new DateTime(2014, 12, 20);
            //DateTime initialDate = new DateTime(2013, 1, 20);
            //DateTime estimationDate = new DateTime(2013, 1, 5);
            //int windowLength = 15;
            //double strike = 12;
            //String[] shareIdList = { "BNP FP" };
            //VanillaCall vanillaCall = createVanillaCall("Vanilla Call", maturity, strike, shareIdList);

            //IDataFeedProvider data = new SimulatedDataFeedProvider();
            //List<DataFeed> dataFeedList = data.GetDataFeed(vanillaCall, estimationDate);

            //// Creation du portefeuille
            //CompositionProvider compositionProvider = new VanillaCompositionProvider(vanillaCall);
            //PricingResults pricingResults = compositionProvider.getComposition(dataFeedList, initialDate, windowLength, data.NumberOfDaysPerYear);
            //HedgingPortfolio portfolio = createPortfolio(vanillaCall, pricingResults, dataFeedList, initialDate);

            //// Rebalancement du portfeuille au cours du temps
            //double riskFreeRate = 0;
            //for (int i = windowLength; i < dataFeedList.Count() - 2; i++)
            //{
            //    // Calcul du taux sans risque proratisé
            //    riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[i].Date, dataFeedList[i + 1].Date, data.NumberOfDaysPerYear, false);

            //    // Rebalancement et actualisation de la valeur du portefeuille
            //    pricingResults = compositionProvider.getComposition(dataFeedList, dataFeedList[i].Date, windowLength, data.NumberOfDaysPerYear);
            //    portfolio.update(dataFeedList[i].PriceList, pricingResults.Deltas, riskFreeRate);
            //}
            //// Calcul du taux sans risque proratisé
            //riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[dataFeedList.Count() - 2].Date, dataFeedList[dataFeedList.Count() - 1].Date, data.NumberOfDaysPerYear, false);
            //// Valeur finale du portefeuille
            //portfolio.computeValue(dataFeedList[dataFeedList.Count() - 1].PriceList, riskFreeRate);

            //// Calcul du PayOff
            //double payoff = vanillaCall.GetPayoff(dataFeedList.Last().PriceList);

            //Console.WriteLine(portfolio.Value);
            //Console.WriteLine(payoff);
            //Console.WriteLine(Math.Abs((portfolio.Value - payoff) / 10));

            // ----------- Test Basket Option donnees simulees -------- 
            // Recuperation des donnees
            //DateTime maturity = new DateTime(2014, 12, 20);
            //DateTime initialDate = new DateTime(2013, 1, 20);
            //DateTime estimationDate = new DateTime(2013, 1, 5);
            //int windowLength = 15;
            //double strike = 12;
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
            //}
            //// Calcul du taux sans risque proratisé
            //riskFreeRate = Utilities.computeAccruedRiskFreeRate(dataFeedList[dataFeedList.Count() - 2].Date, dataFeedList[dataFeedList.Count() - 1].Date, data.NumberOfDaysPerYear, false);
            //// Valeur finale du portefeuille
            //portfolio.computeValue(dataFeedList[dataFeedList.Count() - 1].PriceList, riskFreeRate);

            //// Calcul du PayOff
            //double payoff = basketOption.GetPayoff(dataFeedList.Last().PriceList);

            //Console.WriteLine(portfolio.Value);
            //Console.WriteLine(payoff);
            //Console.WriteLine(Math.Abs((portfolio.Value - payoff) / 10));

            //try
            //{
            //    // InitializeComponent();

            //    // Paramètres de l'étude
            //    DateTime maturityDate = new DateTime(2014, 12, 20);
            //    DateTime initialDate = new DateTime(2013, 7, 11);
            //    int window = 10;
            //    double strike = 25;
            //    Share[] shareList = { new Share("BNP PARIBAS", "BNP FP"),
            //                          new Share("ACCOR SA", "ALO FP")};
            //    double[] weights = { 0.4, 0.6 };
            //    BasketOption basketOption = new BasketOption("Basket option on BNP, ACCOR", shareList, weights, maturityDate, strike);
            //    //VanillaCall vanillaCall = new VanillaCall("V1", shareList, maturityDate, strike);

            //    // Recuperation de la date de debut d'estimation (pour volatilite)
            //    // Check validDate doit être appelé avant
            //    System.Collections.Generic.List<DateTime> datesBeforeInitialDate = new System.Collections.Generic.List<DateTime>();
            //    using (DataBaseDataContext mtdc = new DataBaseDataContext())
            //    {
            //        datesBeforeInitialDate = (from historical in mtdc.HistoricalShareValues
            //                                  where (historical.date <= initialDate)
            //                                  select historical.date).Distinct().OrderByDescending(date => date).ToList();
            //    }
            //    DateTime estimationBeginDate = datesBeforeInitialDate[window];

            //    // Récupération des données via le data provider
            //    IDataFeedProvider data = new HistoricalDataFeedProvider("HistoricalData", 365);
            //    //checkValidDate(data.GetMinDate(), initialDate, window);
            //    //IDataFeedProvider data = new SimulatedDataFeedProvider();
            //    List<DataFeed> dataFeedList = data.GetDataFeed(basketOption, estimationBeginDate);

            //    // Récupération de la volatilité et de la matrice choleskyCorrelation
            //    System.Collections.Generic.List<String> shareIdsList = new System.Collections.Generic.List<String>();
            //    foreach (Share share in shareList)
            //    {
            //        shareIdsList.Add(share.Id);
            //    }
            //    BasketOptionCholeskyCorrelation choleskyCorrelation = new BasketOptionCholeskyCorrelation(shareIdsList, window);
            //    double[] volatilities = choleskyCorrelation.computeVolatilities(dataFeedList, initialDate);
            //    double[,] cholesky = choleskyCorrelation.computeCholeskyCorrelation(dataFeedList, initialDate);

            //    // Création du portefeuille de couverture
            //    HedgingPortfolio portfolio = createPortfolio(basketOption, volatilities, cholesky, initialDate, dataFeedList, data.NumberOfDaysPerYear);

            //    // Rebalancement du portfeuille au cours du temps
            //    double riskFreeRate = 0;
            //    for (int i = window; i < dataFeedList.Count() - 2; i++)
            //    {
            //        // Calcul du taux sans risque proratisé
            //        riskFreeRate = computeAccruedRiskFreeRate(dataFeedList[i].Date, dataFeedList[i + 1].Date, data.NumberOfDaysPerYear);

            //        // Mise a jour volatilites et matrice choleskyCorrelation
            //        volatilities = choleskyCorrelation.computeVolatilities(dataFeedList, initialDate);
            //        cholesky = choleskyCorrelation.computeCholeskyCorrelation(dataFeedList, initialDate);

            //        // Rebalancement et actualisation de la valeur du portefeuille
            //        portfolio.updateCall(basketOption, dataFeedList[i].Date, dataFeedList[i].PriceList, volatilities, cholesky, riskFreeRate);
            //    }
            //    // Calcul du taux sans risque proratisé
            //    riskFreeRate = computeAccruedRiskFreeRate(dataFeedList[dataFeedList.Count() - 2].Date, dataFeedList[dataFeedList.Count() - 1].Date, data.NumberOfDaysPerYear);
            //    // Valeur finale du portefeuille
            //    portfolio.computeValue(dataFeedList[dataFeedList.Count() - 1].PriceList, riskFreeRate);

            //    // Calcul du PayOff
            //    double payoff = basketOption.GetPayoff(dataFeedList.Last().PriceList);

            //    // Affichage des résultats
            //    Console.WriteLine(portfolio.Value);
            //    Console.WriteLine(payoff);
            //    Console.WriteLine(Math.Abs((portfolio.Value - payoff) / 30));
            //}
            //catch (ParameterException e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }

        
        //// Crée une option à partir de ses caracteristiques
        //// Lève une exception si jamais le sous jacent n'existe pas
        //public Option createOption(String name, System.DateTime maturity, double strike, String[] UnderlyingShareIds, double[] weights)
        //{
        //    System.Collections.Generic.Dictionary<String, String> UnderlyingSharesNames = new System.Collections.Generic.Dictionary<String, String>();
        //    Share[] underlyingShares = new Share[UnderlyingShareIds.Count()];
        //    IOption option;
        //    using (DataBaseDataContext mtdc = new DataBaseDataContext())
        //    {
        //        UnderlyingSharesNames = (from s in mtdc.ShareNames where (UnderlyingShareIds.Contains(s.id)) select s).ToDictionary(s => s.name, s => s.id);
        //    }
        //    for (int index = 0; index < UnderlyingSharesNames.Count; index++)
        //    {
        //        var item = UnderlyingSharesNames.ElementAt(index);
        //        String itemKey = item.Key;
        //        String itemValue = item.Value;
        //        Share share = new Share(itemKey, itemValue);
        //        underlyingShares[index] = share;
        //    }
        //    if (UnderlyingSharesNames.Count() > 1)
        //    {
        //        option = new BasketOption(name, underlyingShares, weights, maturity, strike);
        //    }
        //    else if (UnderlyingSharesNames.Count() == 1)
        //    {
        //        option = new VanillaCall(name, underlyingShares, maturity, strike);
        //    }
        //    else
        //    {
        //        throw new ParameterException("Share Id not found.");
        //    }
        //    return (Option)option;
        //}

    }
}
