// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Configuration;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Component;

public sealed class MetamodelHolder : IMetamodelBuilder {
    private readonly ICoreConfiguration coreConfiguration;
    private readonly ILogger<MetamodelHolder> logger;

    public MetamodelHolder(ISpecificationCache cache, ICoreConfiguration coreConfiguration, ILogger<MetamodelHolder> logger) {
        Cache = cache;
        this.coreConfiguration = coreConfiguration;
        this.logger = logger;
    }

    private ITypeSpecImmutable GetSpecificationFromCache(Type type) {
        var key = TypeKeyUtils.GetKeyForType(type);
        TypeUtils.GetType(type.FullName); // This should ensure type is cached 
        return Cache.GetSpecification(key);
    }

    #region IMetamodelBuilder Members

    public ITypeSpecImmutable[] AllSpecifications => Cache.AllSpecifications();

    private ITypeSpecImmutable ReturnOrError(ITypeSpecImmutable spec, Type type) =>
        spec switch {
            { } => spec,
            null when coreConfiguration.UsePlaceholderForUnreflectedType => GetSpecification(typeof(NakedFramework.Core.Error.UnreflectedTypePlaceholder)),
            _ => throw new NakedObjectSystemException(logger.LogAndReturn($"Failed to Load Specification for: {type?.FullName} error: unexpected null"))
        };

    public ITypeSpecImmutable GetSpecification(Type type) {
        try {
            return ReturnOrError(GetSpecificationFromCache(TypeKeyUtils.FilterNullableAndProxies(type)), type);
        }
        catch (Exception e) when (e is not NakedObjectSystemException) {
            throw new NakedObjectSystemException(logger.LogAndReturn($"Failed to Load Specification for: {type?.FullName} error: {e}"));
        }
    }

    public ITypeSpecImmutable GetSpecification(string name) => GetSpecification(TypeUtils.GetType(name));

    public void Add(Type type, ITypeSpecBuilder spec) => Cache.Cache(TypeKeyUtils.GetKeyForType(type), spec);

    public void AddMainMenu(IMenuImmutable menu) => Cache.Cache(menu);

    public void ReplaceCache(ISpecificationCache newCache) => Cache = newCache;

    public void SaveToFile(string filePath, Type[] additionalKnownTypes) => Cache.Serialize(filePath, additionalKnownTypes);

    public ISpecificationCache Cache { get; private set; }

    public IMenuImmutable[] MainMenus => Cache.MainMenus();

    #endregion
}