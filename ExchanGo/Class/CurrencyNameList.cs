using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExchanGo
{
    public class CurrencyNameList
    {
        private static DataTable currencyTable = new DataTable("CurrencyTable");
        public DataTable CurrencyNameTable {  get { return currencyTable; } }
        public CurrencyNameList()
        {

            // Define the columns
            currencyTable.Columns.Add("CurrencyCode", typeof(string));
            currencyTable.Columns.Add("FullName", typeof(string));

            // Add rows directly to the DataTable
            currencyTable.Rows.Add("USD", "United States Dollar");
            currencyTable.Rows.Add("JPY", "Japanese Yen");
            currencyTable.Rows.Add("BGN", "Bulgarian Lev");
            currencyTable.Rows.Add("CZK", "Czech Koruna");
            currencyTable.Rows.Add("DKK", "Danish Krone");
            currencyTable.Rows.Add("GBP", "British Pound Sterling");
            currencyTable.Rows.Add("HUF", "Hungarian Forint");
            currencyTable.Rows.Add("PLN", "Polish Zloty");
            currencyTable.Rows.Add("RON", "Romanian Leu");
            currencyTable.Rows.Add("SEK", "Swedish Krona");
            currencyTable.Rows.Add("CHF", "Swiss Franc");
            currencyTable.Rows.Add("ISK", "Icelandic Krona");
            currencyTable.Rows.Add("NOK", "Norwegian Krone");
            currencyTable.Rows.Add("TRY", "Turkish Lira");
            currencyTable.Rows.Add("AUD", "Australian Dollar");
            currencyTable.Rows.Add("BRL", "Brazilian Real");
            currencyTable.Rows.Add("CAD", "Canadian Dollar");
            currencyTable.Rows.Add("CNY", "Chinese Yuan");
            currencyTable.Rows.Add("HKD", "Hong Kong Dollar");
            currencyTable.Rows.Add("IDR", "Indonesian Rupiah");
            currencyTable.Rows.Add("ILS", "Israeli New Shekel");
            currencyTable.Rows.Add("INR", "Indian Rupee");
            currencyTable.Rows.Add("KRW", "South Korean Won");
            currencyTable.Rows.Add("MXN", "Mexican Peso");
            currencyTable.Rows.Add("MYR", "Malaysian Ringgit");
            currencyTable.Rows.Add("NZD", "New Zealand Dollar");
            currencyTable.Rows.Add("PHP", "Philippine Peso");
            currencyTable.Rows.Add("SGD", "Singapore Dollar");
            currencyTable.Rows.Add("THB", "Thai Baht");
            currencyTable.Rows.Add("ZAR", "South African Rand");

        }

        public string ReadFullName(string CurrencyCode)
        {
            string currencyCodeToSearch = CurrencyCode;
            DataRow[] foundRows = currencyTable.Select($"CurrencyCode = '{currencyCodeToSearch}'");

            if (foundRows.Length > 0)
            {
                string fullName = foundRows[0]["FullName"].ToString();
                return fullName;
            }
            else
            {
                return "Name read error";
            }
        }
    }
}
