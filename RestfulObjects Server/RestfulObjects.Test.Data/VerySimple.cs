// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class VerySimple {
        [Key, Hidden]
        public virtual int Id { get; set; }

        [Optionally, Title]
        public virtual string Name { get; set; }

        [Optionally, Title]
        public virtual MostSimple MostSimple { get; set; }

        #region SimpleList (collection)

        private ICollection<MostSimple> _SimpleList = new List<MostSimple>();

        public virtual ICollection<MostSimple> SimpleList {
            get { return _SimpleList; }
            set { _SimpleList = value; }
        }

        public void EmptyTheList() {
            _SimpleList.Clear();
        }

        #endregion

        #region SimpleSet (collection)

        private ICollection<MostSimple> _SimpleSet = new HashSet<MostSimple>();

        public virtual ICollection<MostSimple> SimpleSet {
            get { return _SimpleSet; }
            set { _SimpleSet = value; }
        }

        public void EmptyTheSet() {
            _SimpleSet.Clear();
        }

        #endregion
    }
}