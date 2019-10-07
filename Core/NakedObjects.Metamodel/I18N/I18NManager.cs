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
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Resources;

namespace NakedObjects.Meta.I18N {
    public class I18NManager : II18NManager, IFacetDecorator {
        private const string Action = "action";
        private const string Description = "description";
        private const string Name = "name";
        private const string Parameter = "parameter";
        private const string Property = "property";
        private static readonly ILog Log = LogManager.GetLogger(typeof(I18NManager));
        private readonly IDictionary<string, string> keyCache = new Dictionary<string, string>();

        private ResourceManager resources;

        // make resources testable 
        public ResourceManager Resources {
            private get { return resources ?? Model.ResourceManager; }
            set { resources = value; }
        }

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
            IIdentifier identifier = holder.Identifier;
            Type facetType = facet.FacetType;

            if (facetType == typeof(INamedFacet)) {
                return GetNamedFacet(holder, facet as INamedFacet, identifier);
            }

            if (facetType == typeof(IDescribedAsFacet)) {
                return GetDescriptionFacet(holder, facet as IDescribedAsFacet, identifier);
            }

            return facet;
        }

        public virtual Type[] ForFacetTypes => new[] {typeof(INamedFacet), typeof(IDescribedAsFacet)};

        #endregion

        private IFacet GetDescriptionFacet(ISpecification holder, IDescribedAsFacet facet, IIdentifier identifier) {
            var spec = holder as IActionParameterSpec;
            string i18NDescription = spec == null ? GetDescription(identifier) : GetParameterDescription(identifier, spec.Number);
            return i18NDescription == null ? null : new DescribedAsFacetI18N(i18NDescription, facet.Specification);
        }

        private IFacet GetNamedFacet(ISpecification holder, INamedFacet facet, IIdentifier identifier) {
            var spec = holder as IActionParameterSpec;
            string i18NName = spec == null ? GetName(identifier) : GetParameterName(identifier, spec.Number);
            return i18NName == null ? null : new NamedFacetI18N(i18NName, facet.Specification);
        }

        private string GetText(IIdentifier identifier, string type) {
            string form = identifier.IsField ? Property : Action;
            string key = identifier.ToIdentityString(IdentifierDepth.ClassNameParams) + ":" + form + "/" + type;
            return GetText(key);
        }

        private string GetText(string key) {
            if (key.StartsWith("System.") || key.StartsWith("NakedObjects.")) {
                return null;
            }

            string keyWithUnderscore = CreateKey(key);

            try {
                return Resources.GetString(keyWithUnderscore);
            }
            catch (MissingManifestResourceException e) {
                Log.WarnFormat("Missing manifest resource (culture {0}) for the key {1} ({2}) ->  {3}", Thread.CurrentThread.CurrentUICulture, key, keyWithUnderscore, e.Message);
                return null;
            }
        }

        private string CreateKey(string key) {
            if (keyCache.ContainsKey(key)) {
                return keyCache[key];
            }

            string newKey = key.Replace('.', '_').Replace(':', '_').Replace('#', '_').Replace('/', '_').Replace('(', '_').Replace(')', '_').Replace(',', '_').Replace('?', '_');

            keyCache[key] = newKey;

            return newKey;
        }

        private string GetName(IIdentifier identifier) {
            return GetText(identifier, Name);
        }

        private string GetDescription(IIdentifier identifier) {
            return GetText(identifier, Description);
        }

        private string GetParameterName(IIdentifier identifier, int index) {
            string key = identifier.ToIdentityString(IdentifierDepth.ClassNameParams) + Action + "/" + Parameter + (index + 1) + "/" + Name;
            return GetText(key);
        }

        private string GetParameterDescription(IIdentifier identifier, int index) {
            string key = identifier.ToIdentityString(IdentifierDepth.ClassNameParams) + Action + "/" + Parameter + (index + 1) + "/" + Description;
            return GetText(key);
        }
    }
}

// Copyright (c) Naked Objects Group Ltd.