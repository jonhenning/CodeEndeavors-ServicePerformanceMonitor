using CodeEndeavors.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CodeEndeavors.ServicePerformanceMonitor.Controller
{
    public class MonitorStatisticsController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(string.Format("var performance = {0}", Services.RequestPerformance.Performance.ToJson()), Encoding.UTF8, "text/javascript")
            };
        }

        public HttpResponseMessage Enable()
        {
            Services.RequestPerformance.Enabled = true;
            return new HttpResponseMessage();
        }
        public HttpResponseMessage Disable()
        {
            Services.RequestPerformance.Enabled = false;
            return new HttpResponseMessage();
        }

    }
}
