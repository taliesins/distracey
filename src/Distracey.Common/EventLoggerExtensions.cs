using System.Collections.Generic;

namespace Distracey.Common
{
    public static class EventLoggerExtensions
    {
        public static readonly List<IEventLogger> ApmEventLoggers = new List<IEventLogger>();
    }
}
