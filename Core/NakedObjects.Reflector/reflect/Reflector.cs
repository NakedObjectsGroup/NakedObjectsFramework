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
using NakedObjects.Architecture.Exceptions;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Metamodel.SpecImmutable;
using NakedObjects.Reflector.Reflect;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class Reflector : IReflector {
        private static readonly ILog Log;

        private readonly IClassStrategy classStrategy;
        private readonly IReflectorConfiguration config;
        private readonly FacetDecoratorSet facetDecorator;
        private readonly IntrospectionControlParameters introspectionControlParameters;
        private readonly IMetamodelMutable metamodel;
        private readonly IServicesConfiguration servicesConfig;

        static Reflector() {
            Log = LogManager.GetLogger(typeof (Reflector));
        }

        public Reflector(IClassStrategy classStrategy, IFacetFactorySet facetFactorySet, FacetDecoratorSet facetDecoratorSet, IMetamodelMutable metamodel, IReflectorConfiguration config, IServicesConfiguration servicesConfig) {
            Assert.AssertNotNull(classStrategy);
            Assert.AssertNotNull(facetFactorySet);
            Assert.AssertNotNull(facetDecoratorSet);
            this.classStrategy = classStrategy;
            facetDecorator = facetDecoratorSet;
            this.metamodel = metamodel;
            this.config = config;
            this.servicesConfig = servicesConfig;

            facetFactorySet.Init(this);
            introspectionControlParameters = new IntrospectionControlParameters(facetFactorySet, classStrategy);
            IgnoreCase = false;

            Reflect();
        }

        public static bool IgnoreCase { get; set; }

        #region IReflector Members

        public IClassStrategy ClassStrategy {
            get { return introspectionControlParameters.ClassStrategy; }
        }

        public IFacetFactorySet FacetFactorySet {
            get { return introspectionControlParameters.FacetFactorySet; }
        }


        public virtual IObjectSpecImmutable[] AllObjectSpecImmutables {
            get { return metamodel.AllSpecifications.ToArray(); }
        }

        public IObjectSpecImmutable LoadSpecification(string className) {
            Assert.AssertNotNull("specification class must be specified", className);

            try {
                Type type = TypeFactory.GetTypeFromLoadedAssembly(className);
                return LoadSpecification(type);
            }
            catch (Exception e) {
                // TODO don't think this is a good idea - fail fast ?
                Log.InfoFormat("Failed to Load Specification for: {0} error: {1} trying cache", className, e);
                var spec = metamodel.GetSpecification(className);
                if (spec != null) {
                    Log.InfoFormat("Found {0} in cache", className);
                    return spec;
                }
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

        public virtual IObjectSpecImmutable LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            var actualType = classStrategy.GetType(type);

            var spec = metamodel.GetSpecification(actualType);

            if (spec != null) {
                return spec;
            }

            spec = LoadSpecificationAndCache(actualType);

            // load collections of this type 
            // TODO this is initial naive implementation - optimise later 
            // needs to be more refined what about nullables or tuples ?
            if (!(actualType == typeof (void))) {
                LoadArraySpecification(actualType);
                LoadCollectionSpecifications(actualType);
            }

            return spec;
        }

        #endregion

        private bool IsAlreadyNestedArrayOrGeneric(Type type) {
            if (type.IsGenericType) {
                return type.GetGenericArguments().Any(ga => ga.IsArray || ga.IsGenericType);
            }
            if (type.IsArray) {
                var elementType = type.GetElementType();
                return elementType.IsArray || elementType.IsGenericType;
            }

            return false;
        }


        private void LoadArraySpecification(Type actualType) {
            if (!IsAlreadyNestedArrayOrGeneric(actualType)) {
                try {
                    var aType = actualType.MakeArrayType();
                    LoadSpecification(aType);
                }
                catch (Exception e) {
                    Log.FatalFormat("Failed to create array type from: {0} reason: {1}", actualType.FullName, e.Message);
                    throw;
                }
            }
        }

        private void LoadCollectionSpecifications(Type actualType) {
            if (!IsAlreadyNestedArrayOrGeneric(actualType)) {
                foreach (var gt in config.CollectionsToIntrospect) {
                    try {
                        var cType = gt.GetGenericTypeDefinition().MakeGenericType(actualType);
                        LoadSpecification(cType);
                    }
                    catch (ArgumentException e) {
                        // Odds are contraint on type is wrong so just warn but continue
                        Log.WarnFormat("Failed to create generic type from: {0} on : {1} reason: {2}", gt.FullName, actualType.FullName, e.Message);
                    }
                    catch (Exception e) {
                        Log.FatalFormat("Failed to create generic type from: {0} on : {1} reason: {2}", gt.FullName, actualType.FullName, e.Message);
                        throw;
                    }
                }
            }
        }

        private void Reflect() {
            var s1 = config.MenuServices;
            var s2 = config.ContributedActions;
            var s3 = config.SystemServices;
            Type[] services = s1.Union(s2).Union(s3).ToArray();
            Type[] nonServices = config.TypesToIntrospect;

            InstallSpecifications(services, true);
            InstallSpecifications(nonServices, false);
            PopulateContributedActions(s1.Union(s2).ToArray());

            servicesConfig.AddMenuServices(s1.Select(Activator.CreateInstance).ToArray());
            servicesConfig.AddContributedActions(s2.Select(Activator.CreateInstance).ToArray());
            servicesConfig.AddMenuServices(s3.Select(Activator.CreateInstance).ToArray());
        }

        private void InstallSpecifications(Type[] types, bool isService) {
            types.ForEach(type => InstallSpecification(type, isService));
        }

        private void PopulateContributedActions(Type[] services) {
            AllObjectSpecImmutables.ForEach(s => PopulateAssociatedActions(s, services));
        }

        private void PopulateAssociatedActions(IObjectSpecImmutable spec, Type[] services) {
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

        private void PopulateContributedActions(IObjectSpecImmutable spec, Type[] services) {
            if (!spec.Service) {
                foreach (Type serviceType in services) {
                    if (serviceType != spec.Type) {
                        var serviceSpecification = metamodel.GetSpecification(serviceType);

                        IActionSpecImmutable[] matchingServiceActions = serviceSpecification.ObjectActions.Flattened.Where(serviceAction => serviceAction.IsContributedTo(spec)).ToArray();

                        if (matchingServiceActions.Any()) {
                            IOrderSet<IActionSpecImmutable> os = SimpleOrderSet<IActionSpecImmutable>.CreateOrderSet("", matchingServiceActions);
                            var name = serviceSpecification.GetFacet<INamedFacet>().Value ?? serviceSpecification.ShortName;
                            var id = serviceSpecification.Identifier.ClassName.Replace(" ", "");
                            var t = new Tuple<string, string, IOrderSet<IActionSpecImmutable>>(id, name, os);

                            spec.ContributedActions.Add(t);
                        }
                    }
                }
            }
        }

        private void PopulateRelatedActions(IObjectSpecImmutable spec, Type[] services) {
            foreach (Type serviceType in services) {
                var serviceSpecification = metamodel.GetSpecification(serviceType);
                var matchingActions = new List<IActionSpecImmutable>();

                foreach (var serviceAction in serviceSpecification.ObjectActions.Flattened.Where(a => a.IsFinderMethod)) {
                    var returnType = serviceAction.ReturnType;
                    if (returnType != null && returnType.IsCollection) {
                        var elementType = returnType.GetFacet<ITypeOfFacet>().ValueSpec;
                        if (elementType.IsOfType(spec)) {
                            matchingActions.Add(serviceAction);
                        }
                    }
                    else if (returnType != null && returnType.IsOfType(spec)) {
                        matchingActions.Add(serviceAction);
                    }
                }

                if (matchingActions.Any()) {
                    IOrderSet<IActionSpecImmutable> os = SimpleOrderSet<IActionSpecImmutable>.CreateOrderSet("", matchingActions.ToArray());
                    var name = serviceSpecification.GetFacet<INamedFacet>().Value ?? serviceSpecification.ShortName;
                    var id = serviceSpecification.Identifier.ClassName.Replace(" ", "");
                    var t = new Tuple<string, string, IOrderSet<IActionSpecImmutable>>(id, name, os);

                    spec.RelatedActions.Add(t);
                }
            }
        }

        private void InstallSpecification(Type type, bool isService) {
            var spec = LoadSpecification(type);

            if (isService) {
                spec.MarkAsService();
            }
        }

        private IObjectSpecImmutable LoadSpecificationAndCache(Type type) {
            string proxiedTypeName = type.GetProxiedTypeFullName();
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 

            var specification = CreateSpecification(type);

            if (specification == null) {
                throw new ReflectionException("unrecognised class " + proxiedTypeName);
            }

            // we need the specification available in cache even though not yet full introspected 
            // need to be careful no other thread reads until introspected
            metamodel.Add(proxiedTypeName, specification);

            specification.Introspect(facetDecorator, new Introspector(this, metamodel));

            return specification;
        }

        private IObjectSpecImmutable CreateSpecification(Type type) {
            return new ObjectSpecImmutable(type, metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}