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
        private readonly CompositeReflectorConfiguration reflectorConfiguration;

        private readonly FacetDecoratorSet facetDecoratorSet;
        private readonly IMetamodelBuilder initialMetamodel;
        private readonly ILogger<ParallelReflector> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IMenuFactory menuFactory;

        public ParallelReflector(IMetamodelBuilder metamodel,
                                 IObjectReflectorConfiguration objectReflectorConfiguration,
                                 IFunctionalReflectorConfiguration functionalReflectorConfiguration,
                                 IMenuFactory menuFactory,
                                 IEnumerable<IFacetDecorator> facetDecorators,
                                 IEnumerable<IFacetFactory> facetFactories,
                                 ILoggerFactory loggerFactory,
                                 ILogger<ParallelReflector> logger) {
            ObjectClassStrategy = new ObjectClassStrategy(objectReflectorConfiguration, functionalReflectorConfiguration);
            FunctionalClassStrategy = new FunctionClassStrategy(functionalReflectorConfiguration);
            initialMetamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            reflectorConfiguration = new CompositeReflectorConfiguration(objectReflectorConfiguration, functionalReflectorConfiguration);
            this.menuFactory = menuFactory ?? throw new InitialisationException($"{nameof(menuFactory)} is null");
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            facetDecoratorSet = new FacetDecoratorSet(facetDecorators.ToArray());
            ObjectFacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.ObjectOriented)).ToArray());
            FunctionalFacetFactorySet = new FacetFactorySet(facetFactories.Where(f => f.ReflectionTypes.HasFlag(ReflectionType.Functional)).ToArray());
        }

        public FacetFactorySet FunctionalFacetFactorySet { get; }

        #region IReflector Members

        public bool ConcurrencyChecking => reflectorConfiguration.ConcurrencyChecking;

        public bool IgnoreCase => reflectorConfiguration.IgnoreCase;

        public IClassStrategy ObjectClassStrategy { get; }

        public IClassStrategy FunctionalClassStrategy { get; }

        public IFacetFactorySet ObjectFacetFactorySet { get; }

        public IMetamodel Metamodel => null;

        public ITypeSpecBuilder LoadSpecification(Type type) => throw new NotImplementedException();

        public T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable => throw new NotImplementedException();

        public ITypeSpecBuilder[] AllObjectSpecImmutables => initialMetamodel.AllSpecifications.Cast<ITypeSpecBuilder>().ToArray();

        public (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(Type type, IClassStrategy classStrategy, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (type == null) {
                throw new NakedObjectSystemException("cannot load specification for null");
            }

            var actualType = classStrategy.GetType(type) ?? type;
            var typeKey = TypeKeyUtils.GetKeyForType(actualType);
            return !metamodel.ContainsKey(typeKey) ? LoadPlaceholder(actualType, metamodel) : (metamodel[typeKey], metamodel);
        }

        public (T, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification<T>(Type type, IClassStrategy classStrategy, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) where T : class, ITypeSpecImmutable {
            ITypeSpecBuilder spec;
            (spec, metamodel) = LoadSpecification(type, classStrategy, metamodel);
            return (spec as T, metamodel);
        }





        public void Reflect() {
            var metamodelBuilder = InstallSpecifications(reflectorConfiguration.ObjectTypes, reflectorConfiguration.RecordTypes, reflectorConfiguration.FunctionTypes, initialMetamodel);

            PopulateAssociatedActions(reflectorConfiguration.Services, metamodelBuilder);

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

            var typeKey = TypeKeyUtils.GetKeyForType(actualType);

            return string.IsNullOrEmpty(metamodel[typeKey].FullName)
                ? LoadSpecificationAndCache(actualType, metamodel, getIntrospector)
                : (metamodel[typeKey], metamodel);
        }


        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPlaceholders(IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            var ph = metamodel.Where(i => string.IsNullOrEmpty(i.Value.FullName)).Select(i => i.Value.Type);
            var mm = ph.AsParallel().SelectMany(type => IntrospectSpecification(type, metamodel, getIntrospector).metamodel).Distinct(new TypeSpecKeyComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary();

            return mm.Any(i => string.IsNullOrEmpty(i.Value.FullName))
                ? IntrospectPlaceholders(mm, getIntrospector)
                : mm;
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectObjectTypes(Type[] ooTypes) {
            var placeholders = GetPlaceholders(ooTypes, ObjectClassStrategy);
            return placeholders.Any()
                ? IntrospectPlaceholders(placeholders, () => new Introspector(this,  ObjectFacetFactorySet, ObjectClassStrategy, loggerFactory.CreateLogger<Introspector>()))
                : placeholders;
        }


        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectFunctionalTypes(Type[] records, Type[] functions, IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var allFunctionalTypes = records.Union(functions).ToArray();

            var placeholders = GetPlaceholders(allFunctionalTypes, FunctionalClassStrategy);
            return placeholders.Any()
                ? IntrospectPlaceholders(specDictionary.AddRange(placeholders), () => new FunctionalIntrospector(this, FunctionalFacetFactorySet, FunctionalClassStrategy, functions))
                : specDictionary;
        }

        private IMetamodelBuilder InstallSpecifications(Type[] ooTypes, Type[] records, Type[] functions, IMetamodelBuilder metamodel) {
            var mm = IntrospectObjectTypes(ooTypes);
            mm = IntrospectFunctionalTypes(records, functions, mm);

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
            var menus = reflectorConfiguration.MainMenus()?.Invoke(menuFactory);
            // Unlike other things specified in objectReflectorConfiguration, this one can't be checked when ObjectReflectorConfiguration is constructed.
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

            metamodel = metamodel.Add(TypeKeyUtils.GetKeyForType(type), specification);

            return (specification, metamodel);
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> GetPlaceholders(Type[] types, IClassStrategy classStrategy) => types.Select(t => classStrategy.GetType(t)).Where(t => t != null).Distinct(new TypeKeyComparer(classStrategy)).ToDictionary(t => TypeKeyUtils.GetKeyForType(t), t => GetPlaceholder(t, null)).ToImmutableDictionary();

        private (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecificationAndCache(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel, Func<IIntrospector> getIntrospector) {
            var specification = metamodel[TypeKeyUtils.GetKeyForType(type)];

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

        private bool IsService(Type type) =>  reflectorConfiguration.ServiceTypeSet.Contains(type);

        #region Nested type: TypeKeyComparer

        private class TypeKeyComparer : IEqualityComparer<Type> {
            private readonly IClassStrategy classStrategy;

            public TypeKeyComparer(IClassStrategy classStrategy) => this.classStrategy = classStrategy;

            #region IEqualityComparer<Type> Members

            public bool Equals(Type x, Type y) => TypeKeyUtils.GetKeyForType(x).Equals(TypeKeyUtils.GetKeyForType(y), StringComparison.Ordinal);

            public int GetHashCode(Type obj) => TypeKeyUtils.GetKeyForType(obj).GetHashCode();

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