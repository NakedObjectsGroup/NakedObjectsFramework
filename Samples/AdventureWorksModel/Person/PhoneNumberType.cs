using NakedObjects;
using System;
using System.Collections.Generic;

namespace AdventureWorksModel {
    public partial class PhoneNumberType {
        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }
        [Title][Hidden(WhenTo.Always)]
        public virtual string Name { get; set; }
        [NakedObjectsIgnore]
        public virtual DateTime ModifiedDate { get; set; }
    }
}
