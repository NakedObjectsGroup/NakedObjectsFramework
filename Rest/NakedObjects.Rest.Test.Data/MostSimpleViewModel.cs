// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NakedObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    [NotPersisted]
    [NotMapped]
    public class MostSimpleViewModel : IViewModel {
        private int deriveCheck;
        private int populateCheck;
        public virtual IDomainObjectContainer Container { set; protected get; }

        [Hidden(WhenTo.Always)]
        public virtual string AggregateKey {
            get { return DeriveKeys().Aggregate("", (s, t) => s + " " + t); }
        }

        [Title]
        [ConcurrencyCheck]
        public virtual int Id { get; set; }

        #region IViewModel Members

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            deriveCheck++;

            if (deriveCheck > 1) {
                throw new Exception("Derive called multiple times");
            }

            return new[] {Id.ToString()};
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            populateCheck++;

            if (populateCheck > 1) {
                throw new Exception("PopulateUsingKeys called multiple times");
            }

            Id = int.Parse(keys.First());
        }

        #endregion
    }
}