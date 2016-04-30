using System;

namespace Distracey.MethodHandler
{
    public abstract class ApmMethodHandlerBase
    {
        private readonly IApmContext _apmContext;
        private readonly string _applicationName;
        private readonly Action<IApmContext, ApmMethodHandlerStartInformation> _startAction;
        private readonly Action<IApmContext, ApmMethodHandlerFinishInformation> _finishAction;

        public ApmMethodHandlerBase(IApmContext apmContext, string applicationName, Action<IApmContext, ApmMethodHandlerStartInformation> startAction, Action<IApmContext, ApmMethodHandlerFinishInformation> finishAction)
        {
            _apmContext = apmContext;
            _applicationName = applicationName;
            _startAction = startAction;
            _finishAction = finishAction;
        }

        public ApmMethodHandlerBase InnerHandler { get; set; }

        public void OnActionExecuting()
        {
            //Initialize ApmContext if it does not exist

            LogStartOfRequest(_startAction);

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

            LogStopOfRequest(_finishAction);

            //Dispose ApmContext if it does not exist previously
        }

        private void LogStartOfRequest(Action<IApmContext, ApmMethodHandlerStartInformation> startAction)
        {
            var apmMethodStartInformation = new ApmMethodHandlerStartInformation
            {
                ApplicationName = _applicationName
            };

            startAction(_apmContext, apmMethodStartInformation);
        }

        private void LogStopOfRequest(Action<IApmContext, ApmMethodHandlerFinishInformation> finishAction)
        {
            var apmMethodFinishInformation = new ApmMethodHandlerFinishInformation
            {
                ApplicationName = _applicationName
            };

            //if (!_apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            //{
            //    _apmContext[Constants.TimeTakeMsPropertyKey] = responseTime.ToString();
            //}

            finishAction(_apmContext, apmMethodFinishInformation);
        }
    }
}
