using System;
using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using CodeEndeavors.ServicePerformanceMonitor.Configs;
using CodeEndeavors.ServicePerformanceMonitor.UI;

namespace CodeEndeavors.ServicePerformanceMonitor.Extensions
{
    public static class HttpConfigurationExtensions
    {

        public static CodeEndeavorsServicePerformanceMonitorConfig EnableCodeEndeavorsServicePerformanceMonitor(this HttpConfiguration httpConfig, Action<PerformanceMonitorConfig> configure = null)
        {
            var config = new PerformanceMonitorConfig();
            if (configure != null) configure(config);

            Services.RequestPerformance.Configure(config);
            return new CodeEndeavorsServicePerformanceMonitorConfig(httpConfig, config.GetRootUrl);
        }

        internal static JsonSerializerSettings SerializerSettingsOrDefault(this HttpConfiguration httpConfig)
        {
            var formatter = httpConfig.Formatters.JsonFormatter;
            if (formatter != null)
                return formatter.SerializerSettings;
            return new JsonSerializerSettings();
        }
    }

    public class CodeEndeavorsServicePerformanceMonitorConfig
    {
        private static readonly string DefaultRouteTemplate = "codeendeavors/perfmon";

        private readonly HttpConfiguration _httpConfig;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;

        public CodeEndeavorsServicePerformanceMonitorConfig(HttpConfiguration httpConfig, Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _httpConfig = httpConfig;
            _rootUrlResolver = rootUrlResolver;

            _httpConfig.MessageHandlers.Add(new Http.RequestPerformanceHandler());
        }

        public void EnableUI(Action<PerformanceMonitorUIConfig> configure = null)
        {
            EnableUI(DefaultRouteTemplate, configure);
        }

        public void EnableUI(string routeTemplate, Action<PerformanceMonitorUIConfig> configure = null)
        {

            var config = new PerformanceMonitorUIConfig(_rootUrlResolver);
            if (configure != null) configure(config);

            _httpConfig.Routes.MapHttpRoute(
                name: "codeendeavors_perfmon_statistics",
                routeTemplate: routeTemplate + "/monitor-statistics",
                defaults: new { controller = "MonitorStatistics", action = "Get" });

            _httpConfig.Routes.MapHttpRoute(
                name: "codeendeavors_perfmon_enable",
                routeTemplate: routeTemplate + "/enable",
                defaults: new { controller = "MonitorStatistics", action = "Enable" });

            _httpConfig.Routes.MapHttpRoute(
                name: "codeendeavors_perfmon_disable",
                routeTemplate: routeTemplate + "/disable",
                defaults: new { controller = "MonitorStatistics", action = "Disable" });

            _httpConfig.Routes.MapHttpRoute(
                name: "codeendeavors_perfmon_ui" + routeTemplate,
                routeTemplate: routeTemplate + "/{*assetPath}",
                defaults: null,
                constraints: new { assetPath = @".+" },
                handler: new PerformanceMonitoryUIHandler(config)
            );

            if (routeTemplate == DefaultRouteTemplate)
            {
                _httpConfig.Routes.MapHttpRoute(
                    name: "codeendeavors_perfmon_ui_shortcut",
                    routeTemplate: routeTemplate,
                    defaults: null,
                    constraints: new { uriResolution = new Http.HttpRouteDirectionConstraint(HttpRouteDirection.UriResolution) },
                    handler: new Http.RedirectHandler(_rootUrlResolver, routeTemplate + "/index"));
            }

        }
    }




}
