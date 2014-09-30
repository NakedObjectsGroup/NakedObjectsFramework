// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
    public class DotNetReflector : INakedObjectReflector {
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

        #region INakedObjectReflector Members

        public IClassStrategy ClassStrategy {
            get { return introspectionControlParameters.ClassStrategy; }
        }

        public IFacetFactorySet FacetFactorySet {
            get { return introspectionControlParameters.FacetFactorySet; }
        }

        private Type[] NonSystemServices { get; set; }

        public virtual INakedObjectSpecification[] AllSpecifications {
            get { return cache.AllSpecifications(); }
        }

        public virtual INakedObjectSpecification LoadSpecification(Type type) {
            Assert.AssertNotNull(type);
            return LoadSpecificationAndCache(ClassStrategy.GetType(type));
        }

        public INakedObjectSpecification LoadSpecification(string className) {
            Assert.AssertNotNull("specification class must be specified", className);

            try {
                Type type = TypeFactory.GetTypeFromLoadedAssembly(className);
                return LoadSpecification(type);
            }
            catch (Exception e) {
                Log.InfoFormat("Failed to Load Specification for: {0} error: {1} trying cache", className, e);
                INakedObjectSpecification spec = cache.GetSpecification(className);
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

        public bool IgnoreCase { get; set; }

        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null && property.PropertyType != classToIgnore) {
                    LoadSpecification(property.PropertyType);
                }
            }
        }

        #endregion

        private void InstallServiceSpecification(Type type) {
            INakedObjectSpecification spec = cache.GetSpecification(type.GetProxiedTypeFullName());
            if (spec != null) {
                return;
            }

            NakedObjectSpecificationAbstract specification = Install(type);
            cache.Cache(type.GetProxiedTypeFullName(), specification);
            specification.Introspect(facetDecorator);
            specification.MarkAsService();
        }

        private INakedObjectSpecification LoadSpecificationAndCache(Type type) {
            string proxiedTypeName = type.GetProxiedTypeFullName();
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 
            lock (cache) {
                // this is a double check on the cache to check it was not written to during the unguarded 
                // internal between the last check and the lock
                INakedObjectSpecification specification = cache.GetSpecification(proxiedTypeName);
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

                var introspectableSpecification = specification as IIntrospectableSpecification;
                if (introspectableSpecification != null) {
                    introspectableSpecification.Introspect(facetDecorator);

                    if (!installingServices) {
                        var services = NonSystemServices ?? new Type[] {};
                        introspectableSpecification.PopulateAssociatedActions(services);
                    }
                }

                return specification;
            }
        }

        private INakedObjectSpecification CreateSpecification(Type type) {
            return new DotNetSpecification(type, this);
        }

        private NakedObjectSpecificationAbstract Install(Type type) {
            return new DotNetSpecification(type, this);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}