using System.Net;
using System.Net.Http;
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

        [HttpGet, Route("canary")]
        public HttpResponseMessage Canary()
        {
            var canaryResponse = CanaryTester.RunAllTests(new Task<Canary>[]
            {
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