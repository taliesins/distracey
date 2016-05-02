using System;
using System.Net.Http;

namespace Distracey
{
    public static class ApmExtensions
    {
        public static IApmContext ApmContext(this HttpRequestMessage request)
        {
            object apmContext;
            if (request == null)
            {
                //If people are unit testing request may be null, so just return an instance so there are no problems
                return new ApmContext();
            }

            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContext))
            {
                throw new Exception("Add global filter for ApmWebApiFilterAttribute");
            }

            return (IApmContext)apmContext;
        }
    }
}