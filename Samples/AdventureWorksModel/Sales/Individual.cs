// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("person.png")]
    public class Individual : Customer {

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Contact).Append(",", AccountNumber);
            return t.ToString();
        }


        #region Contact

        [MemberOrder(20)]
        [DisplayName("Name & Contact Details")]
        public virtual Contact Contact { get; set; }

        #endregion

        #region Demographics

        [Hidden]
        public virtual string Demographics { get; set; }


        [DisplayName("Demographics")]
        [MemberOrder(30)]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(500)]
        public virtual string FormattedDemographics
        {
            get { return Utilities.FormatXML(Demographics); }
        }
        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}