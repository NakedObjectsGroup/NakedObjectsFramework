// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class ContributorService {

        public IDomainObjectContainer Container { set; protected get; }

        public virtual MostSimple ANonContributedAction() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }
    }
}