using System;

namespace Distracey
{
    public abstract class ApmMethodHandlerBase
    {
        private readonly string _applicationName;
        private readonly Action<ApmMethodHandlerStartInformation> _startAction;
        private readonly Action<ApmMethodHandlerFinishInformation> _finishAction;

        public ApmMethodHandlerBase(string applicationName, Action<ApmMethodHandlerStartInformation> startAction, Action<ApmMethodHandlerFinishInformation> finishAction)
        {
            _applicationName = applicationName;
            _startAction = startAction;
            _finishAction = finishAction;
        }

        public ApmMethodHandlerBase InnerHandler { get; set; }

        public void LogStartOfRequest(IApmContext apmContext)
        {
            LogStartOfRequest(apmContext, _startAction);
        }

        public void LogStartOfRequest(IApmContext apmContext, Action<ApmMethodHandlerStartInformation> startAction)
        {
            var apmMethodStartInformation = new ApmMethodHandlerStartInformation
            {
                ApplicationName = _applicationName
            };

            if (InnerHandler != null)
            {
                InnerHandler.LogStartOfRequest(apmContext);
            }

            startAction(apmMethodStartInformation);
        }

        public void LogStopOfRequest(IApmContext apmContext)
        {
            LogStopOfRequest(apmContext, _finishAction);
        }

        public void LogStopOfRequest(IApmContext apmContext, Action<ApmMethodHandlerFinishInformation> finishAction)
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishInformation
            {
                ApplicationName = _applicationName
            };

            finishAction(apmMethodFinishInformation);

            if (InnerHandler != null)
            {
                InnerHandler.LogStopOfRequest(apmContext);
            }
        }
    }
}
