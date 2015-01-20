// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    // This is designed to run once, single threaded at startup. It is not intended to be thread safe.
    public class Reflector : IReflector {
        private static readonly ILog Log;

        private readonly IClassStrategy classStrategy;
        private readonly IReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IFacetFactorySet facetFactorySet;
        private readonly IMenuFactory menuFactory;
        private readonly IMetamodelBuilder metamodel;

        static Reflector() {
            Log = LogManager.GetLogger(typeof (Reflector));
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

        public virtual IObjectSpecBuilder[] AllObjectSpecImmutables {
            get { return metamodel.AllSpecifications.Cast<IObjectSpecBuilder>().ToArray(); }
        }

        public virtual IObjectSpecBuilder LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            return (IObjectSpecBuilder) metamodel.GetSpecification(type) ?? LoadSpecificationAndCache(type);
        }

        public void Reflect() {
            Type[] s1 = config.MenuServices;
            Type[] s2 = config.ContributedActions;
            Type[] s3 = config.SystemServices;
            Type[] services = s1.Union(s2).Union(s3).ToArray();
            Type[] nonServices = config.TypesToIntrospect;

            InstallSpecifications(services, true);
            InstallSpecifications(nonServices, false);
            PopulateAssociatedActions(s1.Union(s2).ToArray());

            //Menus installed once rest of metamodel has been built:
            if (config.MainMenus != null) {
                IMenu[] mainMenus = config.MainMenus(menuFactory);
                InstallMainMenus(mainMenus);
            }
            InstallObjectMenus();
        }

        #endregion

        private void InstallSpecifications(Type[] types, bool isService) {
            types.ForEach(type => InstallSpecification(type, isService));
        }

        private void PopulateAssociatedActions(Type[] services) {
            IEnumerable<IObjectSpecBuilder> nonServiceSpecs = AllObjectSpecImmutables.Where(x => !x.Service);
            nonServiceSpecs.ForEach(s => PopulateAssociatedActions(s, services));
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
            if (!spec.Service) {
                IList<IActionSpecImmutable> contributedActions = new List<IActionSpecImmutable>();
                IList<IActionSpecImmutable> collectionContribActions = new List<IActionSpecImmutable>();
                foreach (Type serviceType in services) {
                    if (serviceType != spec.Type) {
                        IObjectSpecImmutable serviceSpecification = metamodel.GetSpecification(serviceType);
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
        }

        private void PopulateFinderActions(IObjectSpecBuilder spec, Type[] services) {
            IList<IActionSpecImmutable> finderActions = new List<IActionSpecImmutable>();
            foreach (Type serviceType in services) {
                IObjectSpecImmutable serviceSpecification = metamodel.GetSpecification(serviceType);
                List<IActionSpecImmutable> matchingActions = serviceSpecification.ObjectActions.Where(a => a.IsFinderMethod).Where(serviceAction => serviceAction.IsFinderMethodFor(spec)).ToList();

                if (matchingActions.Any()) {
                    matchingActions.Sort(new MemberOrderComparator<IActionSpecImmutable>());
                    foreach (IActionSpecImmutable action in matchingActions) {
                        finderActions.Add(action);
                    }
                }
            }
            spec.AddFinderActions(finderActions);
        }

        private void InstallSpecification(Type type, bool isService) {
            IObjectSpecBuilder spec = LoadSpecification(type);

            // Do this here so that if the service spec was found and loaded earlier for any reason it is still marked 
            // as a service
            if (isService) {
                spec.MarkAsService();
            }
        }

        private IObjectSpecBuilder LoadSpecificationAndCache(Type type) {
            Type actualType = classStrategy.GetType(type);

            IObjectSpecBuilder specification = CreateSpecification(actualType);

            if (specification == null) {
                throw new ReflectionException("unrecognised type " + actualType.FullName);
            }

            // We need the specification available in cache even though not yet fully introspected 
            metamodel.Add(actualType, specification);

            specification.Introspect(facetDecoratorSet, new Introspector(this, metamodel));

            return specification;
        }

        private IObjectSpecBuilder CreateSpecification(Type type) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 
            return new ObjectSpecImmutable(type, metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}