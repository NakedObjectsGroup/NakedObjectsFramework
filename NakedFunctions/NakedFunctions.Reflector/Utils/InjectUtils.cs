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
using System.Runtime.CompilerServices;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Util;
using NakedFunctions.Reflector.Component;

namespace NakedFunctions.Reflector.Utils; 

public static class InjectUtils {
    public static ParameterInfo[] FilterParms(MethodInfo m) => m.GetParameters().Where(p => !(p.IsInjectedParameter() || p.IsTargetParameter())).ToArray();

    private static bool IsExtensionMethod(this MemberInfo m) => m.IsDefined(typeof(ExtensionAttribute), false);

    public static bool IsInjectedParameter(this ParameterInfo p) => p.ParameterType.IsAssignableTo(typeof(IContext));

    public static bool IsTargetParameter(this ParameterInfo p) => p.Position == 0 && p.Member.IsExtensionMethod();

    public static Type ContributedToType(this MethodInfo m) => m.GetParameters().SingleOrDefault(IsTargetParameter)?.ParameterType;

    private static object GetParameterValue(this ParameterInfo p, INakedObjectAdapter adapter, INakedFramework framework) =>
        p switch {
            _ when p.IsTargetParameter() => adapter?.Object,
            _ when p.IsInjectedParameter() => new FunctionalContext {Persistor = framework.Persistor, Provider = framework.ServiceProvider},
            _ => null
        };

    private static object GetMatchingParameter(this ParameterInfo p, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
        var key = p.Name?.ToLower() ?? "";
        return parameterNameValues?.ContainsKey(key) == true ? parameterNameValues[key].GetDomainObject() : null;
    }

    private static object GetParameterValue(this ParameterInfo p, INakedObjectAdapter adapter, INakedObjectAdapter[] parmValues, int i, INakedFramework framework) =>
        p switch {
            _ when p.IsTargetParameter() => adapter?.Object,
            _ when p.IsInjectedParameter() => new FunctionalContext {Persistor = framework.Persistor, Provider = framework.ServiceProvider},
            _ => parmValues[i].GetDomainObject()
        };

    public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, INakedFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework)).ToArray();

    public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework) =>
        method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? p.GetMatchingParameter(parameterNameValues)).ToArray();

    public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, string autocomplete, INakedFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? autocomplete).ToArray();

    public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, INakedObjectAdapter parmValue, INakedFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? parmValue.GetDomainObject()).ToArray();

    public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, INakedObjectAdapter[] parmValues, INakedFramework framework) => method.GetParameters().Select((p, i) => p.GetParameterValue(adapter, parmValues, i, framework)).ToArray();

    public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, string[] keys, INakedFramework framework) => method.GetParameters().Select(p => p.GetParameterValue(adapter, framework) ?? keys).ToArray();

}