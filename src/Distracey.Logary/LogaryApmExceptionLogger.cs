using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmExceptionLogger : ExceptionLogger
    {
        private readonly Logger _log;

        public LogaryApmExceptionLogger(Logger log)
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

            _log.Log("An unhandled exception occurred.", LogLevel.Error, (IApmContext)null, null, null, context.Exception, null);
            await base.LogAsync(context, cancellationToken);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            _log.Log("An unhandled exception occurred.", LogLevel.Error, (IApmContext)null, null, null, context.Exception, null);
            base.Log(context);
        }
    }
}