using System;
using System.Configuration;
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

        [HttpGet]
        public CanaryResponse Canary()
        {
            var canaryResponse = CanaryTester.RunAllTests(new Task<ICanary>[]
            {
                new Task<ICanary>(() =>
                {
                    try
                    {
                        var pingResponse = _serviceDepthThreeClient.Ping();

                        return (new LiveCanary
                        {
                            Message = "ServiceDepthThree connectivity passed",
                            Content = pingResponse.ToString()
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

            return canaryResponse;
        }

        [HttpGet]
        public PingResponse Ping()
        {
            return new PingResponse();
        }
    }
}