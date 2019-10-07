// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public abstract class MemberSpecImmutable : Specification, IMemberSpecImmutable {
        private readonly IIdentifier identifier;

        protected MemberSpecImmutable(IIdentifier identifier) {
            this.identifier = identifier;
        }

        public abstract IObjectSpecImmutable ElementSpec { get; }

        #region IMemberSpecImmutable Members

        public override IIdentifier Identifier => identifier;

        public abstract IObjectSpecImmutable ReturnSpec { get; }

        public string Name => GetFacet<INamedFacet>().NaturalName;

        public string Description => GetFacet<IDescribedAsFacet>().Value;

        #endregion

        #region ISerializable

        // The special constructor is used to deserialize values. 
        protected MemberSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            identifier = info.GetValue<IIdentifier>("identifier");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<IIdentifier>("identifier", identifier);
            base.GetObjectData(info, context);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}