// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    public class EmployeeDepartmentHistory : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Department).Append(StartDate, "d", null);
            return t.ToString();
        }


        #endregion

        [Hidden]
        public virtual int EmployeeID { get; set; }

        [Hidden]
        public virtual short DepartmentID { get; set; }

        [Hidden]
        public virtual byte ShiftID { get; set; }

        [Mask("d")]
        [MemberOrder(4)]
        public virtual DateTime StartDate { get; set; }

        [Mask("d")]
        [MemberOrder(5)]
        public virtual DateTime? EndDate { get; set; }

        [MemberOrder(2)]
        public virtual Department Department { get; set; }

        [MemberOrder(1)]
        public virtual Employee Employee { get; set; }

        [MemberOrder(3)]
        public virtual Shift Shift { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}