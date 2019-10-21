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
    public class WithError {
        private IList<MostSimple> aCollection = new List<MostSimple>();

        private MostSimple aReference;
        public IDomainObjectContainer Container { set; protected get; }

        public static bool ThrowErrors { get; set; }

        [Key, Title]
        public virtual int Id { get; set; }

        public virtual int AnErrorValue {
            get { return 0; }
            set {
                if (Container != null && ThrowErrors) {
                    // initialised 
                    throw new DomainException("An error exception");
                }
            }
        }

        public virtual MostSimple AnErrorReference {
            get { return aReference; }
            set {
                if (Container != null && ThrowErrors) {
                    // initialised 
                    throw new DomainException("An error exception");
                }
                aReference = value;
            }
        }

        public virtual ICollection<MostSimple> ACollection {
            get { return aCollection; }
            set { aCollection = value.ToList(); }
        }

        public virtual int AnError() {
            throw new DomainException("An error exception");
        }
    }
}