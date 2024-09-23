using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ExchanGo
{
    public class CurrencyDataGater
    {
        public delegate void DatabaseUpdatedEventHandler(object sender, EventArgs e);

        public static event DatabaseUpdatedEventHandler DatabaseUpdated;

        public async Task<XDocument> GetDataFromHttp(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                XDocument xmlDoc;
                try
                {
                    string xmlContent = await httpClient.GetStringAsync(url);
                    xmlDoc = XDocument.Parse(xmlContent);
                    return xmlDoc;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
        }

        public void SaveToFile(XDocument DocumentToSave, string filePath)
        {
            DocumentToSave.Save(filePath);
        }

        public void UpdateDataBase(XDocument DocumentToSave, string dbConnectionString)
        {
            SqlConnection connection = new SqlConnection(dbConnectionString);
            var currencyRates = DocumentToSave.Descendants()
                                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null && x.Attribute("rate") != null)
                                .Select(x => new
                                {
                                    Currency = x.Attribute("currency").Value,
                                    Rate = x.Attribute("rate").Value
                                });
            //XmlDocument XmlDoc = new XmlDocument();
            //XmlDoc.LoadXml(DocumentToSave.ToString());
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(XmlDoc.NameTable);
            //nsmgr.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");
            //nsmgr.AddNamespace("eurofxref", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            //XmlNodeList xmlCubeList = XmlDoc.SelectNodes("//eurofxref:Cube//eurofxref:Cube//eurofxref:Cube", nsmgr);
            //foreach (XmlNode e in xmlCubeList)
            //{
            //    string rate = e.Attributes["rate"].InnerText;
            //    string currency = e.Attributes["currency"].Value;
            //    double rate2;
            //    string texttest = "1,2323";
            //    bool success = double.TryParse(texttest, out rate2);
            //    if(success)
            //    {
            //        Console.Write("nie zesralo sie");
            //    }

            //}

            try
            {
                connection.Open();

                foreach (var Currency in currencyRates)
                {
                    string insertQuery = "UPDATE CurrencyExchange set Rate = @Rate WHERE CurrencyCode = @CurrencyCode";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        double tempRate = 0;
                        try
                        {
                            bool success = double.TryParse(Currency.Rate, NumberStyles.Any, CultureInfo.CreateSpecificCulture("us-US"), out tempRate);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("The string is not a valid double.");
                        }

                        command.Parameters.AddWithValue("@CurrencyCode", Currency.Currency);
                        command.Parameters.AddWithValue("@Rate", tempRate);
                        command.ExecuteScalar();

                        GlobalSettings.LastCurrencyActualisationDate = DateTime.Now.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to db error");
            }
            finally
            {
                connection.Close();
            }
        }

        public void UpdateHistoricDataBase(XDocument DocumentToSave, string dbConnectionString)
        {
            SqlConnection connection = new SqlConnection(dbConnectionString);
            var currencyRates = DocumentToSave.Descendants()
                                   .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null && x.Attribute("rate") != null)
                                   .Select(x => new
                                   {
                                       Currency = x.Attribute("currency").Value,
                                       Rate = x.Attribute("rate").Value,
                                       Date = x.Parent.Attribute("time").Value
                                   })
                                   .ToList();

            try
            {
                connection.Open();

                foreach (var Currency in currencyRates)
                {
                    string insertQuery = "UPDATE CurrencyHistoric SET Rate = @Rate, Date = @Date WHERE CurrencyCode = @CurrencyCode";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        double tempRate = 0;
                        DateTime tempDate = DateTime.Now;
                        try
                        {
                            bool success = double.TryParse(Currency.Rate, NumberStyles.Any, CultureInfo.CreateSpecificCulture("us-US"), out tempRate);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Rate - The string is not a valid double.");
                        }
                        try
                        {
                            bool success = DateTime.TryParse(Currency.Date, out tempDate);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("DT - The string is not a valid double.");
                        }

                        command.Parameters.AddWithValue("@CurrencyCode", Currency.Currency);
                        command.Parameters.AddWithValue("@Rate", tempRate);
                        command.Parameters.AddWithValue("@Date", tempDate);
                        command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to db error");
            }
            finally
            {
                connection.Close();
            }
        }

        public void CreateDataBaseEntires(XDocument DocumentToSave, string dbConnectionString)
        {
            SqlConnection connection = new SqlConnection(dbConnectionString);

            try
            {
                connection.Open();
                string clearDbQuery = "DELETE FROM CurrencyExchange";
                SqlCommand deleteCommand = new SqlCommand(clearDbQuery, connection);
                deleteCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to db error");
            }
            finally { connection.Close(); };

            var currencyRates = DocumentToSave.Descendants()
                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null && x.Attribute("rate") != null)
                .Select(x => new
                {
                    Currency = x.Attribute("currency").Value,
                    Rate = x.Attribute("rate").Value
                });
            try
            {
                connection.Open();
                foreach (var Currency in currencyRates)
                {
                    string insertQuery = "INSERT INTO CurrencyExchange (CurrencyCode, Rate) VALUES (@CurrencyCode, @Rate)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        double tempRate = 0;
                        try
                        {
                            bool success = double.TryParse(Currency.Rate, NumberStyles.Any, CultureInfo.CreateSpecificCulture("us-US"), out tempRate);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("The string is not a valid double.");
                        }

                        command.Parameters.AddWithValue("@CurrencyCode", Currency.Currency);
                        command.Parameters.AddWithValue("@Rate", tempRate);
                        command.ExecuteScalar();
                    }
                }
                string insertEuroQuery = "INSERT INTO CurrencyExchange (CurrencyCode, Rate) VALUES ('EUR', 1)";
                using (SqlCommand euroInsertCommand = new SqlCommand(insertEuroQuery, connection))
                {
                    euroInsertCommand.ExecuteScalar();
                    GlobalSettings.DataBaseActual = true;
                }
                GlobalSettings.LastCurrencyActualisationDate = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to db error");
            }
            finally
            {
                string insertQuery = "INSERT INTO CurrencyExchange (CurrencyCode, Rate) VALUES ('EUR', 1)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    //command.Parameters.AddWithValue("@CurrencyCode", Currency.Currency);
                    //command.Parameters.AddWithValue("@Rate", tempRate);
                    command.ExecuteScalar();
                    connection.Close();
                    if (GlobalSettings.DataBaseActual) { OnDatabaseUpdated(EventArgs.Empty); }
                }
            }
        }

        private static void OnDatabaseUpdated(EventArgs e)
        {
            DatabaseUpdated?.Invoke(null, e);
        }

        public void CreateHistoricDataBaseEntries(XDocument DocumentToSave, string dbConnectionString)
        {
            SqlConnection connection = new SqlConnection(dbConnectionString);

            try
            {
                connection.Open();
                string clearDbQuery = "DELETE FROM CurrencyHistoric";
                SqlCommand deleteCommand = new SqlCommand(clearDbQuery, connection);
                deleteCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to db error");
            }
            finally { connection.Close(); };

            var currencyRates = DocumentToSave.Descendants()
                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null && x.Attribute("rate") != null)
                .Select(x => new
                {
                    Currency = x.Attribute("currency").Value,
                    Rate = x.Attribute("rate").Value,
                    Date = x.Parent.Attribute("time").Value
                })
                .ToList();

            try
            {
                connection.Open();

                foreach (var Currency in currencyRates)
                {
                    string insertQuery = "INSERT INTO CurrencyHistoric (CurrencyCode, Rate, Date) VALUES (@CurrencyCode, @Rate, @Date)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        double tempRate = 0;
                        DateTime tempDate = DateTime.Now;
                        try
                        {
                            bool success = double.TryParse(Currency.Rate, NumberStyles.Any, CultureInfo.CreateSpecificCulture("us-US"), out tempRate);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Rate - The string is not a valid double.");
                        }
                        try
                        {
                            bool success = DateTime.TryParse(Currency.Date, out tempDate);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("DT - The string is not a valid double.");
                        }

                        command.Parameters.AddWithValue("@CurrencyCode", Currency.Currency);
                        command.Parameters.AddWithValue("@Rate", tempRate);
                        command.Parameters.AddWithValue("@Date", tempDate);
                        command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to db error");
            }
            finally
            {
                connection.Close();
            }
        }
    }
}