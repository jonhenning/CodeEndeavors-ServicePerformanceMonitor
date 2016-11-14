using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeEndeavors.ServicePerformanceMonitor.Models
{
    public class Performance
    {
        private Configs.PerformanceMonitorConfig _config = null;

        private bool _enabled;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                if (_enabled)
                    MonitorStart = DateTimeOffset.Now.ToString();
            }
        }
        public string Version { get; set; }
        public string ApplicationStart { get; set; }
        public string MonitorStart { get; set; }

        public List<Models.Stat> SlowestStats {get; set;}

        private ConcurrentDictionary<string, Models.EndpointStat> _endpointStats; 

        public IEnumerable<Models.EndpointStat> EndpointStats
        {
            get
            {
                return _endpointStats.Values;
            }
        }

        public Performance(Configs.PerformanceMonitorConfig config)
        {
            _config = config;
            Enabled = config.Enabled;

            if (Enabled)
                MonitorStart = DateTimeOffset.Now.ToString();

            Version = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _endpointStats = new ConcurrentDictionary<string, EndpointStat>();
            SlowestStats = new List<Stat>();
        }

        public void AddStat(Models.RequestInstance requestInstance)
        {
            var endpointStat = _endpointStats.GetOrAdd(requestInstance.Endpoint, s => new Models.EndpointStat() { Name = requestInstance.Endpoint });
            var end = DateTime.UtcNow.Ticks;
            var duration = (end - requestInstance.Start) / TimeSpan.TicksPerMillisecond;
            var stat = new Models.Stat() { Name = requestInstance.Endpoint, Duration = duration };

            endpointStat.Record(stat);

            SlowestStats.Add(stat);
            SlowestStats = SlowestStats.OrderByDescending(s => s.Duration).Take(_config.SlowRequestCount).ToList();
        }

    }
}
