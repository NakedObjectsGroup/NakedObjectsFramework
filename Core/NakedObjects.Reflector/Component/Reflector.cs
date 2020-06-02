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

namespace NakedObjects.Reflect.Component {
    // This is designed to run once, single threaded at startup. It is not intended to be thread safe.
    public sealed class Reflector : IReflector {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Reflector));
        private readonly IReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IMenuFactory menuFactory;
        private readonly IMetamodelBuilder metamodel;
        private readonly ISet<Type> serviceTypes = new HashSet<Type>();

        public Reflector(IClassStrategy classStrategy,
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
            this.metamodel = metamodel;
            this.config = config;
            this.menuFactory = menuFactory;
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
            FacetFactorySet = new FacetFactorySet(facetFactories.ToArray());
        }

        #region IReflector Members

        public bool ConcurrencyChecking => config.ConcurrencyChecking;

        public bool IgnoreCase => config.IgnoreCase;

        public IClassStrategy ClassStrategy { get; }

        public IFacetFactorySet FacetFactorySet { get; }

        public IMetamodel Metamodel => metamodel;

        public ITypeSpecBuilder[] AllObjectSpecImmutables => metamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();

        public ITypeSpecBuilder LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            return (ITypeSpecBuilder) metamodel.GetSpecification(type, true) ?? LoadSpecificationAndCache(type);
        }

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable {
            var spec = LoadSpecification(type);
            try {
                return (T) spec;
            }
            catch (Exception) {
                throw new ReflectionException(Log.LogAndReturn($"Specification for type {type.Name} is {spec.GetType().Name}: cannot be cast to {typeof(T).Name}"));
            }
        }

        public void Reflect() {
            var s1 = config.Services;
            var services = s1.ToArray();
            var nonServices = GetTypesToIntrospect();

            services.ForEach(t => serviceTypes.Add(t));

            var allTypes = services.Union(nonServices).ToArray();

            InstallSpecifications(allTypes);

            PopulateAssociatedActions(s1.ToArray());

            //Menus installed once rest of metamodel has been built:
            InstallMainMenus();
            InstallObjectMenus();
        }

        public (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IImmutableDictionary<string, ITypeSpecBuilder> mm) => throw new NotImplementedException();

        public (T, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification<T>(Type type, IImmutableDictionary<string, ITypeSpecBuilder> mm) where T : class, ITypeSpecImmutable => throw new NotImplementedException();

        #endregion

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

        private void InstallSpecifications(Type[] types) => types.ForEach(type => LoadSpecification(type));

        private void PopulateAssociatedActions(Type[] services) {
            var nonServiceSpecs = AllObjectSpecImmutables.OfType<IObjectSpecBuilder>();
            nonServiceSpecs.ForEach(s => PopulateAssociatedActions(s, services));
        }

        private void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services) {
            if (string.IsNullOrWhiteSpace(spec.FullName)) {
                var id = (spec.Identifier != null ? spec.Identifier.ClassName : "unknown") ?? "unknown";
                Log.WarnFormat("Specification with id : {0} as has null or empty name", id);
            }

            if (TypeUtils.IsSystem(spec.FullName) && !spec.IsCollection) {
                return;
            }

            if (TypeUtils.IsNakedObjects(spec.FullName)) {
                return;
            }

            PopulateContributedActions(spec, services);
            PopulateFinderActions(spec, services);
        }

        private void InstallMainMenus() {
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

        private void InstallObjectMenus() {
            var menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }

        private void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services) {
            IList<IActionSpecImmutable> contributedActions = new List<IActionSpecImmutable>();
            IList<IActionSpecImmutable> collectionContribActions = new List<IActionSpecImmutable>();
            foreach (var serviceType in services) {
                if (serviceType != spec.Type) {
                    var serviceSpecification = (IServiceSpecImmutable) metamodel.GetSpecification(serviceType);
                    var serviceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).ToArray();
                    var matchingActionsForObject = serviceActions.Where(sa => sa.IsContributedTo(spec)).ToList();
                    foreach (var action in matchingActionsForObject) {
                        contributedActions.Add(action);
                    }

                    var matchingActionsForCollection = serviceActions.Where(sa => sa.IsContributedToCollectionOf(spec)).ToList();
                    foreach (var action in matchingActionsForCollection) {
                        collectionContribActions.Add(action);
                    }
                }
            }

            spec.AddContributedActions(contributedActions);
            spec.AddCollectionContributedActions(collectionContribActions);
        }

        private void PopulateFinderActions(IObjectSpecBuilder spec, Type[] services) {
            IList<IActionSpecImmutable> finderActions = new List<IActionSpecImmutable>();
            foreach (var serviceType in services) {
                var serviceSpecification = (IServiceSpecImmutable) metamodel.GetSpecification(serviceType);
                var matchingActions =
                    serviceSpecification.ObjectActions.Where(serviceAction => serviceAction.IsFinderMethodFor(spec)).ToList();

                if (matchingActions.Any()) {
                    var orderedActions = matchingActions.OrderBy(a => a, new MemberOrderComparator<IActionSpecImmutable>());
                    foreach (var action in orderedActions) {
                        finderActions.Add(action);
                    }
                }
            }

            spec.AddFinderActions(finderActions);
        }

        private ITypeSpecBuilder LoadSpecificationAndCache(Type type) {
            var actualType = ClassStrategy.GetType(type);

            if (actualType == null) {
                throw new ReflectionException(Log.LogAndReturn($"Attempting to introspect a non-introspectable type {type.FullName} "));
            }

            var specification = CreateSpecification(actualType);

            if (specification == null) {
                throw new ReflectionException(Log.LogAndReturn($"unrecognised type {actualType.FullName}"));
            }

            // We need the specification available in cache even though not yet fully introspected 
            metamodel.Add(actualType, specification);

            specification.Introspect(facetDecoratorSet, new Introspector(this));

            return specification;
        }

        private ITypeSpecBuilder CreateSpecification(Type type) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 

            return IsService(type) ? (ITypeSpecBuilder) ImmutableSpecFactory.CreateServiceSpecImmutable(type, metamodel) : ImmutableSpecFactory.CreateObjectSpecImmutable(type, metamodel);
        }

        private bool IsService(Type type) => serviceTypes.Contains(type);
    }

    // Copyright (c) Naked Objects Group Ltd.
}