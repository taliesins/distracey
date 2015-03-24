using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Monitoring;

namespace Distracey.Examples.ServiceDepthThree.Controllers
{
    public class SmoketestController : ApiController
    {
        public SmoketestController()
        {
        }

        [HttpGet]
        public CanaryResponse Canary()
        {
            var canaryResponse = CanaryTester.RunAllTests(new Task<ICanary>[]
            {
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