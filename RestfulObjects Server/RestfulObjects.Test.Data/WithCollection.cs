// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using Do = NakedObjects.EagerlyAttribute.Do;

namespace RestfulObjects.Test.Data {
    public class WithCollection {
        private IList<MostSimple> anEmptyCollection = new List<MostSimple>();

        private ISet<MostSimple> anEmptySet = new HashSet<MostSimple>();

        public IDomainObjectContainer Container { set; protected get; }

        [Key, Title]
        public virtual int Id { get; set; }

        [PresentationHint("class7 class8")]
        public virtual IList<MostSimple> ACollection {
            get { return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList(); }
            set { }
        }

        public virtual IList<MostSimpleViewModel> ACollectionViewModels {
            get { return new[] {1, 2}.Select(NewViewModel).ToList(); }
            set { }
        }

        public virtual ISet<MostSimple> ASet {
            get { return new HashSet<MostSimple>(Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2)); }
            set { }
        }

        [Disabled]
        public virtual IList<MostSimple> ADisabledCollection {
            get { return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList(); }
            set { }
        }

        [Hidden]
        public virtual IList<MostSimple> AHiddenCollection {
            get { return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList(); }
            set { }
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

        private MostSimpleViewModel NewViewModel(int id) {
            var vm = Container.NewViewModel<MostSimpleViewModel>();
            vm.Id = id;
            return vm;
        }

        [Eagerly(Do.Rendering)]
        public virtual IList<MostSimple> AnEagerCollection {
            get { return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList(); }
            set { }
        }
    }
}