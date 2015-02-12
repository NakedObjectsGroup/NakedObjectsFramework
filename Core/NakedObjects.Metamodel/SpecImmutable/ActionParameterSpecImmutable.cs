// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Spec;
using NakedObjects.Meta.Utils;
using NakedObjects.Architecture.Facet;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public class ActionParameterSpecImmutable : Specification, IActionParameterSpecImmutable {
        private readonly IObjectSpecImmutable specification;

        public ActionParameterSpecImmutable(IObjectSpecImmutable specification) {
            this.specification = specification;
        }

        #region IActionParameterSpecImmutable Members

        public IObjectSpecImmutable Specification {
            get { return specification; }
        }

        public override IIdentifier Identifier {
            get { return null; }
        }

        public bool IsChoicesEnabled {
            get {
                return !IsMultipleChoicesEnabled && (Specification.IsBoundedSet() || ContainsFacet<IActionChoicesFacet>() || ContainsFacet<IEnumFacet>());
            }
        }

        public bool IsMultipleChoicesEnabled {
            get {
                return ContainsFacet<IActionChoicesFacet>() && GetFacet<IActionChoicesFacet>().IsMultiple;
            }
        }

        #endregion

        #region ISerializable

        // The special constructor is used to deserialize values. 
        public ActionParameterSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            specification = info.GetValue<IObjectSpecImmutable>("specification");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<IObjectSpecImmutable>("specification", specification);
            base.GetObjectData(info, context);
        }

        #endregion
    }
}