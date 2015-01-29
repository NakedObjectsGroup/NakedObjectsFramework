// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class SpecialOffer : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(Description);
            return t.ToString();
        }

        public virtual string IconName() {
            if (Type == "No Discount") {
                return "default.png";
            }
            return "scissors.png";
        }

        #endregion

        //private ICollection<SpecialOfferProduct> _SpecialOfferProduct = new List<SpecialOfferProduct>();

        [Hidden]
        public virtual int SpecialOfferID { get; set; }

        [MemberOrder(10)]
        public virtual string Description { get; set; }

        [MemberOrder(20)]
        [Mask("P")]
        public virtual decimal DiscountPct { get; set; }

        [MemberOrder(30)]
        public virtual string Type { get; set; }

        [MemberOrder(40)]
        public virtual string Category { get; set; }

        [MemberOrder(51)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        [MemberOrder(52)]
        [Mask("d")]
        public virtual DateTime EndDate { get; set; }

        [MemberOrder(61)]
        public virtual int MinQty { get; set; }

        [Optionally]
        [MemberOrder(62)]
        public virtual int? MaxQty { get; set; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion

        public virtual string[] ChoicesCategory() {
            return new string[] {"Reseller", "Customer"};
        }

        public virtual DateTime DefaultStartDate() {
            return DateTime.Now;
        }

        public virtual DateTime DefaultEndDate() {
            return DateTime.Now.AddDays(90);
        }
    }
}