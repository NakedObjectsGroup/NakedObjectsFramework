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
    public class WithNestedViewModel : IViewModel {
        private int deriveCheck;
        private int populateCheck;
        public IDomainObjectContainer Container { set; protected get; }

        [Key]
        [Title]
        [ConcurrencyCheck]
        public virtual int Id { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string AggregateKey {
            get { return DeriveKeys().Aggregate("", (s, t) => s + " " + t); }
        }

        public virtual MostSimple AReference { get; set; }

        public virtual WithReferenceViewModel AViewModelReference { get; set; }

        #region IViewModel Members

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            deriveCheck++;

            if (deriveCheck > 1) {
                throw new Exception("Derive called multiple times");
            }

            var keys = new List<string> {AReference.Id.ToString()};
            keys.AddRange(AViewModelReference.TestDeriveKeys());
            return keys.ToArray();
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            populateCheck++;

            if (populateCheck > 1) {
                throw new Exception("PopulateUsingKeys called multiple times");
            }

            var msId = int.Parse(keys[0]);
            var vmKey = keys.Skip(1).ToArray();

            Id = msId;

            AReference = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == msId);

            AViewModelReference = Container.NewViewModel<WithReferenceViewModel>();
            AViewModelReference.PopulateUsingKeys(vmKey);
        }

        #endregion
    }
}