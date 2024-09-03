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
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.CodeDom;

namespace ExchanGo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CurrencyNameList currencyFullNameList = new CurrencyNameList();
        public MainWindow()
        {
            GlobalSettings.DataBaseActual = false;
            InitializeComponent();
            GetAsyncData();
            CurrencyDataGater.DatabaseUpdated += OnDatabaseUpdated;
          
            Chart_LineSeries.Values = new ChartValues<double> { 3, 5, 7, 4 ,1 ,0 ,2 ,5 ,6 ,3 ,2 ,1 ,2 ,5 ,5 ,9 ,6};
            //DataContext = this;

        }

        private void OnDatabaseUpdated(object sender, EventArgs e)
        {
            ShowCurrencies();
            Settings_txtDbActualisationDate.Text = "DataBase actualised on: " + GlobalSettings.LastCurrencyActualisationDate.ToString();
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
            GetActualCurrency.CreateDataBaseEntires(GlobalSettings.DailyCurrencyDoc, GlobalSettings.ConnectionString);
            GetActualCurrency.CreateHistoricDataBaseEntries(GlobalSettings.HistoricalCurrencyDoc, GlobalSettings.ConnectionString);
            // GetActualCurrency.SaveToFile(GlobalSettings.DailyCurrencyDoc, "DailyCur.xml");

        }

        public async void UpdateAsyncData()
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
            GetActualCurrency.UpdateHistoricDataBase(GlobalSettings.HistoricalCurrencyDoc, GlobalSettings.ConnectionString);
            // GetActualCurrency.SaveToFile(GlobalSettings.DailyCurrencyDoc, "DailyCur.xml");

        }

        public void ShowCurrencies()
        {
            string query = "SELECT * FROM CurrencyExchange";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, GlobalSettings.ConnectionString);

            using (sqlDataAdapter)
            {
                DataTable currencyTable = new DataTable();
                sqlDataAdapter.Fill(currencyTable);
                Converter_cbCurrency_ConvertFrom.ItemsSource = currencyTable.DefaultView;
                Converter_cbCurrency_ConvertFrom.DisplayMemberPath = "CurrencyCode";
                Converter_cbCurrency_ConvertFrom.SelectedValuePath = "Rate";
                Converter_cbCurrency_ConvertTo.ItemsSource = currencyTable.DefaultView;
                Converter_cbCurrency_ConvertTo.DisplayMemberPath = "CurrencyCode";
                Converter_cbCurrency_ConvertTo.SelectedValuePath = "Rate";
                DataTable chartTable = currencyTable;
                chartTable.Rows.RemoveAt(chartTable.Rows.Count-1);
                Chart_cbCurrencyChosed.ItemsSource = chartTable.DefaultView;
                Chart_cbCurrencyChosed.DisplayMemberPath = "CurrencyCode";
                Chart_cbCurrencyChosed.SelectedValuePath = "Rate";
                
            }
        }
        
        
        private void Converter_BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedCurrency = 0;
            if (double.TryParse(Converter_txtInputCurrency.Text, NumberStyles.Any, CultureInfo.CreateSpecificCulture("us-US"), out double tempText))
            {
                
                string ConvertFrom = Converter_cbCurrency_ConvertFrom.SelectedValue.ToString();
                string ConvertTo = Converter_cbCurrency_ConvertTo.SelectedValue.ToString();
                double ConvertFrom_Rate = 1;
                double ConvertTo_Rate = 1;
                try
                {
                    ConvertFrom_Rate = (double)Converter_cbCurrency_ConvertFrom.SelectedValue;
                    ConvertTo_Rate = (double)Converter_cbCurrency_ConvertTo.SelectedValue;
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.Message);
                    Converter_txtConvertedValue.Text = "Conversion fault. Wrong data provided.";
                }
                ConvertedCurrency = Math.Round(tempText * ConvertTo_Rate / ConvertFrom_Rate, 2);
                string sConvertedCurrency = ConvertedCurrency.ToString();

                Converter_txtConvertedValue.Text = "Converted currency: " + sConvertedCurrency;
            }
            else { Converter_txtConvertedValue.Text = "Conversion fault. Wrong data provided."; }
            
        }

        private void Converter_BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Converter_txtInputCurrency.Text = "";
            Converter_txtConvertedValue.Text = "";
            Converter_cbCurrency_ConvertFrom.SelectedIndex = -1;
            Converter_cbCurrency_ConvertTo.SelectedIndex = -1;
        }

        private void Converter_cbCurrency_ConvertFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCurrency = "";
            if (Converter_cbCurrency_ConvertFrom.SelectedItem is DataRowView selectedRow)
            {
                selectedCurrency = selectedRow["CurrencyCode"].ToString();
            }

            DataRow[] foundRows = currencyFullNameList.CurrencyNameTable.Select($"CurrencyCode = '{selectedCurrency}'");

            if (foundRows.Length > 0)
            {
                foreach (DataRow row in foundRows)
                {
                    Converter_textCurrency_ConvertFrom_fullName.Text = row["FullName"].ToString();
                }
            }
            else
            {
                Console.WriteLine("Currency not found.");
            }
        }

        private void Converter_cbCurrency_ConvertTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCurrency = "";
            if (Converter_cbCurrency_ConvertTo.SelectedItem is DataRowView selectedRow)
            {
                selectedCurrency = selectedRow["CurrencyCode"].ToString();
            }

            DataRow[] foundRows = currencyFullNameList.CurrencyNameTable.Select($"CurrencyCode = '{selectedCurrency}'");

            if (foundRows.Length > 0)
            {
                foreach (DataRow row in foundRows)
                {
                    Converter_textCurrency_ConvertTo_fullName.Text = row["FullName"].ToString();
                }
            }
            else
            {
                Console.WriteLine("Currency not found.");
            }
        }
        private void Chart_cbCurrencyChosed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCurrency = "";
            if (Chart_cbCurrencyChosed.SelectedItem is DataRowView selectedRow)
            {
                selectedCurrency = selectedRow["CurrencyCode"].ToString();
            }
            SqlConnection connection = new SqlConnection(GlobalSettings.ConnectionString);
            connection.Open();
            try
            {
                string query = "SELECT Date, Rate FROM CurrencyHistoric WHERE CurrencyCode = @CurrencyCode ORDER BY Date";
                DataTable table = new DataTable();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrencyCode", selectedCurrency);
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                var chartValues = new ChartValues<double>();
                List<string> dateLabels = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    double rate = Math.Round(Convert.ToDouble(row["Rate"]),4);
                    chartValues.Add(rate);

                    DateTime tempDateTime = (DateTime)row["Date"];
                    dateLabels.Add(tempDateTime.ToString("dd MMM"));
                }
                /*
                List<DateTime> dates = new List<DateTime>();         
                foreach (DataRow row in table.Rows)
                {
                    DateTime tempDateTime = (DateTime)row["Date"];
                    dates.Add(tempDateTime);
                }
                */
                DataContext = new
                {
                    XAxisDateFormatter = new Func<double, string>(value => new DateTime((long)value).ToString("dd MMM"))
                };
                Chart_LineSeries.Values = chartValues;
                Chart_LineSeries.Title = "For 1Euro: ";
                /*
                Console.WriteLine($"Min Date: {dates.First()}, Min Ticks: {dates.First().Ticks}");
                Console.WriteLine($"Max Date: {dates.Last()}, Max Ticks: {dates.Last().Ticks}");
                
                if (Chart.AxisX.Count > 0)
                {
                    Chart.AxisX[0].MinValue = new DateTime(dates.First().Ticks).ToOADate();
                    Chart.AxisX[0].MaxValue = new DateTime(dates.Last().Ticks).ToOADate();

                }
                else
                {
                    Console.WriteLine("No AxisX configured on the chart.");
                }
                */
                Chart.AxisX[0].Labels = dateLabels;
                Chart.AxisX[0].LabelsRotation = 45;
                Chart.AxisX[0].MinValue = 0;
                Chart.AxisX[0].MaxValue = dateLabels.Count - 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("nie dziaja");
                Console.WriteLine(ex.Message);
            }
            finally { connection.Close(); }
        }
    }
}
