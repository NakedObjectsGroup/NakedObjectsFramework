// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Meta.Component {
    [Serializable]
    public sealed class Metamodel : IMetamodelBuilder {
        private readonly ISpecificationCache cache;
        private readonly IClassStrategy classStrategy;
        private readonly ILogger<Metamodel> logger;

        public Metamodel(IClassStrategy classStrategy, ISpecificationCache cache, ILogger<Metamodel> logger) {
            this.classStrategy = classStrategy;
            this.cache = cache;
            this.logger = logger;
        }

        #region IMetamodelBuilder Members

        public ITypeSpecImmutable[] AllSpecifications => cache.AllSpecifications();

        public ITypeSpecImmutable GetSpecification(Type type, bool allowNull = false) {
            try {
                var spec = GetSpecificationFromCache(classStrategy.FilterNullableAndProxies(type));
                if (spec == null && !allowNull) {
                    throw new NakedObjectSystemException(logger.LogAndReturn($"Failed to Load Specification for: {type?.FullName} error: unexpected null"));
                }

                return spec;
            }
            catch (NakedObjectSystemException e) {
                logger.LogError($"Failed to Load Specification for: {(type == null ? "null" : type.FullName)} error: {e}");
                throw;
            }
            catch (Exception e) {
                throw new NakedObjectSystemException(logger.LogAndReturn($"Failed to Load Specification for: {type?.FullName} error: {e}"));
            }
        }

        public ITypeSpecImmutable GetSpecification(string name) {
            var type = TypeUtils.GetType(name);
            return GetSpecification(type);
        }

        public void Add(Type type, ITypeSpecBuilder spec) => cache.Cache(classStrategy.GetKeyForType(type), spec);

        public void AddMainMenu(IMenuImmutable menu) => cache.Cache(menu);

        public IMenuImmutable[] MainMenus => cache.MainMenus();

        #endregion

        private ITypeSpecImmutable GetSpecificationFromCache(Type type) {
            var key = classStrategy.GetKeyForType(type);
            TypeUtils.GetType(type.FullName); // This should ensure type is cached 
            return cache.GetSpecification(key);
        }
    }
}