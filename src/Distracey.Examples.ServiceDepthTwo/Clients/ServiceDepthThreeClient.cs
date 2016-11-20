﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Common;
using Distracey.Monitoring;

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

            using (var client = new HttpClient(new ApmHttpClientDelegatingHandler(new HttpClientHandler())))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("api/DepthThree/GetDepthThree/{0}", id);
                var response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthThree - {0} {1} {2}", url, response.StatusCode,
                        response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()));
                }

                var results =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(
                        response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());

                return results;
            }
        }

        public async Task<List<string>> GetDepthThreeParallelAsync(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(new ApmHttpClientDelegatingHandler(new HttpClientHandler())))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("api/DepthThree/GetDepthThreeParallel/{0}", id);
                var response = await client.GetAsync(url).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthThreeParallel - {0} {1} {2}", url, response.StatusCode, await response.Content.ReadAsStringAsync().ConfigureAwait(false)));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                return results;
            }
        }

        public List<string> GetDepthThreeException(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(new ApmHttpClientDelegatingHandler(new HttpClientHandler())))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("api/DepthThree/GetDepthThreeException/{0}", id);
                var response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthThreeException - {0} {1} {2}", url, response.StatusCode,
                        response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()));
                }

                var results =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(
                        response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());

                return results;
            }
        }

        public PingResponse Ping()
        {
            var context = ApmContext.GetContext();
            using (var client = new HttpClient(new ApmHttpClientDelegatingHandler(new HttpClientHandler())))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("api/SmokeTest/Ping");
                var response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("Ping - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<PingResponse>(response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());

                return results;
            }
        }
    }
}