






using System;
using NakedFunctions;

namespace AW.Types {
    public class ProductReview {
        [Hidden]
        public int ProductReviewID { get; init; }

        [MemberOrder(1)]
        public string ReviewerName { get; init; } = "";

        [MemberOrder(2)]
        public DateTime ReviewDate { get; init; }

        [MemberOrder(3)]
        public string EmailAddress { get; init; } = "";

        [MemberOrder(4)]
        public int Rating { get; init; }

        [MemberOrder(5)]
        public string? Comments { get; init; }

        [Hidden]
        public int ProductID { get; init; }

        [Hidden]

        public virtual Product Product { get; init; }


        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        
        public override string ToString() => "*****".Substring(0, Rating);


    }
}