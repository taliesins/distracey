using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Distracey.Common;
using Distracey.PerformanceCounter.UnhandledExceptionCounter;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmExceptionLogger : ExceptionLogger
    {
        public PerformanceCounterApmExceptionLogger(string applicationName)
        {
            ApplicationName = applicationName;
        }

        public static string ApplicationName { get; set; }

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IUnhandledExceptionCounter> UnhandledExceptionCounterHandlers = new List<IUnhandledExceptionCounter>()
            {
                new UnhandledExceptionCounterNumberOfOperationsPerSecondHandler("Default", ApplicationName),
                new UnhandledExceptionCounterTotalCountHandler("Default", ApplicationName)
            };

        public async override Task LogAsync(ExceptionLoggerContext context, System.Threading.CancellationToken cancellationToken)
        {
            var apmContext = default(IApmContext);
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContext = (IApmContext) apmContextObject;
            }

            foreach (var counter in UnhandledExceptionCounterHandlers)
            {
                counter.Error(apmContext);
            }
            
            await base.LogAsync(context, cancellationToken).ConfigureAwait(false);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            var apmContext = default(IApmContext);
            object apmContextObject;
            if (context.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContext = (IApmContext)apmContextObject;
            }

            foreach (var counter in UnhandledExceptionCounterHandlers)
            {
                counter.Error(apmContext);
            }

            base.Log(context);
        }

        public static string GetUnhandledExceptionCategoryName(string categoryName)
        {
            return String.Format("{0}-UnhandledException", categoryName);
        }
    }
}