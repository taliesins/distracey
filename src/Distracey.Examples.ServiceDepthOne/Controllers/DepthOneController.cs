using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Common;
using Distracey.Common.Session;
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
            SessionContext.CurrentActivity.Items["id"] = id.ToString();
            return ReadFromFakeDatabaseForDepthOne();
        }

        private IEnumerable<string> ReadFromFakeDatabaseForDepthOne()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "one" });
        }

        public IEnumerable<string> GetDepthTwo(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthTwo(id);
            depth.Add("one");
            return depth;
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthThree(id);
            depth.Add("one");
            return depth;
        }

        public IEnumerable<string> GetDepthOneException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            throw new Exception("one exception");
        }

        public IEnumerable<string> GetDepthTwoException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthTwoException(id);
            depth.Add("one");
            return depth;
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthThreeException(id);
            depth.Add("one");
            return depth;
        }
    }
}