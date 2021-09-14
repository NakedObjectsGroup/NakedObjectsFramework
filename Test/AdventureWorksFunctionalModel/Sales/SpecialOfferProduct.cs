// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public record SpecialOfferProduct {
        [Hidden]
        public virtual int SpecialOfferID { get; init; }

        [MemberOrder(1)]
#pragma warning disable 8618
        public virtual SpecialOffer SpecialOffer { get; init; }
#pragma warning restore 8618

        [Hidden]
        public virtual int ProductID { get; init; }

        [MemberOrder(2)]
#pragma warning disable 8618
        public virtual Product Product { get; init; }
#pragma warning restore 8618

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        public virtual bool Equals(SpecialOfferProduct? other) => ReferenceEquals(this, other);

        public override string ToString() => $"SpecialOfferProduct: {SpecialOfferID}-{ProductID}";

        public override int GetHashCode() => base.GetHashCode();
    }
}