// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework;
using NakedObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    public class WithReferenceViewModelEdit : IViewModelEdit {
        private int deriveCheck;
        private int populateCheck;
        public IDomainObjectContainer Container { set; protected get; }

        [Hidden(WhenTo.Always)]
        public string AggregateKey {
            get { return DeriveKeys().Aggregate("", (s, t) => s + " " + t); }
        }

        [Key]
        [Title]
        [ConcurrencyCheck]
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

        [Eagerly(Do.Rendering)]
        public virtual MostSimple AnEagerReference { get; set; }

        public virtual MostSimple[] ChoicesAChoicesReference() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToArray();
        }

        public virtual string Validate(MostSimple aReference, MostSimple aChoicesReference) {
            if (aReference != null && aReference.Id == 1 && aChoicesReference.Id == 2) {
                return "Cross validation failed";
            }

            return "";
        }

        #region IViewModelEdit Members

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            deriveCheck++;

            if (deriveCheck > 1) {
                throw new Exception("Derive called multiple times");
            }

            var keys = new List<string> {
                AReference.Id.ToString(),
                ADisabledReference.Id.ToString(),
                AHiddenReference.Id.ToString(),
                AChoicesReference.Id.ToString()
            };

            return keys.ToArray();
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            populateCheck++;

            if (populateCheck > 1) {
                throw new Exception("PopulateUsingKeys called multiple times");
            }

            var rId = int.Parse(keys[0]);
            var drId = int.Parse(keys[1]);
            var hrId = int.Parse(keys[2]);
            var crId = int.Parse(keys[3]);

            Id = rId;

            AReference = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == rId);
            ADisabledReference = Container.Instances<MostSimple>().FirstOrDefault(vs => vs.Id == drId);
            AHiddenReference = Container.Instances<MostSimple>().FirstOrDefault(vs => vs.Id == hrId);
            AChoicesReference = Container.Instances<MostSimple>().FirstOrDefault(vs => vs.Id == crId);
            AnEagerReference = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == rId);
        }

        #endregion
    }
}