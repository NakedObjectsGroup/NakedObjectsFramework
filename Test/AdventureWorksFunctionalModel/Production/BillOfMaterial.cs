// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public record BillOfMaterial : IHasModifiedDate {
        [Hidden]
        public virtual int BillOfMaterialID { get; init; }

        public virtual DateTime StartDate { get; init; }
        public virtual DateTime? EndDate { get; init; }
        public virtual short BOMLevel { get; init; }
        public virtual decimal PerAssemblyQty { get; init; }

        [Hidden]
        public virtual int? ProductAssemblyID { get; init; }

        public virtual Product Product { get; init; }

        [Hidden]
        public virtual int ComponentID { get; init; }

        public virtual Product Product1 { get; init; }

        [Hidden]
        public virtual string UnitMeasureCode { get; init; }

        public virtual UnitMeasure UnitMeasure { get; init; }

        public virtual bool Equals(BillOfMaterial other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"BillOfMaterial: {BillOfMaterialID}";

        public override int GetHashCode() => base.GetHashCode();
    }
}