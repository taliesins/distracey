using System.Collections.Generic;

namespace Distracey.Monitoring
{
    public class CanaryResponse
    {
        public List<LiveCanary> LiveCanaries { get; set; }
        public List<DeadCanary> DeadCanaries { get; set; } 
    }
}