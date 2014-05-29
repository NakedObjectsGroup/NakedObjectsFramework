// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace TestCodeOnly {
    public class AbstractPerson : AbstractTestCode{}

    public class Person : AbstractPerson {
        public IDomainObjectContainer Container { protected get; set; }

        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual Product Favourite { get; set; }
        public virtual Address Address { get; set; }

        public object ExposeContainerForTest() {
            return Container;
        }
    }
}