// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class DotNetReflector : INakedObjectReflector {
        // abstract 

        private static readonly ILog Log;
        private ISpecificationCache cache = new SimpleSpecificationCache();
        private FacetDecoratorSet facetDecorator;

        private bool installingServices;
        private bool linked;

        static DotNetReflector() {
            Log = LogManager.GetLogger(typeof(DotNetReflector));
        }

        public virtual FacetDecoratorSet FacetDecorator {
            set { facetDecorator = value; }
        }

        public virtual ISpecificationCache Cache {
            get { return cache; }
            set { cache = value; }
        }

        public IClassStrategy ClassStrategy { get; set; }
        protected IFacetFactorySet FacetFactorySet { get; set; }
        public IntrospectionControlParameters IntrospectionControlParameters { get; private set; }

        #region INakedObjectReflector Members

        public INakedObject[] NonSystemServices { get; set; }

        /// <summary>
        ///     Return all the loaded specifications
        /// </summary>
        public virtual INakedObjectSpecification[] AllSpecifications {
            get { return Cache.AllSpecifications(); }
        }


        public virtual void Shutdown() {
            Log.InfoFormat("shutting down {0}", this);
            Cache.Clear();
            facetDecorator.Shutdown();
        }

        /// <summary>
        ///     Return the specification for the specified class of object
        /// </summary>
        public virtual INakedObjectSpecification LoadSpecification(Type type) {
            Assert.AssertNotNull(type);


            return LoadSpecificationAndCache(ClassStrategy.GetType(type));
        }

        /// <summary>
        ///     Return the specification for the specified class of object
        /// </summary>
        public INakedObjectSpecification LoadSpecification(string className) {
            Assert.AssertNotNull("specification class must be specified", className);

            try {
                Type type = TypeFactory.GetTypeFromLoadedAssembly(className);
                return LoadSpecification(type);
            }
            catch (Exception e) {
                Log.InfoFormat("Failed to Load Specification for: {0} error: {1} trying cache", className, e);
                INakedObjectSpecification spec = Cache.GetSpecification(className);
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
        }

        public virtual void PopulateContributedActions(INakedObject[] services) {
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

        /// <summary>
        ///     Initializes and wires up
        /// </summary>
        public void Init() {
            Log.DebugFormat("Init {0}", this);
            Assert.AssertNotNull("FacetDecorator needed", facetDecorator);
            facetDecorator.Init();
            if (ClassStrategy == null) {
                ClassStrategy = new DefaultClassStrategy();
            }
            if (FacetFactorySet == null) {
                var facetFactorySetImpl = new FacetFactorySetImpl(this);
                facetFactorySetImpl.Init();
                FacetFactorySet = facetFactorySetImpl;
            }
            IntrospectionControlParameters = new IntrospectionControlParameters(FacetFactorySet, ClassStrategy);
            IgnoreCase = false;
        }

        #endregion

        public virtual void InstallServiceSpecification(Type type) {
         

            INakedObjectSpecification spec = Cache.GetSpecification(type.GetProxiedTypeFullName());
            if (spec != null) {
                return;
            }

            NakedObjectSpecificationAbstract specification = Install(type);
            Cache.Cache(type.GetProxiedTypeFullName(), specification);
            specification.Introspect(facetDecorator);
            specification.MarkAsService();
        }

        private INakedObjectSpecification LoadSpecificationAndCache(Type type) {
            string proxiedTypeName = type.GetProxiedTypeFullName();
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 
            lock (cache) {
                // this is a double check on the cache to check it was not written to during the unguarded 
                // internal between the last check and the lock
                INakedObjectSpecification specification = Cache.GetSpecification(proxiedTypeName);
                if (specification != null) {
                    return specification;
                }

                specification = CreateSpecification(type);

                if (specification == null) {
                    throw new ReflectionException("unrecognised class " + proxiedTypeName);
                }

                // we need the specification available in cache even though not yet full introspected 
                // need to be careful no other thread reads until introspected
                Cache.Cache(proxiedTypeName, specification);

                var introspectableSpecification = specification as IIntrospectableSpecification;
                if (introspectableSpecification != null) {
                    introspectableSpecification.Introspect(facetDecorator);

                    if (!installingServices) {
                        introspectableSpecification.PopulateAssociatedActions(NonSystemServices);
                    }
                }

                return specification;
            }
        }

        protected INakedObjectSpecification CreateSpecification(Type type) {
            return new DotNetSpecification(type, this);
        }

        /// <summary>
        ///     Used by the <see cref="DotNetIntrospector" /> created by the <see cref="DotNetSpecification" />
        ///     in <see cref="Install" />
        /// </summary>
        protected NakedObjectSpecificationAbstract Install(Type type) {
            return new DotNetSpecification(type, this);
        }


        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null && property.PropertyType != classToIgnore) {
                    LoadSpecification(property.PropertyType);
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}