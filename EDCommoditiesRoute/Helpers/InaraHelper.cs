using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EDCommoditiesRoute.Models;
using HtmlAgilityPack;

namespace InaraHelper
{
    public class InaraHelper
    {
        internal static HttpClient Client = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

        //private static String _header = "https://inara.cz/commodities/";
        //private static String _header = "https://inara.cz/elite/commodities-list/";
        private static String _header = "https://inara.cz/elite/commodities/?pi1=1&pa1%5B%5D=139&ps1=Volkhabe&pi10=3&pi11=0&pi3=2&pi9=0&pi4=1&pi5=0&pi12=0&pi7=0&pi8=1";
        private static String _headerSystem = "https://www.edsm.net/api-system-v1/";
        private static HttpClient ClientEdsmSystem = new HttpClient() { Timeout = TimeSpan.FromSeconds(30), BaseAddress = new Uri(_headerSystem) };
        private const string WhitelistedTableClass = "whitelisted-table";
        //public static Dictionary<String, Int32> CommoditiesMaxSellPrices = new Dictionary<string, Int32>();
        public static Dictionary<String, List<InaraCommodityInfo>> StationsCommodities = new Dictionary<string, List<InaraCommodityInfo>>();

        /// <summary>
        /// init the lis
        /// </summary>
        /// <returns></returns>
        public static async Task GetCommodities(String initialSystem, CommodityInfo commodityInfo)
        {
            try
            {

                // If the commodity exist, delete it
                if (StationsCommodities.ContainsKey(commodityInfo.Libelle))
                {
                    StationsCommodities[commodityInfo.Libelle].Clear();
                }
                else
                {
                    StationsCommodities.Add(commodityInfo.Libelle, new List<InaraCommodityInfo>());
                }

                _header = $"https://inara.cz/elite/commodities/?formbrief=1&pi1=1&pa1%5B%5D={commodityInfo.Numero}&ps1=" + initialSystem + "&pi10=3&pi11=0&pi3=1&pi9=0&pi4=0&pi5=0&pi12=0&pi7=0&pi8=1&pi14=0";

                //            https://inara.cz/elite/commodities/?formbrief=1&pi1=1&pa1%5B%5D=1&ps1=volkhabe&pi10=3&pi11=0&pi3=1&pi9=0&pi4=0&pi14=0&pi5=0&pi12=0&pi7=0&pi8=1&pi13=0

                HttpResponseMessage response;
                string responseBody;
                string query = string.Empty;
                response = Client.GetAsync($"{_header}").Result;
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

                // From String
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseBody);

                IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(n => n.HasClass("maincontent1"));


                var tableRuntime = nodes.FirstOrDefault()?.SelectSingleNode("//table");
                var HTMLTableTRList = from table in doc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                                      from row in table.SelectNodes("//tr").Cast<HtmlNode>()
                                      from cell in row.SelectNodes("th|td").Cast<HtmlNode>()
                                      select new { Table_Name = table.Id, Cell_Text = cell.InnerText };

                Int32 price = 0;
                Double doubleValue = 0;
                InaraCommodityInfo cv;
                Int32 j;
                String CommodityFamily = "";
                String[] infos;
                String temp;
                for (int i = 7; i < HTMLTableTRList.Count(); i = i + 7)
                {
                    cv = new InaraCommodityInfo();
                    if (HTMLTableTRList.ElementAt(i).Cell_Text == String.Empty)
                    {
                        i++;
                    }
                    else
                    {
                        cv.Location = HTMLTableTRList.ElementAt(i).Cell_Text.ToUpper();
                        cv.Location = RemoveSpecialCaracters(cv.Location);
                        infos = cv.Location.Split('|');
                        cv.Station = infos[0].Trim();
                        cv.System = infos[1].Trim();
                    }

                    j = i + 1;
                    cv.Pad = HTMLTableTRList.ElementAt(j).Cell_Text.Trim();

                    j++;
                    if (Double.TryParse(HTMLTableTRList.ElementAt(j).Cell_Text.Split(' ')[0], out doubleValue))
                        cv.DistanceStation = doubleValue;

                    j++;
                    if (Double.TryParse(HTMLTableTRList.ElementAt(j).Cell_Text.Split(' ')[0].Replace(".", ","), out doubleValue))
                        cv.DistanceSystem = doubleValue;

                    j++;
                    if (Int32.TryParse(HTMLTableTRList.ElementAt(j).Cell_Text.Split(' ')[0].Replace(",", String.Empty),
                        out price))
                    {
                        cv.Supply = price;
                    }
                    else
                    {
                        temp = String.Empty;
                        foreach (Char item in HTMLTableTRList.ElementAt(j).Cell_Text.Split(' ')[0])
                        {
                            if (item >= '0' && item <= '9')
                            {
                                temp += item;
                            }
                        }
                        if (Int32.TryParse(temp, out price))
                        {
                            cv.Supply = price;
                        }
                    }

                    j++;
                    if (Int32.TryParse(HTMLTableTRList.ElementAt(j).Cell_Text.Split(' ')[0].Replace(",", String.Empty),
                        out price))
                        cv.Price = price;

                    j++;
                    cv.Updated = HTMLTableTRList.ElementAt(j).Cell_Text.Trim();

                    StationsCommodities[commodityInfo.Libelle].Add(cv);
                }

                
            }
            catch (Exception)
            {
                // this catch is blank intentionaly. Some commodities are not sell at all
            }
        }

