using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Examples.Website.Clients;
using Distracey.Monitoring;
using Swashbuckle.Swagger;

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

        [HttpGet]
        public CanaryResponse Canary()
        {
            var canaryResponse = CanaryTester.RunAllTests(new Task<ICanary>[]
            {
                new Task<ICanary>(() =>
                {
                    try
                    {
                        var pingResponse = _serviceDepthOneClient.Ping();
                        
                        return (new LiveCanary
                        {
                            Message = "ServiceDepthOne connectivity passed",
                            Content = pingResponse.ToString()
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

            return canaryResponse;
        }

        [HttpGet]
        public PingResponse Ping()
        {
            return new PingResponse();
        }
    }
}