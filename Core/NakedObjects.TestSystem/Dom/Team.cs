// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;

namespace NakedObjects.Testing.Dom {
    public class Team {
        private IList members = new ArrayList();

        public IList Members {
            get { return members; }
        }
    }
  }
 