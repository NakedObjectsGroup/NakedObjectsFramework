// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public sealed class ActionMethodsFacetFactory : MethodPrefixBasedFacetFactoryAbstract, IMethodIdentifyingFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.AutoCompletePrefix,
            RecognisedMethodsAndPrefixes.ParameterDefaultPrefix,
            RecognisedMethodsAndPrefixes.ParameterChoicesPrefix,
            RecognisedMethodsAndPrefixes.DisablePrefix,
            RecognisedMethodsAndPrefixes.HidePrefix,
            RecognisedMethodsAndPrefixes.ValidatePrefix,
            RecognisedMethodsAndPrefixes.DisablePrefix
        };

        private readonly ILogger<ActionMethodsFacetFactory> logger;

        public ActionMethodsFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ActionsAndActionParameters) =>
            logger = loggerFactory.CreateLogger<ActionMethodsFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        #region IMethodIdentifyingFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);

            var type = actionMethod.DeclaringType;
            var facets = new List<IFacet>();

            ITypeSpecBuilder onType;
            (onType, metamodel) = reflector.LoadSpecification(type, metamodel);

            IObjectSpecBuilder returnSpec;
            (returnSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(actionMethod.ReturnType, metamodel);

            IObjectSpecBuilder elementSpec = null;
            var isQueryable = IsQueryOnly(actionMethod) || CollectionUtils.IsQueryable(actionMethod.ReturnType);
            if (returnSpec != null && IsCollection(actionMethod.ReturnType)) {
                var elementType = CollectionUtils.ElementType(actionMethod.ReturnType);
                (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
            }

            RemoveMethod(methodRemover, actionMethod);
            facets.Add(new ActionInvocationFacetViaMethod(actionMethod, onType, returnSpec, elementSpec, action, isQueryable, Logger<ActionInvocationFacetViaMethod>()));

            var methodType = actionMethod.IsStatic ? MethodType.Class : MethodType.Object;
            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            FindAndRemoveValidMethod(reflector, facets, methodRemover, type, methodType, capitalizedName, paramTypes, action);

            DefaultNamedFacet(facets, actionMethod.Name, action); // must be called after the checkForXxxPrefix methods

            AddHideForSessionFacetNone(facets, action);
            AddDisableForSessionFacetNone(facets, action);
            FindDefaultHideMethod(reflector, facets, methodRemover, type, methodType, "ActionDefault", action);
            FindAndRemoveHideMethod(reflector, facets, methodRemover, type, methodType, capitalizedName, action);
            FindDefaultDisableMethod(reflector, facets, methodRemover, type, methodType, "ActionDefault", action);
            FindAndRemoveDisableMethod(reflector, facets, methodRemover, type, methodType, capitalizedName, action);

            if (action is IActionSpecImmutable actionSpecImmutable) {
                // Process the action's parameters names, descriptions and optional
                // an alternative design would be to have another facet factory processing just ActionParameter, and have it remove these
                // supporting methods.  However, the FacetFactory API doesn't allow for methods of the class to be removed while processing
                // action parameters, only while processing Methods (ie actions)
                var actionParameters = actionSpecImmutable.Parameters;
                var paramNames = actionMethod.GetParameters().Select(p => p.Name).ToArray();

                FindAndRemoveParametersAutoCompleteMethod(reflector, methodRemover, type, capitalizedName, paramTypes, actionParameters);
                metamodel = FindAndRemoveParametersChoicesMethod(reflector, methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters, metamodel);
                FindAndRemoveParametersDefaultsMethod(reflector, methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
                FindAndRemoveParametersValidateMethod(reflector, methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
            }

            FacetUtils.AddFacets(facets);

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            var facets = new List<IFacet>();

            if (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                facets.Add(new NullableFacetAlways(holder));
            }

            IObjectSpecBuilder returnSpec;
            (returnSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(parameter.ParameterType, metamodel);

            if (returnSpec != null && IsParameterCollection(parameter.ParameterType)) {
                var elementType = CollectionUtils.ElementType(parameter.ParameterType);
                IObjectSpecImmutable elementSpec;
                (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(elementType, metamodel);
                facets.Add(new ElementTypeFacet(holder, elementType, elementSpec));
            }

            FacetUtils.AddFacets(facets);
            return metamodel;
        }

        public IList<MethodInfo> FindActions(IList<MethodInfo> candidates, IClassStrategy classStrategy) {
            return candidates.Where(methodInfo => methodInfo.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                                                  !methodInfo.IsStatic &&
                                                  !methodInfo.IsGenericMethod &&
                                                  classStrategy.IsTypeToBeIntrospected(methodInfo.ReturnType) &&
                                                  ParametersAreSupported(methodInfo, classStrategy)).ToArray();
        }

        #endregion

        private static bool IsQueryOnly(MethodInfo method) =>
            method.GetCustomAttribute<IdempotentAttribute>() == null &&
            method.GetCustomAttribute<QueryOnlyAttribute>() != null;

        // separate methods to reproduce old reflector behaviour
        private static bool IsParameterCollection(Type type) =>
            type != null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                type == typeof(string) ||
                CollectionUtils.IsCollectionButNotArray(type));

        private static bool IsCollection(Type type) {
            return type != null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                type == typeof(string) ||
                CollectionUtils.IsCollectionButNotArray(type) ||
                IsCollection(type.BaseType) ||
                type.GetInterfaces().Where(i => i.IsPublic).Any(IsCollection));
        }

        private bool ParametersAreSupported(MethodInfo method, IClassStrategy classStrategy) {
            foreach (var parameterInfo in method.GetParameters()) {
                if (!classStrategy.IsTypeToBeIntrospected(parameterInfo.ParameterType)) {
                    // log if not a System or NOF type
                    if (!TypeUtils.IsSystem(method.DeclaringType) && !TypeUtils.IsNakedObjects(method.DeclaringType)) {
                        logger.LogWarning($"Ignoring method: {method.DeclaringType}.{method.Name} because parameter '{parameterInfo.Name}' is of type {parameterInfo.ParameterType}");
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Must be called after the <c>CheckForXxxPrefix</c> methods.
        /// </summary>
        private static void DefaultNamedFacet(ICollection<IFacet> actionFacets, string name, ISpecification action) => actionFacets.Add(new NamedFacetInferred(name, action));

        private void FindAndRemoveValidMethod(IReflector reflector, ICollection<IFacet> actionFacets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] parms, ISpecification action) {
            var method = FindMethod(reflector, type, methodType, RecognisedMethodsAndPrefixes.ValidatePrefix + capitalizedName, typeof(string), parms);
            if (method != null) {
                RemoveMethod(methodRemover, method);
                actionFacets.Add(new ActionValidationFacet(method, action, LoggerFactory.CreateLogger<ActionValidationFacet>()));
            }
        }

        private void FindAndRemoveParametersDefaultsMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, IActionParameterSpecImmutable[] parameters) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];
                var paramName = paramNames[i];

                var methodUsingIndex = FindMethodWithOrWithoutParameters(reflector,
                    type,
                    MethodType.Object,
                    RecognisedMethodsAndPrefixes.ParameterDefaultPrefix + i + capitalizedName,
                    paramType,
                    paramTypes);

                var methodUsingName = FindMethod(
                    reflector,
                    type,
                    MethodType.Object,
                    RecognisedMethodsAndPrefixes.ParameterDefaultPrefix + capitalizedName,
                    paramType,
                    new[] {paramType},
                    new[] {paramName});

                if (methodUsingIndex != null && methodUsingName != null) {
                    logger.LogWarning($"Duplicate defaults parameter methods {methodUsingIndex.Name} and {methodUsingName.Name} using {methodUsingName.Name}");
                }

                var methodToUse = methodUsingName ?? methodUsingIndex;

                if (methodToUse != null) {
                    // deliberately not removing both if duplicate to show that method  is duplicate
                    RemoveMethod(methodRemover, methodToUse);

                    // add facets directly to parameters, not to actions
                    FacetUtils.AddFacet(new ActionDefaultsFacetViaMethod(methodToUse, parameters[i], Logger<ActionDefaultsFacetViaMethod>()));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> FindAndRemoveParametersChoicesMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, IActionParameterSpecImmutable[] parameters, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var paramType = paramTypes[i];
                var paramName = paramNames[i];
                var isMultiple = false;

                if (CollectionUtils.IsGenericEnumerable(paramType)) {
                    paramType = paramType.GetGenericArguments().First();
                    isMultiple = true;
                }

                var returnType = typeof(IEnumerable<>).MakeGenericType(paramType);
                var methodName = RecognisedMethodsAndPrefixes.ParameterChoicesPrefix + i + capitalizedName;

                var methods = FindMethods(
                    reflector,
                    type,
                    MethodType.Object,
                    methodName,
                    returnType);

                if (methods.Length > 1) {
                    methods.Skip(1).ForEach(m => logger.LogWarning($"Found multiple action choices methods: {methodName} in type: {type} ignoring method(s) with params: {m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)}"));
                }

                var methodUsingIndex = methods.FirstOrDefault();

                var methodUsingName = FindMethod(
                    reflector,
                    type,
                    MethodType.Object,
                    RecognisedMethodsAndPrefixes.ParameterChoicesPrefix + capitalizedName,
                    returnType,
                    new[] {paramType},
                    new[] {paramName});

                if (methodUsingIndex != null && methodUsingName != null) {
                    logger.LogWarning($"Duplicate choices parameter methods {methodUsingIndex.Name} and {methodUsingName.Name} using {methodUsingName.Name}");
                }

                var methodToUse = methodUsingName ?? methodUsingIndex;

                if (methodToUse != null) {
                    // deliberately not removing both if duplicate to show that method  is duplicate
                    RemoveMethod(methodRemover, methodToUse);

                    // add facets directly to parameters, not to actions
                    var parameterNamesAndTypes = new List<(string, IObjectSpecImmutable)>();

                    foreach (var p in methodToUse.GetParameters()) {
                        IObjectSpecBuilder oSpec;
                        (oSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(p.ParameterType, metamodel);
                        var name = p.Name.ToLower();
                        parameterNamesAndTypes.Add((name, oSpec));
                    }

                    FacetUtils.AddFacet(new ActionChoicesFacetViaMethod(methodToUse, parameterNamesAndTypes.ToArray(), returnType, parameters[i], Logger<ActionChoicesFacetViaMethod>(), isMultiple));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }

            return metamodel;
        }

        private void FindAndRemoveParametersAutoCompleteMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters) {
            for (var i = 0; i < paramTypes.Length; i++) {
                // only support on strings and reference types
                var paramType = paramTypes[i];
                if (paramType.IsClass || paramType.IsInterface) {
                    //returning an IQueryable ...
                    //.. or returning a single object
                    var method = FindAutoCompleteMethod(reflector, type, capitalizedName, i, typeof(IQueryable<>).MakeGenericType(paramType)) ??
                                 FindAutoCompleteMethod(reflector, type, capitalizedName, i, paramType);

                    //... or returning an enumerable of string
                    if (method == null && TypeUtils.IsString(paramType)) {
                        method = FindAutoCompleteMethod(reflector, type, capitalizedName, i, typeof(IEnumerable<string>));
                    }

                    if (method != null) {
                        var pageSizeAttr = method.GetCustomAttribute<PageSizeAttribute>();
                        var minLengthAttr = (MinLengthAttribute) Attribute.GetCustomAttribute(method.GetParameters().First(), typeof(MinLengthAttribute));

                        var pageSize = pageSizeAttr != null ? pageSizeAttr.Value : 0; // default to 0 ie system default
                        var minLength = minLengthAttr != null ? minLengthAttr.Length : 0;

                        // deliberately not removing both if duplicate to show that method  is duplicate
                        RemoveMethod(methodRemover, method);

                        // add facets directly to parameters, not to actions
                        FacetUtils.AddFacet(new AutoCompleteFacet(method, pageSize, minLength, parameters[i], Logger<AutoCompleteFacet>()));
                        AddOrAddToExecutedWhereFacet(method, parameters[i]);
                    }
                }
            }
        }

        private MethodInfo FindAutoCompleteMethod(IReflector reflector, Type type, string capitalizedName, int i, Type returnType) {
            var method = FindMethod(reflector,
                type,
                MethodType.Object,
                RecognisedMethodsAndPrefixes.AutoCompletePrefix + i + capitalizedName,
                returnType,
                new[] {typeof(string)});
            return method;
        }

        private void FindAndRemoveParametersValidateMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, IActionParameterSpecImmutable[] parameters) {
            for (var i = 0; i < paramTypes.Length; i++) {
                var methodUsingIndex = FindMethod(reflector,
                    type,
                    MethodType.Object,
                    RecognisedMethodsAndPrefixes.ValidatePrefix + i + capitalizedName,
                    typeof(string),
                    new[] {paramTypes[i]});

                var methodUsingName = FindMethod(reflector,
                    type,
                    MethodType.Object,
                    RecognisedMethodsAndPrefixes.ValidatePrefix + capitalizedName,
                    typeof(string),
                    new[] {paramTypes[i]},
                    new[] {paramNames[i]});

                if (methodUsingIndex != null && methodUsingName != null) {
                    logger.LogWarning($"Duplicate validate parameter methods {methodUsingIndex.Name} and {methodUsingName.Name} using {methodUsingName.Name}");
                }

                var methodToUse = methodUsingName ?? methodUsingIndex;

                if (methodToUse != null) {
                    // deliberately not removing both if duplicate to show that method  is duplicate
                    RemoveMethod(methodRemover, methodToUse);

                    // add facets directly to parameters, not to actions
                    FacetUtils.AddFacet(new ActionParameterValidation(methodToUse, parameters[i], Logger<ActionParameterValidation>()));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                    AddAjaxFacet(methodToUse, parameters[i]);
                }
                else {
                    AddAjaxFacet(null, parameters[i]);
                }
            }
        }
    }
}