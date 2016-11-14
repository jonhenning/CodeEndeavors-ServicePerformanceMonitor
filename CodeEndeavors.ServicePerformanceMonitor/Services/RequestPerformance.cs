using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CodeEndeavors.ServicePerformanceMonitor.Services
{
    public class RequestPerformance
    {
        private static Models.Performance _perf = null;
        private static Configs.PerformanceMonitorConfig _config = null;
        private static DateTimeOffset _applicationStart = DateTime.UtcNow;
        public static bool Enabled
        {
            get
            {
                return _perf.Enabled;
            }
            set
            {
                _perf.Enabled = value;
            }
        }

        public static void Configure(Configs.PerformanceMonitorConfig config)
        {
            _perf = new Models.Performance(config);
            _perf.ApplicationStart = _applicationStart.ToString();
            _config = config;
        }

        public static void Reset()
        {
            _perf = new Models.Performance(_config);
            _perf.ApplicationStart = _applicationStart.ToString();
            _perf.Enabled = true;
        }

        public static Models.Performance Performance
        {
            get
            {
                if (_perf == null)
                    throw new Exception("RequestPeformance not configured");
                return _perf;
            }
        }

        public static Models.RequestInstance RecordStart(HttpRequestMessage request)
        {
            if (Enabled)
                return getRequestInstance(request);
            return null;
        }

        public static void RecordEnd(Models.RequestInstance requestInstance, HttpResponseMessage response)
        {
            if (Enabled && requestInstance != null)
                Performance.AddStat(requestInstance);

            //var headers = task.Result.ToString();
            //var body = task.Result.Content.ReadAsStringAsync().Result;
            //var fullResponse = headers + "\n" + body;
        }

        private static Models.RequestInstance getRequestInstance(HttpRequestMessage request)
        {
            var config = request.GetConfiguration();
            if (config != null && config.Routes != null)
            {
                var routeData = config.Routes.GetRouteData(request);
                if (routeData.Values.ContainsKey("controller") && routeData.Values.ContainsKey("action"))
                {
                    return new Models.RequestInstance()
                    {
                        Endpoint = routeData.Values["controller"] + "/" + routeData.Values["action"],
                        Start = DateTime.UtcNow.Ticks
                    };
                }
            }
            return null;
        }


    }
}
