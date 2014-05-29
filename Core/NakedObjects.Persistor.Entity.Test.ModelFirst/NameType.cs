// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;
using NakedObjects.Services;

namespace ModelFirst {
    [ComplexType]
    public class NameType : AbstractTestCode {
        [Root]
        public Person Parent { get; set; }

        public IDomainObjectContainer Container { get; set; }

        public SimpleRepository<Person> Service { get; set; }

        #region Primitive Properties

        public string Firstname { get; set; }
        public string Surname { get; set; }

        #endregion

        public object ExposeContainerForTest() {
            return Container;
        }
    }
}