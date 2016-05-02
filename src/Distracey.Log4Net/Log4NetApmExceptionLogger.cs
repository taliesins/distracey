using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Distracey.Common;
using log4net;

namespace Distracey.Log4Net
{
    public class Log4NetApmExceptionLogger : ExceptionLogger
    {
        private readonly ILog _log;

        public Log4NetApmExceptionLogger(ILog log)
        {
            _log = log;
        }

        public async override Task LogAsync(ExceptionLoggerContext context, System.Threading.CancellationToken cancellationToken)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            _log.Error("An unhandled exception occurred.", context.Exception);
            await base.LogAsync(context, cancellationToken);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            _log.Error("An unhandled exception occurred.", context.Exception);
            base.Log(context);
        }
    }
}