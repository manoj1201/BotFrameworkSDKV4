using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Search
{
    public class APICaller
    {
        public string GetDocument(string qry)
        {
            HttpClient httpClient = new HttpClient();
            var baseUrl = "http://documentsearchapi.azurewebsites.net/";
            var route = "api/document?query=" + qry;
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            string responseString = string.Empty;
            var response = httpClient.GetAsync(route).Result;
            if (response.IsSuccessStatusCode)
            {
                responseString = response.Content.ReadAsStringAsync().Result;
            }
            return responseString;
        }
    }
}
