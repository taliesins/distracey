using System;
using System.Configuration;
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

        [HttpGet]
        public CanaryResponse Canary()
        {
            var canaryResponse = CanaryTester.RunAllTests(new Task<ICanary>[]
            {
                new Task<ICanary>(() =>
                {
                    try
                    {
                        var pingResponse = _serviceDepthTwoClient.Ping();

                        return (new LiveCanary
                        {
                            Message = "ServiceDepthTwo connectivity passed",
                            Content = pingResponse.ToString()
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

            return canaryResponse;
        }

        [HttpGet]
        public PingResponse Ping()
        {
            return new PingResponse();
        }
    }
}