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
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public abstract class MethodPrefixBasedFacetFactoryAbstract : FacetFactoryAbstract, IMethodPrefixBasedFacetFactory {
        private ILogger<MethodPrefixBasedFacetFactoryAbstract> logger;

        protected MethodPrefixBasedFacetFactoryAbstract(int numericOrder, ILoggerFactory loggerFactory, FeatureType featureTypes)
            : base(numericOrder, loggerFactory, featureTypes) =>
            logger = loggerFactory.CreateLogger<MethodPrefixBasedFacetFactoryAbstract>();

        #region IMethodPrefixBasedFacetFactory Members

        public abstract string[] Prefixes { get; }

        #endregion

        protected MethodInfo FindMethodWithOrWithoutParameters(IReflector reflector, Type type, MethodType methodType, string name, Type returnType, Type[] parms) =>
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
        protected MethodInfo[] FindMethods(IReflector reflector,
                                           Type type,
                                           MethodType methodType,
                                           string name,
                                           Type returnType = null) =>
            type.GetMethods(GetBindingFlagsForMethodType(methodType, reflector)).Where(m => m.Name == name).Where(m => m.IsStatic && methodType == MethodType.Class || !m.IsStatic && methodType == MethodType.Object).Where(m => m.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null).Where(m => returnType == null || returnType.IsAssignableFrom(m.ReturnType)).ToArray();

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
        protected MethodInfo FindMethod(IReflector reflector,
                                        Type type,
                                        MethodType methodType,
                                        string name,
                                        Type returnType,
                                        Type[] paramTypes,
                                        string[] paramNames = null) {
            try {
                var method = paramTypes == null
                    ? type.GetMethod(name, GetBindingFlagsForMethodType(methodType, reflector))
                    : type.GetMethod(name, GetBindingFlagsForMethodType(methodType, reflector), null, paramTypes, null);

                if (method == null) {
                    return null;
                }

                // check for static modifier
                if (method.IsStatic && methodType == MethodType.Object) {
                    return null;
                }

                if (!method.IsStatic && methodType == MethodType.Class) {
                    return null;
                }

                if (method.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null) {
                    return null;
                }

                // check for return Type
                if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType)) {
                    return null;
                }

                if (paramNames != null) {
                    var methodParamNames = method.GetParameters().Select(p => p.Name).ToArray();

                    if (!paramNames.SequenceEqual(methodParamNames)) {
                        return null;
                    }
                }

                return method;
            }
            catch (AmbiguousMatchException e) {
                throw new ModelException(logger.LogAndReturn(string.Format(Resources.NakedObjects.AmbiguousMethodError, name, type.FullName)), e);
            }
        }

        private static BindingFlags GetBindingFlagsForMethodType(MethodType methodType, IReflector reflector) =>
            BindingFlags.Public |
            (methodType == MethodType.Object ? BindingFlags.Instance : BindingFlags.Static) |
            (reflector.IgnoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default);

        protected static void RemoveMethod(IMethodRemover methodRemover, MethodInfo method) {
            if (method != null) {
                methodRemover?.RemoveMethod(method);
            }
        }

        protected void FindDefaultDisableMethod(IReflector reflector, IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, ISpecification specification) {
            var method = FindMethodWithOrWithoutParameters(reflector, type, methodType, RecognisedMethodsAndPrefixes.DisablePrefix + capitalizedName, typeof(string), Type.EmptyTypes);
            if (method != null) {
                facets.Add(new DisableForContextFacet(method, specification, Logger<DisableForContextFacet>()));
            }
        }

        protected void FindAndRemoveDisableMethod(IReflector reflector, IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, ISpecification specification) {
            var method = FindMethod(reflector, type, methodType, RecognisedMethodsAndPrefixes.DisablePrefix + capitalizedName, typeof(string), Type.EmptyTypes);
            if (method != null) {
                methodRemover.RemoveMethod(method);
                facets.Add(new DisableForContextFacet(method, specification, Logger<DisableForContextFacet>()));
            }
        }

        protected void FindDefaultHideMethod(IReflector reflector, IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, ISpecification specification) {
            var method = FindMethodWithOrWithoutParameters(reflector, type, methodType, RecognisedMethodsAndPrefixes.HidePrefix + capitalizedName, typeof(bool), Type.EmptyTypes);
            if (method != null) {
                facets.Add(new HideForContextFacet(method, specification, Logger<HideForContextFacet>()));
                AddOrAddToExecutedWhereFacet(method, specification);
            }
        }

        protected void FindAndRemoveHideMethod(IReflector reflector, IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, ISpecification specification) {
            var method = FindMethod(reflector, type, methodType, RecognisedMethodsAndPrefixes.HidePrefix + capitalizedName, typeof(bool), Type.EmptyTypes);
            if (method != null) {
                methodRemover.RemoveMethod(method);
                facets.Add(new HideForContextFacet(method, specification, Logger<HideForContextFacet>()));
                AddOrAddToExecutedWhereFacet(method, specification);
            }
        }

        protected static void AddHideForSessionFacetNone(IList<IFacet> facets, ISpecification specification) => facets.Add(new HideForSessionFacetNone(specification));

        protected static void AddDisableForSessionFacetNone(IList<IFacet> facets, ISpecification specification) => facets.Add(new DisableForSessionFacetNone(specification));

        protected static void AddDisableFacetAlways(IList<IFacet> facets, ISpecification specification) => facets.Add(new DisabledFacetAlways(specification));

        protected static void AddOrAddToExecutedWhereFacet(MethodInfo method, ISpecification holder) {
            var attribute = method.GetCustomAttribute<ExecutedAttribute>();
            if (attribute != null && !attribute.IsAjax) {
                var executedFacet = holder.GetFacet<IExecutedControlMethodFacet>();
                if (executedFacet == null) {
                    FacetUtils.AddFacet(new ExecutedControlMethodFacet(method, attribute.Value, holder));
                }
                else {
                    executedFacet.AddMethodExecutedWhere(method, attribute.Value);
                }
            }
        }

        protected static void AddAjaxFacet(MethodInfo method, ISpecification holder) {
            if (method == null) {
                FacetUtils.AddFacet(new AjaxFacet(holder));
            }
            else {
                var attribute = method.GetCustomAttribute<ExecutedAttribute>();
                if (attribute != null && attribute.IsAjax) {
                    if (attribute.AjaxValue == Ajax.Disabled) {
                        FacetUtils.AddFacet(new AjaxFacet(holder));
                    }
                }
            }
        }
    }
}