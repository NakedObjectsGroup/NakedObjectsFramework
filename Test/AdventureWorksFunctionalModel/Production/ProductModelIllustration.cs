






using System;
using NakedFunctions;

namespace AW.Types {
    public class ProductModelIllustration : IHasModifiedDate {
        [Hidden]
        public int ProductModelID { get; init; }

        [Hidden]
        public int IllustrationID { get; init; }


        public virtual Illustration Illustration { get; init; }


        public virtual ProductModel ProductModel { get; init; }


        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        public override string ToString() => $"ProductModelIllustration: {ProductModelID}-{IllustrationID}";


    }
}