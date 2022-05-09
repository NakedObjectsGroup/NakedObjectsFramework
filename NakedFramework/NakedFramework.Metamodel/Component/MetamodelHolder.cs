// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Component;

public sealed class MetamodelHolder : IMetamodelBuilder {
    private readonly ILogger<MetamodelHolder> logger;

    public MetamodelHolder(ISpecificationCache cache, ILogger<MetamodelHolder> logger) {
        this.Cache = cache;
        this.logger = logger;
    }

    private ITypeSpecImmutable GetSpecificationFromCache(Type type) {
        var key = TypeKeyUtils.GetKeyForType(type);
        TypeUtils.GetType(type.FullName); // This should ensure type is cached 
        return Cache.GetSpecification(key);
    }

    #region IMetamodelBuilder Members

    public ITypeSpecImmutable[] AllSpecifications => Cache.AllSpecifications();

    public ITypeSpecImmutable GetSpecification(Type type, bool allowNull = false) {
        try {
            var spec = GetSpecificationFromCache(TypeKeyUtils.FilterNullableAndProxies(type));
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

    public void Add(Type type, ITypeSpecBuilder spec) => Cache.Cache(TypeKeyUtils.GetKeyForType(type), spec);

    public void AddMainMenu(IMenuImmutable menu) => Cache.Cache(menu);

    public void ReplaceCache(ISpecificationCache newCache) => Cache = newCache;

    public void SaveToFile(string filePath) {
        Cache.Serialize(filePath);
    }

    public ISpecificationCache Cache { get; private set; }

    public IMenuImmutable[] MainMenus => Cache.MainMenus();

    #endregion
}