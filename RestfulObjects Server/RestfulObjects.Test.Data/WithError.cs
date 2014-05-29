// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class WithError {
        private IList<MostSimple> aCollection = new List<MostSimple>();

        public IDomainObjectContainer Container { set; protected get; }

        [Key, Title]
        public virtual int Id { get; set; }

        public virtual int AnErrorValue {
            get { return 0; }
            set { throw new DomainException("An error exception"); }
        }

        public virtual MostSimple AnErrorReference {
            get { return Container.Instances<MostSimple>().LastOrDefault(); }
            set { throw new DomainException("An error exception"); }
        }

        public IList<MostSimple> ACollection {
            get { return aCollection; }
            set { aCollection = value; }
        }

        public virtual int AnError() {
            throw new DomainException("An error exception");
        }
    }
}