using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace ExchanGo
{
    public class CurrencyDataGater
    {
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

                        /*
                        string insertQuery = "INSERT INTO CurrencyExchange (CurrencyCode, Rate) VALUES (@CurrencyCode, @Rate)";

                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            // Add parameters to the SQL command
                            command.Parameters.AddWithValue("@CurrencyCode", rate.Currency);
                            command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(rate.Rate));

                            // Execute the SQL command
                            command.ExecuteNonQuery();
                        */

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

        public void CreateDataBaseEntires(XDocument DocumentToSave, string dbConnectionString)
                {
                    SqlConnection connection = new SqlConnection(dbConnectionString);

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

                        foreach (var rate in currencyRates)
                        {
                            string insertQuery = "INSERT INTO CurrencyExchange (CurrencyCode, Rate) VALUES (@CurrencyCode, @Rate)";

                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@CurrencyCode", rate.Currency);
                                command.Parameters.AddWithValue("@Rate", 0.0);
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
