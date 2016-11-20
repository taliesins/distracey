using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Logary;
using Logary.CSharp;

namespace Distracey.Logary
{
    public class LogaryApmExceptionLogger : ExceptionLogger
    {
        private readonly Logger _log;

        public LogaryApmExceptionLogger(Logger log)
        {
            _log = log;
        }

        public override async Task LogAsync(ExceptionLoggerContext context, System.Threading.CancellationToken cancellationToken)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Common.Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            await _log.LogEvent(LogLevel.Error, "An unhandled exception occurred.", new
            {
            }, exn: context.Exception).ConfigureAwait(false);

            await base.LogAsync(context, cancellationToken).ConfigureAwait(false);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Common.Constants.ApmContextPropertyKey, out apmContextObject))
            {
                return;
            }

            _log.LogEvent(LogLevel.Error, "An unhandled exception occurred.", new
            {
            }, exn: context.Exception).ConfigureAwait(false).GetAwaiter().GetResult();

            base.Log(context);
        }
    }
}