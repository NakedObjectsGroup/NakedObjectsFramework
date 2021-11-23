






using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public class Culture : IHasModifiedDate {
        [Hidden]
        public string CultureID { get; init; } = "";

        [MemberOrder(10)]
        public string Name { get; init; } = "";

        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;


    }
}