// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    [Eagerly(EagerlyAttribute.Do.Rendering)]
    public class VerySimpleEager {
        [Key, Hidden(WhenTo.Always), ConcurrencyCheck]
        public virtual int Id { get; set; }

        [Optionally, Title]
        [MaxLength(101)]
        [RegEx(Validation = @"[A-Z]")]
        public virtual string Name { get; set; }

        [Optionally, Title]
        public virtual MostSimple MostSimple { get; set; }

        #region SimpleList (collection)

        private ICollection<MostSimple> _SimpleList = new List<MostSimple>();

        public virtual ICollection<MostSimple> SimpleList {
            get { return _SimpleList; }
            set { _SimpleList = value; }
        }

        #endregion

        #region SimpleSet (collection)

        private ISet<MostSimple> _SimpleSet = new HashSet<MostSimple>();

        [NotMapped]
        public virtual ISet<MostSimple> SimpleSet {
            get { return _SimpleSet; }
            set { _SimpleSet = value; }
        }

        #endregion
    }
}