        private static string RemoveSpecialCaracters(string location)
        {
            String temp = String.Empty;
            foreach (Char item in location)
            {
                if (
                    (item >= '0' && item <= '9') 
                    || (item >='a' && item <='z') 
                    || (item >= 'A' && item <= 'Z') 
                    || item == '|' 
                    || item == ' ')
                {
                    temp += item;
                }
            }
            return temp;
        }

        /// <summary>
        /// get the price of a commodity. Init the list if the list is empty
        /// </summary>
        /// <param name="commodityName">Commodity name</param>
        /// <param name="forceRefreshPrices">force the list refresh</param>
        /// <returns></returns>
        //public static Int32 GetCommodityMaxSell(String commodityName, bool forceRefreshPrices = false)
        //{
        //    if (CommoditiesPrices.Count == 0)
        //    {
        //        forceRefreshPrices = true;
        //    }
        //    if (forceRefreshPrices)
        //    {
        //        GetCommodities().GetAwaiter().GetResult();
        //    }
        //    if (CommoditiesPrices.ContainsKey(commodityName.ToUpper()))
        //    {
        //        return CommoditiesPrices[commodityName.ToUpper()].MAX_SELL;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //public static CommodityValues GetCommodityValues(String commodityName, bool forceRefreshPrices = false)
        //{
        //    if (CommoditiesPrices.Count == 0)
        //    {
        //        forceRefreshPrices = true;
        //    }
        //    if (forceRefreshPrices)
        //    {
        //        GetCommodities().GetAwaiter().GetResult();
        //    }
        //    if (CommoditiesPrices.ContainsKey(commodityName.ToUpper()))
        //    {
        //        return CommoditiesPrices[commodityName.ToUpper()];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        //private static ICollection<string> ConvertToPlainText(HtmlNodeCollection nodes)
        //{
        //    var texts = new List<string>();
        //    foreach (var node in nodes)
        //    {
        //        var nodeName = node.Name.ToLowerInvariant();

        //        switch (nodeName)
        //        {
        //            case "style":
        //                continue;
        //            case "br":
        //                texts.Add("\n\n");
        //                continue;
        //            default:
        //                break;
        //        }

        //        // Convert table 
        //        if (node.HasClass(WhitelistedTableClass))
        //        {
        //            texts.Add(ConvertTable(node));
        //            continue;
        //        }

