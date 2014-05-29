// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [Bounded]
    [IconName("clock.png")]
    public class Shift : AWDomainObject {

        #region ID

        [Hidden]
        public virtual byte ShiftID { get; set; }

        #endregion

        #region Name

        [Title]
        [MemberOrder(1)]
        [StringLength(50)]
        [TypicalLength(10)]
        public virtual string Name { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region Complex Types

        private TimePeriod times = new TimePeriod();

        [MemberOrder(2)]
        public virtual TimePeriod Times {
            get { return times; }
            set { times = value; }
        }

        #endregion
    }
}