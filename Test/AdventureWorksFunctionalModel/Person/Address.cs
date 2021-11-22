// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public record Address : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int AddressID { get; init; }

        [MemberOrder(11)]
        public string AddressLine1 { get; init; } = "";

        [MemberOrder(12)]
        public string? AddressLine2 { get; init; }

        [MemberOrder(13)]
        public string City { get; init; } = "";

        [MemberOrder(14)]
        public string PostalCode { get; init; } = "";

        [Hidden]
        public int StateProvinceID { get; init; }

        [MemberOrder(15)]
#pragma warning disable 8618
        public virtual StateProvince StateProvince { get; init; }
#pragma warning restore 8618

        public virtual bool Equals(Address? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{AddressLine1}...";

        public override int GetHashCode() => base.GetHashCode();
    }
}