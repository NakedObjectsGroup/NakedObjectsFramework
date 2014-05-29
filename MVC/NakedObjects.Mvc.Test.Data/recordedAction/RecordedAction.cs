// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NakedObjects;

namespace Expenses.RecordedActions {
    [Immutable(WhenTo.OncePersisted)]
    public class RecordedAction {
        [Hidden]
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

        [Hidden, ConcurrencyCheck]
        public DateTime ConcurrencyCheck { get; set; }
    }
}