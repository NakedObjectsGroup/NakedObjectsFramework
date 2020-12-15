﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Core;

namespace TestData {
    public class Product : TestHelper {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Name { get; set; }

        [ConcurrencyCheck]
        public virtual string ModifiedDate { get; set; }

        public override void Persisting() {
            ModifiedDate = DateTime.Now.ToBinary().ToString();
            base.Persisting();
        }

        public override void Persisted() {
            base.Persisted();
            if (Id == 0) {
                throw new UnexpectedCallException("Id must not be null");
            }
        }

        public override void Updating() {
            ModifiedDate = DateTime.Now.ToBinary().ToString();
            base.Updating();
        }
    }
}