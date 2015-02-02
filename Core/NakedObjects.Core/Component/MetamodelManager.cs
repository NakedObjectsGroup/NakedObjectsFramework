// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public class MetamodelManager : IMetamodelManager {
        private readonly IDictionary<ITypeSpecImmutable, ITypeSpec> localCache = new Dictionary<ITypeSpecImmutable, ITypeSpec>();
        private readonly IMetamodel metamodel;
        private readonly SpecFactory specFactory;

        public MetamodelManager(SpecFactory specFactory, IMetamodel metamodel) {
            Assert.AssertNotNull(specFactory);
            Assert.AssertNotNull(metamodel);

            this.specFactory = specFactory;
            this.metamodel = metamodel;
        }

        #region IMetamodelManager Members

        public virtual ITypeSpec[] AllSpecs {
            get { return Metamodel.AllSpecifications.Select(s => specFactory.CreateTypeSpec(s)).ToArray(); }
        }

        public IMetamodel Metamodel {
            get { return metamodel; }
        }

        public ITypeSpec GetSpecification(Type type) {
            return type == null ? null : NewObjectSpec(GetInnerSpec(type));
        }

        public ITypeSpec GetSpecification(string name) {
            return string.IsNullOrWhiteSpace(name) ? null : NewObjectSpec(GetInnerSpec(name));
        }

        public ITypeSpec GetSpecification(ITypeSpecImmutable spec) {
            return spec == null ? null : NewObjectSpec(spec);
        }

        public IMenuImmutable[] MainMenus() {
            return Metamodel.MainMenus;
        }

        public IActionSpec GetActionSpec(IActionSpecImmutable spec) {
            return specFactory.CreateActionSpec(spec);
        }

        #endregion

        private ITypeSpec NewObjectSpec(ITypeSpecImmutable spec) {
            if (!localCache.ContainsKey(spec)) {
                localCache[spec] = specFactory.CreateTypeSpec(spec);
            }

            return localCache[spec];
        }

        private ITypeSpecImmutable GetInnerSpec(Type type) {
            ITypeSpecImmutable innerSpec = Metamodel.GetSpecification(type);
            Assert.AssertNotNull(string.Format("failed to find spec for {0}", type.FullName), innerSpec);
            return innerSpec;
        }

        private ITypeSpecImmutable GetInnerSpec(string name) {
            ITypeSpecImmutable innerSpec = Metamodel.GetSpecification(name);
            Assert.AssertNotNull(string.Format("failed to find spec for {0}", name), innerSpec);
            return innerSpec;
        }
    }
}