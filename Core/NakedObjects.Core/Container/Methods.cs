// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Container {
    public static class Methods {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Methods));

        public static void InjectContainer(object target, object container) {
            InjectContainer(target, container, new[] {"Container", "DomainObjectContainer", "ProxyContainer"});
        }

        public static void InjectRoot(object root, object inlineObject) {
            PropertyInfo property = inlineObject.GetType().GetProperties().SingleOrDefault(p => p.GetCustomAttribute<RootAttribute>() != null &&
                                                                                                p.PropertyType.IsAssignableFrom(root.GetType()) &&
                                                                                                p.CanWrite);
            if (property != null) {
                property.SetValue(inlineObject, root, null);
                Log.DebugFormat("Injected root {0} into instance of {1}", root, inlineObject.GetType().FullName);
            }
        }

        public static void InjectServices(object target, object[] services) {
            services.ForEach(s => InjectService(target, s));
        }

        private static void InjectContainer(object target, object container, string[] name) {
            IEnumerable<PropertyInfo> properties = target.GetType().GetProperties().Where(p => name.Contains(p.Name) &&
                                                                                               p.PropertyType.IsAssignableFrom(container.GetType()) &&
                                                                                               p.CanWrite);
            foreach (PropertyInfo pi in properties) {
                pi.SetValue(target, container, null);
                Log.DebugFormat("Injected container {0} into instance of {1}", container, target.GetType().FullName);
            }
        }

        private static void InjectService(object target, object service) {
            IEnumerable<PropertyInfo> properties = target.GetType().GetProperties().Where(p => p.PropertyType != typeof (object) &&
                                                                                               p.PropertyType.IsAssignableFrom(service.GetType()) &&
                                                                                               p.CanWrite);
            foreach (PropertyInfo pi in properties) {
                pi.SetValue(target, service, null);
                Log.DebugFormat("Injected service {0} into instance of {1}", service, target.GetType().FullName);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}