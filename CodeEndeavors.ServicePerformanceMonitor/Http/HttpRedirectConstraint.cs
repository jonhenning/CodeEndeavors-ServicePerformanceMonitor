using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace CodeEndeavors.ServicePerformanceMonitor.Http
{
    public class HttpRouteDirectionConstraint : IHttpRouteConstraint
    {
        private readonly HttpRouteDirection _allowedDirection;

        public HttpRouteDirectionConstraint(HttpRouteDirection allowedDirection)
        {
            _allowedDirection = allowedDirection;
        }

        public bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            return routeDirection == _allowedDirection;
        }
    }
}
