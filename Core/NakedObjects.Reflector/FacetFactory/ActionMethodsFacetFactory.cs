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
using NakedObjects.Architecture.Util;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Metamodel.Utils;
using NakedObjects.Util;
using NakedObjects.Metamodel.SpecImmutable;

namespace NakedObjects.Reflector.FacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public class ActionMethodsFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;

        private static readonly ILog Log;

        static ActionMethodsFacetFactory() {
            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.AutoCompletePrefix,
                PrefixesAndRecognisedMethods.ParameterDefaultPrefix,
                PrefixesAndRecognisedMethods.ParameterChoicesPrefix,
                PrefixesAndRecognisedMethods.DisablePrefix,
                PrefixesAndRecognisedMethods.HidePrefix,
                PrefixesAndRecognisedMethods.ValidatePrefix,
                PrefixesAndRecognisedMethods.DisablePrefix
            };
            Log = LogManager.GetLogger(typeof (ActionMethodsFacetFactory));
        }

        /// <summary>
        ///     The <see cref="IFacet" />s registered are the generic ones from no-architecture (where they exist)
        /// </summary>
        public ActionMethodsFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ActionsAndParameters) {}

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override bool Process(MethodInfo actionMethod, IMethodRemover methodRemover, ISpecification action) {
            string capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);

            Type type = actionMethod.DeclaringType;
            var facets = new List<IFacet>();
            var onType = Reflector.LoadSpecification(type);
            var returnSpec = Reflector.LoadSpecification(actionMethod.ReturnType);

            RemoveMethod(methodRemover, actionMethod);
            facets.Add(new ActionInvocationFacetViaMethod(actionMethod, onType, returnSpec, action));

            MethodType methodType = actionMethod.IsStatic ? MethodType.Class : MethodType.Object;
            Type[] paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            FindAndRemoveValidMethod(facets, methodRemover, type, methodType, capitalizedName, paramTypes, action);

            DefaultNamedFacet(facets, capitalizedName, action); // must be called after the checkForXxxPrefix methods

            AddHideForSessionFacetNone(facets, action);
            AddDisableForSessionFacetNone(facets, action);
            FindDefaultHideMethod(facets, methodRemover, type, methodType, "ActionDefault", paramTypes, action);
            FindAndRemoveHideMethod(facets, methodRemover, type, methodType, capitalizedName, paramTypes, action);
            FindDefaultDisableMethod(facets, methodRemover, type, methodType, "ActionDefault", paramTypes, action);
            FindAndRemoveDisableMethod(facets, methodRemover, type, methodType, capitalizedName, paramTypes, action);

            if (action is ActionSpecImmutable) {
                var nakedObjectActionPeer = (ActionSpecImmutable) action;
                // Process the action's parameters names, descriptions and optional
                // an alternative design would be to have another facet factory processing just ActionParameter, and have it remove these
                // supporting methods.  However, the FacetFactory API doesn't allow for methods of the class to be removed while processing
                // action parameters, only while processing Methods (ie actions)
                IActionParameterSpecImmutable[] actionParameters = nakedObjectActionPeer.Parameters;
                string[] paramNames = actionMethod.GetParameters().Select(p => p.Name).ToArray();

                FindAndRemoveParametersAutoCompleteMethod(methodRemover, type, capitalizedName, paramTypes, actionParameters);
                FindAndRemoveParametersChoicesMethod(methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
                FindAndRemoveParametersDefaultsMethod(methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
                FindAndRemoveParametersValidateMethod(methodRemover, type, capitalizedName, paramTypes, paramNames, actionParameters);
            }
            return FacetUtils.AddFacets(facets);
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            IFacet facet = null;

            if (parameter.ParameterType.IsGenericType && (parameter.ParameterType.GetGenericTypeDefinition() == typeof (Nullable<>))) {
                facet = new NullableFacetAlways(holder);
            }

            return FacetUtils.AddFacet(facet);
        }


        /// <summary>
        ///     Must be called after the <c>CheckForXxxPrefix</c> methods.
        /// </summary>
        private static void DefaultNamedFacet(ICollection<IFacet> actionFacets, string capitalizedName, ISpecification action) {
            actionFacets.Add(new NamedFacetInferred(NameUtils.NaturalName(capitalizedName), action));
        }

        private void FindAndRemoveValidMethod(ICollection<IFacet> actionFacets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] parms, ISpecification action) {
            MethodInfo method = FindMethod(type, methodType, PrefixesAndRecognisedMethods.ValidatePrefix + capitalizedName, typeof (string), parms);
            if (method != null) {
                RemoveMethod(methodRemover, method);
                actionFacets.Add(new ActionValidationFacet(method, action));
            }
        }

        private void FindAndRemoveParametersDefaultsMethod(IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, ISpecification[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                Type paramType = paramTypes[i];
                string paramName = paramNames[i];


                MethodInfo methodUsingIndex = FindMethodWithOrWithoutParameters(type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ParameterDefaultPrefix + i + capitalizedName,
                    paramType,
                    paramTypes);

                MethodInfo methodUsingName = FindMethod(type,
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

        private void FindAndRemoveParametersChoicesMethod(IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, ISpecification[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                Type paramType = paramTypes[i];
                string paramName = paramNames[i];
                bool isMultiple = false;

                if (CollectionUtils.IsGenericEnumerable(paramType)) {
                    paramType = paramType.GetGenericArguments().First();
                    isMultiple = true;
                }

                Type returnType = typeof (IEnumerable<>).MakeGenericType(paramType);

                MethodInfo[] methods = FindMethods(type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ParameterChoicesPrefix + i + capitalizedName,
                    returnType);

                if (methods.Length > 1) {
                    methods.Skip(1).ForEach(m => Log.WarnFormat("Found multiple action choices methods: {0} in type: {1} ignoring method(s) with params: {2}",
                        PrefixesAndRecognisedMethods.ParameterChoicesPrefix + i + capitalizedName,
                        type,
                        m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)));
                }

                MethodInfo methodUsingIndex = methods.FirstOrDefault();

                MethodInfo methodUsingName = FindMethod(type,
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
                    var parameterNamesAndTypes = methodToUse.GetParameters().Select(p => new Tuple<string, IObjectSpecImmutable>(p.Name.ToLower(), Reflector.LoadSpecification(p.ParameterType))).ToArray();
                    FacetUtils.AddFacet(new ActionChoicesFacetViaMethod(methodToUse, parameterNamesAndTypes, returnType, parameters[i], isMultiple));
                    AddOrAddToExecutedWhereFacet(methodToUse, parameters[i]);
                }
            }
        }

        private void FindAndRemoveParametersAutoCompleteMethod(IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                // only support on strings and reference types 

                if (paramTypes[i].IsClass || paramTypes[i].IsInterface) {
                    Type returnType = typeof (IQueryable<>).MakeGenericType(paramTypes[i]);

                    MethodInfo method = FindMethod(type,
                        MethodType.Object,
                        PrefixesAndRecognisedMethods.AutoCompletePrefix + i + capitalizedName,
                        returnType,
                        new[] {typeof (string)});

                    if (method != null) {
                        var pageSizeAttr = AttributeUtils.GetCustomAttribute<PageSizeAttribute>(method);
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


        private void FindAndRemoveParametersValidateMethod(IMethodRemover methodRemover, Type type, string capitalizedName, Type[] paramTypes, string[] paramNames, ISpecification[] parameters) {
            for (int i = 0; i < paramTypes.Length; i++) {
                MethodInfo methodUsingIndex = FindMethod(type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.ValidatePrefix + i + capitalizedName,
                    typeof (string),
                    new[] {paramTypes[i]});

                MethodInfo methodUsingName = FindMethod(type,
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
                    FacetUtils.AddFacet(new ActionParameterValidation(methodToUse, i, parameters[i]));
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