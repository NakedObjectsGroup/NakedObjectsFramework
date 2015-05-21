// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using System.Text;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ProjectCode {
            [NakedObjectsIgnore, Key]
            public int Id { get; set; }

            #region Code

            public virtual string Code { get; set; }

            #endregion

            #region Description

            [MultiLine(NumberOfLines = 2, Width = 10)]
            public virtual string Description { get; set; }

            #endregion

            #region Title & Icon

            public virtual string Title() {
                var t = new StringBuilder();
                t.Append(Code).Append(" ").Append(Description);
                return t.ToString();
            }

            public virtual string IconName() {
                return "LookUp";
            }

            #endregion
        }
    }
} //end of root namespace