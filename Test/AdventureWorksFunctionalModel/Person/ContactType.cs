






using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public class ContactType : IHasModifiedDate {
        [Hidden]
        public int ContactTypeID { get; init; }

        [MemberOrder(1)]
        public string Name { get; init; } = "";

        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;


    }
}