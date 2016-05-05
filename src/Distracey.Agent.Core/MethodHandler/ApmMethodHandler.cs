using System;
using Distracey.Common;
using Distracey.Common.EventAggregator;

namespace Distracey.Agent.Common.MethodHandler
{
    public class ApmMethodHandler
    {
        private readonly IApmContext _apmContext;

        public ApmMethodHandler(IApmContext apmContext)
        {
            _apmContext = apmContext;
        }

        public ApmMethodHandler InnerHandler { get; set; }

        public void OnActionExecuting()
        {
            //Initialize ApmContext if it does not exist

            LogStartOfRequest();

            if (InnerHandler != null)
            {
                InnerHandler.OnActionExecuting();
            }
        }

        public void OnActionExecuted(Exception exception)
        {
            if (InnerHandler != null)
            {
                InnerHandler.OnActionExecuted(exception);
            }

            LogStopOfRequest(exception);

            //Dispose ApmContext if it does not exist previously
        }

        private void LogStartOfRequest()
        {
            var apmMethodStartInformation = new ApmMethodHandlerStartedMessage
            {
            };

            var eventContext = new ApmEvent<ApmMethodHandlerStartedMessage>
            {
                ApmContext = _apmContext,
                Event = apmMethodStartInformation
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfRequest(Exception exception)
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishedMessage
            {
                Exception = exception
            };

            var eventContext = new ApmEvent<ApmMethodHandlerFinishedMessage>
            {
                ApmContext = _apmContext,
                Event = apmMethodFinishInformation
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
