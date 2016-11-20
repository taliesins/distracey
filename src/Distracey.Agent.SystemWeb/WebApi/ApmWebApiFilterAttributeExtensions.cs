using System;
using System.Web.Http;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public static class ApmWebApiFilterAttributeExtensions
    {
        private static readonly Lazy<ApmWebApiFilterAttribute> ApmWebApiFilterAttribute = new Lazy<ApmWebApiFilterAttribute>(
            () => new ApmWebApiFilterAttribute(true));

        private static readonly object FilterLock = new object();

        public static void AddApmWebApiFilter(this HttpConfiguration configuration)
        {
            lock (FilterLock)
            {
                if (!ApmWebApiFilterAttribute.IsValueCreated)
                {
                    configuration.Filters.Add(ApmWebApiFilterAttribute.Value);
                }                
            }
        }
    }
}
