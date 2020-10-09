// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
        public record ProductInventory : IHasRowGuid, IHasModifiedDate {

        [NakedObjectsIgnore]
        public virtual int ProductID { get; init; }

        [NakedObjectsIgnore]
        public virtual short LocationID { get; init; }

        [MemberOrder(40)]
        public virtual string Shelf { get; init; }

        [MemberOrder(50)]
        public virtual byte Bin { get; init; }

        [MemberOrder(10)]
        public virtual short Quantity { get; init; }

        [MemberOrder(30)]
        public virtual Location Location { get; init; }

        [MemberOrder(20)]
        public virtual Product Product { get; init; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Quantity} in {Location} - {Shelf}";
    }
}