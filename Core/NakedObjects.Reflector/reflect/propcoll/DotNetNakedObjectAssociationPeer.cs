// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Reflect.Propcoll {
    public abstract class DotNetNakedObjectAssociationPeer : DotNetNakedObjectMemberPeer, INakedObjectAssociationPeer {
        protected readonly IMetadata Metadata;
        protected readonly Type ReturnType;
        private readonly bool oneToMany;

        protected DotNetNakedObjectAssociationPeer(IMetadata metadata, IIdentifier identifier, Type returnType, bool oneToMany)
            : base(identifier) {
            Metadata = metadata;
            ReturnType = returnType;
            this.oneToMany = oneToMany;
        }

        #region INakedObjectAssociationPeer Members

        public virtual INakedObjectSpecification Specification {
            get { return ReturnType == null ? null : Metadata.GetSpecification(ReturnType); }
        }

        public bool IsOneToMany {
            get { return oneToMany; }
        }

        public bool IsOneToOne {
            get { return !IsOneToMany; }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}