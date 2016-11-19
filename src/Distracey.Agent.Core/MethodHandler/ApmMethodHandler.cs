using System;
using System.Diagnostics;
using Distracey.Common;
using Distracey.Common.Message;
using Distracey.Common.Timer;

namespace Distracey.Agent.Core.MethodHandler
{
    public class ApmMethodHandler
    {
        private readonly IApmContext _apmContext;
        private readonly IExecutionTimer _executionTimer;
        private TimeSpan _offset;

        public ApmMethodHandler(IApmContext apmContext)
        {
            _apmContext = apmContext;
            _executionTimer = new ExecutionTimer(new Stopwatch());
        }

        public ApmMethodHandler InnerHandler { get; set; }

        public void OnActionExecuting()
        {
            ApmContext.StartActivityClientSend(_apmContext);

            //Initialize ApmContext if it does not exist
            _offset = _executionTimer.Start();

            LogStartOfRequest(_apmContext, _offset);

            if (InnerHandler != null)
            {
                InnerHandler.OnActionExecuting();
            }
        }

        public void OnActionExecuted(Exception exception)
        {
            try
            {
                if (InnerHandler != null)
                {
                    InnerHandler.OnActionExecuted(exception);
                }

                LogStopOfRequest(_apmContext, _offset, exception);
            }
            finally
            {
                ApmContext.StopActivityClientReceived();
            }

            //Dispose ApmContext if it does not exist previously
        }

        private void LogStartOfRequest(IApmContext apmContext, TimeSpan offset)
        {
            var apmMethodStartInformation = new ApmMethodHandlerStartedMessage()
                .AsMessage(apmContext)
                .AsTimedMessage(offset);

            apmMethodStartInformation.PublishMessage(apmContext, this);
        }

        private void LogStopOfRequest(IApmContext apmContext, TimeSpan offset, Exception exception)
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishedMessage
            {
                Exception = exception,
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmMethodFinishInformation.PublishMessage(apmContext, this);         
        }
    }
}
