// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ExpenseType {
            [Hidden(WhenTo.Always), Key]
            public int Id { get; set; }

            #region TitleString

            public virtual string TitleString { get; set; }

            #endregion

            #region Title & Icon

            public override string ToString() {
                return TitleString;
            }

            public virtual string IconName() {
                return TitleString;
            }

            #endregion

            #region Corresponding Class

            /// <summary> This method potentially allows each instance of ExpenseType to have the same icon as its corresponding classname.</summary>
            [Hidden(WhenTo.Always)]
            public virtual string CorrespondingClassName { get; set; }

            /// <summary> Converts the correspondingClassName into a system type.</summary>
            public virtual Type CorrespondingClass() {
                return Type.GetType(CorrespondingClassName);
            }

            #endregion
        }
    }
} //end of root namespace