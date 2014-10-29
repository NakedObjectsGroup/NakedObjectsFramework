// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.spec;
using NakedObjects.Core.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Managers {
    public class MetamodelManager : IMetamodelManager {
        private readonly IDictionary<IObjectSpecImmutable, IObjectSpec> localCache = new Dictionary<IObjectSpecImmutable, IObjectSpec>();
        private readonly SpecFactory memberFactory;
        private readonly IMetamodel metamodel;

        public MetamodelManager(SpecFactory memberFactory, IMetamodel metamodel) {
            this.memberFactory = memberFactory;
            this.metamodel = metamodel;
        }

        #region IMetamodelManager Members

        public IObjectSpec GetSpecification(Type type) {
            return type == null ? null : NewObjectSpec(GetInnerSpec(type));
        }

        public IObjectSpec GetSpecification(string name) {
            return string.IsNullOrWhiteSpace(name) ? null : NewObjectSpec(GetInnerSpec(name));
        }

        public IObjectSpec GetSpecification(IObjectSpecImmutable spec) {
            return spec == null ? null : NewObjectSpec(GetInnerSpec(spec.Type));
        }

        #endregion

        private IObjectSpec NewObjectSpec(IObjectSpecImmutable spec) {
            if (!localCache.ContainsKey(spec)) {
                localCache[spec] = new ObjectSpec(memberFactory, this, spec);
            }

            return localCache[spec];
        }

        private IObjectSpecImmutable GetInnerSpec(Type type) {
            var innerSpec = metamodel.GetSpecification(type);
            Assert.AssertNotNull(string.Format("failed to find spec for {0}", type.FullName), innerSpec);
            return innerSpec;
        }

        private IObjectSpecImmutable GetInnerSpec(string name) {
            var innerSpec = metamodel.GetSpecification(name);
            Assert.AssertNotNull(string.Format("failed to find spec for {0}", name), innerSpec);
            return innerSpec;
        }
    }
}