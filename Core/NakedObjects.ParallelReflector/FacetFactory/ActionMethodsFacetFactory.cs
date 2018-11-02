// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Common.Logging;
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
            PrefixesAndRecognisedMethods.AutoCompletePrefix,
            PrefixesAndRecognisedMethods.ParameterDefaultPrefix,
            PrefixesAndRecognisedMethods.ParameterChoicesPrefix,
            PrefixesAndRecognisedMethods.DisablePrefix,
            PrefixesAndRecognisedMethods.HidePrefix,
            PrefixesAndRecognisedMethods.ValidatePrefix,
            PrefixesAndRecognisedMethods.DisablePrefix
        };

        private static readonly ILog Log = LogManager.GetLogger(typeof (ActionMethodsFacetFactory));

        public ActionMethodsFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ActionsAndActionParameters) {}

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        private bool IsQueryOnly(MethodInfo method) {
            return (method.GetCustomAttribute<IdempotentAttribute>() == null) &&
                   (method.GetCustomAttribute<QueryOnlyAttribute>() != null);
        }

        public override void Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action) {
            string capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);

            Type type = actionMethod.DeclaringType;
            var facets = new List<IFacet>();
            ITypeSpecBuilder onType = reflector.LoadSpecification(type);
            var returnSpec = reflector.LoadSpecification<IObjectSpecBuilder>(actionMethod.ReturnType);

            IObjectSpecImmutable elementSpec = null;
            bool isQueryable = IsQueryOnly(actionMethod) || CollectionUtils.IsQueryable(actionMethod.ReturnType);
            if (returnSpec != null && returnSpec.IsCollection) {
                Type elementType = CollectionUtils.ElementType(actionMethod.ReturnType);
                elementSpec = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
            }

            RemoveMethod(methodRemover, actionMethod);
            facets.Add(new ActionInvocationFacetViaMethod(actionMethod, onType, returnSpec, elementSpec, action, isQueryable));

            MethodType methodType = actionMethod.IsStatic ? MethodType.Class : MethodType.Object;
            Type[] paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            FindAndRemoveValidMethod(reflector, facets, methodRemover, type, methodType, capitalizedName, paramTypes, action);

            DefaultNamedFacet(facets, actionMethod.Name, action); // must be called after the checkForXxxPrefix methods

            AddHideForSessionFacetNone(facets, action);
            AddDisableForSessionFacetNone(facets, action);
            FindDefaultHideMethod(reflector, facets, methodRemover, type, methodType, "ActionDefault", action);
            FindAndRemoveHideMethod(reflector, facets, methodRemover, type, methodType, capitalizedName, action);
            FindDefaultDisableMethod(reflector, facets, methodRemover, type, methodType, "ActionDefault", action);
            FindAndRemoveDisableMethod(reflector, facets, methodRemover, type, methodType, capitalizedName, action);

            var actionSpecImmutable = action as IActionSpecImmutable;
            if (actionSpecImmutable != null) {
                // Process the action's parameters names, descriptions and optional
                // an alternative design would be to have another facet factory processing just ActionParameter, and have it remove these
                // supporting methods.  However, the FacetFactory API doesn't allow for methods of the class to be removed while processing
                // action parameters, only while processing Methods (ie actions)
                IActionParameterSpecImmutable[] actionParameters = actionSpecImmutable.Parameters;
                string[] paramNames = actionMethod.GetParameters().Select(p => p.Name).ToArray();

                FindAndRemoveParametersAutoCompleteMethod(reflector, methodRemover, type, capitalizedName, paramTypes, actionParameters);
                FindAndRemoveParametersChoicesMethod(reflector, methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
                FindAndRemoveParametersDefaultsMethod(reflector, methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
                FindAndRemoveParametersValidateMethod(reflector, methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
            }
            FacetUtils.AddFacets(facets);
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var facets = new List<IFacet>();

            if (parameter.ParameterType.IsGenericType && (parameter.ParameterType.GetGenericTypeDefinition() == typeof (Nullable<>))) {
                facets.Add(new NullableFacetAlways(holder));
            }

            var returnSpec = reflector.LoadSpecification<IObjectSpecBuilder>(parameter.ParameterType);

            if (returnSpec != null && returnSpec.IsCollection) {
                Type elementType = CollectionUtils.ElementType(parameter.ParameterType);
                var elementSpec = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
                facets.Add(new ElementTypeFacet(holder, elementType, elementSpec));
            }

            FacetUtils.AddFacets(facets);
        }

        private bool ParametersAreSupported(MethodInfo method, IClassStrategy classStrategy) {
            foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                if (!classStrategy.IsTypeToBeIntrospected(parameterInfo.ParameterType)) {
                    Log.InfoFormat("Ignoring method: {0}.{1} because parameter '{2}' is of type {3}", method.DeclaringType, method.Name, parameterInfo.Name, parameterInfo.ParameterType);
                    return false;
                }
            }
            return true;
        }

        public IList<MethodInfo> FindActions(IList<MethodInfo> candidates, IClassStrategy classStrategy) {
            return candidates.Where(methodInfo => methodInfo.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                                                  !methodInfo.IsStatic &&
                                                  !methodInfo.IsGenericMethod &&
                                                  classStrategy.IsTypeToBeIntrospected(methodInfo.ReturnType) &&
                                                  ParametersAreSupported(methodInfo, classStrategy)).ToList();
        }

        /// <summary>
        ///     Must be called after the <c>CheckForXxxPrefix</c> methods.
        /// </summary>
        private static void DefaultNamedFacet(ICollection<IFacet> actionFacets, string name, ISpecification action) {
            actionFacets.Add(new NamedFacetInferred(name, action));
        }

        private void FindAndRemoveValidMethod(IReflector reflector, ICollection<IFacet> actionFacets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] parms, ISpecification action) {
            MethodInfo method = FindMethod(reflector, type, methodType, PrefixesAndRecognisedMethods.ValidatePrefix + capitalizedName, typeof (string), parms);
            if (method != null) {
                RemoveMethod(methodRemover, method);
                actionFacets.Add(new ActionValidationFacet(method, action));
            }
        }

        private void FindAndRemoveParametersDefaultsMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, IActionParameterSpecImmutable[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                Type paramType = paramTypes[i];
                string paramName = paramNames[i];

                MethodInfo methodUsingIndex = FindMethodWithOrWithoutParameters(reflector,
                    type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ParameterDefaultPrefix + i + capitalizedName,
                    paramType,
                    paramTypes);

                MethodInfo methodUsingName = FindMethod(
                    reflector,
                    type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ParameterDefaultPrefix + capitalizedName,
                    paramType,
                    new[] {paramType},
                    new[] {paramName});

                if (methodUsingIndex != null && methodUsingName != null) {
                    Log.WarnFormat("Duplicate defaults parameter methods {0} and {1} using {1}", methodUsingIndex.Name, methodUsingName.Name);
                }

                MethodInfo methodToUse = methodUsingName ?? methodUsingIndex;

                if (methodToUse != null) {
                    // deliberately not removing both if duplicate to show that method  is duplicate
                    RemoveMethod(methodRemover, methodToUse);

                    // add facets directly to parameters, not to actions 
                    FacetUtils.AddFacet(new ActionDefaultsFacetViaMethod(methodToUse, parameters[i]));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }
        }

        private void FindAndRemoveParametersChoicesMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, IActionParameterSpecImmutable[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                Type paramType = paramTypes[i];
                string paramName = paramNames[i];
                bool isMultiple = false;

                if (CollectionUtils.IsGenericEnumerable(paramType)) {
                    paramType = paramType.GetGenericArguments().First();
                    isMultiple = true;
                }

                Type returnType = typeof (IEnumerable<>).MakeGenericType(paramType);
                string methodName = PrefixesAndRecognisedMethods.ParameterChoicesPrefix + i + capitalizedName;

                MethodInfo[] methods = FindMethods(
                    reflector,
                    type,
                    MethodType.Object,
                    methodName,
                    returnType);

                if (methods.Length > 1) {
                    methods.Skip(1).ForEach(m => Log.WarnFormat("Found multiple action choices methods: {0} in type: {1} ignoring method(s) with params: {2}",
                        methodName,
                        type,
                        m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)));
                }

                MethodInfo methodUsingIndex = methods.FirstOrDefault();

                MethodInfo methodUsingName = FindMethod(
                    reflector,
                    type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ParameterChoicesPrefix + capitalizedName,
                    returnType,
                    new[] {paramType},
                    new[] {paramName});

                if (methodUsingIndex != null && methodUsingName != null) {
                    Log.WarnFormat("Duplicate choices parameter methods {0} and {1} using {1}", methodUsingIndex.Name, methodUsingName.Name);
                }

                MethodInfo methodToUse = methodUsingName ?? methodUsingIndex;

                if (methodToUse != null) {
                    // deliberately not removing both if duplicate to show that method  is duplicate
                    RemoveMethod(methodRemover, methodToUse);

                    // add facets directly to parameters, not to actions 
                    Tuple<string, IObjectSpecImmutable>[] parameterNamesAndTypes = methodToUse.GetParameters().Select(p => new Tuple<string, IObjectSpecImmutable>(p.Name.ToLower(), reflector.LoadSpecification<IObjectSpecImmutable>(p.ParameterType))).ToArray();
                    FacetUtils.AddFacet(new ActionChoicesFacetViaMethod(methodToUse, parameterNamesAndTypes, returnType, parameters[i], isMultiple));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }
        }

        private void FindAndRemoveParametersAutoCompleteMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                // only support on strings and reference types 
                Type paramType = paramTypes[i];
                if (paramType.IsClass || paramType.IsInterface) {

                    //returning an IQueryable ...
                    MethodInfo method = FindAutoCompleteMethod(reflector, type, capitalizedName, i,
                        typeof(IQueryable<>).MakeGenericType(paramType));

                    //.. or returning a single object
                    if (method == null ) {
                        method = FindAutoCompleteMethod(reflector, type, capitalizedName, i, paramType);
                    }

                    //... or returning an enumerable of string
                    if (method == null && (TypeUtils.IsString(paramType))) {                      
                        method = FindAutoCompleteMethod(reflector, type, capitalizedName, i, typeof(IEnumerable<string>));
                    }

                    if (method != null) {
                        var pageSizeAttr = method.GetCustomAttribute<PageSizeAttribute>();
                        var minLengthAttr = (MinLengthAttribute) Attribute.GetCustomAttribute(method.GetParameters().First(), typeof (MinLengthAttribute));

                        int pageSize = pageSizeAttr != null ? pageSizeAttr.Value : 0; // default to 0 ie system default
                        int minLength = minLengthAttr != null ? minLengthAttr.Length : 0;

                        // deliberately not removing both if duplicate to show that method  is duplicate
                        RemoveMethod(methodRemover, method);

                        // add facets directly to parameters, not to actions 
                        FacetUtils.AddFacet(new AutoCompleteFacet(method, pageSize, minLength, parameters[i]));
                        AddOrAddToExecutedWhereFacet(method, parameters[i]);
                    }
                }
            }
        }

        private MethodInfo FindAutoCompleteMethod(IReflector reflector, Type type, string capitalizedName, int i, Type returnType) {
            MethodInfo method = FindMethod(reflector,
                type,
                MethodType.Object,
                PrefixesAndRecognisedMethods.AutoCompletePrefix + i + capitalizedName,
                returnType,
                new[] { typeof(string) });
            return method;
        }

        private void FindAndRemoveParametersValidateMethod(IReflector reflector, IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, IActionParameterSpecImmutable[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                MethodInfo methodUsingIndex = FindMethod(reflector,
                    type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ValidatePrefix + i + capitalizedName,
                    typeof (string),
                    new[] {paramTypes[i]});

                MethodInfo methodUsingName = FindMethod(reflector,
                    type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ValidatePrefix + capitalizedName,
                    typeof (string),
                    new[] {paramTypes[i]},
                    new[] {paramNames[i]});

                if (methodUsingIndex != null && methodUsingName != null) {
                    Log.WarnFormat("Duplicate validate parameter methods {0} and {1} using {1}", methodUsingIndex.Name, methodUsingName.Name);
                }

                MethodInfo methodToUse = methodUsingName ?? methodUsingIndex;

                if (methodToUse != null) {
                    // deliberately not removing both if duplicate to show that method  is duplicate
                    RemoveMethod(methodRemover, methodToUse);

                    // add facets directly to parameters, not to actions 
                    FacetUtils.AddFacet(new ActionParameterValidation(methodToUse, parameters[i]));
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