using System;
using System.Collections.Generic;
using System.Web.Http;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Common;
using Distracey.Common.Session;

namespace Distracey.Examples.ServiceDepthThree.Controllers
{
    public class DepthThreeController : ApiController
    {
        public IEnumerable<string> GetDepthThree(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            return ReadFromFakeDatabaseForDepthThree();
        }

        private IEnumerable<string> ReadFromFakeDatabaseForDepthThree()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "three" });
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            throw new Exception("three exception");
        }
    }
}