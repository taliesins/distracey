using System;

namespace Distracey.Agent.Common.MethodHandler
{
    public static class ApmMethodHandlerExtensions
    {
        public static void Execute(this ApmMethodHandler apmMethodHandler, Action action)
        {
            apmMethodHandler.OnActionExecuting();
            try
            {
                action();
                apmMethodHandler.OnActionExecuted(null);
            }
            catch (Exception exception)
            {
                apmMethodHandler.OnActionExecuted(exception);
                throw;
            }
        }

        public static TResult Execute<TResult>(this ApmMethodHandler apmMethodHandler, Func<TResult> func)
        {
            apmMethodHandler.OnActionExecuting();
            try
            {
                var result = func();
                apmMethodHandler.OnActionExecuted(null);
                return result;
            }
            catch (Exception exception)
            {
                apmMethodHandler.OnActionExecuted(exception);
                throw;
            }
        }
    }
}