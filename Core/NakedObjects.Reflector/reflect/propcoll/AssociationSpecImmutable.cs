// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Reflect.Propcoll {
    public abstract class AssociationSpecImmutable : MemberSpecImmutable, IAssociationSpecImmutable {

        protected readonly Type ReturnType;
        private readonly IObjectSpecImmutable returnSpec;

        protected AssociationSpecImmutable(IIdentifier identifier, Type returnType, IObjectSpecImmutable returnSpec)
            : base(identifier) {
       
            ReturnType = returnType;
            this.returnSpec = returnSpec;
        }

        #region INakedObjectAssociationPeer Members

        public override  IObjectSpecImmutable Specification {
            get {
                return returnSpec;
            }
        }

        public abstract bool IsOneToMany { get; }
        public abstract bool IsOneToOne { get; }

        #endregion

        public IAssociationSpecImmutable Peer { get { return this; } }
        public IOrderSet<IAssociationSpecImmutable> Set { get { return null; } }
    }

    // Copyright (c) Naked Objects Group Ltd.
}