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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Metamodel.SpecImmutable;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Reflect;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;
using NakedObjects.Architecture.Exceptions;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class DotNetReflector : IReflector, IMetamodel {
        private static readonly ILog Log;
        private readonly ISpecificationCache cache = new SimpleSpecificationCache();
        private readonly FacetDecoratorSet facetDecorator;
        private readonly IntrospectionControlParameters introspectionControlParameters;

        private bool installingServices;
        private bool linked;

        static DotNetReflector() {
            Log = LogManager.GetLogger(typeof (DotNetReflector));
        }

        public DotNetReflector(IClassStrategy classStrategy, IFacetFactorySet facetFactorySet, FacetDecoratorSet facetDecoratorSet) {
            Assert.AssertNotNull(classStrategy);
            Assert.AssertNotNull(facetFactorySet);
            Assert.AssertNotNull(facetDecoratorSet);
            facetDecorator = facetDecoratorSet;

            facetFactorySet.Init(this);
            introspectionControlParameters = new IntrospectionControlParameters(facetFactorySet, classStrategy);
            IgnoreCase = false;
        }

        private Type[] NonSystemServices { get; set; }

        #region IMetamodel Members

        public virtual IObjectSpecImmutable[] AllSpecifications {
            get { return cache.AllSpecifications(); }
        }

        public virtual IObjectSpecImmutable[] AllObjectSpecImmutables {
            get { return cache.AllSpecifications().ToArray(); }
        }

        public IObjectSpecImmutable GetSpecification(Type type) {
            return  LoadSpecification(type);
        }

        public IObjectSpecImmutable GetSpecification(string name) {
            return LoadSpecification(name);
        }


        #endregion

        #region INakedObjectReflector Members

        public IClassStrategy ClassStrategy {
            get { return introspectionControlParameters.ClassStrategy; }
        }

        public IFacetFactorySet FacetFactorySet {
            get { return introspectionControlParameters.FacetFactorySet; }
        }

        public virtual IObjectSpecImmutable LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            return LoadSpecificationAndCache(ClassStrategy.GetType(type));
        }

        public IObjectSpecImmutable LoadSpecification(string className) {
            Assert.AssertNotNull("specification class must be specified", className);

            try {
                Type type = TypeFactory.GetTypeFromLoadedAssembly(className);
                return LoadSpecification(type);
            }
            catch (Exception e) {
                Log.InfoFormat("Failed to Load Specification for: {0} error: {1} trying cache", className, e);
                var spec = cache.GetSpecification(className);
                if (spec != null) {
                    Log.InfoFormat("Found {0} in cache", className);
                    return spec;
                }
                throw;
            }
        }

        public virtual void InstallServiceSpecifications(Type[] type) {
            installingServices = true;
            type.ForEach(InstallServiceSpecification);
            NonSystemServices = type;
        }

        public virtual void PopulateContributedActions(Type[] services) {
            try {
                if (!linked) {
                    AllObjectSpecImmutables.ForEach(s => PopulateAssociatedActions(s, services));
                }
            }
            finally {
                installingServices = false;
                linked = true;
            }
        }

        public void PopulateAssociatedActions(IObjectSpecImmutable spec, Type[] services) {
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


        private void PopulateContributedActions(IObjectSpecImmutable spec,  Type[] services) {
            if (!spec.Service) {
                foreach (Type serviceType in services) {
                    if (serviceType != spec.Type) {
                        var serviceSpecification = GetSpecification(serviceType);

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
                var serviceSpecification = GetSpecification(serviceType);
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


        public static bool IgnoreCase { get; set; }

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null && property.PropertyType != classToIgnore) {
                    LoadSpecification(property.PropertyType);
                }
            }
        }

        #endregion

        private void InstallServiceSpecification(Type type) {
            var spec = cache.GetSpecification(type.GetProxiedTypeFullName());
            if (spec != null) {
                return;
            }

            var specification = Install(type);
            cache.Cache(type.GetProxiedTypeFullName(), specification);
            specification.Introspect(facetDecorator, new DotNetIntrospector(this));
            specification.MarkAsService();
        }

        private IObjectSpecImmutable LoadSpecificationAndCache(Type type) {
            string proxiedTypeName = type.GetProxiedTypeFullName();
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 
            lock (cache) {
                // this is a double check on the cache to check it was not written to during the unguarded 
                // internal between the last check and the lock
                var specification = cache.GetSpecification(proxiedTypeName);
                if (specification != null) {
                    return specification;
                }

                specification = CreateSpecification(type);

                if (specification == null) {
                    throw new ReflectionException("unrecognised class " + proxiedTypeName);
                }

                // we need the specification available in cache even though not yet full introspected 
                // need to be careful no other thread reads until introspected
                cache.Cache(proxiedTypeName, specification);

                specification.Introspect(facetDecorator, new DotNetIntrospector(this));

                if (!installingServices) {
                    var services = NonSystemServices ?? new Type[] {};
                    PopulateAssociatedActions(specification, services);
                }


                return specification;
            }
        }

        private IObjectSpecImmutable CreateSpecification(Type type) {
            return new ObjectSpecImmutable(type, this);
        }

        private IObjectSpecImmutable Install(Type type) {
            return new ObjectSpecImmutable(type, this);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}