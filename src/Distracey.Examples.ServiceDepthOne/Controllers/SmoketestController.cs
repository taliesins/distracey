using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Examples.ServiceDepthOne.Clients;
using Distracey.Monitoring;

namespace Distracey.Examples.ServiceDepthOne.Controllers
{
    public class SmoketestController : ApiController
    {
        private readonly ServiceDepthTwoClient _serviceDepthTwoClient;

        public SmoketestController()
            : this(new ServiceDepthTwoClient(new Uri(ConfigurationManager.AppSettings["ServiceDepthTwoBaseUrl"])))
        {    
        }

        public SmoketestController(ServiceDepthTwoClient serviceDepthTwoClient)
        {
            _serviceDepthTwoClient = serviceDepthTwoClient;
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
                        var pingResponse = _serviceDepthTwoClient.Ping();
                        
                        return (new Canary
                        {
                            Message = "ServiceDepthTwo connectivity passed",
                            Content = pingResponse
                        });
                    }
                    catch (Exception ex)
                    {
                        return (new DeadCanary
                        {
                            Message = "ServiceDepthTwo connectivity failed",
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