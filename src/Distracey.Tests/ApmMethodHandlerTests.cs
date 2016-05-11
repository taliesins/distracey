using System;
using System.Threading.Tasks;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmMethodHandlerTests
    {
        private Action<IApmContext, ApmMethodHandlerStartedMessage> _startAction;
        private Action<IApmContext, ApmMethodHandlerFinishedMessage> _finishAction;

        [SetUp]
        public void Setup()
        {
            _startAction = (context, information) => { };
            _finishAction = (context, information) => { };

            this.Subscribe<ApmEvent<ApmMethodHandlerStartedMessage>>(OnApmMethodHandlerStartInformation).ConfigureAwait(false).GetAwaiter().GetResult();
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>(OnApmMethodHandlerFinishInformation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TearDown]
        public void TearDown()
        {
            this.Unsubscribe<ApmEvent<ApmMethodHandlerStartedMessage>>().ConfigureAwait(false).GetAwaiter().GetResult(); ;
            this.Unsubscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>().ConfigureAwait(false).GetAwaiter().GetResult(); ;
        }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            _startAction(apmContext, apmMethodHandlerStartInformation);

            return Task.FromResult(false);
        }

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerFinishInformation = apmEvent.Event;

            _finishAction(apmContext, apmMethodHandlerFinishInformation);

            return Task.FromResult(false);
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

            var testApmMethodHandler = new ApmMethodHandler(apmContext);
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

            var testApmMethodHandler = new ApmMethodHandler(apmContext);
            testApmMethodHandler.OnActionExecuting();
            testApmMethodHandler.OnActionExecuted(null);

            Assert.IsTrue(finishActionLogged);
        }
    }
}
