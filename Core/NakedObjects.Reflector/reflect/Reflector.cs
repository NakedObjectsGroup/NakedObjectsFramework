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
using NakedObjects.Architecture;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;
using NakedObjects.Menu;

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

        public IObjectSpecBuilder LoadSpecification(string className) {
            Assert.AssertNotNull("specification class must be specified", className);

            try {
                Type type = TypeFactory.GetTypeFromLoadedAssembly(className);
                return LoadSpecification(type);
            }
            catch (Exception e) {
                Log.FatalFormat("Failed to Load Specification for: {0} error: {1} trying cache", className, e);
                throw;
            }
        }

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null && property.PropertyType != classToIgnore) {
                    LoadSpecification(property.PropertyType);
                }
            }
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
            PopulateRelatedActions(spec, services);
        }

        private void InstallMainMenus(IMenu[] menus) {
          foreach (IMenuImmutable menu in menus) {
                metamodel.AddMainMenu(menu);
            }
        }

        private void InstallObjectMenus() {
            IEnumerable<IMenuFacet> menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }

        private void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services) {
            if (!spec.Service) {
                IList<ActionsForService> contributedActions = new List<ActionsForService>();
                foreach (Type serviceType in services) {
                    if (serviceType != spec.Type) {
                        IObjectSpecImmutable serviceSpecification = metamodel.GetSpecification(serviceType);

                        var matchingServiceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).Where(sa => sa.IsContributedTo(spec)).ToList();

                        foreach (var action in matchingServiceActions) {
                            string subMenu = action.GetFacet<IContributedActionFacet>().SubMenuWhenContributedTo(spec);
                            string id = serviceSpecification.Identifier.ClassName.Replace(" ", "");
                            //TODO: Get rid of ActionsForService & use simple dictionary of subName & action spec
                            var t = new ActionsForService() { Id = id, Name = subMenu, Specs = new List<IActionSpecImmutable> { action } };
                            contributedActions.Add(t);
                        }
                    }
                }
                spec.AddContributedActions(contributedActions);
            }
        }

        private void PopulateRelatedActions(IObjectSpecBuilder spec, Type[] services) {
            IList<ActionsForService> relatedActions = new List<ActionsForService>();
            foreach (Type serviceType in services) {
                IObjectSpecImmutable serviceSpecification = metamodel.GetSpecification(serviceType);
                var matchingActions = new List<IActionSpecImmutable>();

                foreach (IActionSpecImmutable serviceAction in serviceSpecification.ObjectActions.Where(s => s != null).Where(a => a.IsFinderMethod)) {
                    IObjectSpecImmutable returnType = serviceAction.ReturnType;
                    if (returnType != null && returnType.IsCollection) {
                        IObjectSpecImmutable elementType = serviceAction.ElementType;
                        if (elementType.IsOfType(spec)) {
                            matchingActions.Add(serviceAction);
                        }
                    }
                    else if (returnType != null && returnType.IsOfType(spec)) {
                        matchingActions.Add(serviceAction);
                    }
                }

                if (matchingActions.Any()) {
                    matchingActions.Sort(new MemberOrderComparator<IActionSpecImmutable>());
                    string name = serviceSpecification.GetFacet<INamedFacet>().Value ?? serviceSpecification.ShortName;
                    string id = serviceSpecification.Identifier.ClassName.Replace(" ", "");
                    //var t = new Tuple<string, string, IList<IActionSpecImmutable>>(id, name, matchingActions);

                    var t = new ActionsForService() {Id = id, Name = name, Specs = matchingActions};
                    relatedActions.Add(t);
                }
            }
            spec.AddRelatedActions(relatedActions);
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
            var actualType = classStrategy.GetType(type);

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