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
        public int ProductID { get; init; }

        [Hidden]
        public int VendorID { get; init; }

        [MemberOrder(30)]
        public int AverageLeadTime { get; init; }

        [Mask("C")]
        [MemberOrder(40)]
        public decimal StandardPrice { get; init; }

        [Mask("C")]
        [MemberOrder(41)]
        public decimal? LastReceiptCost { get; init; }

        [Mask("d")]
        [MemberOrder(50)]
        public DateTime? LastReceiptDate { get; init; }

        [MemberOrder(60)]
        public int MinOrderQty { get; init; }

        [MemberOrder(61)]
        public int MaxOrderQty { get; init; }

        [MemberOrder(62)]
        public int? OnOrderQty { get; init; }

        [MemberOrder(10)]
#pragma warning disable 8618
        public virtual Product Product { get; init; }
#pragma warning restore 8618

        [Hidden]
        public string UnitMeasureCode { get; init; } = "";

        [MemberOrder(20)]
#pragma warning disable 8618
        public virtual UnitMeasure UnitMeasure { get; init; }
#pragma warning restore 8618

        [Hidden]
#pragma warning disable 8618
        public virtual Vendor Vendor { get; init; }
#pragma warning restore 8618

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public virtual bool Equals(ProductVendor? other) => ReferenceEquals(this, other);

        public override string ToString() => $"ProductVendor: {ProductID}-{VendorID}";

        public override int GetHashCode() => base.GetHashCode();
    }
}