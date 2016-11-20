using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<string>> GetDepthThreeParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            return await ReadFromFakeDatabaseForDepthThreeParallel().ConfigureAwait(false);
        }

        private async Task<IEnumerable<string>> ReadFromFakeDatabaseForDepthThreeParallel()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "threeA", "threeB", "threeC" });
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            throw new Exception("three exception");
        }
    }
}