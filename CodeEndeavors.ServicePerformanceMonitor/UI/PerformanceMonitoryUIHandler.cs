using System.Net.Http;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using CodeEndeavors.ServicePerformanceMonitor.Providers;
using CodeEndeavors.ServicePerformanceMonitor.Configs;

namespace CodeEndeavors.ServicePerformanceMonitor.UI
{
    class PerformanceMonitoryUIHandler : HttpMessageHandler
    {
        private readonly PerformanceMonitorUIConfig _config;

        public PerformanceMonitoryUIHandler(PerformanceMonitorUIConfig config)
        {
            _config = config;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uiProvider = _config.GetUIProvider();
            var rootUrl = _config.GetRootUrl(request);
            var assetPath = request.GetRouteData().Values["assetPath"].ToString();

            try
            {
                var webAsset = uiProvider.GetAsset(rootUrl, assetPath);
                var content = ContentFor(webAsset);
                return TaskFor(new HttpResponseMessage { Content = content });
            }
            catch (AssetNotFound ex)
            {
                return TaskFor(request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }
        }

        private HttpContent ContentFor(Asset webAsset)
        {
            var content = new StreamContent(webAsset.Stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(webAsset.MediaType);
            return content;
        }

        private Task<HttpResponseMessage> TaskFor(HttpResponseMessage response)
        {
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }
    }
}
