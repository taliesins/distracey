using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<string>> GetDepthOneParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();
            return await ReadFromFakeDatabaseForDepthOneParallel();
        }

        private async  Task<IEnumerable<string>> ReadFromFakeDatabaseForDepthOneParallel()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();
            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "oneA", "oneB", "oneC" });
        }

        public IEnumerable<string> GetDepthTwo(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthTwo(id);
            depth.Add("one");
            return depth;
        }

        public async Task<IEnumerable<string>> GetDepthTwoParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var instance1Task = _serviceDepthTwoClient.GetDepthTwoParallelAsync(id);
            var instance2Task = _serviceDepthTwoClient.GetDepthTwoParallelAsync(id);
            var instance3Task = _serviceDepthTwoClient.GetDepthTwoParallelAsync(id);

            Task.WaitAll(
                instance1Task,
                instance2Task,
                instance3Task
            );

            var result = new List<string>();
            result.AddRange(await instance1Task.ConfigureAwait(false));
            result.Add("oneA");
            result.AddRange(await instance2Task.ConfigureAwait(false));
            result.Add("oneB");
            result.AddRange(await instance3Task.ConfigureAwait(false));
            result.Add("oneC");
            return result;
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthTwoClient.GetDepthThree(id);
            depth.Add("one");
            return depth;
        }

        public async Task<IEnumerable<string>> GetDepthThreeParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var instance1Task = _serviceDepthTwoClient.GetDepthThreeParallelAsync(id);
            var instance2Task = _serviceDepthTwoClient.GetDepthThreeParallelAsync(id);
            var instance3Task = _serviceDepthTwoClient.GetDepthThreeParallelAsync(id);

            Task.WaitAll(
                instance1Task,
                instance2Task,
                instance3Task
            );

            var result = new List<string>();
            result.AddRange(await instance1Task.ConfigureAwait(false));
            result.Add("oneA");
            result.AddRange(await instance2Task.ConfigureAwait(false));
            result.Add("oneB");
            result.AddRange(await instance3Task.ConfigureAwait(false));
            result.Add("oneC");
            return result;
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