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
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Facet;
using NakedFramework.ParallelReflector.Utils;

#pragma warning disable 612

namespace NakedObjects.Reflector.Utils {
    public static class ObjectMethodHelpers {

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
        public static MethodInfo[] FindMethods(IReflector reflector,
                                               Type type,
                                               MethodType methodType,
                                               string name,
                                               Type returnType = null) =>
            type.GetMethods(MethodHelpers.GetBindingFlagsForMethodType(methodType, reflector)).Where(m => m.Name == name).Where(m => m.IsStatic && methodType == MethodType.Class || !m.IsStatic && methodType == MethodType.Object).Where(m => m.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null).Where(m => returnType == null || returnType.IsAssignableFrom(m.ReturnType)).ToArray();



        public static void FindDefaultHideMethod(IReflector reflector, IList<IFacet> facets, Type type, MethodType methodType, string capitalizedName, ISpecification specification, ILoggerFactory loggerFactory) {
            var method = MethodHelpers.FindMethodWithOrWithoutParameters(reflector, type, methodType, RecognisedMethodsAndPrefixes.HidePrefix + capitalizedName, typeof(bool), Type.EmptyTypes);
            if (method != null) {
                facets.Add(new HideForContextFacet(method, specification, loggerFactory.CreateLogger<HideForContextFacet>()));
            }
        }

        public static void FindAndRemoveHideMethod(IReflector reflector, IList<IFacet> facets, Type type, MethodType methodType, string capitalizedName, ISpecification specification, ILoggerFactory loggerFactory, IMethodRemover methodRemover = null) {
            var method = MethodHelpers.FindMethod(reflector, type, methodType, RecognisedMethodsAndPrefixes.HidePrefix + capitalizedName, typeof(bool), Type.EmptyTypes);
            if (method != null) {
                methodRemover?.RemoveMethod(method);
                facets.Add(new HideForContextFacet(method, specification, loggerFactory.CreateLogger<HideForContextFacet>()));
            }
        }
    }
}