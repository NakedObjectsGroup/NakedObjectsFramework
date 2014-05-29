// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("org_chart.png")]
    [Bounded]
    [Immutable]
    public class Department : AWDomainObject {

        #region Properties

        [Hidden]
        public virtual short DepartmentID { get; set; }

        [Title]
        [MemberOrder(1)]
        public virtual string Name { get; set; }

        [MemberOrder(2)]
        public virtual string GroupName { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region DELETED RELATIONSHIPS

        //public ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistory {
        //    get {

        //        return _EmployeeDepartmentHistory;
        //    }
        //    set {
        //        _EmployeeDepartmentHistory = value;

        //    }
        //}

        #endregion
    }
}