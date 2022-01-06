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
using NakedLegacy.Reflector.Facet;
using NakedLegacy.Types;

namespace NakedLegacy.Reflector.FacetFactory;

/// <summary>
///     Sets up all the <see cref="IFacet" />s for an action in a single shot
/// </summary>
public sealed class LegacyFacetFactory : LegacyFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodIdentifyingFacetFactory, IPropertyOrCollectionIdentifyingFacetFactory {
    private static readonly string[] FixedPrefixes = {
        "about"
    };

    private readonly ILogger<LegacyFacetFactory> logger;

    public LegacyFacetFactory(IFacetFactoryOrder<LegacyFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.EverythingButActionParameters) =>
        logger = loggerFactory.CreateLogger<LegacyFacetFactory>();

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
        (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(typeof(object), metamodel);

        methodRemover.SafeRemoveMethod(actionMethod);

        if (actionMethod.IsStatic) {
            facets.Add(new StaticMethodFacet(action));
            facets.Add(new StaticMenuMethodFacet(action));
            facets.Add(new ActionInvocationFacetViaStaticMethod(actionMethod, onType, returnSpec, elementSpec, action, false, Logger<ActionInvocationFacetViaStaticMethod>()));
        }
        else {
            facets.Add(new ActionInvocationFacetViaMethod(actionMethod, onType, returnSpec, elementSpec, action, false, Logger<ActionInvocationFacetViaMethod>()));
        }

        var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name[6..]); //remove 'action' from front 

        facets.Add(new NamedFacetInferred(capitalizedName, action));

        var methodType = actionMethod.IsStatic ? MethodType.Class : MethodType.Object;
        var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType);
        var aboutParamTypes = new List<Type> { typeof(ActionAbout) }.Union(paramTypes).ToArray();

        var method = MethodHelpers.FindMethod(reflector, type, methodType, $"{"about"}{actionMethod.Name}", null, aboutParamTypes);
        methodRemover.SafeRemoveMethod(method);

        if (method is not null) {
            facets.Add(new HideActionForContextViaAboutFacet(method, action, AboutHelpers.AboutType.Action, LoggerFactory.CreateLogger<HideActionForContextViaAboutFacet>()));
            facets.Add(new DisableActionForContextViaAboutFacet(method, action, AboutHelpers.AboutType.Action, LoggerFactory.CreateLogger<DisableActionForContextViaAboutFacet>()));
        }

        MethodHelpers.AddHideForSessionFacetNone(facets, action);
        MethodHelpers.AddDisableForSessionFacetNone(facets, action);

        FacetUtils.AddFacets(facets);

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        //ignore non Legacy properties 
        if (property.PropertyType.Namespace?.StartsWith("NakedLegacy") is false or null) {
            return metamodel;
        }

        var capitalizedName = property.Name;
        var paramTypes = new[] { property.PropertyType };

        var facets = new List<IFacet> { new PropertyAccessorFacet(property, specification) };

        if (property.GetSetMethod() != null) {
            facets.Add(new PropertySetterFacetViaSetterMethod(property, specification));
            facets.Add(new PropertyInitializationFacet(property, specification));
        }
        else {
            facets.Add(new NotPersistedFacet(specification));
            facets.Add(new DisabledFacetAlways(specification));
        }

        var method = MethodHelpers.FindMethod(reflector, property.DeclaringType, MethodType.Object, $"{"about"}{capitalizedName}", null, null);
        methodRemover.SafeRemoveMethod(method);

        if (method is not null) {
            facets.Add(new HideActionForContextViaAboutFacet(method, specification, AboutHelpers.AboutType.Field, LoggerFactory.CreateLogger<HideActionForContextViaAboutFacet>()));
        }

        FacetUtils.AddFacets(facets);
        return metamodel;
    }

    #endregion
}