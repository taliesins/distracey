using System;
using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmMethodHandlerBaseTests
    {
        private Action<IApmContext, ApmMethodHandlerStartInformation> _startAction;
        private Action<IApmContext, ApmMethodHandlerFinishInformation> _finishAction;

        [SetUp]
        public void Setup()
        {
            _startAction = (context, information) => { };
            _finishAction = (context, information) => { };

            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation).ConfigureAwait(false).GetAwaiter().GetResult();
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TearDown]
        public void TearDown()
        {
            this.Unsubscribe<ApmEvent<ApmMethodHandlerStartInformation>>().ConfigureAwait(false).GetAwaiter().GetResult(); ;
            this.Unsubscribe<ApmEvent<ApmMethodHandlerFinishInformation>>().ConfigureAwait(false).GetAwaiter().GetResult(); ;
        }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            _startAction(apmContext, apmMethodHandlerStartInformation);

            return Task.FromResult(false);
        }

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishInformation>> task)
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
            testApmMethodHandler.OnActionExecuted();

            Assert.IsTrue(finishActionLogged);
        }
    }
}
