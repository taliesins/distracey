using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Examples.Website.Clients;
using Distracey.Monitoring;

namespace Distracey.Examples.Website.Controllers
{
    public class SmoketestController : ApiController
    {
        private readonly ServiceDepthOneClient _serviceDepthOneClient;

        public SmoketestController()
            : this(new ServiceDepthOneClient(new Uri(ConfigurationManager.AppSettings["ServiceDepthOneBaseUrl"])))
        {    
        }

        public SmoketestController(ServiceDepthOneClient serviceDepthOneClient)
        {
            _serviceDepthOneClient = serviceDepthOneClient;
        }

        [HttpGet, Route("canary")]
        public HttpResponseMessage Canary()
        {
            var canaryResponse = CanaryTester.RunAllTests(new Task<Canary>[]
            {
                new Task<Canary>(() =>
                {
                    try
                    {
                        var pingResponse = _serviceDepthOneClient.Ping();
                        
                        return (new Canary
                        {
                            Message = "ServiceDepthOne connectivity passed",
                            Content = pingResponse
                        });
                    }
                    catch (Exception ex)
                    {
                        return (new DeadCanary
                        {
                            Message = "ServiceDepthOne connectivity failed",
                            ExceptionDetails = ex.ToString()
                        });
                    }
                }), 
            });

            return Request.CreateResponse(HttpStatusCode.OK, canaryResponse);
        }

        [HttpGet, Route("ping")]
        public HttpResponseMessage Ping()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}