        //        if (node.HasChildNodes)
        //        {
        //            if (node.Attributes != null && node.Attributes.Count > 0 && node.Attributes.Contains("href"))
        //            {
        //                texts.Add(LinkToTxt(node) + "\n\n");
        //            }
        //            else
        //            {
        //                texts.AddRange(ConvertToPlainText(node.ChildNodes));
        //            }
        //        }
        //        else
        //        {
        //            var innerText = node.InnerText;
        //            if (!string.IsNullOrWhiteSpace(innerText))
        //            {
        //                texts.Add(innerText + "\n\n");
        //            }
        //        }
        //    }

        //    return texts;
        //}

        private static string LinkToTxt(HtmlNode node)
        {
            var link = node.Attributes["href"].Value;
            var linkLabel = node.InnerText.ToString();

            return linkLabel + ": " + link;
        }

        private static string ConvertTable(HtmlNode node)
        {
            var thColumns = new List<string>();
            var rows = new List<string>();
            var trs = node.SelectNodes("tr");

            // Attempt to parse thead 
            var theadColValues = ParseTHead(node);
            if (theadColValues.Count > 0)
            {
                thColumns.AddRange(theadColValues);
            }

            //Loop through child nodes of table
            if (trs != null && trs.Count > 0)
            {
                foreach (var row in trs)
                {
                    // In the event that thead is not present 
                    // and <th>'s are only under a <tr>  
                    // Loop through th's (column names)
                    var ths = row.SelectNodes("th");
                    if (ths != null)
                    {
                        foreach (var cell in ths)
                        {
                            thColumns.Add(cell.InnerText);
                        }
                    }

                    //Build list of key value pair 
                    var tds = row.SelectNodes("td");
                    if (tds != null)
                    {
                        int idx = 0;
                        var rowValues = new List<string>();
                        foreach (var cell in tds)
                        {
                            var columnValue = thColumns[idx];
                            rowValues.Add(columnValue + ": " + cell.InnerText);
                            ++idx;
                        }
                        rows.Add(string.Join("\n", rowValues));
                    }
                }
            }

            // Attempt to parse tbody
            var tBodyValues = ParseTBody(thColumns, node);
            if (tBodyValues.Count > 0)
            {
                rows.Add(string.Join("\n\n", tBodyValues));
                return string.Join("", rows);
            }

            return string.Join("\n\n", rows);
        }

        // Will return empty array if there is no thead to parse
        private static List<string> ParseTHead(HtmlNode node)
        {
            var thColumns = new List<string>();

            var thead = node.SelectSingleNode("thead");
            if (thead != null)
            {
                var theadTrs = thead.SelectNodes("tr");
                //Loop through child nodes of table
                foreach (var row in theadTrs)
                {
                    var ths = row.SelectNodes("th");
                    if (ths != null)
                    {
                        foreach (var cell in ths)
                        {
                            thColumns.Add(cell.InnerText);
                        }
                    }
                }
            }

            return thColumns;
        }

        // Will return empty array if there is no tbody to parse
        private static List<string> ParseTBody(List<string> thColumns, HtmlNode node)
        {
            var tBodyValues = new List<string>();

            var tbody = node.SelectSingleNode("tbody");
            if (tbody != null)
            {
                var trs = tbody.SelectNodes("tr");
                //Loop through child nodes of table
                foreach (var row in trs)
                {
                    var rowValues = new List<string>();
                    var tds = row.SelectNodes("td");
                    if (tds != null)
                    {
                        int idx = 0;
                        foreach (var cell in tds)
                        {
                            var columnValue = thColumns[idx];
                            rowValues.Add(columnValue + ": " + cell.InnerText);
                            ++idx;
                        }
                    }
                    tBodyValues.Add(string.Join("\n", rowValues));
                }
            }

            return tBodyValues;
        }
    }
}
