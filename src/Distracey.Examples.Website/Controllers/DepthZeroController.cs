using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Common;
using Distracey.Common.Session;
using Distracey.Examples.Website.Clients;

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
            SessionContext.CurrentActivity.Items["id"] = id.ToString();
            return ReadFromFakeDatabaseForDepthZero();
        }

        private IEnumerable<string> ReadFromFakeDatabaseForDepthZero()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();

            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "zero" });
        }

        public async Task<IEnumerable<string>> GetDepthZeroParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();
            return await ReadFromFakeDatabaseForDepthZeroParallel().ConfigureAwait(false);
        }

        private async Task<IEnumerable<string>> ReadFromFakeDatabaseForDepthZeroParallel()
        {
            var apmContext = ApmContext.GetContext();
            var methodHandler = apmContext.GetMethodHander();

            return methodHandler.Execute<IEnumerable<string>>(() => new[] { "zeroA", "zeroB", "zeroC" });
        }

        public IEnumerable<string> GetDepthOne(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthOne(id);
            depth.Add("zero");
            return depth;
        }

        public async Task<IEnumerable<string>> GetDepthOneParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();
            
            var instance1Task = _serviceDepthOneClient.GetDepthOneParallelAsync(id);
            var instance2Task = _serviceDepthOneClient.GetDepthOneParallelAsync(id);
            var instance3Task = _serviceDepthOneClient.GetDepthOneParallelAsync(id);

            Task.WaitAll(
                instance1Task,
                instance2Task,
                instance3Task
            );

            var result = new List<string>();
            result.AddRange(await instance1Task.ConfigureAwait(false));
            result.Add("zeroA");
            result.AddRange(await instance2Task.ConfigureAwait(false));
            result.Add("zeroB");
            result.AddRange(await instance3Task.ConfigureAwait(false));
            result.Add("zeroC");
            return result;
        }

        public IEnumerable<string> GetDepthTwo(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthTwo(id);
            depth.Add("zero");
            return depth;
        }

        public async Task<IEnumerable<string>> GetDepthTwoParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var instance1Task = _serviceDepthOneClient.GetDepthTwoParallelAsync(id);
            var instance2Task = _serviceDepthOneClient.GetDepthTwoParallelAsync(id);
            var instance3Task = _serviceDepthOneClient.GetDepthTwoParallelAsync(id);

            Task.WaitAll(
                instance1Task,
                instance2Task,
                instance3Task
            );

            var result = new List<string>();
            result.AddRange(await instance1Task.ConfigureAwait(false));
            result.Add("zeroA");
            result.AddRange(await instance2Task.ConfigureAwait(false));
            result.Add("zeroB");
            result.AddRange(await instance3Task.ConfigureAwait(false));
            result.Add("zeroC");
            return result;
        }

        public IEnumerable<string> GetDepthThree(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthThree(id);
            depth.Add("zero");
            return depth;
        }

        public async Task<IEnumerable<string>> GetDepthThreeParallel(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var instance1Task = _serviceDepthOneClient.GetDepthThreeParallelAsync(id);
            var instance2Task = _serviceDepthOneClient.GetDepthThreeParallelAsync(id);
            var instance3Task = _serviceDepthOneClient.GetDepthThreeParallelAsync(id);

            Task.WaitAll(
                instance1Task,
                instance2Task,
                instance3Task
            );

            var result = new List<string>();
            result.AddRange(await instance1Task.ConfigureAwait(false));
            result.Add("zeroA");
            result.AddRange(await instance2Task.ConfigureAwait(false));
            result.Add("zeroB");
            result.AddRange(await instance3Task.ConfigureAwait(false));
            result.Add("zeroC");
            return result;
        }

        public IEnumerable<string> GetDepthZeroException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            throw new Exception("zero exception");
        }

        public IEnumerable<string> GetDepthOneException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthOneException(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthTwoException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthTwoException(id);
            depth.Add("zero");
            return depth;
        }

        public IEnumerable<string> GetDepthThreeException(int id)
        {
            SessionContext.CurrentActivity.Items["id"] = id.ToString();

            var depth = _serviceDepthOneClient.GetDepthThreeException(id);
            depth.Add("zero");
            return depth;
        }
    }
}
