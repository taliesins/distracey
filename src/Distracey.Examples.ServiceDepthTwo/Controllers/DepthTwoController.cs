using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Common;
using Distracey.Common.Session;
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
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            return ReadFromFakeDatabaseForDepthTwo();
        }

        private IEnumerable<string> ReadFromFakeDatabaseForDepthTwo()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "two" });
        }

        public async  Task<IEnumerable<string>> GetDepthTwoParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            return await ReadFromFakeDatabaseForDepthTwoParallel().ConfigureAwait(false);
        }

        private async Task<IEnumerable<string>> ReadFromFakeDatabaseForDepthTwoParallel()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "twoA", "twoB", "twoC" });
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthThreeClient.GetDepthThree(id);
            depth.Add("two");
            return depth;
        }

        public async Task<IEnumerable<string>> GetDepthThreeParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var instance1Task = _serviceDepthThreeClient.GetDepthThreeParallelAsync(id);
            var instance2Task = _serviceDepthThreeClient.GetDepthThreeParallelAsync(id);
            var instance3Task = _serviceDepthThreeClient.GetDepthThreeParallelAsync(id);

            Task.WaitAll(
                instance1Task,
                instance2Task,
                instance3Task
            );

            var result = new List<string>();
            result.AddRange(await instance1Task.ConfigureAwait(false));
            result.Add("twoA");
            result.AddRange(await instance2Task.ConfigureAwait(false));
            result.Add("twoB");
            result.AddRange(await instance3Task.ConfigureAwait(false));
            result.Add("twoC");
            return result;
        }

        public IEnumerable<string> GetDepthTwoException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            throw new Exception("two exception");
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthThreeClient.GetDepthThreeException(id);
            depth.Add("two");
            return depth;
        }
    }
}