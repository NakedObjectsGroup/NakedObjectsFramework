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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.Component {
    public sealed class ParallelReflector : IReflector {
        private readonly IObjectReflectorConfiguration config;
        private readonly IFunctionalReflectorConfiguration functionalConfig;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IMetamodelBuilder initialMetamodel;
        private readonly ILogger<ParallelReflector> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IMenuFactory menuFactory;
        private readonly ISet<Type> serviceTypes = new HashSet<Type>();

        public ParallelReflector(IClassStrategy classStrategy,
                                 IMetamodelBuilder metamodel,
                                 IObjectReflectorConfiguration config,
                                 IFunctionalReflectorConfiguration functionalConfig,
                                 IMenuFactory menuFactory,
                                 IEnumerable<IFacetDecorator> facetDecorators,
                                 IEnumerable<IFacetFactory> facetFactories,
                                 ILoggerFactory loggerFactory,
                                 ILogger<ParallelReflector> logger) {
            ClassStrategy = classStrategy ?? throw new InitialisationException($"{nameof(classStrategy)} is null");
            initialMetamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            this.config = config ?? throw new InitialisationException($"{nameof(config)} is null");
            this.functionalConfig = functionalConfig;
            this.menuFactory = menuFactory ?? throw new InitialisationException($"{nameof(menuFactory)} is null");
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
            FacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.ObjectOriented)).ToArray());

            FunctionalFacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.Functional)).ToArray());

        }

        public FacetFactorySet FunctionalFacetFactorySet { get; }

        #region IReflector Members

        public bool ConcurrencyChecking => config.ConcurrencyChecking;

        public bool IgnoreCase => config.IgnoreCase;

        public IClassStrategy ClassStrategy { get; }

        public IFacetFactorySet FacetFactorySet { get; }

        public IMetamodel Metamodel => null;

        public ITypeSpecBuilder LoadSpecification(Type type) => throw new NotImplementedException();

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable => throw new NotImplementedException();

        public ITypeSpecBuilder[] AllObjectSpecImmutables => initialMetamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();

        public (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (type == null) {
                throw new NakedObjectSystemException("cannot load specification for null");
            }

            var actualType = ClassStrategy.GetType(type) ?? type;
            var typeKey = ClassStrategy.GetKeyForType(actualType);
            return !metamodel.ContainsKey(typeKey) ? LoadPlaceholder(actualType, metamodel) : (metamodel[typeKey], metamodel);
        }

        public (T, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification<T>(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) where T : class, ITypeSpecImmutable {
            ITypeSpecBuilder spec;
            (spec, metamodel) = LoadSpecification(type, metamodel);
            return (spec as T, metamodel);
        }

        public void Reflect() {
            Type[] s1 = config.Services.Union(functionalConfig.Services).ToArray();
            var services = s1;
            var nonServices = GetTypesToIntrospect();

            services.ForEach(t => serviceTypes.Add(t));

            var allObjectTypes = services.Union(nonServices).ToArray();

            var metamodelBuilder = InstallSpecifications(allObjectTypes, functionalConfig.Types, functionalConfig.Functions, initialMetamodel);

            PopulateAssociatedActions(s1, metamodelBuilder);

            PopulateAssociatedFunctions(metamodelBuilder);

            //Menus installed once rest of metamodel has been built:
            InstallMainMenus(metamodelBuilder);
            InstallObjectMenus(metamodelBuilder);
        }

        #endregion

        public (ITypeSpecBuilder typeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) IntrospectSpecification(Type actualType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            if (actualType == null) {
                throw new ReflectionException("cannot introspect null");
            }

            var typeKey = ClassStrategy.GetKeyForType(actualType);

            return string.IsNullOrEmpty(metamodel[typeKey].FullName)
                ? LoadSpecificationAndCache(actualType, metamodel, getIntrospector)
                : (metamodel[typeKey], metamodel);
        }

        private static Type EnsureGenericTypeIsComplete(Type type) {
            if (type.IsGenericType && !type.IsConstructedGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericParms = genericType.GetGenericArguments().Select(a => typeof(object)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
            }

            return type;
        }

        private Type[] GetTypesToIntrospect() {
            var types = config.TypesToIntrospect.Select(EnsureGenericTypeIsComplete);
            var systemTypes = config.SupportedSystemTypes.Select(EnsureGenericTypeIsComplete);
            return types.Union(systemTypes).ToArray();
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPlaceholders(IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            var ph = metamodel.Where(i => string.IsNullOrEmpty(i.Value.FullName)).Select(i => i.Value.Type);
            var mm = ph.AsParallel().SelectMany(type => IntrospectSpecification(type, metamodel, getIntrospector).metamodel).Distinct(new TypeSpecKeyComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary();

            return mm.Any(i => string.IsNullOrEmpty(i.Value.FullName))
                ? IntrospectPlaceholders(mm, getIntrospector)
                : mm;
        }

        private IMetamodelBuilder InstallSpecifications(Type[] ooTypes, Type[] records, Type[] functions, IMetamodelBuilder metamodel)
        {
            // first oo
            var mm = GetPlaceholders(ooTypes);
            mm = IntrospectPlaceholders(mm, () => new Introspector(this, FacetFactorySet, loggerFactory.CreateLogger<Introspector>()));
            // then functional
            var allFunctionalTypes = records.Union(functions).ToArray();

            var mm2 = GetPlaceholders(allFunctionalTypes);
            if (mm2.Any())
            {
                mm = mm.AddRange(mm2);
                mm = IntrospectPlaceholders(mm, () => new FunctionalIntrospector(this, FunctionalFacetFactorySet, functions));
            }

            mm.ForEach(i => metamodel.Add(i.Value.Type, i.Value));
            return metamodel;
        }

        private bool IsStatic(ITypeSpecImmutable spec)
        {
            return spec.Type.IsAbstract && spec.Type.IsSealed;
        }

        private bool IsNotStatic(ITypeSpecImmutable spec)
        {
            return !IsStatic(spec);
        }

        private bool IsContributedFunction(IActionSpecImmutable sa, ITypeSpecImmutable ts)
        {
            var f = sa.GetFacet<IContributedFunctionFacet>();
            return f != null && f.IsContributedTo(ts);
        }

        private void PopulateContributedFunctions(ITypeSpecBuilder spec, ITypeSpecImmutable[] functions, IMetamodel metamodel)
        {
            var result = functions.AsParallel().SelectMany(functionsSpec => {

                var serviceActions = functionsSpec.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForObject = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions)
                {

                    if (IsContributedFunction(sa, spec))
                    {
                        matchingActionsForObject.Add(sa);
                    }
                }

                return matchingActionsForObject;
            }).ToList();

            spec.AddContributedFunctions(result);
        }

        private void PopulateAssociatedFunctions(IMetamodelBuilder metamodel)
        {
            // todo add facet for this 
            var functions = metamodel.AllSpecifications.Where(IsStatic).ToArray();
            var objects = metamodel.AllSpecifications.Where(IsNotStatic).Cast<ITypeSpecBuilder>();

            foreach (var spec in objects)
            {
                PopulateContributedFunctions(spec, functions, metamodel);
            }
        }

        private void PopulateAssociatedActions(Type[] services, IMetamodelBuilder metamodel) {
            var nonServiceSpecs = AllObjectSpecImmutables.OfType<IObjectSpecBuilder>();

            foreach (var spec in nonServiceSpecs) {
                PopulateAssociatedActions(spec, services, metamodel);
            }
        }

        private void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services, IMetamodelBuilder metamodel) {
            if (string.IsNullOrWhiteSpace(spec.FullName)) {
                var id = spec.Identifier?.ClassName ?? "unknown";
                logger.LogWarning($"Specification with id : {id} has null or empty name");
            }

            if (FasterTypeUtils.IsSystem(spec.FullName) && !spec.IsCollection) {
                return;
            }

            if (FasterTypeUtils.IsNakedObjects(spec.FullName)) {
                return;
            }

            PopulateContributedActions(spec, services, metamodel);
        }

        private void InstallMainMenus(IMetamodelBuilder metamodel) {
            var menus = config.MainMenus?.Invoke(menuFactory);
            // Unlike other things specified in config, this one can't be checked when ObjectReflectorConfiguration is constructed.
            // Allows developer to deliberately not specify any menus
            if (menus != null) {
                if (!menus.Any()) {
                    //Catches accidental non-specification of menus
                    throw new ReflectionException(logger.LogAndReturn("No MainMenus specified."));
                }

                foreach (var menu in menus.OfType<IMenuImmutable>()) {
                    metamodel.AddMainMenu(menu);
                }
            }
        }

        private static void InstallObjectMenus(IMetamodelBuilder metamodel) {
            var menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }

        private static void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services, IMetamodel metamodel) {
            var (contribActions, collContribActions, finderActions) = services.AsParallel().Select(serviceType => {
                var serviceSpecification = (IServiceSpecImmutable) metamodel.GetSpecification(serviceType);
                var serviceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForObject = new List<IActionSpecImmutable>();
                var matchingActionsForCollection = new List<IActionSpecImmutable>();
                var matchingFinderActions = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions) {
                    if (serviceType != spec.Type) {
                        if (sa.IsContributedTo(spec)) {
                            matchingActionsForObject.Add(sa);
                        }

                        if (sa.IsContributedToCollectionOf(spec)) {
                            matchingActionsForCollection.Add(sa);
                        }
                    }

                    if (sa.IsFinderMethodFor(spec)) {
                        matchingFinderActions.Add(sa);
                    }
                }

                return (matchingActionsForObject, matchingActionsForCollection, matchingFinderActions.OrderBy(a => a, new MemberOrderComparator<IActionSpecImmutable>()).ToList());
            }).Aggregate((new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>()),
                (a, t) => {
                    var (contrib, collContrib, finder) = a;
                    var (ca, cca, fa) = t;
                    contrib.AddRange(ca);
                    collContrib.AddRange(cca);
                    finder.AddRange(fa);
                    return a;
                });

            var groupedContribActions = contribActions.GroupBy(i => i.OwnerSpec.Type, i => i, (service, actions) => new {service, actions}).OrderBy(a => Array.IndexOf(services, a.service)).SelectMany(a => a.actions).ToList();

            spec.AddContributedActions(groupedContribActions);
            spec.AddCollectionContributedActions(collContribActions);
            spec.AddFinderActions(finderActions);
        }

        private ITypeSpecBuilder GetPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            return specification;
        }

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = metamodel.Add(ClassStrategy.GetKeyForType(type), specification);

            return (specification, metamodel);
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types) => types.Select(t => ClassStrategy.GetType(t)).Where(t => t != null).Distinct(new TypeKeyComparer(ClassStrategy)).ToDictionary(t => ClassStrategy.GetKeyForType(t), t => GetPlaceholder(t, null)).ToImmutableDictionary();

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecificationAndCache(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            var specification = metamodel[ClassStrategy.GetKeyForType(type)];

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            //metamodel = specification.Introspect(facetDecoratorSet, new Introspector(this, loggerFactory.CreateLogger<Introspector>()), metamodel);
            metamodel = specification.Introspect(facetDecoratorSet, getIntrospector(), metamodel);

            return (specification, metamodel);
        }

        private ITypeSpecBuilder CreateSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached

            return IsService(type) ? (ITypeSpecBuilder) ImmutableSpecFactory.CreateServiceSpecImmutable(type, metamodel) : ImmutableSpecFactory.CreateObjectSpecImmutable(type, metamodel);
        }

        private bool IsService(Type type) => serviceTypes.Contains(type);

        #region Nested type: TypeKeyComparer

        private class TypeKeyComparer : IEqualityComparer<Type> {
            private readonly IClassStrategy classStrategy;

            public TypeKeyComparer(IClassStrategy classStrategy) => this.classStrategy = classStrategy;

            #region IEqualityComparer<Type> Members

            public bool Equals(Type x, Type y) => classStrategy.GetKeyForType(x).Equals(classStrategy.GetKeyForType(y), StringComparison.Ordinal);

            public int GetHashCode(Type obj) => classStrategy.GetKeyForType(obj).GetHashCode();

            #endregion
        }

        #endregion

        #region Nested type: TypeSpecKeyComparer

        private class TypeSpecKeyComparer : IEqualityComparer<KeyValuePair<string, ITypeSpecBuilder>> {
            #region IEqualityComparer<KeyValuePair<string,ITypeSpecBuilder>> Members

            public bool Equals(KeyValuePair<string, ITypeSpecBuilder> x, KeyValuePair<string, ITypeSpecBuilder> y) => x.Key.Equals(y.Key, StringComparison.Ordinal);

            public int GetHashCode(KeyValuePair<string, ITypeSpecBuilder> obj) => obj.Key.GetHashCode();

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}