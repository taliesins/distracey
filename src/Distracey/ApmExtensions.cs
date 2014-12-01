using System;
using System.Net.Http;

namespace Distracey
{
    public static class ApmExtensions
    {
        public static IApmContext ApmContext(this HttpRequestMessage request)
        {
            object apmContext;
            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContext))
            {
                throw new Exception("Add global filter for ApmWebApiFilterAttributeBase");
            }

            return (IApmContext)apmContext;
        }
    }
}