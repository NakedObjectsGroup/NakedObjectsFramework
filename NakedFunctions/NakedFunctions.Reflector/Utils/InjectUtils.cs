// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NakedFunctions;
using NakedFunctions.Reflector.Component;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Utils {
    public static class InjectUtils {

        public static ParameterInfo[] FilterParms(MethodInfo m) =>
            m.GetParameters().Where(p => !p.IsInjectedParameter() && (!m.IsDefined(typeof(ExtensionAttribute), false) || p.Position > 0)).ToArray();

        public static bool IsExtensionMethod(this MemberInfo m) => m.IsDefined(typeof(ExtensionAttribute), false);

        public static bool IsInjectedParameter(this ParameterInfo p) => p.ParameterType.IsAssignableTo(typeof(IContext));

        public static bool IsTargetParameter(this ParameterInfo p) => p.Position == 0 && p.Member.IsExtensionMethod();


        // ReSharper disable once UnusedMember.Global
        // maybe called reflectively
        public static IQueryable<T> GetInjectedQueryableValue<T>(IObjectPersistor persistor) where T : class => persistor.Instances<T>();

        private static object GetParameterValue(this ParameterInfo p, INakedObjectAdapter adapter, INakedObjectsFramework framework) =>
            p switch {
                _ when p.IsTargetParameter() => adapter.Object,
                _ when p.IsInjectedParameter() => new Context() {Persistor = framework.Persistor, Provider = framework.ServiceProvider},
                _ => null
            };

        private static object GetMatchingParameter(this ParameterInfo p, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            if (parameterNameValues != null &&  parameterNameValues.ContainsKey(p.Name.ToLower())) {
                return parameterNameValues[p.Name.ToLower()].Object;
            }

            return null;
        }

        private static INakedObjectAdapter GetNext(this IEnumerator parmValues) {
            parmValues.MoveNext();
             return( INakedObjectAdapter) parmValues.Current;
        }


        private static object GetParameterValue(this ParameterInfo p, INakedObjectAdapter adapter, INakedObjectAdapter[] parmValues, int i, INakedObjectsFramework framework) =>
            p switch
            {
                _ when p.IsTargetParameter() => adapter.Object,
                _ when p.IsInjectedParameter() => new Context {Persistor = framework.Persistor, Provider = framework.ServiceProvider},
                _ => parmValues[i].GetDomainObject()
            };


        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, INakedObjectsFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework)).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedObjectsFramework framework) => 
            method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? p.GetMatchingParameter(parameterNameValues)).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, string autocomplete, INakedObjectsFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? autocomplete).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, INakedObjectAdapter parmValue, INakedObjectsFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? parmValue.GetDomainObject()).ToArray();

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, INakedObjectAdapter[] parmValues, INakedObjectsFramework framework) {
            var index = 0;
            return method.GetParameters().Select(p => p.GetParameterValue(adapter, parmValues, index++, framework)).ToArray();
        }

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, string[] keys, INakedObjectsFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? keys).ToArray();

        public static void PerformAction<T>(object action, T toInject) => ((Action<T>)action)(toInject);
    }
}