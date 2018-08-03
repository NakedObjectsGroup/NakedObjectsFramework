// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
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
using NakedObjects.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;
using System.Threading.Tasks;

namespace NakedObjects.Reflect.Component {
    // This is designed to run once, single threaded at startup. It is not intended to be thread safe.
    public sealed class Reflector : IReflector {
        private static readonly ILog Log;
        private readonly IClassStrategy classStrategy;
        private readonly IReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IFacetFactorySet facetFactorySet;
        private readonly IMenuFactory menuFactory;
        private readonly IMetamodelBuilder metamodel;
        private readonly ISet<Type> serviceTypes = new HashSet<Type>();

        static Reflector() {
            Log = LogManager.GetLogger(typeof(Reflector));
        }

        public Reflector(IClassStrategy classStrategy,
                         IMetamodelBuilder metamodel,
                         IReflectorConfiguration config,
                         IMenuFactory menuFactory,
                         IFacetDecorator[] facetDecorators,
                         IFacetFactory[] facetFactories) {
            Assert.AssertNotNull(classStrategy);
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(config);
            Assert.AssertNotNull(menuFactory);

            this.classStrategy = classStrategy;
            this.metamodel = metamodel;
            this.config = config;
            this.menuFactory = menuFactory;
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators);
            facetFactorySet = new FacetFactorySet(facetFactories);
        }

        // exposed for testing
        public IFacetDecoratorSet FacetDecoratorSet {
            get { return facetDecoratorSet; }
        }

        #region IReflector Members

        public bool IgnoreCase {
            get { return config.IgnoreCase; }
        }

        public IClassStrategy ClassStrategy {
            get { return classStrategy; }
        }

        public IFacetFactorySet FacetFactorySet {
            get { return facetFactorySet; }
        }

        public IMetamodel Metamodel {
            get { return metamodel; }
        }

