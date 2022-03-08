// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedObjects.Resources;

namespace NakedFramework.Metamodel.I18N;

public class I18NManager : II18NManager, IFacetDecorator {
    private const string Action = "action";
    private const string Description = "description";
    private const string Name = "name";
    private const string Parameter = "parameter";
    private const string Property = "property";
    private readonly IDictionary<string, string> keyCache = new Dictionary<string, string>();
    private readonly ILogger<I18NManager> logger;

    private ResourceManager resources;

    public I18NManager(ILogger<I18NManager> logger) => this.logger = logger;

    public static ResourceManager TestResourceManager { get; set; } = null;

    // make resources testable 
    public ResourceManager Resources {
        private get => resources ??= TestResourceManager ?? Model.ResourceManager;
        set => resources = value;
    }

    private IFacet GetDescriptionFacet(ISpecification holder, IDescribedAsFacet facet, IIdentifier identifier) {
        var i18NDescription = holder is IActionParameterSpec spec ? GetParameterDescription(identifier, spec.Number) : GetDescription(identifier);
        return i18NDescription is null ? null : new DescribedAsFacetI18N(i18NDescription);
    }

    private IFacet GetNamedFacet(ISpecification holder, INamedFacet facet, IIdentifier identifier) {
        var i18NName = GetName(identifier);
        return i18NName is null ? null : new NamedFacetI18N(i18NName);
    }

    private IFacet GetMemberNamedFacet(ISpecification holder, IMemberNamedFacet facet, IIdentifier identifier) {
        var i18NName = holder is IActionParameterSpec spec ? GetParameterName(identifier, spec.Number) : GetName(identifier);
        return i18NName is null ? null : new MemberNamedFacetI18N(i18NName);
    }

    private string GetText(IIdentifier identifier, string type) {
        var form = identifier.IsField ? Property : Action;
        var key = $"{identifier.ToIdentityString(IdentifierDepth.ClassNameParams)}:{form}/{type}";
        return GetText(key);
    }

    private string GetText(string key) {
        var keyWithUnderscore = CreateKey(key);

        try {
            return Resources.GetString(keyWithUnderscore);
        }
        catch (MissingManifestResourceException e) {
            logger.LogWarning($"Missing manifest resource (culture {Thread.CurrentThread.CurrentUICulture}) for the key {key} ({keyWithUnderscore}) ->  {e.Message}");
            return null;
        }
    }

    private string CreateKey(string key) {
        lock (keyCache) {
            if (keyCache.ContainsKey(key)) {
                return keyCache[key];
            }

            var newKey = key.Replace('.', '_').Replace(':', '_').Replace('#', '_').Replace('/', '_').Replace('(', '_').Replace(')', '_').Replace(',', '_').Replace('?', '_');

            keyCache[key] = newKey;

            return newKey;
        }
    }

    private string GetName(IIdentifier identifier) => GetText(identifier, Name);

    private string GetDescription(IIdentifier identifier) => GetText(identifier, Description);

    private string GetParameterName(IIdentifier identifier, int index) {
        var key = $"{identifier.ToIdentityString(IdentifierDepth.ClassNameParams)}{Action}/{Parameter}{index + 1}/{Name}";
        return GetText(key);
    }

    private string GetParameterDescription(IIdentifier identifier, int index) {
        var key = $"{identifier.ToIdentityString(IdentifierDepth.ClassNameParams)}{Action}/{Parameter}{index + 1}/{Description}";
        return GetText(key);
    }

    #region IFacetDecorator Members

    public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
        var identifier = holder.Identifier;
        var facetType = facet.FacetType;

        if (facetType == typeof(INamedFacet)) {
            return GetNamedFacet(holder, facet as INamedFacet, identifier);
        }

        if (facetType == typeof(IMemberNamedFacet)) {
            return GetMemberNamedFacet(holder, facet as IMemberNamedFacet, identifier);
        }

        if (facetType == typeof(IDescribedAsFacet)) {
            return GetDescriptionFacet(holder, facet as IDescribedAsFacet, identifier);
        }

        return facet;
    }

    public virtual Type[] ForFacetTypes => new[] { typeof(INamedFacet), typeof(IMemberNamedFacet), typeof(IDescribedAsFacet) };

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.