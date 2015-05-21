// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NakedObjects;

namespace Expenses.RecordedActions {
    [Immutable(WhenTo.OncePersisted)]
    public class RecordedAction {
        [NakedObjectsIgnore, Key]
        public int Id { get; set; }

        [NakedObjectsIgnore]
        public virtual IRecordedActionContext Context { get; set; }

        [MemberOrder(Sequence = "1"), Disabled]
        public virtual DateTime Date { get; set; }

        [MemberOrder(Sequence = "2"), Disabled]
        public virtual string Type { get; set; }

        [MemberOrder(Sequence = "3"), Disabled]
        public virtual string Name { get; set; }

        [MemberOrder(Sequence = "5"), Disabled]
        public virtual string Details { get; set; }

        [MemberOrder(Sequence = "6"), Disabled]
        public virtual IActor Actor { get; set; }

        [NakedObjectsIgnore, ConcurrencyCheck]
        public DateTime ConcurrencyCheck { get; set; }

        #region Title

        /// <summary> Defines the title that will be displayed on the user
        /// interface in order to identity this object.
        /// </summary>
        public virtual string Title() {
            var t = new StringBuilder();
            t.Append(Type).Append(" ").Append(Name);
            return t.ToString();
        }

        #endregion

        public void Persisting() {
            ConcurrencyCheck = DateTime.Now;
        }

        #region Context field

        #endregion

        #region Date field

        #endregion

        #region Type field

        public static string ACTION = "Action";
        public static string CHANGE = "Change";

        #endregion

        #region Name field

        #endregion

        #region Details field

        #endregion

        #region User field

        #endregion
    }
}