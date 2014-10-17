// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.spec;
using NakedObjects.Core.Spec;

namespace NakedObjects.Managers {
    public class MetamodelManager : IMetamodelManager {
        private readonly SpecFactory memberFactory;
        private readonly IMetamodel metamodel;

        public MetamodelManager(SpecFactory memberFactory,  IMetamodel metamodel) {
            this.memberFactory = memberFactory;
            this.metamodel = metamodel;
        }

        #region IMetamodelManager Members

        public virtual IObjectSpec[] AllSpecs {
            get { return metamodel.AllSpecifications.Select(s => new ObjectSpec(memberFactory, this, s)).Cast<IObjectSpec>().ToArray(); }
        }

        public IObjectSpec GetSpecification(Type type) {
            return new ObjectSpec(memberFactory, this, metamodel.GetSpecification(type));
        }

        public IObjectSpec GetSpecification(string name) {
            return new ObjectSpec(memberFactory, this, metamodel.GetSpecification(name));
        }

        public IObjectSpec GetSpecification(IObjectSpecImmutable spec) {
            return new ObjectSpec(memberFactory,this, metamodel.GetSpecification(spec.Type));
        }

        #endregion
    }
}