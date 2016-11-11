using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;

namespace CodeEndeavors.ServicePerformanceMonitor.Configs
{
    public class PerformanceMonitorConfig
    {
        public bool Enabled { get; set; }
        public int SlowRequestCount {get;set;}
        private Func<HttpRequestMessage, string> _rootUrlResolver;

        public PerformanceMonitorConfig()
        {
            SlowRequestCount = 10;
            _rootUrlResolver = DefaultRootUrlResolver;
        }

        public void SlowestRequestCount(int count)
        {
            SlowRequestCount = count;
        }
        public void Enable(bool enabled)
        {
            Enabled = enabled;
        }

        public void RootUrl(Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _rootUrlResolver = rootUrlResolver;
        }

        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        public static string DefaultRootUrlResolver(HttpRequestMessage request)
        {
            var scheme = GetHeaderValue(request, "X-Forwarded-Proto") ?? request.RequestUri.Scheme;
            var host = GetHeaderValue(request, "X-Forwarded-Host") ?? request.RequestUri.Host;
            var port = GetHeaderValue(request, "X-Forwarded-Port") ?? request.RequestUri.Port.ToString(CultureInfo.InvariantCulture);

            var httpConfiguration = request.GetConfiguration();
            var virtualPathRoot = httpConfiguration.VirtualPathRoot.TrimEnd('/');

            return string.Format("{0}://{1}:{2}{3}", scheme, host, port, virtualPathRoot);
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> list;
            return request.Headers.TryGetValues(headerName, out list) ? list.FirstOrDefault() : null;
        }
    }
}