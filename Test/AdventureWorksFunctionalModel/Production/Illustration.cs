






using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public class Illustration {
        public int IllustrationID { get; init; }
        public string? Diagram { get; init; }

        public virtual ICollection<ProductModelIllustration> ProductModelIllustration { get; init; } = new List<ProductModelIllustration>();

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => $"Illustration: {IllustrationID}";


    }
}