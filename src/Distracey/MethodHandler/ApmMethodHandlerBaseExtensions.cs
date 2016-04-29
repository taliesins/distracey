using System;

namespace Distracey.MethodHandler
{
    public static class ApmMethodHandlerBaseExtensions
    {
        public static void Execute(this ApmMethodHandlerBase apmMethodHandlerBase, Action action)
        {

            apmMethodHandlerBase.OnActionExecuting();
            try
            {
                action();
            }
            finally
            {
                apmMethodHandlerBase.OnActionExecuted();
            }
        }

        public static TResult Execute<TResult>(this ApmMethodHandlerBase apmMethodHandlerBase, Func<TResult> func)
        {
            apmMethodHandlerBase.OnActionExecuting();
            try
            {
                return func();
            }
            finally
            {
                apmMethodHandlerBase.OnActionExecuted();
            }
        }
    }
}
