using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Distracey.Examples.ServiceDepthOne.Clients;

namespace Distracey.Examples.ServiceDepthOne.Controllers
{
    public class DepthOneController : ApiController
    {
        private readonly ServiceDepthTwoClient _serviceDepthTwoClient;

        public DepthOneController()
            : this(new ServiceDepthTwoClient(new Uri(ConfigurationManager.AppSettings["ServiceDepthTwoBaseUrl"])))
        {
            
        }

        public DepthOneController(ServiceDepthTwoClient serviceDepthTwoClient)
        {
            _serviceDepthTwoClient = serviceDepthTwoClient;
        }

        public IEnumerable<string> GetDepthOne(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            return new[] { "one" };
        }

        public IEnumerable<string> GetDepthTwo(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthTwo(id);
            depth.Add("one");
            return depth;
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthThree(id);
            depth.Add("one");
            return depth;
        }
    }
}