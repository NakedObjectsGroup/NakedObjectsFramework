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
using NakedObjects.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.Component {
    // This is designed to run once, single threaded at startup. It is not intended to be thread safe.
    public sealed class ParallelReflector : IReflector {
        private static readonly ILog Log;
        private readonly IClassStrategy classStrategy;
        private readonly IReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IFacetFactorySet facetFactorySet;
        private readonly IMenuFactory menuFactory;
        private readonly ISet<Type> serviceTypes = new HashSet<Type>();
        private readonly IMetamodelBuilder initialMetamodel;

        static ParallelReflector() {
            Log = LogManager.GetLogger(typeof(ParallelReflector));
        }

        public ParallelReflector(IClassStrategy classStrategy,
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
            this.initialMetamodel = metamodel;
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
            get { return null; }
        }

        public ITypeSpecBuilder LoadSpecification(Type type) {
            throw new NotImplementedException();
        }

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable {
            throw new NotImplementedException();
        }

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            throw new NotImplementedException();
        }

        public ITypeSpecBuilder[] AllObjectSpecImmutables {
            get { return initialMetamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray(); }
        }

      
        public Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>> LoadSpecification(Type type, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Assert.AssertNotNull(type);

            var actualType = ClassStrategy.GetType(type);
            var typeKey = ClassStrategy.GetKeyForType(actualType);
            if (!metamodel.ContainsKey(typeKey)) {
                return LoadPlaceholder(actualType, metamodel);
            }

            return new Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>>(metamodel[typeKey], metamodel);
        }

        public void Reflect() {
            Type[] s1 = config.Services;
            Type[] services = s1.ToArray();
            Type[] nonServices = GetTypesToIntrospect();

            services.ForEach(t => serviceTypes.Add(t));

            var allTypes = services.Union(nonServices).ToArray();

            var mm = InstallSpecificationsParallel(allTypes, initialMetamodel, config);

            PopulateAssociatedActions(s1, mm);

            //Menus installed once rest of metamodel has been built:
            if (config.MainMenus != null) {
                IMenu[] mainMenus = config.MainMenus(menuFactory);
                InstallMainMenus(mainMenus, mm);
            }
            InstallObjectMenus(mm);
        }

        #endregion

        public Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>> IntrospectSpecification(Type actualType, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Assert.AssertNotNull(actualType);

            var typeKey = ClassStrategy.GetKeyForType(actualType);

            if (string.IsNullOrEmpty(metamodel[typeKey].FullName)) {
                return LoadSpecificationAndCache(actualType, metamodel);
            }

            return new Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>>(metamodel[typeKey], metamodel);
        }

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

        private ImmutableDictionary<string, ITypeSpecBuilder> IntrospectPlaceholders(ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var ph = metamodel.Where(i => string.IsNullOrEmpty(i.Value.FullName)).Select(i => i.Value.Type);
            var mm = ph.AsParallel().SelectMany(type => IntrospectSpecification(type, metamodel).Item2).Distinct(new DE()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary();

            if (mm.Any(i => string.IsNullOrEmpty(i.Value.FullName))) {
                return IntrospectPlaceholders(mm);
            }

            return mm;
        }

        private IMetamodelBuilder InstallSpecificationsParallel(Type[] types, IMetamodelBuilder metamodel, IReflectorConfiguration config) {
            var mm = GetPlaceholders(types);
            mm = IntrospectPlaceholders(mm);
            mm.ForEach(i => metamodel.Add(i.Value.Type, i.Value));
            return metamodel;
        }


      

        private void PopulateAssociatedActions(Type[] services, IMetamodelBuilder metamodel) {
            IEnumerable<IObjectSpecBuilder> nonServiceSpecs = AllObjectSpecImmutables.OfType<IObjectSpecBuilder>();
            nonServiceSpecs.ForEach(s => PopulateAssociatedActions(s, services, metamodel));
        }

        private void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services, IMetamodelBuilder metamodel) {
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

            PopulateContributedActions(spec, services, metamodel);
            //PopulateFinderActions(spec, services, metamodel);
        }

        private void InstallMainMenus(IMenu[] menus, IMetamodelBuilder metamodel) {
            foreach (IMenuImmutable menu in menus.OfType<IMenuImmutable>()) {
                metamodel.AddMainMenu(menu);
            }
        }

        private void InstallObjectMenus(IMetamodelBuilder metamodel) {
            IEnumerable<IMenuFacet> menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }


        private void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services, IMetamodel metamodel) {

            var result = services.
                AsParallel().
                Select(serviceType => {
                    var serviceSpecification = (IServiceSpecImmutable)metamodel.GetSpecification(serviceType);
                    IActionSpecImmutable[] serviceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).ToArray();

                    var matchingActionsForObject = serviceType != spec.Type ? serviceActions.Where(sa => sa.IsContributedTo(spec)).ToList() : new List<IActionSpecImmutable>();
                    var matchingActionsForCollection = serviceType != spec.Type ? serviceActions.Where(sa => sa.IsContributedToCollectionOf(spec)).ToList() : new List<IActionSpecImmutable>();
                    var finderActions = serviceActions.
                        Where(sa => sa.IsFinderMethodFor(spec)).
                        OrderBy(a => a, new MemberOrderComparator<IActionSpecImmutable>()).
                        ToList();

                    return new Tuple<List<IActionSpecImmutable>, List<IActionSpecImmutable>, List<IActionSpecImmutable>>(matchingActionsForObject, matchingActionsForCollection, finderActions);
                }).
                Aggregate(new Tuple<List<IActionSpecImmutable>, List<IActionSpecImmutable>, List<IActionSpecImmutable>>(new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>()),
                (a, t) => {
                    a.Item1.AddRange(t.Item1);
                    a.Item2.AddRange(t.Item2);
                    a.Item3.AddRange(t.Item3);
                    return a;
                });

            // 
            var contribActions = new List<IActionSpecImmutable>();

            // group by service - probably do this better - TODO
            foreach (var service in services) {
                var matching = result.Item1.Where(i => i.OwnerSpec.Type == service);
                contribActions.AddRange(matching);
            }

            spec.AddContributedActions(contribActions);
            spec.AddCollectionContributedActions(result.Item2);
            spec.AddFinderActions(result.Item3);
        }

        private ITypeSpecBuilder GetPlaceholder(Type type, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            ITypeSpecBuilder specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException(string.Format("unrecognised type {0}", type.FullName));
            }

            return specification;
        }

       

        private Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>> LoadPlaceholder(Type type, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            ITypeSpecBuilder specification = CreateSpecification(type, metamodel);

            if (specification == null) {
                throw new ReflectionException("unrecognised type " + type.FullName);
            }

            metamodel = metamodel.Add(ClassStrategy.GetKeyForType(type), specification);

            return new Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>>(specification, metamodel);
        }

        private ImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types) {
            return types.Select(t => ClassStrategy.GetType(t)).Where(t => t != null).Distinct(new DR(ClassStrategy)).ToDictionary(t => ClassStrategy.GetKeyForType(t), t => GetPlaceholder(t, null)).ToImmutableDictionary();
        }

        private Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>> LoadSpecificationAndCache(Type type, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            ITypeSpecBuilder specification = metamodel[ClassStrategy.GetKeyForType(type)];

            if (specification == null) {
                throw new ReflectionException("unrecognised type " + type.FullName);
            }

            metamodel = specification.Introspect(facetDecoratorSet, new Introspector(this), metamodel);

            return new Tuple<ITypeSpecBuilder, ImmutableDictionary<string, ITypeSpecBuilder>>(specification, metamodel);
        }

        private ITypeSpecBuilder CreateSpecification(Type type, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            TypeUtils.GetType(type.FullName); // This should ensure type is cached

            return IsService(type) ? (ITypeSpecBuilder)ImmutableSpecFactory.CreateServiceSpecImmutable(type, metamodel) : ImmutableSpecFactory.CreateObjectSpecImmutable(type, metamodel);
        }

        private bool IsService(Type type) {
            return serviceTypes.Contains(type);
        }



        #region Nested type: DE

        private class DE : IEqualityComparer<KeyValuePair<string, ITypeSpecBuilder>> {
            #region IEqualityComparer<KeyValuePair<string,ITypeSpecBuilder>> Members

            public bool Equals(KeyValuePair<string, ITypeSpecBuilder> x, KeyValuePair<string, ITypeSpecBuilder> y) {
                return x.Key.Equals(y.Key, StringComparison.Ordinal);
            }

            public int GetHashCode(KeyValuePair<string, ITypeSpecBuilder> obj) {
                return obj.Key.GetHashCode();
            }

            #endregion
        }

        #endregion

        #region Nested type: DR

        private class DR : IEqualityComparer<Type> {
            private readonly IClassStrategy classStrategy;

            public DR(IClassStrategy classStrategy) {
                this.classStrategy = classStrategy;
            }

            #region IEqualityComparer<Type> Members

            public bool Equals(Type x, Type y) {
                return classStrategy.GetKeyForType(x).Equals(classStrategy.GetKeyForType(y), StringComparison.Ordinal);
            }

            public int GetHashCode(Type obj) {
                return classStrategy.GetKeyForType(obj).GetHashCode();
            }

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}