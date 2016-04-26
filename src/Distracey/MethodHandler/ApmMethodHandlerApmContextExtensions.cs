using System.Collections.Generic;
using System.Linq;

namespace Distracey.MethodHandler
{
    public static class ApmMethodHandlerApmContextExtensions
    {
        public static readonly List<IApmMethodHandlerFactory> ApmMethodHttpFactories = new List<IApmMethodHandlerFactory>();

        public static ApmMethodHandlerBase GetMethodHander(this IApmContext apmContext)
        {
            if (!ApmMethodHttpFactories.Any())
            {
                return null;
            }

            ApmMethodHandlerBase apmMethodHandler = null;

            foreach (var apmMethodHttpFactory in ApmMethodHttpFactories)
            {
                var currentApmMethod = apmMethodHttpFactory.Create(apmContext);
                if (apmMethodHandler != null)
                {
                    currentApmMethod.InnerHandler = apmMethodHandler;
                }

                apmMethodHandler = currentApmMethod;
            }

            return apmMethodHandler;
        }
    }
}
