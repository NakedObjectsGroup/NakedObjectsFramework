// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedFunctions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Utils {
    public static class InjectUtils {

        private static readonly Random Random = new Random();

        public static DateTime GetInjectedDateTimeValue() => DateTime.Now;

        public static Guid GetInjectedGuidValue() => Guid.NewGuid();

        public static int GetInjectedRandomValue() => Random.Next();

        public static IPrincipal GetInjectedIPrincipalValue(ISession session) => session.Principal;

        public static Type GetMatchingImpl(Type typeOfQueryable) {
            if (typeOfQueryable.IsInterface) {
                // get matching impl type by convention for the moment 
                var implTypeName = $"{typeOfQueryable.Namespace}.{typeOfQueryable.Name.Remove(0, 1)}";
                var implType = typeOfQueryable.Assembly.GetType(implTypeName);
                return implType;
            }

            return typeOfQueryable;
        }


        // ReSharper disable once UnusedMember.Global
        // maybe called reflectively
        public static IQueryable<T> GetInjectedQueryableValue<T>(IObjectPersistor persistor) where T : class => persistor.UntrackedInstances<T>();

        private static object GetParameterValue(this ParameterInfo p, INakedObjectAdapter adapter, ISession session, IObjectPersistor persistor) {
            if (p.Position == 0 && !(adapter.Spec is IServiceSpec)) {
                return adapter.Object;
            }

            if (p.GetCustomAttribute<InjectedAttribute>() is not null) {
                var parameterType = p.ParameterType;
                if (parameterType == typeof(DateTime)) {
                    return GetInjectedDateTimeValue();
                }

                if (parameterType == typeof(Guid)) {
                    return GetInjectedGuidValue();
                }

                if (parameterType == typeof(int)) {
                    return GetInjectedRandomValue();
                }

                if (parameterType == typeof(IPrincipal)) {
                    return GetInjectedIPrincipalValue(session);
                }

                if (CollectionUtils.IsQueryable(parameterType)) {
                    var elementType = GetMatchingImpl(parameterType.GetGenericArguments().First());
                    var f = typeof(InjectUtils).GetMethod("GetInjectedQueryableValue")?.MakeGenericMethod(elementType);
                    return f?.Invoke(null, new object[] {persistor});
                }
            }

            return null;
        }

        private static object GetMatchingParameter(this ParameterInfo p, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            if (parameterNameValues != null &&  parameterNameValues.ContainsKey(p.Name.ToLower())) {
                return parameterNameValues[p.Name.ToLower()].Object;
            }

            return null;
        }

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, ISession session, IObjectPersistor persistor) => method.GetParameters().Select(p => p.GetParameterValue(adapter, session, persistor)).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, ISession session, IObjectPersistor persistor) => method.GetParameters().Select(p => p.GetParameterValue(adapter, session, persistor) ?? p.GetMatchingParameter(parameterNameValues)).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, string autocomplete, ISession session, IObjectPersistor persistor) => method.GetParameters().Select(p => p.GetParameterValue(adapter, session, persistor) ?? autocomplete).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, string[] keys, ISession session, IObjectPersistor persistor) => method.GetParameters().Select(p => p.GetParameterValue(adapter, session, persistor) ?? keys).ToArray();

        public static void PerformAction<T>(object action, T toInject) => ((Action<T>)action)(toInject);
    }
}