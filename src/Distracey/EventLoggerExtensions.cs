using System.Collections.Generic;

namespace Distracey
{
    public static class EventLoggerExtensions
    {
        public static readonly List<IEventLogger> ApmEventLoggers = new List<IEventLogger>();
    }
}
