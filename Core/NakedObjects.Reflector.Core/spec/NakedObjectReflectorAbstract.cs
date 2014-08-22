// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.Spec {
    public abstract class NakedObjectReflectorAbstract : INakedObjectReflector {
        private static readonly ILog Log;
        private ISpecificationCache cache = new SimpleSpecificationCache();
        private FacetDecoratorSet facetDecorator;

        private bool installingServices;
        private bool linked; 

        static NakedObjectReflectorAbstract() {
            Log = LogManager.GetLogger(typeof (NakedObjectReflectorAbstract));
        }

        public virtual FacetDecoratorSet FacetDecorator {
            set { facetDecorator = value; }
        }

        public virtual ISpecificationCache Cache {
            get { return cache; }
            set { cache = value; }
        }

        public INakedObject[] NonSystemServices { get; set; }

        #region INakedObjectReflector Members

        /// <summary>
        ///     Return all the loaded specifications
        /// </summary>
        public virtual INakedObjectSpecification[] AllSpecifications {
            get { return Cache.AllSpecifications(); }
        }

        public virtual void Init() {
            Log.DebugFormat("Init {0}", this);
            Assert.AssertNotNull("FacetDecorator needed", facetDecorator);
            facetDecorator.Init();
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
            return LoadSpecificationAndCache(type);
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

        #endregion

        protected abstract NakedObjectSpecificationAbstract Install(Type type);

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

        /// <summary>
        ///     Hook method for language-specific subclass to create the appropriate type of
        ///     <see cref="INakedObjectSpecification" />
        /// </summary>
        protected abstract INakedObjectSpecification CreateSpecification(Type type);
    }

    // Copyright (c) Naked Objects Group Ltd.
}