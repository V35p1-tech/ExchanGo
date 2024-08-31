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
using System.Configuration;
using LiveCharts;
using LiveCharts.Wpf;
using System.Xml.Linq;

namespace ExchanGo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetAsyncData();

            // Initialize chart data
            Chart_LineSeries.Values = new ChartValues<double> { 3, 5, 7, 4 ,1 ,0 ,2 ,5 ,6 ,3 ,2 ,1 ,2 ,5 ,5 ,9 ,6};
            // Set the DataContext for binding
            DataContext = this;

        }

        public async void GetAsyncData()
        {
            CurrencyDataGater GetActualCurrency = new CurrencyDataGater();
            CurrencyDataGater GetHistoricallCurrency = new CurrencyDataGater();
            try
            {
                GlobalSettings.DailyCurrencyDoc = await GetActualCurrency.GetDataFromHttp(GlobalSettings.HttpCurrencyExchangeDaily);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            try
            {
                GlobalSettings.HistoricalCurrencyDoc = await GetActualCurrency.GetDataFromHttp(GlobalSettings.HttpsCurrencyExchangeHistoric); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            GetActualCurrency.UpdateDataBase(GlobalSettings.DailyCurrencyDoc,GlobalSettings.ConnectionString);
           // GetActualCurrency.SaveToFile(GlobalSettings.DailyCurrencyDoc, "DailyCur.xml");

        }
    }
}
