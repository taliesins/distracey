using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Distracey.Examples.ServiceDepthTwo.Clients
{
    public class ServiceDepthThreeClient
    {
        private readonly Uri _baseUrl;

        public ServiceDepthThreeClient(Uri baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public List<string> GetDepthThree(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(context.GetDelegatingHandler()))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("{0}/GetDepthThree", id);
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthThree - {0} {1} {2}", url, response.StatusCode,
                        response.Content.ReadAsStringAsync().Result));
                }

                var results =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(
                        response.Content.ReadAsStringAsync().Result);

                return results;
            }
        }
    }
}