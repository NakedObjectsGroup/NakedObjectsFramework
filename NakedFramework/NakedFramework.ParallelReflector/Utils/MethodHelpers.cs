﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;

#pragma warning disable 612

namespace NakedFramework.ParallelReflector.Utils;

public static class MethodHelpers {
    public static MethodInfo FindMethodWithOrWithoutParameters(IReflector reflector, Type type, MethodType methodType, string name, Type returnType, Type[] parms) =>
        FindMethod(reflector, type, methodType, name, returnType, parms) ??
        FindMethod(reflector, type, methodType, name, returnType, Type.EmptyTypes);

    /// <summary>
    ///     Returns  specific public methods that: have the specified prefix; have the specified return Type, or
    ///     void, and has the specified number of parameters. If the returnType is specified as null then the return
    ///     Type is ignored.
    /// </summary>
    /// <param name="reflector"></param>
    /// <param name="type"></param>
    /// <param name="methodType"></param>
    /// <param name="name"></param>
    /// <param name="returnType"></param>
    /// <param name="paramTypes">the set of parameters the method should have, if null then is ignored</param>
    /// <param name="paramNames">the names of the parameters the method should have, if null then is ignored</param>
    public static MethodInfo FindMethod(IReflector reflector,
                                        Type type,
                                        MethodType methodType,
                                        string name,
                                        Type returnType,
                                        Type[] paramTypes,
                                        string[] paramNames = null) {
        try {
            var method = paramTypes is null
                ? type.GetMethod(name, GetBindingFlagsForMethodType(methodType, reflector))
                : type.GetMethod(name, GetBindingFlagsForMethodType(methodType, reflector), null, paramTypes, null);

            if (method is null) {
                return null;
            }

            // check for static modifier
            if (method.IsStatic && methodType is MethodType.Object) {
                return null;
            }

            if (!method.IsStatic && methodType is MethodType.Class) {
                return null;
            }

            if (reflector.ClassStrategy.IsIgnored(method)) {
                return null;
            }

            // check for return Type
            if (returnType is not null && !returnType.IsAssignableFrom(method.ReturnType)) {
                return null;
            }

            if (paramNames is not null) {
                var methodParamNames = method.GetParameters().Select(p => p.Name).ToArray();

                if (!paramNames.SequenceEqual(methodParamNames)) {
                    return null;
                }
            }

            return method;
        }
        catch (AmbiguousMatchException e) {
            throw new ModelException(string.Format(NakedObjects.Resources.NakedObjects.AmbiguousMethodError, name, type.FullName), e);
        }
    }

    public static BindingFlags GetBindingFlagsForMethodType(MethodType methodType, IReflector reflector) =>
        BindingFlags.Public |
        (methodType is MethodType.Object ? BindingFlags.Instance : BindingFlags.Static) |
        (reflector.IgnoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default);

    public static void SafeRemoveMethod(this IMethodRemover methodRemover, MethodInfo method) {
        if (method is not null) {
            methodRemover?.RemoveMethod(method);
        }
    }

    public static void AddHideForSessionFacetNone(IList<IFacet> facets, ISpecification specification) => facets.Add(HideForSessionFacetNone.Instance);

    public static void AddDisableForSessionFacetNone(IList<IFacet> facets, ISpecification specification) => facets.Add(DisableForSessionFacetNone.Instance);

    public static void AddDisableFacetAlways(IList<IFacet> facets, ISpecification specification) => facets.Add(DisabledFacetAlways.Instance);
}