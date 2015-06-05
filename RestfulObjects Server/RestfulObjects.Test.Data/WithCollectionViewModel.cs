// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class WithCollectionViewModel : IViewModel {
        private IList<MostSimple> anEmptyCollection = new List<MostSimple>();

        private ISet<MostSimple> anEmptySet = new HashSet<MostSimple>();

        public IDomainObjectContainer Container { set; protected get; }


        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        public virtual IList<MostSimple> ACollection { get; set; }

        public virtual IList<MostSimpleViewModel> ACollectionViewModels { get; set; }

        public virtual ISet<MostSimple> ASet { get; set; }

        [Disabled]
        public virtual IList<MostSimple> ADisabledCollection { get; set; }

        [Hidden(WhenTo.Always)]
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

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        public virtual IList<MostSimple> AnEagerCollection { get; set; }

        #region IViewModel Members

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
            ACollectionViewModels = new[] {fId, lId}.Select(NewVM).ToList();
            ASet = new HashSet<MostSimple>(Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2));
            ADisabledCollection = Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
            AHiddenCollection = Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
            AnEagerCollection = Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        #endregion

        private MostSimpleViewModel NewVM(int id) {
            var vm = Container.NewViewModel<MostSimpleViewModel>();
            vm.Id = id;
            return vm;
        }
    }
}