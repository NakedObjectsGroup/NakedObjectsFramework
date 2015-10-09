using NakedObjects;
using System;
using System.Collections.Generic;

namespace AdventureWorksModel
{   
    public partial class PersonPhone
    {
        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }
        public virtual string PhoneNumber { get; set; }
        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }
        
        public virtual DateTime ModifiedDate { get; set; }
    
        [NakedObjectsIgnore]
        public virtual Person Person { get; set; }
        public virtual PhoneNumberType PhoneNumberType { get; set; }
    }
}
