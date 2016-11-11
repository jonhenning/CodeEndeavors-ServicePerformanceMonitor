using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using CodeEndeavors.ServicePerformanceMonitor.Providers;
using System.Reflection;
using System.Text;

namespace CodeEndeavors.ServicePerformanceMonitor.Configs
{
    public class PerformanceMonitorUIConfig
    {
        private readonly Dictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly Dictionary<string, string> _templateParams;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;

        public PerformanceMonitorUIConfig(Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _pathToAssetMap = new Dictionary<string, EmbeddedAssetDescriptor>();

            _templateParams = new Dictionary<string, string>
            {
                { "%(StylesheetIncludes)", "" },
                { "%(CustomScripts)", "" }
            };

            _rootUrlResolver = rootUrlResolver;

            MapPathsForUiAssets();

            var thisAssembly = GetType().Assembly;
            CustomAsset("index", thisAssembly, "CodeEndeavors.ServicePerformanceMonitor.CustomAssets.index.html", isTemplate: true);

        }

        public void InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen", bool isTemplate = false)
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(StylesheetIncludes)"]);
            stringBuilder.AppendLine("<link href='" + path + "' media='" + media + "' rel='stylesheet' type='text/css' />");
            _templateParams["%(StylesheetIncludes)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName, isTemplate);
        }

        public void InjectJavascript(Assembly resourceAssembly, string resourceName, bool isTemplate = false)
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(CustomScripts)"]);
            if (stringBuilder.Length > 0)
                stringBuilder.Append("|");

            stringBuilder.Append(path);
            _templateParams["%(CustomScripts)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName, isTemplate);
        }

        public void CustomAsset(string path, Assembly resourceAssembly, string resourceName, bool isTemplate = false)
        {
            _pathToAssetMap[path] = new EmbeddedAssetDescriptor(resourceAssembly, resourceName, isTemplate);
        }

        internal IAssetProvider GetUIProvider()
        {
            return new EmbeddedAssetProvider(_pathToAssetMap, _templateParams);
        }

        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        private void MapPathsForUiAssets()
        {
            var thisAssembly = GetType().Assembly;
            foreach (var resourceName in thisAssembly.GetManifestResourceNames())
            {
                //if (resourceName.Contains("CodeEndeavors.ServicePerformanceMonitor.CustomAssets")) continue; // original assets only

                var path = resourceName
                    .Replace("CodeEndeavors.ServicePerformanceMonitor.CustomAssets.", "")
                    .Replace("css.", "css/")
                    .Replace("fonts.", "fonts/")
                    .Replace("js.", "js/")
                    .Replace("\\", "/")
                    .Replace(".", "-"); // extensionless to avoid RUMMFAR

                _pathToAssetMap[path] = new EmbeddedAssetDescriptor(thisAssembly, resourceName, path == "index");
            }
        }

    }
}