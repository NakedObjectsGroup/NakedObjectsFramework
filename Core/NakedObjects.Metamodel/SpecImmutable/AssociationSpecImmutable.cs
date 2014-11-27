// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public abstract class AssociationSpecImmutable : MemberSpecImmutable, IAssociationSpecImmutable {
        private readonly IObjectSpecImmutable returnSpec;

        protected AssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable returnSpec)
            : base(identifier) {
            this.returnSpec = returnSpec;
        }

        #region IAssociationSpecImmutable Members

        public override IObjectSpecImmutable ReturnSpec {
            get { return returnSpec; }
        }

        public abstract bool IsOneToMany { get; }
        public abstract bool IsOneToOne { get; }

        public IAssociationSpecImmutable Spec {
            get { return this; }
        }

        public IList<IOrderableElement<IAssociationSpecImmutable>> Set {
            get { return null; }
        }

        public string GroupFullName {
            get { return ""; }
        }

        #endregion

        #region ISerializable

        // The special constructor is used to deserialize values. 
        protected AssociationSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            returnSpec = info.GetValue<IObjectSpecImmutable>("returnSpec");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<IObjectSpecImmutable>("returnSpec", returnSpec);
            base.GetObjectData(info, context);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}