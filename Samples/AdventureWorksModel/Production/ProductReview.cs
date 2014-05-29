// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class ProductReview : AWDomainObject {
        [Hidden]
        public virtual int ProductReviewID { get; set; }

        [MemberOrder(1)]
        public virtual string ReviewerName { get; set; }

        [MemberOrder(2)]
        public virtual DateTime ReviewDate { get; set; }

        [MemberOrder(3)]
        public virtual string EmailAddress { get; set; }

        [MemberOrder(4)]
        public virtual int Rating { get; set; }

        [MemberOrder(5)]
        public virtual string Comments { get; set; }

        [Hidden]
        public virtual Product Product { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        public override string ToString() {
            var t = new TitleBuilder();
            string stars = "*****".Substring(0, Rating);
            t.Append(stars);
            return t.ToString();
        }
    }
}