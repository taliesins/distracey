using System;
using Distracey.Common;
using Distracey.Common.Message;

namespace Distracey.Agent.Core.MethodHandler
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
            var apmMethodStartInformation = new ApmMethodHandlerStartedMessage()
                .AsMessage(_apmContext);

            apmMethodStartInformation.PublishMessage(_apmContext, this);
        }

        private void LogStopOfRequest(Exception exception)
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishedMessage
            {
                Exception = exception,
            }.AsMessage(_apmContext);

            apmMethodFinishInformation.PublishMessage(_apmContext, this);
        }
    }
}
