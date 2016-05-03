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
            }
            finally
            {
                apmMethodHandler.OnActionExecuted();
            }
        }

        public static TResult Execute<TResult>(this ApmMethodHandler apmMethodHandler, Func<TResult> func)
        {
            apmMethodHandler.OnActionExecuting();
            try
            {
                return func();
            }
            finally
            {
                apmMethodHandler.OnActionExecuted();
            }
        }
    }
}
