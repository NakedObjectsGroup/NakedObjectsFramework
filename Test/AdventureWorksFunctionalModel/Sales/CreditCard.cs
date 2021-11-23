






using System;
using System.Collections.Generic;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    public class CreditCard {
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

        
        public override string? ToString() => CreditCard_Functions.ObfuscatedNumber(this);


    }
}