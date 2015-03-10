using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Examples.ServiceDepthTwo.Clients;
using Distracey.Monitoring;

namespace Distracey.Examples.ServiceDepthTwo.Controllers
{
    public class SmoketestController : ApiController
    {
        private readonly ServiceDepthThreeClient _serviceDepthThreeClient;

        public SmoketestController()
            : this(new ServiceDepthThreeClient(new Uri(ConfigurationManager.AppSettings["ServiceDepthThreeBaseUrl"])))
        {    
        }

        public SmoketestController(ServiceDepthThreeClient serviceDepthThreeClient)
        {
            _serviceDepthThreeClient = serviceDepthThreeClient;
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
                        var pingResponse = _serviceDepthThreeClient.Ping();
                        
                        return (new Canary
                        {
                            Message = "ServiceDepthThree connectivity passed",
                            Content = pingResponse
                        });
                    }
                    catch (Exception ex)
                    {
                        return (new DeadCanary
                        {
                            Message = "ServiceDepthThree connectivity failed",
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