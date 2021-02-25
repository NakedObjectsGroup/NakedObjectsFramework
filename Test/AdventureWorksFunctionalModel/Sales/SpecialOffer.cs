// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types
{
    public record SpecialOffer: IHasModifiedDate, IHasRowGuid     {

        [Hidden]
        public virtual int SpecialOfferID { get; init; }

        [MemberOrder(10)]
        public virtual string Description { get; init; }

        [MemberOrder(20)]
        [Mask("P")]
        public virtual decimal DiscountPct { get; init; }

        [MemberOrder(30)]
        public virtual string Type { get; init; }

        [MemberOrder(40)]
        public virtual string Category { get; init; }

        [MemberOrder(51)]
        [Mask("d")]
        public virtual DateTime StartDate { get; init; }

        [MemberOrder(52)]
        [Mask("d")]
        public virtual DateTime EndDate { get; init; }

        [MemberOrder(61)]
        public virtual int MinQty { get; init; }

        [MemberOrder(62)]
        public virtual int? MaxQty { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => Description;

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(SpecialOffer other) => ReferenceEquals(this, other);
    }
}
