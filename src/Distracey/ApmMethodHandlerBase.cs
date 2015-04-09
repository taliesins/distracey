using System;

namespace Distracey
{
    public abstract class ApmMethodHandlerBase
    {
        private readonly string _applicationName;
        private readonly Action<IApmContext, ApmMethodHandlerStartInformation> _startAction;
        private readonly Action<IApmContext, ApmMethodHandlerFinishInformation> _finishAction;

        public ApmMethodHandlerBase(string applicationName, Action<IApmContext, ApmMethodHandlerStartInformation> startAction, Action<IApmContext, ApmMethodHandlerFinishInformation> finishAction)
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

        public void LogStartOfRequest(IApmContext apmContext, Action<IApmContext, ApmMethodHandlerStartInformation> startAction)
        {
            var apmMethodStartInformation = new ApmMethodHandlerStartInformation
            {
                ApplicationName = _applicationName
            };

            if (InnerHandler != null)
            {
                InnerHandler.LogStartOfRequest(apmContext);
            }

            startAction(apmContext, apmMethodStartInformation);
        }

        public void LogStopOfRequest(IApmContext apmContext)
        {
            LogStopOfRequest(apmContext, _finishAction);
        }

        public void LogStopOfRequest(IApmContext apmContext, Action<IApmContext, ApmMethodHandlerFinishInformation> finishAction)
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishInformation
            {
                ApplicationName = _applicationName
            };

            //if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            //{
            //    apmContext[Constants.TimeTakeMsPropertyKey] = responseTime.ToString();
            //}

            finishAction(apmContext, apmMethodFinishInformation);

            if (InnerHandler != null)
            {
                InnerHandler.LogStopOfRequest(apmContext);
            }
        }
    }
}
