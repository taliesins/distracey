using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Distracey.Common;
using Distracey.Examples.Website.Clients;
using Distracey.MethodHandler;
using Distracey.Web;

namespace Distracey.Examples.Website.Controllers
{
    public class DepthZeroController : ApiController
    {
        private readonly ServiceDepthOneClient _serviceDepthOneClient;

        public DepthZeroController() : this(new ServiceDepthOneClient(new Uri(ConfigurationManager.AppSettings["ServiceDepthOneBaseUrl"])))
        {
            
        }

        public DepthZeroController(ServiceDepthOneClient serviceDepthOneClient)
        {
            _serviceDepthOneClient = serviceDepthOneClient;
        }

        // GET api/values/5
        public IEnumerable<string> GetDepthZero(int id)
        {
            Request.ApmContext()["id"] = id.ToString();
            return ReadFromFakeDatabaseForDepthZero();
        }

        private IEnumerable<string> ReadFromFakeDatabaseForDepthZero()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] {"zero"});
        }

        public IEnumerable<string> GetDepthOne(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthOne(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthTwo(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthTwo(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthThree(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthZeroException(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            throw new Exception("zero exception");
        }

        public IEnumerable<string> GetDepthOneException(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthOneException(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthTwoException(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthTwoException(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            Request.ApmContext()["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthThreeException(id);
            depth.Add("zero");
            return depth;
        }
    }
}
