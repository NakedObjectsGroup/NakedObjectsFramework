// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public record ProductVendor {
        [Hidden]
        public virtual int ProductID { get; init; }

        [Hidden]
        public virtual int VendorID { get; init; }

        [MemberOrder(30)]
        public virtual int AverageLeadTime { get; init; }

        [Mask("C")]
        [MemberOrder(40)]
        public virtual decimal StandardPrice { get; init; }

        [Mask("C")]
        [MemberOrder(41)]
        public virtual decimal? LastReceiptCost { get; init; }

        [Mask("d")]
        [MemberOrder(50)]
        public virtual DateTime? LastReceiptDate { get; init; }

        [MemberOrder(60)]
        public virtual int MinOrderQty { get; init; }

        [MemberOrder(61)]
        public virtual int MaxOrderQty { get; init; }

        [MemberOrder(62)]
        public virtual int? OnOrderQty { get; init; }

        [MemberOrder(10)]
        public virtual Product Product { get; init; }

        [Hidden]
        public virtual string UnitMeasureCode { get; init; }

        [MemberOrder(20)]
        public virtual UnitMeasure UnitMeasure { get; init; }

        [Hidden]
        public virtual Vendor Vendor { get; init; }

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        public virtual bool Equals(ProductVendor other) => ReferenceEquals(this, other);

        public override string ToString() => $"ProductVendor: {ProductID}-{VendorID}";

        public override int GetHashCode() => base.GetHashCode();
    }
}