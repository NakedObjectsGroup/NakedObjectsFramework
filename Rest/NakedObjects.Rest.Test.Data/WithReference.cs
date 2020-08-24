// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NakedObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    public class WithReference {
        public IDomainObjectContainer Container { set; protected get; }

        [Key]
        [Title]
        [ConcurrencyCheck]
        [DefaultValue(0)]
        public virtual int Id { get; set; }

        public virtual MostSimple AReference { get; set; }

        [Optionally]
        public virtual MostSimple ANullReference { get; set; }

        [Disabled]
        public virtual MostSimple ADisabledReference { get; set; }

        [Hidden(WhenTo.Always)]
        [Optionally]
        public virtual MostSimple AHiddenReference { get; set; }

        public virtual MostSimple AChoicesReference { get; set; }

        public virtual MostSimple AnAutoCompleteReference { get; set; }
        public virtual MostSimple AConditionalChoicesReference { get; set; }

        [FindMenu]
        [NotMapped]
        public virtual MostSimple AFindMenuReference { get; set; }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        public virtual MostSimple AnEagerReference { get; set; }

        public virtual MostSimple[] ChoicesAChoicesReference() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToArray();
        }

        public virtual IQueryable<MostSimple> AutoCompleteAnAutoCompleteReference([MinLength(2)] string name) {
            return Container.Instances<MostSimple>().Where(ms => name.Contains(ms.Id.ToString()));
        }

        public virtual string Validate(MostSimple aReference, MostSimple aChoicesReference) {
            if (aReference != null && aReference.Id == 1 && aChoicesReference.Id == 2) {
                return "Cross validation failed";
            }

            return "";
        }

        public virtual MostSimple[] ChoicesAConditionalChoicesReference(MostSimple aReference) {
            if (aReference != null) {
                return Container.Instances<MostSimple>().Where(ms => ms.Id != aReference.Id).ToArray();
            }

            return new MostSimple[] { };
        }
    }
}