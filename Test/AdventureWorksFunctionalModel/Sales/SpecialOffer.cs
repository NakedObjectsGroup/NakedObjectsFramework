// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public class SpecialOffer : IHasModifiedDate, IHasRowGuid {
        public SpecialOffer() { }

        public SpecialOffer(SpecialOffer cloneFrom)
        {
            SpecialOfferID = cloneFrom.SpecialOfferID;
            Description = cloneFrom.Description;
            DiscountPct = cloneFrom.DiscountPct;
            Type = cloneFrom.Type;
            Category = cloneFrom.Category;
            StartDate = cloneFrom.StartDate;
            EndDate = cloneFrom.EndDate;
            MinQty = cloneFrom.MinQty;
            MaxQty = cloneFrom.MaxQty;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
        }
        [Hidden]
        public int SpecialOfferID { get; init; }

        [MemberOrder(10)]
        public string Description { get; init; } = "";

        [MemberOrder(20)]
        [Mask("P")]
        public decimal DiscountPct { get; init; }

        [MemberOrder(30)]
        public string Type { get; init; } = "";

        [MemberOrder(40)]
        public string Category { get; init; } = "";

        [MemberOrder(51)]
        [Mask("d")]
        public DateTime StartDate { get; init; }

        [MemberOrder(52)]
        [Mask("d")]
        public DateTime EndDate { get; init; }

        [MemberOrder(61)]
        public int MinQty { get; init; }

        [MemberOrder(62)]
        public int? MaxQty { get; init; }

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => Description;
    }
}