// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public sealed class OneToManyAssociationSpecImmutable : AssociationSpecImmutable, IOneToManyAssociationSpecImmutable {
        private readonly IObjectSpecImmutable defaultElementSpec;
        private readonly Type defaultElementType;

        public OneToManyAssociationSpecImmutable(IIdentifier name, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec, IObjectSpecImmutable defaultElementSpec)
            : base(name, returnSpec) {
            defaultElementType = defaultElementSpec.Type;
            OwnerSpec = ownerSpec;
            this.defaultElementSpec = defaultElementSpec;
        }

        #region IOneToManyAssociationSpecImmutable Members

        /// <summary>
        ///     Return the <see cref="IObjectSpec" /> for the  Type that the collection holds.
        /// </summary>
        public override IObjectSpecImmutable ElementSpec {
            get {
                var typeOfFacet = GetFacet<IElementTypeFacet>();
                return typeOfFacet != null ? typeOfFacet.ValueSpec : defaultElementSpec;
            }
        }

        public override IObjectSpecImmutable OwnerSpec { get; }

        #endregion

        public override string ToString() => $"OneToManyAssociation [name=\"{Identifier}\",Type={ReturnSpec} ]";

        #region ISerializable

        // The special constructor is used to deserialize values. 
        public OneToManyAssociationSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            defaultElementType = info.GetValue<Type>("defaultElementType");
            defaultElementSpec = info.GetValue<IObjectSpecImmutable>("defaultElementSpec");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<Type>("defaultElementType", defaultElementType);
            info.AddValue<IObjectSpecImmutable>("defaultElementSpec", defaultElementSpec);
            base.GetObjectData(info, context);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}