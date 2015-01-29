// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    [Immutable(WhenTo.OncePersisted)]
    public class EmployeePayHistory : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(Rate, "C", null).Append(" from", RateChangeDate, "d", null);
            return t.ToString();
        }

        #endregion

        #region Life Cycle methods

        public void Persisted() {
            Employee.PayHistory.Add(this);
        }

        #endregion

        #region EmployeeID

        [Hidden]
        public virtual int EmployeeID { get; set; }

        #endregion

        #region RateChangeDate

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime RateChangeDate { get; set; }

        #endregion

        #region Rate

        [Mask("C")]
        [MemberOrder(2)]
        public virtual decimal Rate { get; set; }

        #endregion

        #region PayFrequency

        public virtual byte PayFrequency { get; set; }

        public byte[] ChoicesPropertyName() {
            return new byte[] {1, 2};
        }

        #endregion

        #region Employee

        [Disabled]
        [MemberOrder(4)]
        public virtual Employee Employee { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}