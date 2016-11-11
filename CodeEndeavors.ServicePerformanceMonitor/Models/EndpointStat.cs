using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeEndeavors.ServicePerformanceMonitor.Models
{
    public class EndpointStat
    {
        private int _count = 0;
        public string Name { get; set; }
        public int Count
        {
            get
            {
                return _count;
            }
        }
        public void Record(Models.Stat stat)
        {
            Interlocked.Increment(ref _count);
        }

    }
}
