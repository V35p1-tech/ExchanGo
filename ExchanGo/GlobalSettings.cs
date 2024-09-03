using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExchanGo
{
    internal static class GlobalSettings
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["ExchanGo.Properties.Settings.ExchanGo_dbConnectionString"].ConnectionString;
        private static string _httpCurrencyExchangeDaily = "http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml";
        private static string _httpCurrencyExchangeHistoric = "http://www.ecb.int/stats/eurofxref/eurofxref-hist-90d.xml";
        private static string _lastCurrencyActualisationDate;
        private static bool _dbActual; 
        private static XDocument _dailyCurrency;
        private static XDocument _historicalCurrency;

        public static string ConnectionString
        { get { return _connectionString; } }
        public static string HttpCurrencyExchangeDaily
        { get { return _httpCurrencyExchangeDaily; } }
        public static string HttpsCurrencyExchangeHistoric
        { get { return _httpCurrencyExchangeHistoric; } }
        public static XDocument DailyCurrencyDoc
        {   
            get { return _dailyCurrency; } 
            set { _dailyCurrency = value; }
        }
        public static XDocument HistoricalCurrencyDoc
        {
            get { return _historicalCurrency; }
            set { _historicalCurrency = value; }
        }
        public static string LastCurrencyActualisationDate
        { 
            get { return _lastCurrencyActualisationDate; }
            set { _lastCurrencyActualisationDate = value; }
        }
        public static bool DataBaseActual
        {
            get { return _dbActual; }
            set { _dbActual = value; }
        }
    }
}
