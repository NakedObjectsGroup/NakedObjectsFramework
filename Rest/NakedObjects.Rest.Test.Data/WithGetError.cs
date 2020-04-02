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
    public class WithGetError {
        private readonly IList<MostSimple> anErrorCollection = new List<MostSimple>();
        public IDomainObjectContainer Container { set; protected get; }

        public static bool ThrowErrors { get; set; }

        [Key]
        [Title]
        public virtual int Id { get; set; }

        public virtual int AnErrorValue {
            get {
                if (Container != null && ThrowErrors) {
                    // so no errors on startup 
                    throw new DomainException("An error exception");
                }

                return 0;
            }
            set { }
        }

        public virtual MostSimple AnErrorReference {
            get {
                if (Container != null && ThrowErrors) {
                    // so no errors on startup 
                    throw new DomainException("An error exception");
                }

                return Container == null ? null : Container.Instances<MostSimple>().FirstOrDefault();
            }
            set { }
        }

        public virtual ICollection<MostSimple> AnErrorCollection {
            get {
                if (Container != null && ThrowErrors) {
                    // so no errors on startup 
                    throw new DomainException("An error exception");
                }

                return anErrorCollection;
            }
            set { }
        }

        public virtual int AnError() => throw new DomainException("An error exception");
    }
}