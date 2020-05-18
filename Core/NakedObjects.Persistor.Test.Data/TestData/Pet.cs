// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace TestData {
    public class Pet : TestHelper {
        [Key]
        [ForeignKey("Owner")]
        public virtual int PetId { get; set; }

        [Title]
        [Optionally]
        public virtual string Name { get; set; }

        public virtual Person Owner { get; set; }

        public virtual string ValidateOwner(Person owner) {
            if (owner.Name == "Cruella") {
                return "Bad owner";
            }

            return null;
        }
    }
}