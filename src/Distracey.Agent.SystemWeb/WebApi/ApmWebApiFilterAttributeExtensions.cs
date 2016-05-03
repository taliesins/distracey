using System.Linq;
using System.Web.Http;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public static class ApmWebApiFilterAttributeExtensions
    {
        public static void AddApmWebApiFilter(this HttpConfiguration configuration, bool addResponseHeaders)
        {
            if (configuration.Filters.Any(x => x.GetType() == typeof(ApmWebApiFilterAttribute))) return;

            var apmWebApiFilterAttribute = new ApmWebApiFilterAttribute(addResponseHeaders);
            configuration.Filters.Add(apmWebApiFilterAttribute);
        }
    }
}
