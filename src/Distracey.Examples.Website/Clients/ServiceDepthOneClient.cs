﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Distracey.Examples.Website.Clients
{
    public class ServiceDepthOneClient
    {
        private readonly Uri _baseUrl;

        public ServiceDepthOneClient(Uri baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public List<string> GetDepthOne(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(context.GetDelegatingHandler()))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("{0}/GetDepthOne", id);
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthOne - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

                return results;
            }
        }

        public List<string> GetDepthTwo(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(context.GetDelegatingHandler()))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("{0}/GetDepthTwo", id);
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthTwo - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

                return results;
            }
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
                    throw new Exception(string.Format("GetDepthThree - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

                return results;
            }
        }


        public List<string> GetDepthOneException(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(context.GetDelegatingHandler()))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("{0}/GetDepthOneException", id);
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthOneException - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

                return results;
            }
        }

        public List<string> GetDepthTwoException(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(context.GetDelegatingHandler()))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("{0}/GetDepthTwoException", id);
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthTwoException - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

                return results;
            }
        }

        public List<string> GetDepthThreeException(int id)
        {
            var context = ApmContext.GetContext();
            context["id"] = id.ToString();

            using (var client = new HttpClient(context.GetDelegatingHandler()))
            {
                client.BaseAddress = _baseUrl;
                var url = string.Format("{0}/GetDepthThreeException", id);
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("GetDepthThreeException - {0} {1} {2}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result));
                }

                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

                return results;
            }
        }
    }
}