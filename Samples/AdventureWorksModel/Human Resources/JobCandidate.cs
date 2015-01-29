// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class JobCandidate : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Employee);
            return t.ToString();
        }

        #endregion

        public virtual int JobCandidateID { get; set; }

        public virtual string Resume { get; set; }

        public Employee Employee { get; set; }

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }
    }
}