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
        private string _results;
        private double _strike = 10;
        private string _maturity = "01/01/2013";
        private PlotModel _plotModel;

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
            PlotModel = new PlotModel();
            SetUpModel();
            StartCommand = new DelegateCommand(StartTicker, CanStartTicker);           
        }

        private bool CanStartTicker()
        {
            return !TickerStarted;
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

        private void StartTicker()
        {
            
        }

    }
}
