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
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NakedObjects.Reflector.Facet;
using NakedObjects.Reflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class PropertyMethodsFacetFactory : DomainObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IPropertyOrCollectionIdentifyingFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.ModifyPrefix
    };

    private readonly ILogger<PropertyMethodsFacetFactory> logger;

    public PropertyMethodsFacetFactory(IFacetFactoryOrder<PropertyMethodsFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Properties) =>
        logger = loggerFactory.CreateLogger<PropertyMethodsFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    public override IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
        candidates = candidates.Where(property => !CollectionUtils.IsQueryable(property.PropertyType)).ToArray();
        return PropertiesToBeIntrospected(candidates, classStrategy);
    }

    private static IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) =>
        candidates.Where(property => property.HasPublicGetter() &&
                                     !classStrategy.IsIgnored(property.PropertyType) &&
                                     !classStrategy.IsIgnored(property)).ToList();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var capitalizedName = property.Name;
        var paramTypes = new[] { property.PropertyType };

        var facets = new List<IFacet> { new PropertyAccessorFacet(property, Logger<PropertyAccessorFacet>()) };

        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
            facets.Add(NullableFacetAlways.Instance);
        }

        if (property.GetSetMethod() != null) {
            if (property.PropertyType == typeof(byte[])) {
                facets.Add(DisabledFacetAlways.Instance);
            }
            else {
                facets.Add(new PropertySetterFacetViaSetterMethod(property, Logger<PropertySetterFacetViaSetterMethod>()));
            }

            facets.Add(new PropertyInitializationFacet(property, Logger<PropertyInitializationFacet>()));
        }
        else {
            facets.Add(NotPersistedFacet.Instance);
            facets.Add(DisabledFacetAlways.Instance);
        }

        FindAndRemoveModifyMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, paramTypes, specification);

        FindAndRemoveAutoCompleteMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
        metamodel = FindAndRemoveChoicesMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification, metamodel);
        FindAndRemoveDefaultMethod(reflector, facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
        FindAndRemoveValidateMethod(reflector, facets, methodRemover, property.DeclaringType, paramTypes, capitalizedName, specification);

        MethodHelpers.AddHideForSessionFacetNone(facets, specification);
        MethodHelpers.AddDisableForSessionFacetNone(facets, specification);
        ObjectMethodHelpers.FindDefaultHideMethod(reflector, facets, property.DeclaringType, MethodType.Object, "PropertyDefault", specification, LoggerFactory);
        ObjectMethodHelpers.FindAndRemoveHideMethod(reflector, facets, property.DeclaringType, MethodType.Object, capitalizedName, specification, LoggerFactory, methodRemover);
        ObjectMethodHelpers.FindDefaultDisableMethod(reflector, facets, property.DeclaringType, MethodType.Object, "PropertyDefault", specification, LoggerFactory);
        ObjectMethodHelpers.FindAndRemoveDisableMethod(reflector, facets, property.DeclaringType, MethodType.Object, capitalizedName, specification, LoggerFactory, methodRemover);

        FacetUtils.AddFacets(facets, specification);
        return metamodel;
    }

    private void FindAndRemoveModifyMethod(IReflector reflector,
                                           ICollection<IFacet> propertyFacets,
                                           IMethodRemover methodRemover,
                                           Type type,
                                           string capitalizedName,
                                           Type[] parms,
                                           ISpecification property) {
        var method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ModifyPrefix + capitalizedName, typeof(void), parms);
        methodRemover.SafeRemoveMethod(method);
        if (method is not null) {
            propertyFacets.Add(new PropertySetterFacetViaModifyMethod(method, capitalizedName, Logger<PropertySetterFacetViaModifyMethod>()));
        }
    }

    private void FindAndRemoveValidateMethod(IReflector reflector, ICollection<IFacet> propertyFacets, IMethodRemover methodRemover, Type type, Type[] parms, string capitalizedName, ISpecification property) {
        var method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ValidatePrefix + capitalizedName, typeof(string), parms);
        methodRemover.SafeRemoveMethod(method);
        if (method is not null) {
            propertyFacets.Add(new PropertyValidateFacetViaMethod(method, Logger<PropertyValidateFacetViaMethod>()));
        }
    }

    private void FindAndRemoveDefaultMethod(IReflector reflector,
                                            ICollection<IFacet> propertyFacets,
                                            IMethodRemover methodRemover,
                                            Type type,
                                            string capitalizedName,
                                            Type returnType,
                                            ISpecification property) {
        var method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.DefaultPrefix + capitalizedName, returnType, Type.EmptyTypes);
        methodRemover.SafeRemoveMethod(method);
        if (method is not null) {
            propertyFacets.Add(new PropertyDefaultFacetViaMethod(method, Logger<PropertyDefaultFacetViaMethod>()));
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
        var methods = ObjectMethodHelpers.FindMethods(reflector,
                                                      type,
                                                      MethodType.Object,
                                                      RecognisedMethodsAndPrefixes.ChoicesPrefix + capitalizedName,
                                                      typeof(IEnumerable<>).MakeGenericType(returnType));

        if (methods.Length > 1) {
            var name = $"{RecognisedMethodsAndPrefixes.ChoicesPrefix}{capitalizedName}";
            methods.Skip(1).ForEach(m => logger.LogWarning($"Found multiple choices methods: {name} in type: {type} ignoring method(s) with params: {m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)}"));
        }

        var method = methods.FirstOrDefault();
        methodRemover.SafeRemoveMethod(method);
        if (method is not null) {
            var parameterNamesAndTypes = new List<(string, Type)>();

            foreach (var p in method.GetParameters()) {
                (_, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(p.ParameterType, metamodel);
                parameterNamesAndTypes.Add((p.Name.ToLower(), p.ParameterType));
            }

            propertyFacets.Add(new PropertyChoicesFacet(method, parameterNamesAndTypes.ToArray(), Logger<PropertyChoicesFacet>()));
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
            var method = FindAutoCompleteMethod(reflector, type, capitalizedName, typeof(IQueryable<>).MakeGenericType(returnType)) ??
                         FindAutoCompleteMethod(reflector, type, capitalizedName, returnType); //.. or returning a single object

            //... or returning an enumerable of string
            if (method is null && TypeUtils.IsString(returnType)) {
                method = FindAutoCompleteMethod(reflector, type, capitalizedName, typeof(IEnumerable<string>));
            }

            if (method is not null) {
                var pageSizeAttr = method.GetCustomAttribute<PageSizeAttribute>();
                var minLengthAttr = (MinLengthAttribute)Attribute.GetCustomAttribute(method.GetParameters().First(), typeof(MinLengthAttribute));

                var pageSize = pageSizeAttr?.Value ?? 0; // default to 0 ie system default
                var minLength = minLengthAttr?.Length ?? 0;

                methodRemover.SafeRemoveMethod(method);
                propertyFacets.Add(new AutoCompleteFacet(method, pageSize, minLength, Logger<AutoCompleteFacet>()));
            }
        }
    }

    private static MethodInfo FindAutoCompleteMethod(IReflector reflector, Type type, string capitalizedName, Type returnType) =>
        MethodHelpers.FindMethod(reflector,
                                 type,
                                 MethodType.Object,
                                 $"{RecognisedMethodsAndPrefixes.AutoCompletePrefix}{capitalizedName}",
                                 returnType,
                                 new[] { typeof(string) });
}