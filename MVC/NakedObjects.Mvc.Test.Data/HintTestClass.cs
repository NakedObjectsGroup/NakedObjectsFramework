// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [PresentationHint("classHint")]
    [Bounded]
    public class HintTestClass {
        private IList<HintTestClass> hintCollection = new List<HintTestClass>();

        [Title]
        [PresentationHint("propertyHint")]
        public string TestString { get; set; }

        [PresentationHint("collectionHint")]
        public IList<HintTestClass> HintCollection {
            get { return hintCollection; }
            set { hintCollection = value; }
        }

        [PresentationHint("actionHint")]
        public void SimpleAction() {}

        public void ActionWithParms([PresentationHint("parmHint1")] int parm1, [PresentationHint("parmHint2")] int parm2) {}
    }
}