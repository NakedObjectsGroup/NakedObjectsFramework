using NakedObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{   
    public partial class PersonPhone {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Title
        
        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(PhoneNumberType).Append(":", PhoneNumber);
            return t.ToString();
        }
      
        #endregion
        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }
        public virtual string PhoneNumber { get; set; }
        [NakedObjectsIgnore]
        public virtual int PhoneNumberTypeID { get; set; }

        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    
        [NakedObjectsIgnore]
        public virtual Person Person { get; set; }
        public virtual PhoneNumberType PhoneNumberType { get; set; }
    }
}
