using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchanGo
{
    internal static class GlobalSettings
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["ExchanGo.Properties.Settings.ExchanGo_dbConnectionString"].ConnectionString;
        private static string _httpCurrencyExchangeDaily = "http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml";
        private static string _httpCurrencyExchangeHistoric = "http://www.ecb.int/stats/eurofxref/eurofxref-hist-90d.xml";
        public static string ConnectionString
        { get { return _connectionString; } }
        public static string HttpCurrencyExchangeDaily
        { get { return _httpCurrencyExchangeDaily; } }
        public static string HttpsCurrencyExchangeHistoric
        { get { return _httpCurrencyExchangeHistoric; } }
    }
}
