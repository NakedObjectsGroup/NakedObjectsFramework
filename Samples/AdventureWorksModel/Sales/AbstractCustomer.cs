// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [DisplayName("Customer")]
    public abstract class AbstractCustomer : AWDomainObject {
        [MemberOrder(10)]
        [DisplayName("Account Details")]
        public abstract Customer Customer { get; set; }



    }
}