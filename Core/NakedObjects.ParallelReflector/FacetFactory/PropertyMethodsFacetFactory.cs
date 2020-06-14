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
using Common.Logging;
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
    public sealed class PropertyMethodsFacetFactory : PropertyOrCollectionIdentifyingFacetFactoryAbstract {

        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ModifyPrefix
        };

        private readonly ILogger<PropertyMethodsFacetFactory> logger;

        public PropertyMethodsFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.Properties) =>
            logger = loggerFactory.CreateLogger<PropertyMethodsFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = property.Name;
            var paramTypes = new[] {property.PropertyType};

            var facets = new List<IFacet> {new PropertyAccessorFacet(property, specification)};

            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                facets.Add(new NullableFacetAlways(specification));
            }

            if (property.GetSetMethod() != null) {
                if (property.PropertyType == typeof(byte[])) {
                    facets.Add(new DisabledFacetAlways(specification));
                }
                else {
                    facets.Add(new PropertySetterFacetViaSetterMethod(property, specification));
                }

                facets.Add(new PropertyInitializationFacet(property, specification));
            }
            else {
                facets.Add(new NotPersistedFacet(specification));
                facets.Add(new DisabledFacetAlways(specification));
            }

            FindAndRemoveModifyMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, paramTypes, specification);

            FindAndRemoveAutoCompleteMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
            metamodel = FindAndRemoveChoicesMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification, metamodel);
            FindAndRemoveDefaultMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
            FindAndRemoveValidateMethod(reflector, facets, methodRemover, property.DeclaringType, paramTypes, capitalizedName, specification);

            AddHideForSessionFacetNone(facets, specification);
            AddDisableForSessionFacetNone(facets, specification);
            FindDefaultHideMethod(reflector, facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", specification);
            FindAndRemoveHideMethod(reflector, facets, methodRemover, property.DeclaringType, MethodType.Object, capitalizedName, specification);
            FindDefaultDisableMethod(reflector, facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", specification);
            FindAndRemoveDisableMethod(reflector, facets, methodRemover, property.DeclaringType, MethodType.Object, capitalizedName, specification);

            FacetUtils.AddFacets(facets);
            return metamodel;
        }

        private void FindAndRemoveModifyMethod(IReflector reflector,
                                               ICollection<IFacet> propertyFacets,
                                               IMethodRemover methodRemover,
                                               Type type,
                                               string capitalizedName,
                                               Type[] parms,
                                               ISpecification property) {
            var method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ModifyPrefix + capitalizedName, typeof(void), parms);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertySetterFacetViaModifyMethod(method, capitalizedName, property, Logger<PropertySetterFacetViaModifyMethod>()));
            }
        }

        private void FindAndRemoveValidateMethod(IReflector reflector, ICollection<IFacet> propertyFacets, IMethodRemover methodRemover, Type type, Type[] parms, string capitalizedName, ISpecification property) {
            var method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ValidatePrefix + capitalizedName, typeof(string), parms);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyValidateFacetViaMethod(method, property, Logger<PropertyValidateFacetViaMethod>()));
                AddAjaxFacet(method, property);
            }
            else {
                AddAjaxFacet(null, property);
            }
        }

        private void FindAndRemoveDefaultMethod(IReflector reflector,
                                                ICollection<IFacet> propertyFacets,
                                                IMethodRemover methodRemover,
                                                Type type,
                                                string capitalizedName,
                                                Type returnType,
                                                ISpecification property) {
            var method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.DefaultPrefix + capitalizedName, returnType, Type.EmptyTypes);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyDefaultFacetViaMethod(method, property, Logger<PropertyDefaultFacetViaMethod>()));
                AddOrAddToExecutedWhereFacet(method, property);
            }
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> FindAndRemoveChoicesMethod(IReflector reflector,
                                                                                          ICollection<IFacet> propertyFacets,
                                                                                          IMethodRemover methodRemover,
                                                                                          Type type,
                                                                                          string capitalizedName,
                                                                                          Type returnType,
                                                                                          ISpecification property,
                                                                                          IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var methods = FindMethods(reflector,
                type,
                MethodType.Object,
                RecognisedMethodsAndPrefixes.ChoicesPrefix + capitalizedName,
                typeof(IEnumerable<>).MakeGenericType(returnType));

            if (methods.Length > 1) {
                var name = RecognisedMethodsAndPrefixes.ChoicesPrefix + capitalizedName;
                methods.Skip(1).ForEach(m => logger.LogWarning($"Found multiple choices methods: {name} in type: {type} ignoring method(s) with params: {m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)}"));
            }

            var method = methods.FirstOrDefault();
            RemoveMethod(methodRemover, method);
            if (method != null) {
                var parameterNamesAndTypes = new List<(string, IObjectSpecImmutable)>();

                foreach (var p in method.GetParameters()) {
                    IObjectSpecBuilder oSpec;
                    (oSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(p.ParameterType, metamodel);
                    parameterNamesAndTypes.Add((p.Name.ToLower(), oSpec));
                }

                propertyFacets.Add(new PropertyChoicesFacet(method, parameterNamesAndTypes.ToArray(), property, Logger<PropertyChoicesFacet>()));
                AddOrAddToExecutedWhereFacet(method, property);
            }

            return metamodel;
        }

        private void FindAndRemoveAutoCompleteMethod(IReflector reflector,
                                                     ICollection<IFacet> propertyFacets,
                                                     IMethodRemover methodRemover,
                                                     Type type,
                                                     string capitalizedName,
                                                     Type returnType,
                                                     ISpecification property) {
            // only support if property is string or domain type
            if (returnType.IsClass || returnType.IsInterface) {
                var method = FindAutoCompleteMethod(reflector, type, capitalizedName,
                    typeof(IQueryable<>).MakeGenericType(returnType));

                //.. or returning a single object
                if (method == null) {
                    method = FindAutoCompleteMethod(reflector, type, capitalizedName, returnType);
                }

                //... or returning an enumerable of string
                if (method == null && TypeUtils.IsString(returnType)) {
                    method = FindAutoCompleteMethod(reflector, type, capitalizedName, typeof(IEnumerable<string>));
                }

                if (method != null) {
                    var pageSizeAttr = method.GetCustomAttribute<PageSizeAttribute>();
                    var minLengthAttr = (MinLengthAttribute) Attribute.GetCustomAttribute(method.GetParameters().First(), typeof(MinLengthAttribute));

                    var pageSize = pageSizeAttr != null ? pageSizeAttr.Value : 0; // default to 0 ie system default
                    var minLength = minLengthAttr != null ? minLengthAttr.Length : 0;

                    RemoveMethod(methodRemover, method);
                    propertyFacets.Add(new AutoCompleteFacet(method, pageSize, minLength, property, Logger<AutoCompleteFacet>()));
                    AddOrAddToExecutedWhereFacet(method, property);
                }
            }
        }

        private MethodInfo FindAutoCompleteMethod(IReflector reflector, Type type, string capitalizedName, Type returnType) {
            var method = FindMethod(reflector,
                type,
                MethodType.Object,
                RecognisedMethodsAndPrefixes.AutoCompletePrefix + capitalizedName,
                returnType,
                new[] {typeof(string)});
            return method;
        }

        public override IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            candidates = candidates.Where(property => !CollectionUtils.IsQueryable(property.PropertyType)).ToArray();
            return PropertiesToBeIntrospected(candidates, classStrategy);
        }
    }
}