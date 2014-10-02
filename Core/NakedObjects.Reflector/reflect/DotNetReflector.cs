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
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class DotNetReflector : INakedObjectReflector, IMetadata {
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

        #region IMetadata Members

        public virtual INakedObjectSpecification[] AllSpecifications {
            get { return cache.AllSpecifications().Select(s => new NakedObjectSpecification(this, s)).Cast<INakedObjectSpecification>().ToArray(); }
        }

        public INakedObjectSpecification GetSpecification(Type type) {
            return new NakedObjectSpecification(this, LoadSpecification(type));
        }

        public INakedObjectSpecification GetSpecification(string name) {
            return new NakedObjectSpecification(this, LoadSpecification(name));
        }

        #endregion

        #region INakedObjectReflector Members

        public IClassStrategy ClassStrategy {
            get { return introspectionControlParameters.ClassStrategy; }
        }

        public IFacetFactorySet FacetFactorySet {
            get { return introspectionControlParameters.FacetFactorySet; }
        }

        public virtual IIntrospectableSpecification LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            return LoadSpecificationAndCache(ClassStrategy.GetType(type));
        }

        public IIntrospectableSpecification LoadSpecification(string className) {
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
                    AllSpecifications.OfType<IIntrospectableSpecification>().ForEach(s => s.PopulateAssociatedActions(services));
                }
            }
            finally {
                installingServices = false;
                linked = true;
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
            specification.Introspect(facetDecorator);
            specification.MarkAsService();
        }

        private IIntrospectableSpecification LoadSpecificationAndCache(Type type) {
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


                specification.Introspect(facetDecorator);

                if (!installingServices) {
                    var services = NonSystemServices ?? new Type[] {};
                    specification.PopulateAssociatedActions(services);
                }


                return specification;
            }
        }

        private IIntrospectableSpecification CreateSpecification(Type type) {
            return new IntrospectableSpecification(type, this);
        }

        private IIntrospectableSpecification Install(Type type) {
            return new IntrospectableSpecification(type, this);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}