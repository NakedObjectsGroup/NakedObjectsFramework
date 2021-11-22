// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    public record CreditCard {
        [Hidden]
        public int CreditCardID { get; init; }

        [Hidden]
        public string CardType { get; init; } = "";

        [Hidden]
        public string CardNumber { get; init; } = "";

        [Hidden]
        public byte ExpMonth { get; init; }

        [Hidden]
        public short ExpYear { get; init; }

        [Named("Persons")] [MemberOrder(5)] [TableView(false, nameof(PersonCreditCard.Person))]
        public virtual ICollection<PersonCreditCard> PersonLinks { get; init; } = new List<PersonCreditCard>();

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public virtual bool Equals(CreditCard? other) => ReferenceEquals(this, other);

        public override string? ToString() => CreditCard_Functions.ObfuscatedNumber(this);

        public override int GetHashCode() => base.GetHashCode();
    }
}