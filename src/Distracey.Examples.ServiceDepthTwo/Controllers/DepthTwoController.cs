using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Agent.SystemWeb;
using Distracey.Common;
using Distracey.Examples.ServiceDepthTwo.Clients;

namespace Distracey.Examples.ServiceDepthTwo.Controllers
{
    public class DepthTwoController : ApiController
    {
        private readonly ServiceDepthThreeClient _serviceDepthThreeClient;

        public DepthTwoController()
            : this(new ServiceDepthThreeClient(new Uri(ConfigurationManager.AppSettings["ServiceDepthThreeBaseUrl"])))
        {
            
        }

        public DepthTwoController(ServiceDepthThreeClient serviceDepthThreeClient)
        {
            _serviceDepthThreeClient = serviceDepthThreeClient;
        }

        public IEnumerable<string> GetDepthTwo(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            return ReadFromFakeDatabaseForDepthTwo();
        }

        private IEnumerable<string> ReadFromFakeDatabaseForDepthTwo()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "two" });
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthThreeClient.GetDepthThree(id);
            depth.Add("two");
            return depth;
        }

        public IEnumerable<string> GetDepthTwoException(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            throw new Exception("two exception");
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthThreeClient.GetDepthThreeException(id);
            depth.Add("two");
            return depth;
        }
    }
}