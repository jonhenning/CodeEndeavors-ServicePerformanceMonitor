﻿using System.Reflection;

namespace CodeEndeavors.ServicePerformanceMonitor.Providers
{
    public class EmbeddedAssetDescriptor
    {
        public EmbeddedAssetDescriptor(Assembly containingAssembly, string name, bool isTemplate)
        {
            Assembly = containingAssembly;
            Name = name;
            IsTemplate = isTemplate;
        }

        public Assembly Assembly { get; private set; }

        public string Name { get; private set; }

        public bool IsTemplate { get; private set; }
    }
}
