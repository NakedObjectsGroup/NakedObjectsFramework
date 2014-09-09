// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using Do = NakedObjects.EagerlyAttribute.Do;

namespace RestfulObjects.Test.Data {
    public class WithCollectionViewModel : IViewModel {
        private IList<MostSimple> anEmptyCollection = new List<MostSimple>();

        private ISet<MostSimple> anEmptySet = new HashSet<MostSimple>();

        public IDomainObjectContainer Container { set; protected get; }

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            var keys = new List<string> {
                ACollection.First().Id.ToString(),
                ACollection.Last().Id.ToString()
            };

            return keys.ToArray();
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {


           int fId = int.Parse(keys[0]);
           int lId = int.Parse(keys[1]);
         

            Id = fId;

            ACollection = Container.Instances<MostSimple>().Where(ms => ms.Id == fId || ms.Id == lId).ToList();
            ACollectionViewModels = new[] { fId, lId }.Select(NewVM).ToList();
            ASet = new HashSet<MostSimple>(Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2));
            ADisabledCollection =  Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
            AHiddenCollection = Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
            AnEagerCollection = Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }



        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        public virtual IList<MostSimple> ACollection { get; set; }
 
        private  MostSimpleViewModel NewVM(int id) {
            var vm = Container.NewViewModel<MostSimpleViewModel>();
            vm.Id = id;
            return vm; 
        }

        public virtual IList<MostSimpleViewModel> ACollectionViewModels { get; set; }

        public virtual ISet<MostSimple> ASet { get; set; }

        [Disabled]
        public virtual IList<MostSimple> ADisabledCollection { get; set; }

        [Hidden]
        public virtual IList<MostSimple> AHiddenCollection { get; set; }

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
        public virtual IList<MostSimple> AnEagerCollection { get; set; }
    }
}