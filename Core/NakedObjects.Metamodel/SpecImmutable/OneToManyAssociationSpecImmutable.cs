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
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public class OneToManyAssociationSpecImmutable : AssociationSpecImmutable {
        private readonly Type defaultElementType;
        private readonly IObjectSpecImmutable defaultElementSpec;

        // TODO remork so that either elemnt type is passed in or guarantee elementtype facet is available
        // so that do not need to pass in metamodel
        public OneToManyAssociationSpecImmutable(IIdentifier name, Type returnType, IObjectSpecImmutable returnSpec, Type defaultElementType, IObjectSpecImmutable defaultElementSpec)
            : base(name, returnType, returnSpec) {
            this.defaultElementType = defaultElementType;
            this.defaultElementSpec = defaultElementSpec;
        }

        public Type ElementType {
            get {
                var typeOfFacet = GetFacet<IElementTypeFacet>();
                return typeOfFacet != null ? typeOfFacet.Value : defaultElementType;
            }
        }

        /// <summary>
        ///     Return the <see cref="IObjectSpec" /> for the  Type that the collection holds.
        /// </summary>
        public override IObjectSpecImmutable Specification {
            get {
                var typeOfFacet = GetFacet<IElementTypeFacet>();
                return typeOfFacet != null ? typeOfFacet.ValueSpec : defaultElementSpec;
            }
        }

        public override bool IsOneToMany {
            get { return true; }
        }

        public override bool IsOneToOne {
            get { return false; }
        }

        public override string ToString() {
            return "OneToManyAssociation [name=\"" + Identifier + "\",Type=" + Specification + " ]";
        }

        #region ISerializable

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("defaultElementType", defaultElementType);
            info.AddValue("defaultElementSpec", defaultElementSpec);
            base.GetObjectData(info, context);
        }

        // The special constructor is used to deserialize values. 
        public OneToManyAssociationSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            defaultElementType = (Type)info.GetValue("defaultElementType", typeof(Type));
            defaultElementSpec = (IObjectSpecImmutable)info.GetValue("defaultElementSpec", typeof(IObjectSpecImmutable));
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}