// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Core.Component {
    public sealed class MetamodelManager : IMetamodelManager {
        private readonly IDictionary<ITypeSpecImmutable, ITypeSpec> localCache = new Dictionary<ITypeSpecImmutable, ITypeSpec>();
        private readonly SpecFactory specFactory;

        public MetamodelManager(SpecFactory specFactory, IMetamodel metamodel) {
            this.specFactory = specFactory ?? throw new InitialisationException($"{nameof(specFactory)} is null");
            Metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
        }

        #region IMetamodelManager Members

        public ITypeSpec[] AllSpecs {
            get { return Metamodel.AllSpecifications.Select(s => specFactory.CreateTypeSpec(s)).ToArray(); }
        }

        public IMetamodel Metamodel { get; }

        public ITypeSpec GetSpecification(Type type) => type == null ? null : NewObjectSpec(GetInnerSpec(type));

        public ITypeSpec GetSpecification(string name) => string.IsNullOrWhiteSpace(name) ? null : NewObjectSpec(GetInnerSpec(name));

        public ITypeSpec GetSpecification(ITypeSpecImmutable spec) => spec == null ? null : NewObjectSpec(spec);

        public IObjectSpec GetSpecification(IObjectSpecImmutable spec) => GetSpecification(spec as ITypeSpecImmutable) as IObjectSpec;

        public IServiceSpec GetSpecification(IServiceSpecImmutable spec) => GetSpecification(spec as ITypeSpecImmutable) as IServiceSpec;

        public IMenuImmutable[] MainMenus() => Metamodel.MainMenus;

        public IActionSpec GetActionSpec(IActionSpecImmutable spec) => specFactory.CreateActionSpec(spec);

        #endregion

        private ITypeSpec NewObjectSpec(ITypeSpecImmutable spec) {
            if (!localCache.ContainsKey(spec)) {
                localCache[spec] = specFactory.CreateTypeSpec(spec);
            }

            return localCache[spec];
        }

        private ITypeSpecImmutable GetInnerSpec(Type type) => Metamodel.GetSpecification(type);

        private ITypeSpecImmutable GetInnerSpec(string name) => Metamodel.GetSpecification(name);
    }
}