// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;
using static AW.Utilities;
using AW.Functions;

namespace AW.Types {
        public record CreditCard {
        [Hidden]
        public virtual int CreditCardID { get; init; }

        [MemberOrder(1)]
        public virtual string CardType { get; init; }

        [MemberOrder(2)]
        public virtual string CardNumber { get; init; }
        
        [MemberOrder(3)]
        public virtual byte ExpMonth { get; init; }

        [MemberOrder(4)]
        public virtual short ExpYear { get; init; }

        [Named("Persons"), MemberOrder(5), TableView(false, nameof(PersonCreditCard.Person))]
        public virtual ICollection<PersonCreditCard> PersonLinks { get; init; }  = new List<PersonCreditCard>();

        [MemberOrder(99)]
        //[Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => CreditCard_Functions.ObfuscatedNumber(this);

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(CreditCard other) => ReferenceEquals(this, other);
    }
}