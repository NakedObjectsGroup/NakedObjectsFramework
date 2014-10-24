// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class Metamodel : IMetamodelMutable {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Metamodel));

        private readonly ISpecificationCache cache;
        private readonly IClassStrategy classStrategy;

        public Metamodel(IClassStrategy classStrategy, ISpecificationCache cache) {
            this.classStrategy = classStrategy;
            this.cache = cache;
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

        public void Add(Type type, IObjectSpecImmutable spec) {
            cache.Cache(GetKeyForType(type), spec);
        }

        #endregion

        private string GetKeyForType(Type type) {
            if (CollectionUtils.IsGenericType(type, typeof(IEnumerable<>))) {
                return type.Namespace + "." + type.Name;
            }

            if (type.IsArray) {
                return "System.Array";
            }

            return type.GetProxiedTypeFullName();
        }

        private IObjectSpecImmutable GetSpecificationFromCache(Type type) {
            string key = GetKeyForType(type);
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 

            return cache.GetSpecification(key);
        }
    }
}