using System;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class Metamodel : IMetamodelMutable {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Metamodel));

        private readonly ISpecificationCache cache = new SimpleSpecificationCache();
        private readonly IClassStrategy classStrategy;

        public Metamodel(IClassStrategy classStrategy) {
            this.classStrategy = classStrategy;
        }

        #region IMetamodelMutable Members

        public virtual IObjectSpecImmutable[] AllSpecifications {
            get { return cache.AllSpecifications(); }
        }

        public IObjectSpecImmutable GetSpecification(Type type) {
            return GetSpecificationFromCache(classStrategy.GetType(type));
        }

        public IObjectSpecImmutable GetSpecification(string name) {
            try {
                Type type = TypeFactory.GetTypeFromLoadedAssembly(name);
                return GetSpecification(type);
            }
            catch (Exception e) {
                Log.InfoFormat("Failed to Load Specification for: {0} error: {1} trying cache", name, e);
                var spec = cache.GetSpecification(name);
                if (spec != null) {
                    Log.InfoFormat("Found {0} in cache", name);
                    return spec;
                }
                throw;
            }
        }

        public void Add(string name, IObjectSpecImmutable spec) {
            cache.Cache(name, spec);
        }

        #endregion

        private IObjectSpecImmutable GetSpecificationFromCache(Type type) {
            string proxiedTypeName = type.GetProxiedTypeFullName();
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 

            return cache.GetSpecification(proxiedTypeName);
        }
    }
}