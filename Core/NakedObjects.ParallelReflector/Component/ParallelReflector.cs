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
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.Component {
    public sealed class ParallelReflector : IReflector {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ParallelReflector));
        private readonly IReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IMetamodelBuilder initialMetamodel;
        private readonly IMenuFactory menuFactory;
        private readonly ISet<Type> serviceTypes = new HashSet<Type>();

        public ParallelReflector(IClassStrategy classStrategy,
                                 IMetamodelBuilder metamodel,
                                 IReflectorConfiguration config,
                                 IMenuFactory menuFactory,
                                 IEnumerable<IFacetDecorator> facetDecorators,
                                 IEnumerable<IFacetFactory> facetFactories) {
            Assert.AssertNotNull(classStrategy);
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(config);
            Assert.AssertNotNull(menuFactory);

            ClassStrategy = classStrategy;
            initialMetamodel = metamodel;
            this.config = config;
            this.menuFactory = menuFactory;
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
            FacetFactorySet = new FacetFactorySet(facetFactories.ToArray());
        }

        // exposed for testing
        public IFacetDecoratorSet FacetDecoratorSet => facetDecoratorSet;

        #region IReflector Members

        public bool ConcurrencyChecking => config.ConcurrencyChecking;

        public bool IgnoreCase => config.IgnoreCase;

        public IClassStrategy ClassStrategy { get; }

        public IFacetFactorySet FacetFactorySet { get; }

        public IMetamodel Metamodel => null;

        public ITypeSpecBuilder LoadSpecification(Type type) => throw new NotImplementedException();

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable => throw new NotImplementedException();

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) => throw new NotImplementedException();

        public ITypeSpecBuilder[] AllObjectSpecImmutables => initialMetamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();

        public (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Assert.AssertNotNull(type);

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
            var s1 = config.Services;
            var services = s1;
            var nonServices = GetTypesToIntrospect();

            services.ForEach(t => serviceTypes.Add(t));

            var allTypes = services.Union(nonServices).ToArray();

            var mm = InstallSpecificationsParallel(allTypes, initialMetamodel);

            PopulateAssociatedActions(s1, mm);

            //Menus installed once rest of metamodel has been built:
            InstallMainMenus(mm);
            InstallObjectMenus(mm);
        }

        #endregion

        public (ITypeSpecBuilder tupeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) IntrospectSpecification(Type actualType, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Assert.AssertNotNull(actualType);

            var typeKey = ClassStrategy.GetKeyForType(actualType);

            return string.IsNullOrEmpty(metamodel[typeKey].FullName)
                ? LoadSpecificationAndCache(actualType, metamodel)
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

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPlaceholders(IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var ph = metamodel.Where(i => string.IsNullOrEmpty(i.Value.FullName)).Select(i => i.Value.Type);
            var mm = ph.AsParallel().SelectMany(type => IntrospectSpecification(type, metamodel).metamodel).Distinct(new TypeSpecKeyComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary();

            return mm.Any(i => string.IsNullOrEmpty(i.Value.FullName))
                ? IntrospectPlaceholders(mm)
                : mm;
        }

        private IMetamodelBuilder InstallSpecificationsParallel(Type[] types, IMetamodelBuilder metamodel) {
            var mm = GetPlaceholders(types);
            mm = IntrospectPlaceholders(mm);
            mm.ForEach(i => metamodel.Add(i.Value.Type, i.Value));
            return metamodel;
        }

        private void PopulateAssociatedActions(Type[] services, IMetamodelBuilder metamodel) {
            var nonServiceSpecs = AllObjectSpecImmutables.OfType<IObjectSpecBuilder>();

            foreach (var spec in nonServiceSpecs) {
                PopulateAssociatedActions(spec, services, metamodel);
            }
        }

        private static void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services, IMetamodelBuilder metamodel) {
            if (string.IsNullOrWhiteSpace(spec.FullName)) {
                var id = (spec.Identifier != null ? spec.Identifier.ClassName : "unknown") ?? "unknown";
                Log.WarnFormat("Specification with id : {0} as has null or empty name", id);
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
            // Unlike other things specified in config, this one can't be checked when ReflectorConfiguration is constructed.
            // Allows developer to deliberately not specify any menus
            if (menus != null) {
                if (!menus.Any()) {
                    //Catches accidental non-specification of menus
                    throw new ReflectionException(Log.LogAndReturn("No MainMenus specified."));
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

            //
            var groupedContribActions = new List<IActionSpecImmutable>();

            // group by service - probably do this better - TODO
            foreach (var service in services) {
                var matching = contribActions.Where(i => i.OwnerSpec.Type == service);
                groupedContribActions.AddRange(matching);
            }

            spec.AddContributedActions(groupedContribActions);
            spec.AddCollectionContributedActions(collContribActions);
            spec.AddFinderActions(finderActions);
        }

        private ITypeSpecBuilder GetPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(Log.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            return specification;
        }

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadPlaceholder(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(Log.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = metamodel.Add(ClassStrategy.GetKeyForType(type), specification);

            return (specification, metamodel);
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types) => types.Select(t => ClassStrategy.GetType(t)).Where(t => t != null).Distinct(new TypeKeyComparer(ClassStrategy)).ToDictionary(t => ClassStrategy.GetKeyForType(t), t => GetPlaceholder(t, null)).ToImmutableDictionary();

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecificationAndCache(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specification = metamodel[ClassStrategy.GetKeyForType(type)];

            if (specification == null) {
                throw new ReflectionException(Log.LogAndReturn($"unrecognised type {type.FullName}"));
            }

            metamodel = specification.Introspect(facetDecoratorSet, new Introspector(this), metamodel);

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