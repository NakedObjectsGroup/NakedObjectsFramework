// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using NOF2.About;
using NOF2.Reflector.Facet;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.FacetFactory;

/// <summary>
///     Sets up all the <see cref="IFacet" />s for an action in a single shot
/// </summary>
public sealed class AboutsFacetFactory : AbstractNOF2FacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodIdentifyingFacetFactory, IPropertyOrCollectionIdentifyingFacetFactory {
    private static readonly string[] FixedPrefixes = {
        NOF2Helpers.AboutPrefix
    };

    private readonly ILogger<AboutsFacetFactory> logger;

    public AboutsFacetFactory(IFacetFactoryOrder<AboutsFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.EverythingButActionParameters) =>
        logger = loggerFactory.CreateLogger<AboutsFacetFactory>();

    public IList<MethodInfo> FindActions(IList<MethodInfo> candidates, IClassStrategy classStrategy) {
        return candidates.Where(methodInfo => methodInfo.Name.ToLower().StartsWith("action") &&
                                              !classStrategy.IsIgnored(methodInfo) &&
                                              !methodInfo.IsGenericMethod &&
                                              !classStrategy.IsIgnored(methodInfo.ReturnType)).ToArray();
    }

    public string[] Prefixes => FixedPrefixes;

    public override IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
        candidates = candidates.Where(property => !CollectionUtils.IsQueryable(property.PropertyType)).ToArray();
        return PropertiesToBeIntrospected(candidates, classStrategy);
    }

    private static IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) =>
        candidates.Where(property => property.HasPublicGetter() &&
                                     classStrategy.IsTypeRecognizedByReflector(property.PropertyType) &&
                                     !classStrategy.IsIgnored(property)).ToList();

    #region IMethodIdentifyingFacetFactory Members

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var type = actionMethod.DeclaringType;
        var facets = new List<IFacet>();

        ITypeSpecBuilder onType;
        (onType, metamodel) = reflector.LoadSpecification(type, metamodel);

        IObjectSpecBuilder returnSpec;
        (returnSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(actionMethod.ReturnType, metamodel);

        IObjectSpecBuilder elementSpec;
        if (actionMethod.ReturnType.IsGenericType) {
            var elementType = actionMethod.ReturnType.GetGenericArguments().First();

            (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
            facets.Add(new ElementTypeFacet(action, elementType, elementSpec));
            facets.Add(new TypeOfFacetInferredFromGenerics(action));
        }
        else {
            (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(typeof(object), metamodel);
        }

        methodRemover.SafeRemoveMethod(actionMethod);

        if (actionMethod.IsStatic) {
            facets.Add(new StaticMethodFacet(action));
            facets.Add(new StaticMenuMethodFacet(action));
            facets.Add(new ActionInvocationFacetViaStaticMethod(actionMethod, onType, returnSpec, elementSpec, action, false, Logger<ActionInvocationFacetViaStaticMethod>()));
        }
        else {
            facets.Add(new ActionInvocationFacetViaMethod(actionMethod, onType, returnSpec, elementSpec, action, false, Logger<ActionInvocationFacetViaMethod>()));
        }

        var aName = actionMethod.Name;

        var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name[6..]); //remove 'action' from front 

        facets.Add(new MemberNamedFacetInferred(capitalizedName, action));

        var methodType = actionMethod.IsStatic ? MethodType.Class : MethodType.Object;
        var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType);
        var aboutParamTypes = new List<Type> { typeof(ActionAbout) };
        aboutParamTypes.AddRange(paramTypes);
        var aboutParams = aboutParamTypes.ToArray();

        var method = MethodHelpers.FindMethod(reflector, type, methodType, $"{NOF2Helpers.AboutPrefix}{actionMethod.Name}", typeof(void), aboutParams) ??
                     MethodHelpers.FindMethod(reflector, type, methodType, $"{NOF2Helpers.AboutPrefix}{actionMethod.Name}", typeof(void), new[] { typeof(ActionAbout) });
        methodRemover.SafeRemoveMethod(method);

        if (method is not null) {
            facets.Add(new DescribedAsViaAboutMethodFacet(method, action, AboutHelpers.AboutType.Action, Logger<DescribedAsViaAboutMethodFacet>()));
            facets.Add(new DisableForContextViaAboutMethodFacet(method, action, AboutHelpers.AboutType.Action, Logger<DisableForContextViaAboutMethodFacet>()));
            facets.Add(new HideForContextViaAboutMethodFacet(method, action, AboutHelpers.AboutType.Action, Logger<HideForContextViaAboutMethodFacet>()));
            facets.Add(new MemberNamedViaAboutMethodFacet(method, action, AboutHelpers.AboutType.Action, actionMethod.Name, Logger<MemberNamedViaAboutMethodFacet>()));
            facets.Add(new ActionValidateViaAboutMethodFacet(method, action, AboutHelpers.AboutType.Action, Logger<ActionValidateViaAboutMethodFacet>()));

            var actionSpec = (IActionSpecImmutable)action;
            var parameterFacets = new List<IFacet>();

            var index = 0; // about is 0
            foreach (var parameterSpec in actionSpec.Parameters) {
                parameterFacets.Add(new MemberNamedViaAboutMethodFacet(method, parameterSpec, AboutHelpers.AboutType.Action, parameterSpec.Identifier.MemberParameterNames, index, Logger<MemberNamedViaAboutMethodFacet>()));
                parameterFacets.Add(new ActionDefaultsViaAboutMethodFacet(method, parameterSpec, index, Logger<ActionDefaultsViaAboutMethodFacet>()));
                parameterFacets.Add(new ActionChoicesViaAboutMethodFacet(method, parameterSpec, index, Logger<ActionChoicesViaAboutMethodFacet>()));
                index++;
            }

            FacetUtils.AddFacets(parameterFacets);
        }

        MethodHelpers.AddHideForSessionFacetNone(facets, action);
        MethodHelpers.AddDisableForSessionFacetNone(facets, action);

        FacetUtils.AddFacets(facets);

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var capitalizedName = property.Name;
        var paramTypes = new[] { property.PropertyType };

        var facets = new List<IFacet> { new PropertyAccessorFacet(property, specification) };

        if (property.GetSetMethod() is not null) {
            facets.Add(new PropertySetterFacetViaSetterMethod(property, specification));
            facets.Add(new PropertyInitializationFacet(property, specification));
        }

        facets.Add(new MemberNamedFacetInferred(specification.Identifier.MemberName, specification));

        var method = MethodHelpers.FindMethod(reflector, property.DeclaringType, MethodType.Object, $"{NOF2Helpers.AboutPrefix}{capitalizedName}", typeof(void), new[] { typeof(FieldAbout), property.PropertyType }) ??
                     MethodHelpers.FindMethod(reflector, property.DeclaringType, MethodType.Object, $"{NOF2Helpers.AboutPrefix}{capitalizedName}", typeof(void), new[] { typeof(FieldAbout) });

        methodRemover.SafeRemoveMethod(method);

        if (method is not null) {
            facets.Add(new DescribedAsViaAboutMethodFacet(method, specification, AboutHelpers.AboutType.Field, LoggerFactory.CreateLogger<DescribedAsViaAboutMethodFacet>()));
            facets.Add(new DisableForContextViaAboutMethodFacet(method, specification, AboutHelpers.AboutType.Field, LoggerFactory.CreateLogger<DisableForContextViaAboutMethodFacet>()));
            facets.Add(new HideForContextViaAboutMethodFacet(method, specification, AboutHelpers.AboutType.Field, LoggerFactory.CreateLogger<HideForContextViaAboutMethodFacet>()));
            facets.Add(new MemberNamedViaAboutMethodFacet(method, specification, AboutHelpers.AboutType.Field, property.Name, LoggerFactory.CreateLogger<MemberNamedViaAboutMethodFacet>()));
            facets.Add(new PropertyValidateViaAboutMethodFacet(method, specification, AboutHelpers.AboutType.Field, LoggerFactory.CreateLogger<PropertyValidateViaAboutMethodFacet>()));
            facets.Add(new PropertyChoicesViaAboutMethodFacet(method, specification, LoggerFactory.CreateLogger<PropertyChoicesViaAboutMethodFacet>()));
        }

        var valueType = NOF2Helpers.IsOrImplementsValueHolder(property.PropertyType);

        if (valueType is not null) {
            var setter = typeof(PropertySetterFacetViaValueHolder<,>).MakeGenericType(property.PropertyType, valueType);
            var setterFacet = (IFacet)Activator.CreateInstance(setter, property, specification);
            facets.Add(setterFacet);
        }

        FacetUtils.AddFacets(facets);
        return metamodel;
    }

    #endregion
}