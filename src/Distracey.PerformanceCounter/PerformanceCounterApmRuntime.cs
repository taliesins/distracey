using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using Distracey.Reflection;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmRuntime
    {
        /// <summary>
        /// Uninstalls performance counters in the current assembly using PerfItFilterAttribute.
        /// </summary>
        /// <param name="installerAssembly"></param>
        /// <param name="categoryName">if you have provided a categoryName for the installation, you must supply the same here</param>
        public static void Uninstall(Assembly installerAssembly, string categoryName = null)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = installerAssembly.GetName().Name;
            }

            var httpActionDescriptorCategoryName = PerformanceCounterApmApiFilterAttribute.GetCategoryName(categoryName);

            try
            {
                if (PerformanceCounterCategory.Exists(httpActionDescriptorCategoryName))
                {
                    PerformanceCounterCategory.Delete(httpActionDescriptorCategoryName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var httpClientCategoryName = PerformanceCounterApmHttpClientDelegatingHandler.GetCategoryName(categoryName);
            try
            {
                if (PerformanceCounterCategory.Exists(httpClientCategoryName))
                {
                    PerformanceCounterCategory.Delete(httpClientCategoryName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Installs performance counters in the current assembly using PerfItFilterAttribute.
        /// </summary>
        /// <param name="installerAssembly"></param>
        /// <param name="categoryName">category name for the metrics. If not provided, it will use the assembly name</param>
        public static void Install(Assembly installerAssembly, string categoryName = null)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = installerAssembly.GetName().Name;
            }

            Uninstall(installerAssembly, categoryName);

            var httpActionDescriptors = FindAllHttpActionDescriptors(installerAssembly).Distinct().ToArray();
            var httpActionDescriptorCounters = GetCounterCreationDataCollectionForHttpActionDescriptors(httpActionDescriptors);
            var httpActionDescriptorCategoryName = PerformanceCounterApmApiFilterAttribute.GetCategoryName(categoryName);
            PerformanceCounterCategory.Create(httpActionDescriptorCategoryName, "APM api filter category for " + categoryName, PerformanceCounterCategoryType.MultiInstance, httpActionDescriptorCounters);
            Trace.TraceInformation("Built category '{0}' with {1} items", httpActionDescriptorCategoryName, httpActionDescriptorCounters.Count);

            var apmContextUsage = FindAllApmContextUsage(installerAssembly).Distinct().ToArray();
            var apmContextUsageCounters = GetCounterCreationDataCollectionForApmContextUsage(apmContextUsage);
            var httpClientCategoryName = PerformanceCounterApmHttpClientDelegatingHandler.GetCategoryName(categoryName);
            PerformanceCounterCategory.Create(httpClientCategoryName, "APM http client category for " + categoryName, PerformanceCounterCategoryType.MultiInstance, apmContextUsageCounters);
            Trace.TraceInformation("Built category '{0}' with {1} items", httpClientCategoryName, apmContextUsageCounters.Count);
        }

        private static CounterCreationDataCollection GetCounterCreationDataCollectionForHttpActionDescriptors(ReflectedHttpActionDescriptor[] httpActionDescriptors)
        {
            var counterCreationDataCollection = new CounterCreationDataCollection();

            Trace.TraceInformation("Number of controller actions: {0}", httpActionDescriptors.Length);

            foreach (var httpActionDescriptor in httpActionDescriptors)
            {
                var methodType = httpActionDescriptor.SupportedHttpMethods.First();

                var controllerName = httpActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = httpActionDescriptor.ActionName;

                var param = httpActionDescriptor.MethodInfo.GetParameters()
                 .Select(parameter => string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name))
                 .ToArray();

                var arguments = string.Join(", ", param);

                var methodIdentifier = ApmWebApiFilterAttributeBase.GetMethodIdentifier(methodType, controllerName, actionName, arguments);
                var eventName = ApmWebApiFilterAttributeBase.GetEventName(methodType, actionName, controllerName);

                Trace.TraceInformation("Setting up controller action '{0}' for event '{1}'", methodIdentifier, eventName);

                //Setup action performance counters
                foreach (var counterHandler in PerformanceCounterApmApiFilterAttribute.CounterHandlers)
                {
                    if (counterCreationDataCollection.Cast<CounterCreationData>().Any(x => x.CounterName == methodIdentifier))
                    {
                        Trace.TraceInformation("Counter for method '{0}' was duplicate", methodIdentifier);
                    }
                    else
                    {
                        var countersToCreate = counterHandler.GetCreationData(methodIdentifier);
                        foreach (var counterToCreate in countersToCreate)
                        {
                            Trace.TraceInformation("Added counter for method '{0}'", counterToCreate.CounterName);    
                        }
                        
                        counterCreationDataCollection.AddRange(countersToCreate);
                    }
                }
            }
            return counterCreationDataCollection;
        }

        /// <summary>
        /// Extracts all filters in the current assembly defined on ApiControllers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ReflectedHttpActionDescriptor> FindAllHttpActionDescriptors(Assembly assembly)
        {
            var httpActionDescriptors = new List<ReflectedHttpActionDescriptor>();
            var apiControllers = assembly.GetExportedTypes()
                                        .Where(t => typeof(ApiController).IsAssignableFrom(t) && !t.IsAbstract).ToArray();

            Trace.TraceInformation("Found '{0}' controllers", apiControllers.Length);

            foreach (var apiController in apiControllers)
            {
                var methodInfos = apiController.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Where(x=>!x.IsAbstract);

                foreach (var methodInfo in methodInfos)
                {
                    var controllerNameString = apiController.Name;
                    controllerNameString = controllerNameString.Substring(0, controllerNameString.Length - "Controller".Length);

                    var httpConfiguration = new HttpConfiguration();
                    var httpControllerDescriptor = new HttpControllerDescriptor(httpConfiguration, controllerNameString, apiController);
                    var httpActionDescriptor = new ReflectedHttpActionDescriptor(httpControllerDescriptor, methodInfo);

                    httpActionDescriptors.Add(httpActionDescriptor);
                }
            }

            return httpActionDescriptors;
        }

        private static CounterCreationDataCollection GetCounterCreationDataCollectionForApmContextUsage(MethodInfo[] apmContextUsages)
        {
            var counterCreationDataCollection = new CounterCreationDataCollection();

            Trace.TraceInformation("Number of get context uses actions: {0}", apmContextUsages.Length);

            foreach (var apmContextUsage in apmContextUsages)
            {
                var methodIdentifier = ApmHttpClientDelegatingHandlerBase.GetMethodIdentifier(apmContextUsage);
                var eventName = ApmHttpClientDelegatingHandlerBase.GetEventName(apmContextUsage);

                Trace.TraceInformation("Setting up get context uses '{0}' for event '{1}'", methodIdentifier, eventName);

                //Setup action performance counters
                foreach (var counterHandler in PerformanceCounterApmHttpClientDelegatingHandler.CounterHandlers)
                {
                    if (counterCreationDataCollection.Cast<CounterCreationData>().Any(x => x.CounterName == methodIdentifier))
                    {
                        Trace.TraceInformation("Counter for method '{0}' was duplicate", methodIdentifier);
                    }
                    else
                    {
                        var countersToCreate = counterHandler.GetCreationData(methodIdentifier);
                        foreach (var counterToCreate in countersToCreate)
                        {
                            Trace.TraceInformation("Added counter for method '{0}'", counterToCreate.CounterName);
                        }

                        counterCreationDataCollection.AddRange(countersToCreate);
                    }
                }
            }

            return counterCreationDataCollection;
        }

        /// <summary>
        /// Extract all methods that make use of GetContext method
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> FindAllApmContextUsage(Assembly assembly)
        {
            var methodsToScan = assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                .Where(methodInfo => MethodBodyReader.GetInstructions(methodInfo)
                    .Any(instruction => instruction.Operand is MethodInfo && (instruction.Operand as MethodInfo).Name == "GetContext" && typeof(ApmContext).IsAssignableFrom((instruction.Operand as MethodInfo).DeclaringType))
                );

            return methodsToScan;
        }
    }
}