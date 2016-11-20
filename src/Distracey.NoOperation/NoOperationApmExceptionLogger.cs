using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Distracey.Common;

namespace Distracey.NoOperation
{
    public class NoOperationApmExceptionLogger : ExceptionLogger
    {
        public async override Task LogAsync(ExceptionLoggerContext context, System.Threading.CancellationToken cancellationToken)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            await base.LogAsync(context, cancellationToken).ConfigureAwait(false);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            base.Log(context);
        }
    }
}