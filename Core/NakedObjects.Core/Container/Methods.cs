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
using System;

namespace NakedObjects.Core.Container {
    internal static class Methods {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Methods));

        public static void InjectContainer(object target, object container) {
            InjectContainer(target, container, new[] {"Container", "DomainObjectContainer", "ProxyContainer"});
        }

        public static void InjectRoot(object root, object inlineObject) {
            PropertyInfo property = inlineObject.GetType().GetProperties().SingleOrDefault(p => p.GetCustomAttribute<RootAttribute>() != null &&
                                                                                                p.PropertyType.IsInstanceOfType(root) &&
                                                                                                p.CanWrite);
            if (property != null) {
                property.SetValue(inlineObject, root, null);
                Log.DebugFormat("Injected root {0} into instance of {1}", root, inlineObject.GetType().FullName);
            }
        }

        public static void InjectServices(object target, object[] services) {
            IEnumerable<PropertyInfo> properties = target.GetType().GetProperties()
                .Where(p => p.CanWrite && p.PropertyType != typeof(object) && p.PropertyType != typeof(object[]));
            foreach (var prop in properties) {
                if (prop.PropertyType.IsArray) {
                    var elementType = prop.PropertyType.GetElementType();
                    var matches = ServicesMatchingType(services, elementType);
                    int count = matches.Count();
                    if (count > 0) {
                        Array arr = Array.CreateInstance(elementType, count);
                        matches.CopyTo(arr, 0);
                        prop.SetValue(target, arr, null);
                        Log.DebugFormat("Injected array of {0} services matching {1} into instance of {2}", count, elementType, target.GetType().FullName);
                    }
                } else {
                    var matches = ServicesMatchingType(services, prop.PropertyType);
                    int count = matches.Count();
                    if (count > 0) {
                        if (count == 1) {
                            var service = matches[0];
                            prop.SetValue(target, service, null);
                            Log.DebugFormat("Injected service {0} into instance of {1}", service, target.GetType().FullName);
                            continue;
                        }
                        throw new DomainException(string.Format("Cannot inject service into property {0} on target {1}" +
                        " because there are {2} services implementing type {3}",
                        prop.Name, target.GetType().FullName, count, prop.PropertyType));
                    }
                }
            }
        }

        private static object[] ServicesMatchingType( object[] services, Type type) {
            return services.Where(s => type.IsInstanceOfType(s)).ToArray();
        }

        private static void InjectContainer(object target, object container, string[] name) {
            IEnumerable<PropertyInfo> properties = target.GetType().GetProperties().Where(p => name.Contains(p.Name) &&
                                                                                               p.PropertyType.IsInstanceOfType(container) &&
                                                                                               p.CanWrite);
            foreach (PropertyInfo pi in properties) {
                pi.SetValue(target, container, null);
                Log.DebugFormat("Injected container {0} into instance of {1}", container, target.GetType().FullName);
            }
        }
    }
}