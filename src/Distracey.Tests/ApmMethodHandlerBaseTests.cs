using System;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmMethodHandlerBaseTests
    {
        private string _applicationName;
        private Action<IApmContext, ApmMethodHandlerStartInformation> _startAction;
        private Action<IApmContext, ApmMethodHandlerFinishInformation> _finishAction;

        [SetUp]
        public void Setup()
        {
            _applicationName = "ApplicationName";
            _startAction = (context, information) => { };
            _finishAction = (context, information) => { };
        }

        [Test]
        public void WhenLoggingStartOfRequest()
        {
            var apmContext = ApmContext.GetContext();

            var startActionLogged = false;

            _startAction = (context, information) =>
            {
                startActionLogged = true;
            };

            var testApmMethodHandler = new TestApmMethodHandler(apmContext, _applicationName, _startAction, _finishAction);
            testApmMethodHandler.OnActionExecuting();

            Assert.IsTrue(startActionLogged);
        }

        [Test]
        public void WhenLoggingEndOfRequest()
        {
            var apmContext = ApmContext.GetContext();

            var finishActionLogged = false;

            _finishAction = (context, information) =>
            {
                finishActionLogged = true;
            };

            var testApmMethodHandler = new TestApmMethodHandler(apmContext, _applicationName, _startAction, _finishAction);
            testApmMethodHandler.OnActionExecuting();
            testApmMethodHandler.OnActionExecuted();

            Assert.IsTrue(finishActionLogged);
        }
    }
}
