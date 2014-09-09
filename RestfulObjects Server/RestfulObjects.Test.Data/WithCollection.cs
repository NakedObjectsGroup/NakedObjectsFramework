// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using Do = NakedObjects.EagerlyAttribute.Do;

namespace RestfulObjects.Test.Data {
    public class WithCollection {
        private IList<MostSimple> anEmptyCollection = new List<MostSimple>();

        private ISet<MostSimple> anEmptySet = new HashSet<MostSimple>();
        private IList<MostSimple> aCollection = new List<MostSimple>();
        private IList<MostSimpleViewModel> aCollectionViewModels = new List<MostSimpleViewModel>();
        private ISet<MostSimple> aSet = new HashSet<MostSimple>();
        private IList<MostSimple> aDisabledCollection = new List<MostSimple>();
        private IList<MostSimple> aHiddenCollection = new List<MostSimple>();
        private IList<MostSimple> anEagerCollection = new List<MostSimple>();

        public IDomainObjectContainer Container { set; protected get; }

        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

     
        [PresentationHint("class7 class8")]
        public virtual IList<MostSimple> ACollection {
            get { return aCollection; }
            set { aCollection = value; }
        }

        public virtual IList<MostSimpleViewModel> ACollectionViewModels {
            get { return aCollectionViewModels; }
            set { aCollectionViewModels = value; }
        }

        public virtual ISet<MostSimple> ASet {
            get { return aSet; }
            set { aSet = value; }
        }

        [Disabled]
        public virtual IList<MostSimple> ADisabledCollection {
            get { return aDisabledCollection; }
            set { aDisabledCollection = value; }
        }

        [Hidden]
        public virtual IList<MostSimple> AHiddenCollection {
            get { return aHiddenCollection; }
            set { aHiddenCollection = value; }
        }

        [DescribedAs("an empty collection for testing")]
        [MemberOrder(Sequence = "2")]
        public virtual IList<MostSimple> AnEmptyCollection {
            get { return anEmptyCollection; }
            set { anEmptyCollection = value; }
        }

        [DescribedAs("an empty set for testing")]
        [MemberOrder(Sequence = "2")]
        public virtual ISet<MostSimple> AnEmptySet {
            get { return anEmptySet; }
            set { anEmptySet = value; }
        }

        [Eagerly(Do.Rendering)]
        public virtual IList<MostSimple> AnEagerCollection {
            get { return anEagerCollection; }
            set { anEagerCollection = value; }
        }
    }
}