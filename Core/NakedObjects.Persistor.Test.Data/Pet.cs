// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace TestData {
    public class Pet : TestHelper {

        [Key, ForeignKey("Owner")]
        public virtual int PetId { get; set; }

        [Title, Optionally]
        public virtual string Name { get; set; }

        public virtual Person Owner { get; set; }

        public virtual string ValidateOwner(Person owner) {
            if (owner.Name == "Cruella") {
                return "Bad owner";
            }
            return null; 
        }

    }
}
