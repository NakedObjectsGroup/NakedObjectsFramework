// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;
using static AW.Utilities;


namespace AW.Types
{
    public record Address : IHasRowGuid, IHasModifiedDate
    {
        [Hidden]
        public virtual int AddressID { get; init; }

        [MemberOrder(11)]
        public virtual string AddressLine1 { get; init; }

        [MemberOrder(12)]
        public virtual string AddressLine2 { get; init; }

        [MemberOrder(13)]
        public virtual string City { get; init; }

        [MemberOrder(14)]
        public virtual string PostalCode { get; init; }

        [Hidden]
        public virtual int StateProvinceID { get; init; }

        [MemberOrder(15)]
        public virtual StateProvince StateProvince { get; init; }

        [MemberOrder(16)]
        public virtual CountryRegion CountryRegion { get; init; }

        [MemberOrder(10)]
        public virtual AddressType AddressType { get; init; }

        public virtual BusinessEntity AddressFor { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{AddressLine1}...";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(Address other) => ReferenceEquals(this, other);
    }
}