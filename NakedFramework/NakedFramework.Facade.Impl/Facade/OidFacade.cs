// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Facade.Facade;

namespace NakedObjects.Facade.Impl {
    public class OidFacade : IOidFacade {
        private readonly IOid oid;

        public OidFacade(IOid oid) => this.oid = oid ?? throw new NullReferenceException($"{nameof(oid)} is null");

        #region IOidFacade Members

        public object Value => oid;

        #endregion

        public override bool Equals(object obj) => obj is OidFacade of && Equals(of);

        public bool Equals(OidFacade other) {
            if (ReferenceEquals(null, other)) { return false; }

            return ReferenceEquals(this, other) || Equals(other.oid, oid);
        }

        public override int GetHashCode() => oid != null ? oid.GetHashCode() : 0;

        public override string ToString() => oid.ToString();
    }
}