        public ITypeSpecBuilder[] AllObjectSpecImmutables {
            get { return metamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray(); }
        }

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null && property.PropertyType != classToIgnore) {
                    LoadSpecification(property.PropertyType);
                }
            }
        }

        public ITypeSpecBuilder LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            return (ITypeSpecBuilder)metamodel.GetSpecification(type, true) ?? LoadSpecificationAndCache(type);
        }

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable {
            var spec = LoadSpecification(type);
            try {
                return (T)spec;
            } catch (Exception) {
                throw new ReflectionException(string.Format(
                    "Specification for type {0} is {1}: cannot be cast to {2}",
                    type.Name, spec.GetType().Name, typeof(T).Name
                    ));
            }
        }

        public void Reflect() {
            Type[] s1 = config.Services;
            Type[] services = s1.ToArray();
            Type[] nonServices = GetTypesToIntrospect();

            services.ForEach(t => serviceTypes.Add(t));

            var allTypes = services.Union(nonServices).ToArray();

            InstallSpecifications(allTypes);

            PopulateAssociatedActions(s1.ToArray());

            //Menus installed once rest of metamodel has been built:
            if (config.MainMenus != null) {
                IMenu[] mainMenus = config.MainMenus(menuFactory);
                InstallMainMenus(mainMenus);
            }
            InstallObjectMenus();
        }

        #endregion

        private Type EnsureGenericTypeIsComplete(Type type) {
            if (type.IsGenericType && !type.IsConstructedGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericParms = genericType.GetGenericArguments().Select(a => typeof (Object)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
            }
            return type;
        }

        private Type[] GetTypesToIntrospect() {
            var types = config.TypesToIntrospect.Select(EnsureGenericTypeIsComplete);
            var systemTypes = config.SupportedSystemTypes.Select(EnsureGenericTypeIsComplete);
            return types.Union(systemTypes).ToArray();
        }

        private void InstallSpecifications(Type[] types) {
            if (this.config.ParallelReflectionMode)
            {
                Parallel.ForEach(types, type => LoadSpecification(type));
            }
            else
            {
                types.ForEach(type => LoadSpecification(type));
            }            
        }

        private void PopulateAssociatedActions(Type[] services) {
            IEnumerable<IObjectSpecBuilder> nonServiceSpecs = AllObjectSpecImmutables.OfType<IObjectSpecBuilder>();

            if (this.config.ParallelReflectionMode)
            {
                Parallel.ForEach(nonServiceSpecs, s => PopulateAssociatedActions(s, services));
            }
            else
            {
                nonServiceSpecs.ForEach(s => PopulateAssociatedActions(s, services));
            }
        }

        private void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services) {
            if (string.IsNullOrWhiteSpace(spec.FullName)) {
                string id = (spec.Identifier != null ? spec.Identifier.ClassName : "unknown") ?? "unknown";
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

        private void InstallMainMenus(IMenu[] menus) {
            foreach (IMenuImmutable menu in menus.OfType<IMenuImmutable>()) {
                metamodel.AddMainMenu(menu);
            }
        }

        private void InstallObjectMenus() {
            IEnumerable<IMenuFacet> menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }

        private void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services) {
            IList<IActionSpecImmutable> contributedActions = new List<IActionSpecImmutable>();
            IList<IActionSpecImmutable> collectionContribActions = new List<IActionSpecImmutable>();
            foreach (Type serviceType in services) {
                if (serviceType != spec.Type) {
                    var serviceSpecification = (IServiceSpecImmutable)metamodel.GetSpecification(serviceType);
                    IActionSpecImmutable[] serviceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).ToArray();
                    List<IActionSpecImmutable> matchingActionsForObject = serviceActions.Where(sa => sa.IsContributedTo(spec)).ToList();
                    foreach (IActionSpecImmutable action in matchingActionsForObject) {
                        contributedActions.Add(action);
                    }

                    List<IActionSpecImmutable> matchingActionsForCollection = serviceActions.Where(sa => sa.IsContributedToCollectionOf(spec)).ToList();
                    foreach (IActionSpecImmutable action in matchingActionsForCollection) {
                        collectionContribActions.Add(action);
                    }
                }
            }
            spec.AddContributedActions(contributedActions);
            spec.AddCollectionContributedActions(collectionContribActions);
        }

        private void PopulateFinderActions(IObjectSpecBuilder spec, Type[] services) {
            IList<IActionSpecImmutable> finderActions = new List<IActionSpecImmutable>();
            foreach (Type serviceType in services) {
                var serviceSpecification = (IServiceSpecImmutable)metamodel.GetSpecification(serviceType);
                List<IActionSpecImmutable> matchingActions =
                    serviceSpecification.ObjectActions.
                        Where(serviceAction => serviceAction.IsFinderMethodFor(spec)).ToList();

                if (matchingActions.Any()) {
                    matchingActions.Sort(new MemberOrderComparator<IActionSpecImmutable>());
                    foreach (IActionSpecImmutable action in matchingActions) {
                        finderActions.Add(action);
                    }
                }
            }
            spec.AddFinderActions(finderActions);
        }

        private ITypeSpecBuilder LoadSpecificationAndCache(Type type) {
            Type actualType = classStrategy.GetType(type);

            if (actualType == null) {
                throw new ReflectionException("Attempting to introspect a non-introspectable type " + type.FullName + " ");
            }

            ITypeSpecBuilder specification = CreateSpecification(actualType);

            if (specification == null) {
                throw new ReflectionException("unrecognised type " + actualType.FullName);
            }

            // We need the specification available in cache even though not yet fully introspected 
            metamodel.Add(actualType, specification);

            specification.Introspect(facetDecoratorSet, new Introspector(this, this.config.ParallelReflectionMode));

            return specification;
        }

        private ITypeSpecBuilder CreateSpecification(Type type) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 

            return IsService(type) ? (ITypeSpecBuilder)ImmutableSpecFactory.CreateServiceSpecImmutable(type, metamodel) : ImmutableSpecFactory.CreateObjectSpecImmutable(type, metamodel);
        }

        private bool IsService(Type type) {
            return serviceTypes.Contains(type);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
