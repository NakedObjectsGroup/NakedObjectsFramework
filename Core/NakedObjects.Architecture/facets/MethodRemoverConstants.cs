// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Reflection;

namespace NakedObjects.Architecture.Facets {
    public class MethodRemoverConstants {
        public static IMethodRemover NULL;

        static MethodRemoverConstants() {
            NULL = new NullMethodRemover();
        }

        #region Nested type: NullMethodRemover

        public class NullMethodRemover : IMethodRemover {
            #region IMethodRemover Members

            public virtual void RemoveMethod(MethodInfo method) {}

            public virtual void RemoveMethods(IList<MethodInfo> methodList) {}

            #endregion
        }

        #endregion
    }
}