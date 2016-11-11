using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeEndeavors.ServicePerformanceMonitor.Models
{
    public class Stat
    {
        public string Name { get; set; }
        public long Duration { get; set; }

        public void Record(long startTicks)
        {
            Duration = (DateTime.UtcNow.Ticks - startTicks) / TimeSpan.TicksPerMillisecond;
        }
    }
}
