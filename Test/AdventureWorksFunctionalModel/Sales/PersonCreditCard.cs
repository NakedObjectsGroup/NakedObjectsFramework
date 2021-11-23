






using System;
using NakedFunctions;

namespace AW.Types {
    public class PersonCreditCard {
        [Hidden]
        public int PersonID { get; init; }

        [Hidden]
        public int CreditCardID { get; init; }


        public virtual Person Person { get; init; }



        public virtual CreditCard CreditCard { get; init; }


        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"PersonCreditCard: {PersonID}-{CreditCardID}";


    }
}