using Distracey.Common.EventAggregator;

namespace Distracey.MethodHandler
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

        public void OnActionExecuted()
        {
            if (InnerHandler != null)
            {
                InnerHandler.OnActionExecuted();
            }

            LogStopOfRequest();

            //Dispose ApmContext if it does not exist previously
        }

        private void LogStartOfRequest()
        {
            var apmMethodStartInformation = new ApmMethodHandlerStartInformation
            {
            };

            var eventContext = new ApmEvent<ApmMethodHandlerStartInformation>
            {
                ApmContext = _apmContext,
                Event = apmMethodStartInformation
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfRequest()
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishInformation
            {
            };

            var eventContext = new ApmEvent<ApmMethodHandlerFinishInformation>
            {
                ApmContext = _apmContext,
                Event = apmMethodFinishInformation
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
