// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class WithGetError {
        private readonly IList<MostSimple> anErrorCollection = new List<MostSimple>();
        private int getCount;

        public IDomainObjectContainer Container { set; protected get; }


        [Key, Title]
        public virtual int Id { get; set; }


        public virtual int AnErrorValue {
            get {
                if (getCount++ > 5) {
                    // so no errors on startup 
                   // throw new DomainException("An error exception");
                }
                return 0;
            }
            set { }
        }


        public virtual MostSimple AnErrorReference {
            get {
                if (getCount++ > 4) {
                    // so no errors on startup 
                   // throw new DomainException("An error exception");
                }
                return Container == null ?  null :  Container.Instances<MostSimple>().FirstOrDefault();
            }
            set { }
        }

        public virtual IList<MostSimple> AnErrorCollection {
            get {
                if (getCount++ > 4) {
                    // so no errors on startup 
                   // throw new DomainException("An error exception");
                }
                return anErrorCollection;
            }
            set { }
        }


        public virtual int AnError() {
            throw new DomainException("An error exception");
        }
    }
}