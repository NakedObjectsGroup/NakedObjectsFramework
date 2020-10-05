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
using NakedObjects.Util;

namespace NakedObjects.Reflect.Component {
    // This is designed to run once, single threaded at startup. It is not intended to be thread safe.
    public sealed class Reflector : IReflector {
        private readonly IObjectReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly ILogger<Reflector> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IMenuFactory menuFactory;
        private readonly IMetamodelBuilder metamodel;
        private readonly ISet<Type> serviceTypes = new HashSet<Type>();

        public Reflector(IClassStrategy classStrategy,
                         IMetamodelBuilder metamodel,
                         IObjectReflectorConfiguration config,
                         IMenuFactory menuFactory,
                         IEnumerable<IFacetDecorator> facetDecorators,
                         IEnumerable<IFacetFactory> facetFactories,
                         ILoggerFactory loggerFactory,
                         ILogger<Reflector> logger) {
            ClassStrategy = classStrategy ?? throw new InitialisationException($"{nameof(classStrategy)} is null");
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            this.config = config ?? throw new InitialisationException($"{nameof(config)} is null");
            this.menuFactory = menuFactory ?? throw new InitialisationException($"{nameof(menuFactory)} is null");
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
            ObjectFacetFactorySet = new FacetFactorySet(facetFactories.ToArray());
        }

        #region IReflector Members

        public bool ConcurrencyChecking => config.ConcurrencyChecking;

        public bool IgnoreCase => config.IgnoreCase;

        public IClassStrategy ClassStrategy { get; }

        public IFacetFactorySet ObjectFacetFactorySet { get; }

        public IMetamodel Metamodel => metamodel;

        public ITypeSpecBuilder[] AllObjectSpecImmutables => metamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();

        public ITypeSpecBuilder LoadSpecification(Type type) {
            if (type == null) {
                throw new NakedObjectSystemException("cannot load specification for null");
            }

            return (ITypeSpecBuilder) metamodel.GetSpecification(type, true) ?? LoadSpecificationAndCache(type);
        }

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable {
            var spec = LoadSpecification(type);
            try {
                return (T) spec;
            }
            catch (Exception) {
                throw new ReflectionException(logger.LogAndReturn($"Specification for type {type.Name} is {spec.GetType().Name}: cannot be cast to {typeof(T).Name}"));
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
                logger.LogWarning($"Specification with id : {id} as has null or empty name");
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
            var menuList = config.MainMenus;

            if (menuList is not null && menuList.Any()) {

                var menus = menuList.Select(tuple => {
                    var (type, name, addAll, action) = tuple;
                    var menu = menuFactory.NewMenu(type, addAll, name);
                    action?.Invoke(menu);
                    return menu;
                }).ToArray();

                //var menus = config.MainMenus?.Invoke(menuFactory);
                // Unlike other things specified in config, this one can't be checked when ObjectReflectorConfiguration is constructed.
                // Allows developer to deliberately not specify any menus
                //if (menus != null) {
                    if (!menus.Any()) {
                        //Catches accidental non-specification of menus
                        throw new ReflectionException(logger.LogAndReturn("No MainMenus specified."));
                    }

                    foreach (var menu in menus.OfType<IMenuImmutable>()) {
                        metamodel.AddMainMenu(menu);
                    }
                //}
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
                throw new ReflectionException(logger.LogAndReturn($"Attempting to introspect a non-introspectable type {type.FullName} "));
            }

            var specification = CreateSpecification(actualType);

            if (specification == null) {
                throw new ReflectionException(logger.LogAndReturn($"unrecognised type {actualType.FullName}"));
            }

            // We need the specification available in cache even though not yet fully introspected 
            metamodel.Add(actualType, specification);

            specification.Introspect(facetDecoratorSet, new Introspector(this, loggerFactory.CreateLogger<Introspector>()));

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