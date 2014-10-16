// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Reflect.Properties {
    public class OneToOneAssociationSpecImmutable : AssociationSpecImmutable {
        public OneToOneAssociationSpecImmutable(IIdentifier identifier, Type returnType, IObjectSpecImmutable returnSpec)
            : base(identifier, returnType, returnSpec) {}

        public override bool IsOneToMany {
            get { return false; }
        }

        public override bool IsOneToOne {
            get { return true; }
        }

        public override string ToString() {
            return "Reference Association [name=\"" + Identifier + ", Type=" + Specification + " ]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}