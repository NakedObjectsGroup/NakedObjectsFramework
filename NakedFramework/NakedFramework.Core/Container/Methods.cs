// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Util;
using NakedFramework.Error;
using NakedObjects;

namespace NakedFramework.Core.Container {
    internal static class Methods {
        private static readonly MethodInfo CreateMethod = typeof(LoggerFactoryExtensions).GetMethods().Single(m => m.Name == "CreateLogger" && m.ContainsGenericParameters);
        public static void InjectContainer(object target, object container) => InjectContainer(target, container, new[] {"Container", "DomainObjectContainer", "ProxyContainer"});

        public static void InjectRoot(object root, object inlineObject) {
            var property = inlineObject.GetType().GetProperties().SingleOrDefault(p => p.GetCustomAttribute<RootAttribute>() != null &&
                                                                                       p.PropertyType.IsInstanceOfType(root) &&
                                                                                       p.CanWrite);
            property?.SetValue(inlineObject, root, null);
        }

        public static void InjectServices(object target, object[] services) {
            var properties = target.GetType().GetProperties()
                                   .Where(p => p.CanWrite && p.PropertyType != typeof(object) && p.PropertyType != typeof(object[]));
            foreach (var prop in properties) {
                if (prop.PropertyType.IsArray) {
                    var elementType = prop.PropertyType.GetElementType();
                    var matches = ServicesMatchingType(services, elementType);
                    var count = matches.Length;
                    if (count > 0) {
                        var arr = Array.CreateInstance(elementType, count);
                        matches.CopyTo(arr, 0);
                        prop.SetValue(target, arr, null);
                    }
                }
                else {
                    var matches = ServicesMatchingType(services, prop.PropertyType);
                    var count = matches.Length;
                    if (count > 0) {
                        if (count == 1) {
                            var service = matches[0];
                            prop.SetValue(target, service, null);
                            continue;
                        }

                        var msg = new StringBuilder();
                        msg.Append($"Cannot inject service into property {prop.Name} on target {target.GetType().FullName}" + $" because multiple services implement type {prop.PropertyType}: ");
                        foreach (var serv in matches) {
                            msg.Append(serv.GetType().FullName).Append("; ");
                        }

                        throw new DomainException(msg.ToString());
                    }
                }
            }
        }

        public static void InjectLogger(object target, ILoggerFactory loggerFactory) {
            var targetType = target.GetType();
            var targetProperties = targetType.GetProperties().Where(p => p.CanWrite).ToList();
            targetProperties.Where(p => p.PropertyType == typeof(ILoggerFactory)).ForEach(tf => tf.SetValue(target, loggerFactory));
            var loggerType = typeof(ILogger<>).MakeGenericType(targetType);
            var targetLoggers = targetProperties.Where(p => p.PropertyType == loggerType).ToList();
            if (targetLoggers.Any()) {
                var createLoggerType = CreateMethod.MakeGenericMethod(targetType);
                var logger = createLoggerType.Invoke(null, new object[] {loggerFactory});
                targetLoggers.ForEach(l => l.SetValue(target, logger));
            }
        }

        private static object[] ServicesMatchingType(object[] services, Type type) => services.Where(type.IsInstanceOfType).ToArray();

        private static void InjectContainer(object target, object container, string[] name) {
            var properties = target.GetType().GetProperties().Where(p => name.Contains(p.Name) &&
                                                                         p.PropertyType.IsInstanceOfType(container) &&
                                                                         p.CanWrite);
            foreach (var pi in properties) {
                pi.SetValue(target, container, null);
            }
        }
    }
}