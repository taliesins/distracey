using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distracey.Monitoring
{
    public static class CanaryTester
    {
        public static CanaryResponse RunAllTests(params Task<Canary>[] testFunctionsToRun)
        {
            var success = new List<LiveCanary>();
            var errors = new List<DeadCanary>();

            Parallel.ForEach(testFunctionsToRun, testFunction =>
            {
                if (testFunction.Status == TaskStatus.Created)
                {
                    testFunction.Start();
                }
                Canary canary;
                try
                {
                    canary = testFunction.Result;
                }
                catch (Exception ex)
                {
                    canary = new DeadCanary
                    {
                        ExceptionDetails = ex.ToString()
                    };
                }
                var deadCanary = canary as DeadCanary;
                if (deadCanary != null)
                {
                    errors.Add(deadCanary);
                }
                else
                {
                    var liveCanary = canary as LiveCanary;
                    if (liveCanary != null)
                    {
                        success.Add(liveCanary);
                    }
                    else
                    {
                        var list = errors;
                        var errorMessage = new DeadCanary
                        {
                            Message = string.Format("Type of object {0} is not supported as message type.", canary)
                        };
                        list.Add(errorMessage);
                    }
                }
            });
            return new CanaryResponse
            {
                DeadCanaries = errors,
                LiveCanaries = success
            };
        }
    }
}